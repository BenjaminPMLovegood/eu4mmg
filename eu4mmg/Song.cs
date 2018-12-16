using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eu4mmg {
    internal class Song {
        public FileInfo SongFile;
        public string Name;
        public string InternalName;
        public FileInfo SongFileInTempDir;

        private Song() { }

        public void ConvertToTempDir(DirectoryInfo dir) {
            SongFileInTempDir = new FileInfo(Path.Combine(dir.FullName, InternalName + ".ogg"));

            if (SongFile.Extension.ToLower() == ".ogg") {
                SongFile.CopyTo(SongFileInTempDir.FullName, true);
            } else {
                OggConverter.Convert(SongFile.FullName, SongFileInTempDir.FullName);
            }
        }

        public static Song Parse(string param, string modName) {
            var rv = new Song();
            var sp = param.Split(';');

            if (sp.Length > 3) Program.IssueWarn($"Out of range song param {param}, continue parsing.");
            // useless: if (sp.Length < 1) { Program.IssueWarn($"Out of range song param {i}, skip it."); continue; }

            var path = sp[0].Trim();

            try {
                rv.SongFile = new FileInfo(path);
            } catch (Exception) {
                Program.IssueWarn($"Cannot access song file {path}, skip it");
                return null;
            }

            if (!rv.SongFile.Exists) {
                Program.IssueWarn($"Song file {path} doesn't exists, skip it");
                return null;
            }

            rv.Name = sp.Length >= 2 ? sp[1].Trim() : Path.ChangeExtension(Path.ChangeExtension(rv.SongFile.Name, null), null);
            NameHelper.DoAsciify(ref rv.Name, "song name");

            rv.InternalName = sp.Length >= 3 ? sp[2].Trim() : rv.Name;
            NameHelper.DoIdify(ref rv.InternalName, "song internal name");

            return rv;
        }
    }
}
