namespace MudBlazor;

#nullable enable
public interface IConverter<in TIn, out TOut>
{
    TOut Convert(TIn input);
}
