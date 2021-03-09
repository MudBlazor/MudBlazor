using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudEnchancedChart : MudComponentBase
    {
        [Parameter] public RenderFragment Chart { get; set; }

        #region Legend

        [Parameter] public Position LegendPosition { get; set; } = Position.Right;
        [Parameter] public Align LegendAlignment { get; set; } = Align.Center;
        [Parameter] public Boolean ShowLegend { get; set; } = true;

        #endregion

        #region Title

        [Parameter] public Position TitlePosition { get; set; } = Position.Top;
        [Parameter] public Align TitleAlignment { get; set; } = Align.Center;
        [Parameter] public Boolean ShowTitle { get; set; } = true;
        [Parameter] public String Title { get; set; } = String.Empty;
        [Parameter] public RenderFragment<String> TitleDrawer { get; set; } = DefaultTitleDrawerFragment;

        private static RenderFragment DefaultTitleDrawerFragment(String context)
        {
            return builder =>
            {
                builder.OpenComponent(0, typeof(MudText));
                builder.AddAttribute(1, nameof(MudText.Typo), Typo.h4);
                builder.AddAttribute(2, nameof(MudText.ChildContent), (RenderFragment)((builder2) => {
                    builder2.AddContent(3, context);
                }));
                builder.CloseElement();
            };
        }

        #endregion

        #region View helper

        protected string Classname =>
        new CssBuilder("mud-chart")
          .AddClass(Class)
          .AddClass("d-flex")
          .AddClass(GetFlexDirectionForChartContainer())
        .Build();

        public String GetFlexDirectionForChartContainer() => TitlePosition switch
        {
            Position.Bottom or Position.Top => "flex-column",
            Position.Left or Position.Right => "flex-row",
            _ => String.Empty,
        };

        public String GetFlexOrderForTitleContainer() => TitlePosition switch
        {
            Position.Bottom or Position.Right => "order-last",
            Position.Left or Position.Top => "order-first",
            _ => String.Empty,
        };

        public String GetFlexDirectionForDrawerContainer() => LegendPosition switch
        {
            Position.Bottom or Position.Top => "flex-column",
            Position.Left or Position.Right => "flex-row",
            _ => String.Empty,
        };

        public String GetFlexOrderForLegendContainer() => LegendPosition switch
        {
            Position.Bottom or Position.Right => "order-last",
            Position.Left or Position.Top => "order-first",
            _ => String.Empty,
        };

        public virtual String GetAlignClassBasendOnPosition(Align align, Position position) => (align, position) switch
        {
            ((Align.Center or Align.Justify), (Position.Bottom or Position.Top)) => "justify-center",
            ((Align.Center or Align.Justify), (Position.Left or Position.Right)) => "align-center",

            (Align.Left, (Position.Bottom or Position.Top)) => "justify-start",
            (Align.Left, (Position.Left or Position.Right)) => "align-start",

            (Align.Right, (Position.Bottom or Position.Top)) => "justify-end",
            (Align.Right, (Position.Left or Position.Right)) => "align-end",

            _ => String.Empty,
        };

        public virtual String GetAlignClassForLegendContainer() => GetAlignClassBasendOnPosition(LegendAlignment, LegendPosition);
        public virtual String GetAlignClassForTitleContainer() => GetAlignClassBasendOnPosition(TitleAlignment, TitlePosition);

        #endregion
    }
}
