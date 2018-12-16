using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eu4mmg {
    internal partial class RuntimeOptions {
        internal void FromCommandLineOptions(CommandLineOptions opt) {
            // Log level
            if (opt.Quiet) LogThreshold = LogThresholdLevel.Silence;
            else if (opt.Detail) LogThreshold = LogThresholdLevel.All;
            else LogThreshold = LogThresholdLevel.Default;

            Program.IssueInfo("Checking params...");

            // name
            if (opt.DisplayName == null) {
                ModDisplayName = "Music Mod";
                Program.IssueWarn($"Mod display name not specified. Set it to \"{ModDisplayName}\".");
            } else {
                ModDisplayName = opt.DisplayName;
            }

            NameHelper.DoAsciify(ref ModDisplayName, "mod display name");

            if (opt.Name == null) {
                ModName = ModDisplayName.ToLower();
                Program.IssueWarn($"Mod name not specified. Set it to \"{ModName}\".");
            } else {
                ModName = opt.Name;
            }

            NameHelper.DoIdify(ref ModName, "mod name");

            // mod file
            if (opt.ModFile == null) {
                ModFilePath = ModName + Def.ModFileDefaultExt;
                Program.IssueWarn($"Mod file name not specified. Set it to \"{ModFilePath}\".");
            } else {
                ModFilePath = opt.ModFile;
            }
            
            ModFileInfo = new FileInfo(ModFilePath);
            ModFileName = ModFileInfo.Name;
            // though i know it won't be null, but to make static analyzer satisfied......
            ModFileLocation = ModFileInfo.DirectoryName ?? "";

            if (NameHelper.DoAsciify(ref ModFileName, "mod file name")) {
                ModFileInfo = new FileInfo(ModFilePath = Path.Combine(ModFileLocation, ModFileName));
            }

            if (ModFileInfo.Extension.ToLower() != Def.ModFileDefaultExt) {
                Program.IssueWarn($"Invalid mod file name extension \"{ModFileInfo.Extension}\", changed to \"{Def.ModFileDefaultExt}\".");
                ModFileName = ModFileName + Def.ModFileDefaultExt;
                ModFileInfo = new FileInfo(ModFilePath = Path.Combine(ModFileLocation, ModFileName));
            }

            ModFileMainName = Path.ChangeExtension(ModFileName, null);

            // output mode and destination
            if (opt.ModDir) {
                if (opt.ModAr) Program.IssueError("--mod-dir cannot be combined with --mod-ar.");
                OutputToArchive = false;
            } else {
                if (!opt.ModAr) Program.IssueWarn("Neither --mod-dir nor --mod-ar is specified. Use --mod-ar.");
                OutputToArchive = true;
            }

            if (opt.Output == null) {
                Output = Path.Combine(ModFileLocation, ModFileMainName + (OutputToArchive ? ".zip" : Path.PathSeparator + ""));
                Program.IssueWarn($"Output path not specified. Set it to {Output}.");
            } else {
                Output = opt.Output;
            }

            if (OutputToArchive) {
                OutputArchive = new FileInfo(Output);
            } else {
                OutputDirectory = new DirectoryInfo(Output);
            }

            Program.IssueInfo($"Output determined. Name: \"{ModName}\", mod file: \"{ModFilePath}\", output type: {(OutputToArchive ? "archive" : "directory")}, output: \"{Output}\"");
            
            // songs
            Songs = new List<Song>();
            foreach (var i in opt.SongStrings) {
                var song = Song.Parse(i, ModName);

                if (song == null) continue;
                Songs.Add(song);
            }

            if (Songs.Count == 0) Program.IssueError("No valid songs provided.");

            // ensure version exists
            if (opt.Ver == null) Program.IssueError("No version specified.");
            Version = opt.Ver;
            
            // temp dir
            TempDir = opt.TempDir == null ? new DirectoryInfo("temp") : new DirectoryInfo(opt.TempDir);
            if (!TempDir.Exists) {
                Program.IssueInfo($"Temp dir \"{TempDir.FullName}\" doesn't exist. Create it.");
                TempDir.Create();
                TempDir.Refresh();
            }

            TempMusicDir = TempDir.CreateSubdirectory("music");
            TempLocalisationDir = TempDir.CreateSubdirectory("localisation");

            TempMusicListFile = new FileInfo(Path.Combine(TempMusicDir.FullName, $"song_{ModName}.txt"));
            TempMusicAssetFile = new FileInfo(Path.Combine(TempMusicDir.FullName, $"song_{ModName}.asset"));

            TempLocalisationFile = new FileInfo(Path.Combine(TempLocalisationDir.FullName, $"song_{ModName}_l_english.yml"));

            TempDescriptorFile = new FileInfo(Path.Combine(TempDir.FullName, "descriptor.mod"));
        }
    }
}
