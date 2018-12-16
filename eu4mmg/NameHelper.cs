using System.Linq;
using System.Text;

namespace eu4mmg {
    internal static class NameHelper {
        public static bool Asciify(string source, out string asciified) {
            if (source.All(x => 31 < x && x < 128)) {
                asciified = source;
                return false;
            } else {
                var sb = new StringBuilder();

                foreach (var x in source) {
                    if (31 < x && x < 128) sb.Append(x);
                    else sb.Append($"\\u{(int)x:x4}");
                }

                asciified = sb.ToString();
                return true;
            }
        }

        public static bool DoAsciify(ref string source, string nameInHint) {
            if (Asciify(source, out var sourceAsciified)) {
                Program.IssueWarn($"Invalid {nameInHint}: \"{source}\", changed to \"{sourceAsciified}\"");
                source = sourceAsciified;
                return true;
            }

            return false;
        }

        // idify? make a string to a c-style identifier (contains letters, numbers and underlines only)
        public static bool Idify(string source, out string idified) {
            if (source.All(x => ('a' <= x && x < 'z') || ('A' <= x && x < 'Z') || ('0' <= x && x < '9') || (x == '_'))) {
                idified = source;
                return false;
            } else {
                var sb = new StringBuilder();

                foreach (var x in source) {
                    if (('a' <= x && x < 'z') || ('A' <= x && x < 'Z') || ('0' <= x && x < '9') || (x == '_')) sb.Append(x);
                    else if (x < 128) sb.Append('_');
                    else sb.Append($"_u{(int)x:x4}");
                }

                idified = sb.ToString();
                return true;
            }
        }

        public static bool DoIdify(ref string source, string nameInHint) {
            if (Idify(source, out var sourceAsciified)) {
                Program.IssueWarn($"Invalid {nameInHint}: \"{source}\", changed to \"{sourceAsciified}\"");
                source = sourceAsciified;
                return true;
            }

            return false;
        }
    }
}