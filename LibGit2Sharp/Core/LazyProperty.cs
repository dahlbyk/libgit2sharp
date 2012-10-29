using System;
using System.Collections.Generic;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp.Core
{
    internal class LazyProperty<TType> : IEvaluator<GitObjectSafeHandle>
    {
        private readonly Func<GitObjectSafeHandle, TType> evaluator;
        private readonly GitObjectLazyGroup lazyGroup;

        private TType value;
        private bool hasBeenEvaluated;

        public LazyProperty(Func<GitObjectSafeHandle, TType> evaluator, GitObjectLazyGroup lazyGroup)
        {
            this.evaluator = evaluator;
            this.lazyGroup = lazyGroup;
        }

        public TType Value
        {
            get { return Evaluate(); }
        }

        private TType Evaluate()
        {
            if (!hasBeenEvaluated)
            {
                lock (lazyGroup)
                {
                    if (!hasBeenEvaluated)
                    {
                        lazyGroup.TriggerEvaluation();
                    }
                }
            }

            return value;
        }

        void IEvaluator<GitObjectSafeHandle>.Evaluate(GitObjectSafeHandle objectPtr)
        {
            hasBeenEvaluated = true;
            value = evaluator(objectPtr);
        }
    }

    internal interface IEvaluator<T>
    {
        void Evaluate(T input);
    }

    internal class GitObjectLazyGroup
    {
        private readonly Repository repo;
        private readonly ObjectId id;

        private readonly IList<IEvaluator<GitObjectSafeHandle>> lazies = new List<IEvaluator<GitObjectSafeHandle>>();

        public GitObjectLazyGroup(Repository repo, ObjectId id)
        {
            this.repo = repo;
            this.id = id;
        }

        public LazyProperty<TType> AddLazy<TType>(Func<GitObjectSafeHandle, TType> evaluator)
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
                    lazy.Evaluate(osw.ObjectPtr);
                }
            }
        }
    }
}