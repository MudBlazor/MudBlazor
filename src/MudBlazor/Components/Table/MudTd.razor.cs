using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A cell within a <see cref="MudTr" />, <see cref="MudTHeadRow"/>, or <see cref="MudTFootRow"/> row component.
    /// </summary>
    public partial class MudTd : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-table-cell")
                .AddClass("mud-table-cell-hide", HideSmall)
                .AddClass(Class)
                .Build();

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
    }
}
