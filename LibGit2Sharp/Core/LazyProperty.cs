using System;
using System.Collections.Generic;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp.Core
{
    internal class LazyProperty<TType> : LazyPropertyBase
    {
        public LazyProperty(Func<GitObjectSafeHandle, object> evaluator, LazyGroup lazyGroup)
            : base(evaluator, lazyGroup)
        { }

        public TType Value
        {
            get { return (TType)base.Value; }
        }
    }

    internal class LazyPropertyBase
    {
        private readonly Func<GitObjectSafeHandle, object> evaluator;
        private readonly LazyGroup lazyGroup;

        private object value;
        private bool hasBeenEvaluated;

        public LazyPropertyBase(Func<GitObjectSafeHandle, object> evaluator, LazyGroup lazyGroup)
        {
            this.evaluator = evaluator;
            this.lazyGroup = lazyGroup;
        }

        public object Value
        {
            get { return Evaluate(); }
        }

        internal LazyGroup Group
        {
            get { return lazyGroup; }
        }

        private object Evaluate()
        {
            if (!hasBeenEvaluated)
            {
                lock (Group)
                {
                    if (!hasBeenEvaluated)
                    {
                        Group.TriggerEvaluation();
                    }
                }
            }

            return value;
        }

        internal void InternalEvaluate(GitObjectSafeHandle objectPtr)
        {
            hasBeenEvaluated = true;
            value = evaluator(objectPtr);
        }
    }

    internal class LazyGroup
    {
        private readonly Repository repo;
        private readonly ObjectId id;

        private readonly IList<LazyPropertyBase> lazies = new List<LazyPropertyBase>();

        public LazyGroup(Repository repo, ObjectId id)
        {
            this.repo = repo;
            this.id = id;
        }

        public LazyProperty<TType> AddLazy<TType>(Func<GitObjectSafeHandle, object> evaluator)
        {
            var lazy = new LazyProperty<TType>(evaluator, this);
            lazies.Add(lazy);
            return lazy;
        }

        public void TriggerEvaluation()
        {
            using (var osw = new ObjectSafeWrapper(id, repo.Handle))
            {
                foreach (var lazy in lazies)
                {
                    lazy.InternalEvaluate(osw.ObjectPtr);
                }
            }
        }
    }
}