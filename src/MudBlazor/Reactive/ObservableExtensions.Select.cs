using System.Reactive.Subjects;

#nullable enable
// ReSharper disable CheckNamespace
namespace System.Reactive.Linq
// ReSharper restore CheckNamespace
{
    internal static partial class ObservableExtensions
    {
        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the result sequence, obtained by running the selector function for each element in the source sequence.</typeparam>
        /// <param name="source">A sequence of elements to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element.</param>
        /// <returns>An sequence whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static IObservable<TResult> Select<TSource, TResult>(this IObservable<TSource> source, Func<TSource, TResult> selector)
            => new SelectorSubject<TSource, TResult>(source ?? throw new ArgumentNullException(nameof(source)), selector ?? throw new ArgumentNullException(nameof(selector)));

        private class SelectorSubject<TSource, TResult> : Subject<TResult>
        {
            private IDisposable? _subscription;
            private Func<TSource, TResult>? _selector;

            public SelectorSubject(IObservable<TSource> source, Func<TSource, TResult> selector)
            {
                _selector = selector;
                _subscription = source.Subscribe(
                    next => OnNext(_selector(next)),
                    OnError,
                    OnCompleted);
            }

            public override void Dispose()
            {
                base.Dispose();
                _subscription?.Dispose();
                _subscription = null;
                _selector = null;
            }
        }
    }
}
