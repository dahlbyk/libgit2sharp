using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   A TagAnnotation
    /// </summary>
    public class TagAnnotation : GitObject
    {
        private readonly GitObjectLazyGroup group;
        private readonly ILazy<GitObject> lazyTarget;
        private readonly ILazy<string> lazyName;
        private readonly ILazy<string> lazyMessage;
        private readonly ILazy<Signature> lazyTagger;

        /// <summary>
        ///   Needed for mocking purposes.
        /// </summary>
        protected TagAnnotation()
        { }

        internal TagAnnotation(Repository repo, ObjectId id)
            : base(id)
        {
            group = new GitObjectLazyGroup(repo, id);
            lazyName = group.AddLazy(0, Proxy.git_tag_name);
            lazyTagger = group.AddLazy(1, Proxy.git_tag_tagger);
            lazyMessage = group.AddLazy(1, Proxy.git_tag_message);
            lazyTarget = group.AddLazy(2, obj => repo.Lookup<GitObject>(Proxy.git_tag_target_oid(obj)));
        }

        /// <summary>
        ///   Gets the name of this tag.
        /// </summary>
        public virtual string Name { get { return lazyName.Value; } }

        /// <summary>
        ///   Gets the message of this tag.
        /// </summary>
        public virtual string Message { get { return lazyMessage.Value; } }

        /// <summary>
        ///   Gets the <see cref = "GitObject" /> that this tag annotation points to.
        /// </summary>
        public virtual GitObject Target { get { return lazyTarget.Value; } }

        /// <summary>
        ///   Gets the tagger.
        /// </summary>
        public virtual Signature Tagger { get { return lazyTagger.Value; } }
    }
}
