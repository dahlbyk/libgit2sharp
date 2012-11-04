using System;
using System.IO;
using System.Text;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public class SelfCleaningDirectory
    {
        public SelfCleaningDirectory(IPostTestDirectoryRemover directoryRemover)
            : this(directoryRemover, BuildTempPath())
        {
        }

        public SelfCleaningDirectory(IPostTestDirectoryRemover directoryRemover, string path)
        {
            if (Directory.Exists(path))
            {
                throw new InvalidOperationException(string.Format("Directory '{0}' already exists.", path));
            }

            DirectoryPath = path;
            RootedDirectoryPath = Path.GetFullPath(path);

            directoryRemover.Register(DirectoryPath);
        }

        public string DirectoryPath { get; private set; }
        public string RootedDirectoryPath { get; private set; }

        protected static string BuildTempPath()
        {
            return Path.Combine(Constants.TemporaryReposPath, Guid.NewGuid().ToString().Substring(0, 8));
        }

        public void Touch(string file, string content = null)
        {
            TouchInternal(DirectoryPath, file, content);
        }

        protected void TouchInternal(string parent, string file, string content = null)
        {
            var lastIndex = file.LastIndexOf('/');
            if (lastIndex > 0)
            {
                var parents = file.Substring(0, lastIndex);
                Directory.CreateDirectory(Path.Combine(parent, parents));
            }

            var filePath = Path.Combine(parent, file);
            File.WriteAllText(filePath, content ?? "", Encoding.ASCII);
        }
    }
}
