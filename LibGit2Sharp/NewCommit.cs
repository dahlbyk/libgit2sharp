using System.Collections;
using System.Collections.Generic;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Compat;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    internal class ParentsList : IEnumerable<NewCommit>
    {
        private readonly Lazy<IList<NewCommit>> _parents;
        private readonly Lazy<int> _count;

        public ParentsList(Repository repo, ObjectId id)
        {
            _count = new Lazy<int>(() => Proxy.git_commit_parentcount(repo.Handle, id));
            _parents = new Lazy<IList<NewCommit>>(() => RetrieveParentsOfCommit(repo, id));
        }

        private IList<NewCommit> RetrieveParentsOfCommit(Repository repo, ObjectId oid)
        {
            var parents = new List<NewCommit>();

            using (var obj = new ObjectSafeWrapper(oid, repo.Handle))
            {
                int parentsCount = _count.Value;

                for (uint i = 0; i < parentsCount; i++)
                {
                    ObjectId parentCommitId = Proxy.git_commit_parent_oid(obj.ObjectPtr, i);
                    parents.Add(new NewCommit(repo, parentCommitId));
                }
            }

            return parents;
        }

        public IEnumerator<NewCommit> GetEnumerator()
        {
            return _parents.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _parents.Value.Count; }
        }
    }

    public class NewCommit : GitObject
    {
        private readonly Repository repo;

        private readonly GitObjectLazyGroup group1;
        private readonly GitObjectLazyGroup group2;

        private readonly ParentsList parents;
        private readonly LazyProperty<string> _lazyMessage;
        private readonly LazyProperty<string> _lazyEncoding;
        private readonly LazyProperty<Signature> _lazyAuthor;
        private readonly LazyProperty<Signature> _lazyCommitter;
        private readonly LazyProperty<ObjectId> _lazyTreeId;

        protected NewCommit()
        {}

        public NewCommit(Repository repo, ObjectId id)
            : base(id)
        {
            this.repo = repo;
            group1 = new GitObjectLazyGroup(repo, id);
            group2 = new GitObjectLazyGroup(repo, id);

            _lazyTreeId = group1.AddLazy<ObjectId>(Proxy.git_commit_tree_oid);
            _lazyAuthor = group1.AddLazy<Signature>(Proxy.git_commit_author);
            _lazyMessage = group1.AddLazy<string>(Proxy.git_commit_message);

            _lazyEncoding = group2.AddLazy<string>(RetrieveEncodingOf);
            _lazyCommitter = group2.AddLazy<Signature>(Proxy.git_commit_committer);

            parents = new ParentsList(repo, id);
        }

        // Lazy batch loaded properies
        private ObjectId TreeId { get { return _lazyTreeId.Value; } }
        public Signature Author { get { return _lazyAuthor.Value; } }
        public string Message { get { return _lazyMessage.Value; } }

        public string Encoding { get { return _lazyEncoding.Value; } }
        public Signature Committer { get { return _lazyCommitter.Value; } }

        // On demand lazy loaded properties
        public IEnumerable<NewCommit> Parents { get { return parents; } }

        // Other properties
        public int ParentsCount { get { return parents.Count; } }

        public Tree Tree { get { return repo.Lookup<Tree>(TreeId); } }

        private static string RetrieveEncodingOf(GitObjectSafeHandle obj)
        {
            string encoding = Proxy.git_commit_message_encoding(obj);

            return encoding ?? "UTF-8";
        }


    }
}
