// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;

namespace MudBlazor
{
    /// <summary>
    /// A mask which can change its pattern based on partial input.
    /// </summary>
    /// <remarks>
    /// A multi-mask consists of multiple <see cref="MaskOption"/> values which define when a particular mask is used.<br />
    /// For example: a credit card number can be from any card provider, yet each provider has their own numbering rules.  A multi-mask would allow each provider's rules to be used together in a single mask.
    /// </remarks>
    /// <seealso cref="BlockMask" />
    /// <seealso cref="DateMask" />
    /// <seealso cref="PatternMask" />
    /// <seealso cref="RegexMask" />
    public class MultiMask : PatternMask
    {
        private string _defaultMask;
        private MaskOption[]? _options;

        /// <summary>
        /// Occurs when <see cref="DetectedOption" /> has changed.
        /// </summary>
        public Action<MaskOption?, string?>? OptionDetected { get; set; }

        /// <summary>
        /// The currently used mask.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Changes automatically as the input changes.
        /// </remarks>
        public MaskOption? DetectedOption { get; private set; }

        /// <summary>
        /// Creates a new multi mask from the specified mask options.
        /// </summary>
        /// <param name="defaultMask">The starting mask to use for input values.</param>
        /// <param name="options">The list of masks to use depending on the input so far.</param>
        /// <remarks>
        /// A multi-mask consists of multiple <see cref="MaskOption"/> values which define when a particular mask is used.<br />
        /// For example: a credit card number can be from any card provider, yet each provider has their own numbering rules.  A multi-mask would allow each provider's rules to be used together in a single mask.
        /// </remarks>
        public MultiMask(string defaultMask, params MaskOption[]? options) : base(defaultMask)
        {
            _defaultMask = defaultMask;
            _options = options ?? [];
        }

        /// <inheritdoc />
        public override void Insert(string? input)
        {
            DoCheckAndRedo(() => base.Insert(input));
        }

        /// <inheritdoc />
        public override void Delete()
        {
            DoCheckAndRedo(base.Delete);
        }

        /// <inheritdoc />
        public override void Backspace()
        {
            DoCheckAndRedo(base.Backspace);
        }

        /// <summary>
        /// Selects the current <see cref="MaskOption"/> and applies an input character.
        /// </summary>
        /// <param name="action">The action to apply to the input.</param>
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
            ForceReinitialize();
            // do it again!
            action();
            OptionDetected?.Invoke(newOption, Text);
        }

        /// <summary>
        /// Chooses the best <see cref="MaskOption"/> based on the input so far.
        /// </summary>
        /// <returns></returns>
        protected virtual MaskOption? CheckOption()
        {
            var text = Text ?? string.Empty;
            if (_options is null)
            {
                return null;
            }

            foreach (var option in _options)
            {
                if (option.Regex != null && Regex.IsMatch(text, option.Regex))
                {
                    return option;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public override void UpdateFrom(IMask? mask)
        {
            base.UpdateFrom(mask);
            if (mask is MultiMask multiMask)
            {
                // No need to re-initialize, just update the options
                _defaultMask = multiMask._defaultMask;
                _options = multiMask._options ?? [];
                OptionDetected = multiMask.OptionDetected;
                Refresh();
            }
        }
    }
}
