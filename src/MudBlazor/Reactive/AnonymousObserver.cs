using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

// ReSharper disable CheckNamespace
namespace System.Reactive
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Create an <see cref="IObserver{T}"/> instance from delegate-based implementations 
    /// of the On* methods.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    [ExcludeFromCodeCoverage]
    internal class AnonymousObserver<T> : IObserver<T>
    {
        // ReSharper disable StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Action<Exception> rethrow = e => ExceptionDispatchInfo.Capture(e).Throw();
        // ReSharper disable once InconsistentNaming
        private static readonly Action nop = () => { };
        // ReSharper restore StaticMemberInGenericType

        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        /// <summary>
        /// Creates the observable providing just the <paramref name="onNext"/> action.
        /// </summary>
        public AnonymousObserver(Action<T> onNext)
            : this(onNext, rethrow, nop) { }

        /// <summary>
        /// Creates the observable providing both the <paramref name="onNext"/> and 
        /// <paramref name="onError"/> actions.
        /// </summary>
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError)
            : this(onNext, onError, nop) { }

        /// <summary>
        /// Creates the observable providing both the <paramref name="onNext"/> and 
        /// <paramref name="onCompleted"/> actions.
        /// </summary>
        public AnonymousObserver(Action<T> onNext, Action onCompleted)
            : this(onNext, rethrow, onCompleted) { }

        /// <summary>
        /// Creates the observable providing all three <paramref name="onNext"/>, 
        /// <paramref name="onError"/> and <paramref name="onCompleted"/> actions.
        /// </summary>
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        /// <summary>
        /// Calls the action implementing <see cref="IObserver{T}.OnCompleted()"/>.
        /// </summary>
        public void OnCompleted() => _onCompleted();

        /// <summary>
        /// Calls the action implementing <see cref="IObserver{T}.OnError(Exception)"/>.
        /// </summary>
        public void OnError(Exception error) => _onError(error);

        /// <summary>
        /// Calls the action implementing <see cref="IObserver{T}.OnNext(T)"/>.
        /// </summary>
        public void OnNext(T value) => _onNext(value);
    }
}
