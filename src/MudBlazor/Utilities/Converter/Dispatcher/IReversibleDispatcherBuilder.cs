// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Converter.Dispatcher;

public interface IReversibleDispatcherBuilder<TIn, TOut, out TConverter>
{
    IReversibleDispatcherBuilder<TIn, TOut, TConverter> Add<TSpecific>(IReversibleConverter<TSpecific, TOut> conv);

    //IReversibleDispatcherBuilder<TIn, TOut, TConverter> AddDynamic(DynamicFactory factory);

    TConverter Build();
}
