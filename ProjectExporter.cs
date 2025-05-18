using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rogue
{
    /// <summary>
    /// Walks a C# solution / project folder and produces a single
    /// TXT file with:
    ///   • A “Project tree:” section that uses -> for folders and -- for files
    ///   • Then the complete source of every *.cs file,
    ///     each preceded by "===‹ClassName›==="
    /// </summary>
    public static class ProjectExporter
    {
        /// <param name="rootPath">Absolute or relative path of the project folder.</param>
        /// <param name="outputPath">Path (including file name) of the .txt snapshot.</param>
        /// <param name="includeHidden">Set true if you also want to include hidden/.git folders.</param>
        public static void CreateSnapshot(
            string rootPath,
            string outputPath = "ProjectSnapshot.txt",
            bool includeHidden = false)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Folder not found: {rootPath}");

            var allCsFiles = new List<string>();
            var sb = new StringBuilder();

            sb.AppendLine("Project tree:");

            // 1️⃣  build the folder/file tree
            BuildTree(rootPath, depth: 1, includeHidden, sb, allCsFiles);

            // 2️⃣  dump every file’s code
            foreach (var file in allCsFiles.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                sb.AppendLine();
                sb.AppendLine($"==={Path.GetFileNameWithoutExtension(file)}===");
                sb.AppendLine(File.ReadAllText(file));
            }

            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            Console.WriteLine($"Snapshot written to \"{Path.GetFullPath(outputPath)}\"");
        }

        // ---------- helpers ----------
        private static void BuildTree(string folder,
                                      int depth,
                                      bool includeHidden,
                                      StringBuilder sb,
                                      IList<string> collectedFiles)
        {
            // write the folder line (skip the root itself)
            if (depth > 1)
            {
                string folderPrefix = new string('-', depth - 1) + ">";
                sb.AppendLine($"{folderPrefix}{Path.GetFileName(folder)}:");
            }

            // sub-folders first
            foreach (var subDir in Directory.GetDirectories(folder)
                                            .Where(d => includeHidden || !IsHidden(d))
                                            .OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                BuildTree(subDir, depth + 1, includeHidden, sb, collectedFiles);
            }

            // files in this folder
            foreach (var file in Directory.GetFiles(folder, "*.cs")
                                          .Where(f => includeHidden || !IsHidden(f))
                                          .OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                string filePrefix = new string('-', depth) + Path.GetFileName(file);
                sb.AppendLine(filePrefix);
                collectedFiles.Add(file);
            }
        }

        private static bool IsHidden(string path)
            => (File.GetAttributes(path) & FileAttributes.Hidden) != 0;
    }
}
