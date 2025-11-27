namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
public interface IReversibleConverter<TIn, TOut> : IConverter<TIn, TOut>
{
    TIn ConvertBack(TOut output);
}
