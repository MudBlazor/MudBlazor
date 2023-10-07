//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;

namespace MudBlazor
{
#nullable enable
    public class DefaultsConfiguration
    {
        private Color _color;
        private bool _dense;
        private Margin _margin;
        private Origin _anchorOrigin;
        private Origin _transformOrigin;
        private Size _size;
        private Variant _variant;
        private PickerVariant _pickerVariant;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Defaults.Color = value;
            }
        }

        public bool Dense
        {
            get => _dense;
            set
            {
                _dense = value;
                Defaults.Dense = value;
            }
        }

        public Margin Margin
        {
            get => _margin;
            set
            {
                _margin = value;
                Defaults.Margin = value;
            }
        }

        public Origin AnchorOrigin
        {
            get => _anchorOrigin;
            set
            {
                _anchorOrigin = value;
                Defaults.AnchorOrigin = value;
            }
        }

        public Origin TransformOrigin
        {
            get => _transformOrigin;
            set
            {
                _transformOrigin = value;
                Defaults.TransformOrigin = value;
            }
        }

        public Size Size
        {
            get => _size;
            set
            {
                _size = value;
                Defaults.Size = value;
            }
        }

        public Variant Variant
        {
            get => _variant;
            set
            {
                _variant = value;
                Defaults.Variant = value;
            }
        }

        public PickerVariant PickerVariant
        {
            get => _pickerVariant;
            set
            {
                _pickerVariant = value;
                Defaults.PickerVariant = value;
            }
        }

        public DefaultsConfiguration()
        {
            Color = Color.Default;
            Dense = false;
            Margin = Margin.Normal;
            AnchorOrigin = Origin.TopCenter;
            TransformOrigin = Origin.TopCenter;
            Size = Size.Medium;
            Variant = Variant.Text;
            PickerVariant = PickerVariant.Inline;
        }
    }
}
