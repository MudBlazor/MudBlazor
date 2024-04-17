using System.Collections.Generic;

namespace MudBlazor
{
    public enum TableButtonPosition
    {
        Start,
        End,
        StartAndEnd,
    }

    public static class TableButtonPositionExtentions
    {
        public static bool IsEditable(this TableContext context, bool ignoreEditable) =>
            (context?.Table.IsEditable ?? false) && !ignoreEditable;

        public static bool DisplayTableButtonAtStart(this TableButtonPosition position) =>
            position is TableButtonPosition.Start or TableButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayTableButtonAtStart();

        public static bool DisplayTableButtonAtEnd(this TableButtonPosition position) =>
            position is TableButtonPosition.End or TableButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayTableButtonAtEnd();

        public static bool DisplayEditButtonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.EditButtonPosition.DisplayTableButtonAtStart() && context.Table.EditTrigger == TableEditTrigger.EditButton;

        public static bool DisplayEditButtonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.EditButtonPosition.DisplayTableButtonAtEnd() && context.Table.EditTrigger == TableEditTrigger.EditButton;
    }
}
