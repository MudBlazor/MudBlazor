using System.Reactive.Subjects;

#nullable enable
// ReSharper disable CheckNamespace
namespace System.Reactive.Linq
// ReSharper restore CheckNamespace
{
    internal static partial class ObservableExtensions
    {
        /// <summary>
        /// Filters the elements of an observable sequence based on the specified type.
        /// </summary>
        /// <typeparam name="T">The type to filter the elements in the source sequence on.</typeparam>
        /// <param name="source">The sequence that contains the elements to be filtered.</param>
        /// <param name="predicate">Predicate to apply to elements in <paramref name="source"/> for filtering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, bool> predicate)
            => new WhereSubject<T>(source ?? throw new ArgumentNullException(nameof(source)), predicate ?? throw new ArgumentNullException(nameof(predicate)));

        private class WhereSubject<T> : Subject<T>
        {
            private IDisposable? _subscription;
            private Func<T, bool>? _predicate;

            public WhereSubject(IObservable<T> source, Func<T, bool> predicate)
            {
                _predicate = predicate;
                _subscription = source.Subscribe(
                    next =>
                    {
                        if (_predicate(next))
                            OnNext(next);
                    },
                    OnError,
                    OnCompleted);
            }

            public override void Dispose()
            {
                base.Dispose();
                _subscription?.Dispose();
                _subscription = null;
                _predicate = null;
            }
        }
    }
}
