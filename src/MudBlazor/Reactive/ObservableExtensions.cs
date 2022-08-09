using System.Diagnostics.CodeAnalysis;
using System.Reactive;

#nullable enable
// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Provides a set of static methods for subscribing delegates to observables.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static partial class ObservableExtensions
    {
        /// <summary>
        /// Subscribes to the observable sequence without specifying any handlers.
        /// This method can be used to evaluate the observable sequence for its side-effects only.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this IObservable<T> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.Subscribe(new AnonymousObserver<T>(Stubs<T>.Ignore, Stubs.Throw, Stubs.Nop));
        }

        /// <summary>
        /// Subscribes to the observable providing just the <paramref name="onNext"/> delegate.
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(onNext);

            return source.Subscribe(new AnonymousObserver<T>(onNext, Stubs.Throw, Stubs.Nop));
        }

        /// <summary>
        /// Subscribes to the observable providing both the <paramref name="onNext"/> and 
        /// <paramref name="onError"/> delegates.
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(onNext);
            ArgumentNullException.ThrowIfNull(onError);

            return source.Subscribe(new AnonymousObserver<T>(onNext, onError, Stubs.Nop));
        }

        /// <summary>
        /// Subscribes to the observable providing both the <paramref name="onNext"/> and 
        /// <paramref name="onCompleted"/> delegates.
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(onNext);
            ArgumentNullException.ThrowIfNull(onCompleted);

            return source.Subscribe(new AnonymousObserver<T>(onNext, Stubs.Throw, onCompleted));
        }

        /// <summary>
        /// Subscribes to the observable providing all three <paramref name="onNext"/>, 
        /// <paramref name="onError"/> and <paramref name="onCompleted"/> delegates.
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(onNext);
            ArgumentNullException.ThrowIfNull(onError);
            ArgumentNullException.ThrowIfNull(onCompleted);

            return source.Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));
        }
    }
}
