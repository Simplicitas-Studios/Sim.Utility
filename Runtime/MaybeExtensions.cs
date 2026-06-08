using System;

namespace Sim.Dispositio.Shared
{
    public static class MaybeExtensions
    {
        public static T OrElse<T>(this Maybe<T> maybe, T elseValue)
        {
            return maybe.IsSome ? maybe.Value : elseValue;
        }

        public static T OrElse<T>(this Maybe<T> maybe, Func<T> elseFactory)
        {
            return maybe.IsSome ? maybe.Value : elseFactory.Invoke();
        }

        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return new Maybe<T>(value);
        }
    }
}
