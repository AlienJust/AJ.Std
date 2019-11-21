using System;
using AlienJust.Support.Conversion.Contracts;

namespace AlienJust.Support.Conversion
{
    public sealed class BuilderOneToOneFunc<TRaw, TResult> : IBuilderOneToOne<TRaw, TResult>
    {
        private readonly Func<TRaw, TResult> _func;

        public BuilderOneToOneFunc(Func<TRaw, TResult> func)
        {
            _func = func;
        }

        public TResult Build(TRaw source)
        {
            return _func(source);
        }
    }
}