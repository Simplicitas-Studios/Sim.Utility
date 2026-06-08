using System;
using System.Collections.Generic;

namespace Sim.Dispositio.Shared
{
    public enum Nil
    {
        None
    }

    /// <example>
    ///     Usage
    ///     <code>
    ///         var someString = fun((string s) => s == "hello" ? Some(s) : None{string}());
    ///         var printString = fun((string s) => { WriteLine(s); return true; });
    ///
    ///         "hello"
    ///         .Then(someString)
    ///         .Then(Maybe.Map(printString));
    ///     </code>
    /// </example>
    /// <typeparam name="T"></typeparam>
    public readonly struct Maybe<T>
    {
        #region Public

        // ReSharper disable once UnusedParameter.Local
        public Maybe(Nil no = Nil.None)
        {
            Value = default;
            IsSome = false;
        }

        public Maybe(T value)
        {
            Value = value;
            IsSome = true;
        }

        public void Map(Action<T> k)
        {
            if (IsSome)
            {
                k(Value);
            }
        }

        public Maybe<TResult> Map<TResult>(Func<T, TResult> f)
        {
            if (IsSome)
            {
                return f(Value);
            }

            return Maybe.None<TResult>();
        }

        public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> f)
        {
            if (IsSome)
            {
                return f(Value);
            }

            return Maybe.None<TResult>();
        }

        public static implicit operator Maybe<T>(T v) => new Maybe<T>(v);

        public static implicit operator T(Maybe<T> v) =>
            v.IsSome ? v.Value : throw new Exception("Maybe cast exception");

        public static implicit operator Maybe<T>(Nil v) => new Maybe<T>(Nil.None);

        public override bool Equals(object obj)
        {
            return obj != null && this == (Maybe<T>)obj;
        }

        public override int GetHashCode()
        {
            if (IsSome)
            {
                return Value.GetHashCode();
            }

            throw new AccessViolationException("Cannot generate hash code from nil");
        }

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.IsSome && right.IsSome && EqualityComparer<T>.Default.Equals(left.Value, right.Value) ||
                left.IsNone && right.IsNone
                ;
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !(left == right);
        }

        public bool IsSome { get; }

        public bool IsNone => !IsSome;
        public T Value { get; }

        public T ValueOrDefault => IsSome ? Value : default;

        #endregion
    }

    public static class Maybe
    {
        #region Public

        public const Nil Nope = Nil.None;

        public static Maybe<T> Some<T>(T arg)
        {
            return arg.ToMaybe();
        }

        public static Maybe<T> None<T>()
        {
            return new Maybe<T>();
        }

        public static Action<Maybe<T>> Map<T>(Action<T> k)
        {
            return o =>
            {
                if (o.IsSome)
                {
                    k(o.Value);
                }
            };
        }

        public static Func<Maybe<T>, Maybe<TResult>> Map<T, TResult>(Func<T, TResult> k)
        {
            return o =>
            {
                if (o.IsSome)
                {
                    return new Maybe<TResult>(k(o.Value));
                }

                return new Maybe<TResult>(Nil.None);
            };
        }

        public static Func<Maybe<T>, Maybe<TResult>> Bind<T, TResult>(Func<T, Maybe<TResult>> k)
        {
            return o =>
            {
                if (o.IsSome)
                {
                    return k(o.Value);
                }

                return new Maybe<TResult>(Nil.None);
            };
        }

        #endregion
    }

    internal static class ValueBindingExtensions
    {
        #region Public

        public static bool IfTrue(this bool value, Func<bool> f)
        {
            if (value)
            {
                return f();
            }

            return false;
        }

        public static bool IfTrue<TArg0>(this bool value, Func<TArg0, bool> f, TArg0 arg0)
        {
            if (value)
            {
                return f(arg0);
            }

            return false;
        }

        public static bool IfTrue<TArg0, TArg1>(this bool value, Func<TArg0, TArg1, bool> f, TArg0 arg0, TArg1 arg1)
        {
            if (value)
            {
                return f(arg0, arg1);
            }

            return false;
        }

        public static bool IfTrue<TArg0, TArg1, TArg2>(this bool value, Func<TArg0, TArg1, TArg2, bool> f, TArg0 arg0,
            TArg1 arg1, TArg2 arg2)
        {
            if (value)
            {
                return f(arg0, arg1, arg2);
            }

            return false;
        }

        public static bool IfTrue<TArg0, TArg1, TArg2, TArg3>(this bool value, Func<TArg0, TArg1, TArg2, TArg3, bool> f,
            TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (value)
            {
                return f(arg0, arg1, arg2, arg3);
            }

            return false;
        }

        public static bool IfTrue<TArg0, TArg1, TArg2, TArg3, TArg4>(this bool value,
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, bool> f, TArg0 arg0, TArg1 arg1, TArg2 arg2,
            TArg3 arg3, TArg4 arg4)
        {
            if (value)
            {
                return f(arg0, arg1, arg2, arg3, arg4);
            }

            return false;
        }

        #endregion
    }
}
