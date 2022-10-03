using System.Collections.Generic;

namespace MudBlazor
{
    public enum TableApplyButtonPosition
    {
        Start,
        End,
        StartAndEnd,
    }

    public static class TableApplyButtonPositionExtentions
    {
        public static bool IsEditable(this TableContext context, bool ignoreEditable) =>
            (context?.Table.IsEditable ?? false) && !ignoreEditable;

        public static bool DisplayApplyButtonAtStart(this TableApplyButtonPosition position) =>
            position is TableApplyButtonPosition.Start or TableApplyButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayApplyButtonAtStart();

        public static bool DisplayApplyButtonAtEnd(this TableApplyButtonPosition position) =>
            position is TableApplyButtonPosition.End or TableApplyButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayApplyButtonAtEnd();
    }
}
