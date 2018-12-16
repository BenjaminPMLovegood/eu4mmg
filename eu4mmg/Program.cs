using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CommandLine;
using Ionic.Zip;

namespace eu4mmg {
    internal static class Program {
        private static readonly CommandLineOptions COpt = new CommandLineOptions();
        private static readonly RuntimeOptions ROpt = new RuntimeOptions();

        public static void Done() {
            Environment.Exit(0);
        }

        public static void IssueError(string error, int exitCode = -1) {
            if (ROpt.LogThreshold <= LogThresholdLevel.Error) Console.WriteLine($"ERROR: {error}");
            Environment.Exit(exitCode);
        }

        private static int _globalWarnLevel = 0;
        public static void IssueWarn(string warn, int warnLevel = 1) {
            if (ROpt.LogThreshold <= LogThresholdLevel.Warning) Console.WriteLine($"WARN(level{warnLevel}): {warn}");
            if (warnLevel > _globalWarnLevel) _globalWarnLevel = warnLevel;
        }

        public static void IssueInfo(string info) {
            if (ROpt.LogThreshold <= LogThresholdLevel.Info) Console.WriteLine($"INFO: {info}");
        }

        private static void WriteTo(FileInfo file, string content, bool bom = false) {
            var w = new StreamWriter(file.OpenWrite(), new UTF8Encoding(bom));
            w.Write(content);
            w.Close();
        }

        private static string GetMusicListFileContent() {
            return string.Join("", ROpt.Songs.Select(song => $"song = {{\n\tname = \"{song.InternalName}\"\n\n\tchance = {{\t\n\t\tmodifier = {{\n\t\t\tfactor = 1\n\t\t}}\n\t}}\n}}\n").ToArray());
        }

        private static string GetMusicAssetFileContent() {
            return string.Join("", ROpt.Songs.Select(song => $"music = {{\n\tname = \"{song.InternalName}\"\n\tfile = \"{song.SongFileInTempDir.Name}\"\n}}\n").ToArray());
        }

        private static string GetLocalisationFileContent() {
            return "l_english:\n" + string.Join("", ROpt.Songs.Select(song => $" {song.InternalName}:0 \"{song.Name}\"\n").ToArray());
        }

        private static string GetModFileContent() {
            return $"name=\"{ROpt.ModDisplayName}\"\n{(ROpt.OutputToArchive ? $"archive=\"mod/{ROpt.OutputArchive.Name}\"" : $"path=\"mod/{ROpt.OutputDirectory.Name}\"")}\nsupported_version=\"{ROpt.Version}.*.*\"\n";
        }

        public static int Main(string[] args) {
            var p = Parser.Default;

            if (!p.ParseArgumentsStrict(args, COpt)) {
                IssueError("Cannot parse args.");
            }

            ROpt.FromCommandLineOptions(COpt);
            
            foreach (var song in ROpt.Songs) {
                IssueInfo($"Converting {song.Name}({song.SongFile.FullName})");
                song.ConvertToTempDir(ROpt.TempMusicDir);
            }
            
            WriteTo(ROpt.TempMusicListFile, GetMusicListFileContent());
            WriteTo(ROpt.TempMusicAssetFile, GetMusicAssetFileContent());
            WriteTo(ROpt.TempLocalisationFile, GetLocalisationFileContent(), true); // localisation wants bom, bullshit

            var modFileContent = GetModFileContent();

            if (ROpt.OutputToArchive) {
                WriteTo(ROpt.TempDescriptorFile, modFileContent);

                if (ROpt.OutputArchive.Exists) ROpt.OutputArchive.Delete();

                var zip = new ZipFile(new UTF8Encoding(false));
                zip.AddDirectory(ROpt.TempDir.FullName);
                zip.Save(ROpt.OutputArchive.FullName);
            } else {
                ROpt.TempDir.MoveTo(ROpt.OutputDirectory.FullName);
            }

            WriteTo(ROpt.ModFileInfo, modFileContent);
            
            return _globalWarnLevel;
        }
    }
}
