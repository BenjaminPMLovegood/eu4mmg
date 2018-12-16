using System;
using System.Diagnostics;
using System.IO;

namespace eu4mmg {
    internal static class OggConverter {
        private static readonly string FfmpegPath = "3rd\\ffmpeg.exe";

        public static void Convert(string input, string output) {
            Process.Start(new ProcessStartInfo(FfmpegPath, $"-hide_banner -loglevel panic -y -i \"{input}\" -map 0:a -f ogg \"{output}\"") { UseShellExecute = false })?.WaitForExit();
        }
    }
}
