using System.Collections.Generic;

namespace MudBlazor
{
    public enum TableCommitButtonPosition
    {
        Start,
        End,
        StartAndEnd,
    }

    public static class TableCommitButtonPositionExtentions
    {
        public static bool IsEditable(this TableContext context, bool ignoreEditable) =>
            (context?.Table.IsEditable ?? false) && !ignoreEditable;

        public static bool DisplayCommitButtonAtStart(this TableCommitButtonPosition position) =>
            position is TableCommitButtonPosition.Start or TableCommitButtonPosition.StartAndEnd;

        public static bool DisplayCommitButtonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.CommitButtonPosition.DisplayCommitButtonAtStart();

        public static bool DisplayCommitButtonAtEnd(this TableCommitButtonPosition position) =>
            position is TableCommitButtonPosition.End or TableCommitButtonPosition.StartAndEnd;

        public static bool DisplayCommitButtonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.CommitButtonPosition.DisplayCommitButtonAtEnd();
    }
}
