using System;
using System.IO;

namespace LibGit2Sharp
{
    public abstract class ArchiverBase : IDisposable
    {
        private Repository repo;
        private string targetPath;

        protected string TargetPath
        {
            get { return targetPath; }
        }

        protected Repository Repo
        {
            get { return repo; }
        }

        public virtual void Init(Repository repo, string targetPath)
        {
            this.repo = repo;
            this.targetPath = targetPath;

            InitializeArchive(targetPath);
        }

        protected abstract void InitializeArchive(string archivePath);

        public virtual void AddTree(Tree tree, string path = "")
        {
            foreach (TreeEntry entry in tree)
            {
                // TODO: submodules? symlinks?
                if (entry.Type == GitObjectType.Tree)
                {
                    AddTree((Tree)entry.Target, Path.Combine(path, entry.Name));
                    continue;
                }

                using (Stream contentStream = ((Blob)entry.Target).ContentStream)
                {
                    AddFileToArchive(Path.Combine(path, entry.Name), contentStream);
                }
            }
        }

        protected abstract void AddFileToArchive(string relativePath, Stream contentStream);

        #region Implementation of IDisposable

        public abstract void Dispose();

        #endregion
    }
}