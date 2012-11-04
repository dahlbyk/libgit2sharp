using System.IO;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Provides information about the repository's interactive state.
    /// </summary>
    public class InteractiveState
    {
        private readonly Repository repo;
        private readonly string path;

        protected InteractiveState()
        {
        }

        internal InteractiveState(Repository repo)
        {
            this.repo = repo;
            path = repo.Info.Path;
        }

        /// <summary>
        ///   The name of HEAD when the pending operation began, or the current HEAD.
        /// </summary>
        public virtual string HeadName
        {
            get
            {
                if (!repo.Info.IsHeadDetached)
                    return repo.Head.Name;

                if (Exists("rebase-merge/head-name"))
                    return File.ReadAllText(Path.Combine(path, "rebase-merge/head-name")).Replace("refs/heads/", "");

                var tip = repo.Head.Tip;
                var detachedName = tip == null ? "unknown" : tip.Sha.Substring(0, 7) + "...";
                return "(" + detachedName + ")";
            }
        }

        /// <summary>
        ///   The pending interactive operation.
        /// </summary>
        public virtual RepositoryState PendingOperation
        {
            get { return Proxy.git_repository_state(repo.Handle); }
        }

        private bool Exists(string relativePath)
        {
            return File.Exists(Path.Combine(path, relativePath));
        }
    }
}