// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MudBlazor
{
    public record struct MaskOption(string Id, string Mask, string Regex);

    public class MultiMask : PatternMask
    {
        public MultiMask(string defaultMask, params MaskOption[] options) : base(defaultMask)
        {
            _defaultMask = defaultMask;
            _options = options ?? new MaskOption[0];
        }

        private string _defaultMask;
        private MaskOption[] _options;
        public Action<MaskOption?, string> OptionDetected { get; set; }

        public MaskOption? DetectedOption { get; private set; } = null;

        public override void Insert(string input)
        {
            DoCheckAndRedo(() => base.Insert(input));
        }

        public override void Delete()
        {
            DoCheckAndRedo(base.Delete);
        }

        public override void Backspace()
        {
            DoCheckAndRedo(base.Backspace);
        }

        /// <summary>
        /// This is a nifty trick to avoid writing three times the same code for Insert, Delete and Backspace.
        /// This backs up the state. Executes the action, checks if option changed. If so, apply saved state and
        /// re-apply the action.
        /// </summary>
        /// <param name="action"></param>
        protected void DoCheckAndRedo(Action action)
        {
            // backup state
            var text = Text;
            var pos = CaretPos;
            var sel = Selection;
            // do it!
            action();
            var newOption = CheckOption();
            if (newOption == DetectedOption)
                return;
            // detected a different option
            DetectedOption = newOption;
            // revert saved state
            Text = text;
            CaretPos = pos;
            Selection = sel;
            Mask = newOption != null ? newOption.Value.Mask : _defaultMask;
            // when mask changes we need to re-initialize!
            _initialized = false;
            // do it again!
            action();
            OptionDetected?.Invoke(newOption, Text);
        }

        protected virtual MaskOption? CheckOption()
        {
            var text = Text ?? "";
            foreach (var option in _options)
            {
                if (option.Regex != null && Regex.IsMatch(text, option.Regex))
                    return option;
            }
            return null;
        }

        public override void UpdateFrom(IMask other)
        {
            base.UpdateFrom(other);
            if (other is not MultiMask o)
                return;
            // no need to re-initialize, just update the options
            _defaultMask = o._defaultMask;
            _options = o._options ?? new MaskOption[0];
            OptionDetected = o.OptionDetected;
            Refresh();
        }
    }
}
