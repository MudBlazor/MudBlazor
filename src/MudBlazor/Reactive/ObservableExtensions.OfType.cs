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
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IObservable<T> OfType<T>(this IObservable<object> source)
            => new OfTypeSubject<T>(source ?? throw new ArgumentNullException(nameof(source)));

        private class OfTypeSubject<T> : Subject<T>
        {
            private IDisposable? _subscription;

            public OfTypeSubject(IObservable<object> source)
            {
                _subscription = source.Subscribe(
                    next =>
                    {
                        if (next is T result)
                            OnNext(result);
                    },
                    OnError,
                    OnCompleted);
            }

            public override void Dispose()
            {
                base.Dispose();
                _subscription?.Dispose();
                _subscription = null;
            }
        }
    }
}
