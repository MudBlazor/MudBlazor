namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
public interface IConverter<in TIn, out TOut>
{
    TOut Convert(TIn input);
}
