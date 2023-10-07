using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class MudDefaultsService
    {
        public void SetColor(Color color)
        {
            Defaults.Color = color;
        }

        public void SetDense(bool dense)
        {
            Defaults.Dense = dense;
        }

        public void SetMargin(Margin margin)
        {
            Defaults.Margin = margin;
        }

        public void SetAnchorOrigin(Origin origin)
        {
            Defaults.AnchorOrigin = origin;
        }

        public void SetTransformOrigin(Origin origin)
        {
            Defaults.TransformOrigin = origin;
        }

        public void SetSize(Size size)
        {
            Defaults.Size = size;
        }

        public void SetVariant(Variant variant)
        {
            Defaults.Variant = variant;
        }

        public void SetPickerVariant(PickerVariant pickerVariant)
        {
            Defaults.PickerVariant = pickerVariant;
        }
    }
}
