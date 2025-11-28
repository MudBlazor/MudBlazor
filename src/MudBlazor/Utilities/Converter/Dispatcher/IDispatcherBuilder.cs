// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace MudBlazor.Utilities.Converter.Dispatcher;

public interface IDispatcherBuilder<TIn, in TOut, out TConverter>
{
    IDispatcherBuilder<TIn, TOut, TConverter> Add<TSpecific>(IConverter<TSpecific, TOut> conv);

    //IDispatcherBuilder<TIn, TOut, TConverter> AddDynamic(DynamicFactory factory);

    TConverter Build();
}
