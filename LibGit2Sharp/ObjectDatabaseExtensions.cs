using System;
using System.IO;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Provides helper overloads to a <see cref = "ObjectDatabase" />.
    /// </summary>
    public static class ObjectDatabaseExtensions
    {
        /// <summary>
        ///   Create a TAR archive of the given tree.
        /// </summary>
        /// <param name="odb">The object database.</param>
        /// <param name="tree">The tree.</param>
        /// <param name="archivePath">The archive path.</param>
        public static void Archive(this ObjectDatabase odb, Tree tree, string archivePath)
        {
            using (var archiver = new TarArchiver(archivePath))
            {
                odb.Archive(tree, archiver.AddFileToArchive);
            }
        }

        private class TarArchiver : IDisposable
        {
            private readonly string outputFile;

            public TarArchiver(string outputFile)
            {
                this.outputFile = outputFile;

                Console.WriteLine("Archiving to {0}...", outputFile);
            }

            public void AddFileToArchive(string relativepath, Stream contentstream)
            {
                Console.WriteLine("  Adding {0} to {1}.", relativepath, outputFile);
            }

            public void Dispose()
            {
                Console.WriteLine("Archiving complete.");
            }
        }
    }
}