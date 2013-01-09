using System.Collections;
using System.Collections.Generic;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    /// The commit IDs that are currently being merged into a repository.
    /// </summary>
    public class MergeHeadCollection : IEnumerable<MergeHead>
    {
        internal readonly Repository repo;

        /// <summary>
        ///   Needed for mocking purposes.
        /// </summary>
        protected MergeHeadCollection()
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MergeHeadCollection" /> class.
        /// </summary>
        /// <param name = "repo">The repo.</param>
        internal MergeHeadCollection(Repository repo)
        {
            this.repo = repo;
        }

        #region IEnumerable<Commit> Members

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref = "IEnumerator{T}" /> object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<MergeHead> GetEnumerator()
        {
            int i = 0;
            return Proxy.git_repository_mergehead_foreach(repo.Handle,
                commitId => new MergeHead(repo, new ObjectId(commitId), i++))
                .GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref = "IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}