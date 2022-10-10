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

        public static bool DisplayButtonAtStart(this TableButtonPosition position) =>
            position is TableButtonPosition.Start or TableButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayButtonAtStart();

        public static bool DisplayButtonAtEnd(this TableButtonPosition position) =>
            position is TableButtonPosition.End or TableButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayButtonAtEnd();

        public static bool DisplayEditbuttonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.EditButtonPosition.DisplayButtonAtStart() && context.Table.EditTrigger == TableEditTrigger.Manual;

        public static bool DisplayEditbuttonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.EditButtonPosition.DisplayButtonAtEnd() && context.Table.EditTrigger == TableEditTrigger.Manual;
    }
}
