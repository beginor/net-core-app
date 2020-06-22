using System;
using System.IO;
using System.IO.Compression;

namespace GisHub.SlpkExtractor {

    class Program {

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Please provide a slpk file and a target directory to extract:");
                Console.WriteLine("Beginor.GisHub.SlpkExtractor path-to-my-model.slpk path-to-target-directory");
                return;
            }
            var slpkFile = args[0];
            var destDir = Directory.GetCurrentDirectory();
            if (args.Length > 1) {
                destDir = args[1];
            }
            Console.WriteLine($"Extract {args[0]} -> {args[1]}");
            if (!File.Exists(slpkFile)) {
                Console.WriteLine($"Slpk file {slpkFile} does not exists.");
                return;
            }
            if (!Directory.Exists(destDir)) {
                try {
                    Directory.CreateDirectory(destDir);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Can not create target directory {destDir}.");
                    Console.WriteLine(ex);
                    return;
                }
            }
            var slpkFileName = Path.GetFileNameWithoutExtension(slpkFile);
            var slpkDestDir = Path.Combine(destDir, slpkFileName);
            try {
                Directory.CreateDirectory(slpkDestDir);
            }
            catch (Exception ex) {
                Console.WriteLine($"Can not create slpkDestDir {slpkDestDir}.");
                Console.WriteLine(ex);
                return;
            }
            using var zipArchive = ZipFile.OpenRead(slpkFile);
            for (var i = 0; i < zipArchive.Entries.Count; i++) {
                var entry = zipArchive.Entries[i];
                var percent = i / (float)zipArchive.Entries.Count;
                var entryDest = Path.Combine(slpkDestDir, entry.FullName.Replace('\\', Path.DirectorySeparatorChar));
                Console.WriteLine($"{percent.ToString("p")}: {entry.Name} -> {entryDest}");
                try {
                    Directory.CreateDirectory(Path.GetDirectoryName(entryDest));
                    entry.ExtractToFile(entryDest, true);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            }
        }

    }

}
