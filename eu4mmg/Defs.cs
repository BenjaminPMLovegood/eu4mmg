using System;

namespace eu4mmg {
    internal class DefGetter {
        public Version ProgramVersion;

        public DefGetter() {
            var selfAssembly = GetType().Assembly;

            ProgramVersion = selfAssembly.GetName().Version;
        }
    }

    public static class Def {
        private static readonly DefGetter Getter = new DefGetter();

        public static string ProgramName => "eu4mmg";
        public static Version ProgramVersion => Getter.ProgramVersion;
        public static string Author => "Benjamin P.M. Lovegood (a.k.a. aarkegz)";
        public static string RepositoryAddress => "https://github.com/BenjaminPMLovegood/eu4mmg";
        public static string OpenSourceInfo => $"Open source under MIT License. See {RepositoryAddress}.";

        public const string ModFileDefaultExt = ".mod";
    }
}
