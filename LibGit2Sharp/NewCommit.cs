using System.Collections;
using System.Collections.Generic;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Compat;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    internal class ParentsList : IList<NewCommit>
    {
        private readonly Lazy<IList<NewCommit>> _parents;
        private readonly Lazy<int> _count;

        public ParentsList(Repository repo, NewCommit c)
        {
            _count = new Lazy<int>(() => Proxy.git_commit_parentcount(repo.Handle, c.Id));
            _parents = new Lazy<IList<NewCommit>>(() => RetrieveParentsOfCommit(repo, c.Id, _count));
        }

        private IList<NewCommit> RetrieveParentsOfCommit(Repository repo, ObjectId oid, Lazy<int> pCount)
        {
            var parents = new List<NewCommit>();

            using (var obj = new ObjectSafeWrapper(oid, repo.Handle))
            {
                int parentsCount = pCount.Value;

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
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(NewCommit item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(NewCommit item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(NewCommit[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(NewCommit item)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get { return _count.Value; }
        }

        public bool IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        public int IndexOf(NewCommit item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, NewCommit item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public NewCommit this[int index]
        {
            get { return _parents.Value[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }

    public class NewCommit : GitObject
    {
        private readonly Repository repo;

        private readonly LazyGroup group1;
        private readonly LazyGroup group2;

        private readonly IList<NewCommit> parents;
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
            group1 = new LazyGroup(repo, id);
            group2 = new LazyGroup(repo, id);

            _lazyTreeId = group1.AddLazy<ObjectId>(Proxy.git_commit_tree_oid);
            _lazyAuthor = group1.AddLazy<Signature>(Proxy.git_commit_author);
            _lazyMessage = group1.AddLazy<string>(Proxy.git_commit_message);

            _lazyEncoding = group2.AddLazy<string>(RetrieveEncodingOf);
            _lazyCommitter = group2.AddLazy<Signature>(Proxy.git_commit_committer);

            parents = new ParentsList(repo, this);
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
        public int ParentsCount { get { return Proxy.git_commit_parentcount(repo.Handle, Id); } }

        public Tree Tree { get { return repo.Lookup<Tree>(TreeId); } }

        private static string RetrieveEncodingOf(GitObjectSafeHandle obj)
        {
            string encoding = Proxy.git_commit_message_encoding(obj);

            return encoding ?? "UTF-8";
        }


    }
}
