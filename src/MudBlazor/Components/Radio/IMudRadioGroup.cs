namespace MudBlazor
{
#nullable enable
    internal interface IMudRadioGroup
    {
        //This interface need to throw exception properly.
        void CheckGenericTypeMatch(object selectItem);
    }
}
