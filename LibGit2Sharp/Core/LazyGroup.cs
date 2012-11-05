using System;
using System.Collections.Generic;

namespace LibGit2Sharp.Core
{
    internal abstract class LazyGroup<T>
    {
        private readonly IDictionary<uint, IList<IEvaluator<T>>> evaluators = new Dictionary<uint, IList<IEvaluator<T>>>();
        private readonly object @lock = new object();
        private readonly IDictionary<uint, bool> evaluated = new Dictionary<uint, bool>();

        public ILazy<TResult> AddLazy<TResult>(uint level, Func<T, TResult> func)
        {
            var prop = new Dependent<T, TResult>(level, func, this);

            if (!evaluated.ContainsKey(level))
            {
                evaluated.Add(level, false);
                evaluators.Add(level, new List<IEvaluator<T>>());
            }

            evaluators[level].Add(prop);
            return prop;
        }

        public void Evaluate(uint level)
        {
            if (evaluated[level])
                return;

            lock (@lock)
            {
                if (evaluated[level])
                    return;

                EvaluateInternal(input =>
                                 {
                                     foreach (var e in evaluators[level])
                                         e.Evaluate(input);
                                 });
                evaluated[level] = true;
            }
        }

        protected abstract void EvaluateInternal(Action<T> evaluator);

        private interface IEvaluator<TInput>
        {
            void Evaluate(TInput input);
        }

        private class Dependent<TInput, TOutput> : ILazy<TOutput>, IEvaluator<TInput>
        {
            private readonly Func<TInput, TOutput> valueFactory;
            private readonly LazyGroup<TInput> lazyGroup;
            private readonly uint level;

            private TOutput value;
            private bool hasBeenEvaluated;

            public Dependent(uint level, Func<TInput, TOutput> valueFactory, LazyGroup<TInput> lazyGroup)
            {
                this.valueFactory = valueFactory;
                this.lazyGroup = lazyGroup;
                this.level = level;
            }

            public TOutput Value
            {
                get { return Evaluate(); }
            }

            private TOutput Evaluate()
            {
                if (!hasBeenEvaluated)
                {
                    lazyGroup.Evaluate(level);
                }

                return value;
            }

            void IEvaluator<TInput>.Evaluate(TInput input)
            {
                value = valueFactory(input);
                hasBeenEvaluated = true;
            }
        }
    }
}
