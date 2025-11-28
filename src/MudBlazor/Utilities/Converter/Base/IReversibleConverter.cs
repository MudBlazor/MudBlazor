namespace MudBlazor;

#nullable enable
public interface IReversibleConverter<TIn, TOut> : IConverter<TIn, TOut>
{
    TIn ConvertBack(TOut input);
}
