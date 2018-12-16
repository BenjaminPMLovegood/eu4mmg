using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eu4mmg {
    internal partial class RuntimeOptions {
        public string ModName;
        public string ModDisplayName;
        public string ModFilePath, ModFileLocation, ModFileName, ModFileMainName;
        public FileInfo ModFileInfo;

        public bool OutputToArchive;
        public string Output;
        public FileInfo OutputArchive;
        public DirectoryInfo OutputDirectory;

        public LogThresholdLevel LogThreshold;

        public List<Song> Songs;

        public string Version;

        public DirectoryInfo TempDir;
        public DirectoryInfo TempMusicDir;
        public FileInfo TempMusicListFile;
        public FileInfo TempMusicAssetFile;
        public DirectoryInfo TempLocalisationDir;
        public FileInfo TempLocalisationFile;
        public FileInfo TempDescriptorFile;
    }
}
