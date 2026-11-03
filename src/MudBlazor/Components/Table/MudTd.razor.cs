using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// A cell within a <see cref="MudTr" />, <see cref="MudTHeadRow"/>, or <see cref="MudTFootRow"/> row component.
    /// </summary>
    public partial class MudTd : MudComponentBase, IDisposable
    {
        protected string Classname =>
            new CssBuilder("mud-table-cell")
                .AddClass(Context?.Table?.CellClass)
                .AddClass("mud-table-cell-hide", HideSmall)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The current state of the <see cref="MudTable{T}"/> containing this group.
        /// </summary>
        [CascadingParameter]
        public TableContext? Context { get; set; }

        /// <summary>
        /// The data row containing this cell, when it is a cell of a <see cref="MudTr"/>.
        /// </summary>
        [CascadingParameter]
        private MudTr? Row { get; set; }

        /// <summary>
        /// The row URL when this cell hosts the row link: the first cell of a linked row in the same table.
        /// </summary>
        private string? LinkHref
        {
            get
            {
                // A nested table's cells must not pick up the outer row's link
                if (Row is null || !ReferenceEquals(Row.Context?.Table, Context?.Table))
                {
                    return null;
                }

                var href = Row.Href;
                return href is not null && Row.ClaimLinkCell(this) ? href : null;
            }
        }

        private string? LinkRel => Row?.LinkTarget == "_blank" ? "noopener" : null;

        private readonly string _linkCellId = Identifier.Create("mud-table-cell");

        /// <summary>
        /// The id of this cell, which the row link uses as its accessible name.
        /// </summary>
        private string LinkLabelId => GetEffectiveElementId(_linkCellId);

        /// <summary>
        /// The content within this cell.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The label for this cell when the table is in small-device mode.
        /// </summary>
        [Parameter]
        public string? DataLabel { get; set; }

        /// <summary>
        /// Hides this cell if the breakpoint is smaller than <see cref="MudTableBase.Breakpoint"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool HideSmall { get; set; }

        /// <summary>
        /// Suppresses the browser's default context menu when a custom <c>@oncontextmenu</c> handler is supplied via <see cref="MudComponentBase.UserAttributes"/>.
        /// </summary>
        protected bool PreventContextMenuDefault => UserAttributes is not null && UserAttributes.ContainsKey("oncontextmenu");

        /// <summary>
        /// Releases resources used by this cell.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by this cell.
        /// </summary>
        /// <param name="disposing">When <c>true</c>, managed resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Row?.ReleaseLinkCell(this);
            }
        }
    }
}
