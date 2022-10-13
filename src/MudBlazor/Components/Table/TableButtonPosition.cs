using System.Collections.Generic;

namespace MudBlazor
{
    //public enum TableButtonPosition
    //{
    //    Start,
    //    End,
    //    StartAndEnd,
    //}

    public enum TableEditButtonPosition
    {
        Start,
        End,
        StartAndEnd,
    }

    public enum TableApplyButtonPosition
    {
        Start,
        End,
        StartAndEnd,
    }

    public static class TableButtonPositionExtentions
    {
        public static bool IsEditable(this TableContext context, bool ignoreEditable) =>
            (context?.Table.IsEditable ?? false) && !ignoreEditable;

        public static bool DisplayApplyButtonAtStart(this TableApplyButtonPosition position) =>
            position is TableApplyButtonPosition.Start or TableApplyButtonPosition.StartAndEnd;
        public static bool DisplayEditButtonAtStart(this TableEditButtonPosition position) =>
            position is TableEditButtonPosition.Start or TableEditButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayApplyButtonAtStart();

        public static bool DisplayApplyButtonAtEnd(this TableApplyButtonPosition position) =>
            position is TableApplyButtonPosition.End or TableApplyButtonPosition.StartAndEnd;
        public static bool DisplayEditButtonAtEnd(this TableEditButtonPosition position) =>
            position is TableEditButtonPosition.End or TableEditButtonPosition.StartAndEnd;

        public static bool DisplayApplyButtonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.ApplyButtonPosition.DisplayApplyButtonAtEnd();

        public static bool DisplayEditbuttonAtStart(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.EditButtonPosition.DisplayEditButtonAtStart() && context.Table.EditTrigger == TableEditTrigger.EditButton;

        public static bool DisplayEditbuttonAtEnd(this TableContext context, bool ignoreEditable) =>
            context.IsEditable(ignoreEditable) && context.Table.EditButtonPosition.DisplayEditButtonAtEnd() && context.Table.EditTrigger == TableEditTrigger.EditButton;
    }
}
