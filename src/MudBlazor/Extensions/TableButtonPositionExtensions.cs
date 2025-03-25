// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
public static class TableButtonPositionExtensions
{
    public static bool Editable(this TableContext? context, bool ignoreEditable) =>
        (context?.Table?.Editable ?? false) && !ignoreEditable;

    public static bool DisplayApplyButtonAtStart(this TableApplyButtonPosition position) =>
        position is TableApplyButtonPosition.Start or TableApplyButtonPosition.StartAndEnd;
    public static bool DisplayEditButtonAtStart(this TableEditButtonPosition position) =>
        position is TableEditButtonPosition.Start or TableEditButtonPosition.StartAndEnd;

    public static bool DisplayApplyButtonAtStart(this TableContext? context, bool ignoreEditable) =>
        context is not null && context.Table is not null && context.Editable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayApplyButtonAtStart();

    public static bool DisplayApplyButtonAtEnd(this TableApplyButtonPosition position) =>
        position is TableApplyButtonPosition.End or TableApplyButtonPosition.StartAndEnd;
    public static bool DisplayEditButtonAtEnd(this TableEditButtonPosition position) =>
        position is TableEditButtonPosition.End or TableEditButtonPosition.StartAndEnd;

    public static bool DisplayApplyButtonAtEnd(this TableContext? context, bool ignoreEditable) =>
        context is not null && context.Table is not null && context.Editable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayApplyButtonAtEnd();

    public static bool DisplayEditButtonAtStart(this TableContext? context, bool ignoreEditable) =>
        context is not null && context.Table is not null && context.Editable(ignoreEditable) && context.Table.EditButtonPosition.DisplayEditButtonAtStart() && context.Table.EditTrigger == TableEditTrigger.EditButton;

    public static bool DisplayEditButtonAtEnd(this TableContext? context, bool ignoreEditable) =>
        context is not null && context.Table is not null && context.Editable(ignoreEditable) && context.Table.EditButtonPosition.DisplayEditButtonAtEnd() && context.Table.EditTrigger == TableEditTrigger.EditButton;
}
