using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMaskField<T> : MudBaseInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        private MudInput<string> _elementReference;

        private string _rawValue;

        private int _caretPosition = 0;
        private (int, int)? _selection = null;

        private string _pastedText;

        private Dictionary<int, char> _rawValueDictionary = new();

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }
        [Inject] private IJsEvent _jsEvent { get; set; }
        [Inject] private IJsApiService _jsApiService { get; set; }

        private string _elementId = "maskfield_" + Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public InputType InputType { get; set; } = InputType.Text;

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public MaskType MaskType { get; set; } = MaskType.Default;

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool KeepCharacterPositions { get; set; } = false;

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Dictionary<char, CharacterType> MaskCharacters { get; set; } = new() { 
            ['a'] = CharacterType.Letter,
            ['0'] = CharacterType.Digit,
            ['*'] = CharacterType.LetterOrDigit,
        };

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public List<(char, Regex)> CustomCharacterTypes { get; set; } = new()
        {
            ('c', new Regex("^[a-z]$")),
            ('e', new Regex("^[A-Z]$"))
        };

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public char PlaceholderCharacter { get; set; } = '_';

        private string GetRawValueFromDictionary()
        {
            string rawValue = "";
            //foreach (var item in _rawValueDictionary)
            //{
            //    rawValue += item.Value.ToString();
            //}
            for (int i = 0; i < Mask.Length; i++)
            {
                int a = i;
                if (_rawValueDictionary.ContainsKey(a))
                {
                    rawValue += _rawValueDictionary[a];
                }
            }
            return rawValue;
        }

        protected override async Task SetTextAsync(string text, bool updateValue=true )
        {
            Console.WriteLine($"SetTextAsync: '{text}' updateValue:{updateValue}");
            await base.SetTextAsync(text, updateValue);
        }

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            Console.WriteLine($"SetValueAsync: '{value}' updateText:{updateText}");
            _rawValue = Converter.Set(value);
            if (updateText)
                await ImplementMask(null, Mask, _pastedText);
            // never update text directly. we already did it
            await base.SetValueAsync(value, updateText: false);
        }

        internal async void SetRawValue(string rawValue, bool updateText = false)
        {
            if (_rawValue == rawValue)
                return;
            _rawValue = rawValue;
            await SetValueAsync(Converter.Get(rawValue), updateText);
            _rawValue = rawValue;
        }

        internal string GetRawValueFromText()
        {
            string result = "";
            for (int i = 0; i < Text.Length; i++)
            {
                int a = i;
                if (Regex.IsMatch(Text[a].ToString(), @"^(\p{L}|\d)$") && Mask[a] != Text[a])
                {
                    result += Text[a].ToString();
                }
            }
            return result;
        }

        internal override InputType GetInputType() => InputType;

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            return SetTextAsync(null, updateValue: true);
        }

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public string Mask { get; set; } = "";

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _jsEvent.Connect(_elementId, new JsEventOptions
                {
                    EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    TagName = "INPUT"
                });
                _jsEvent.CaretPositionChanged += OnCaretPositionChanged;
                _jsEvent.Copy += OnCopy;
                _jsEvent.Paste += OnPaste;
                _jsEvent.Select += OnSelect;
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead hilight previous item
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead hilight next item
                        new KeyOptions { Key="Home", PreventDown = "key+none" },
                        new KeyOptions { Key="End", PreventDown = "key+none" },
                        new KeyOptions { Key="PageUp", PreventDown = "key+none" },
                        new KeyOptions { Key="PageDown", PreventDown = "key+none" },
                        new KeyOptions { Key="Escape" },
                        new KeyOptions { Key=@"/^(\w|\d)$/", PreventDown = "key+none|key+shift" },
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                        new KeyOptions { Key="Backspace", PreventDown = "key+none" },
                        new KeyOptions { Key="Delete", PreventDown = "key+none" },
                        new KeyOptions { Key="Shift", PreventDown = "key+none" },
                    },
                });
            }
            if (_isFocused)
                await _elementReference.SelectRangeAsync(_caretPosition, _caretPosition);
            await base.OnAfterRenderAsync(firstRender);
        }

        private void OnCaretPositionChanged(int pos)
        {
            _caretPosition = pos;
            _selection = null;
            //Console.WriteLine($"Caret position: {pos}");
        }

        private void OnCopy()
        {
            var text = _rawValue;
            _jsApiService.CopyToClipboardAsync(text);
            //Console.WriteLine($"Copy: {text}");
        }

        private async void OnPaste(string text)
        {
            //UpdateRawValueDictionary(false, text);
            _pastedText = text;
            await SetValueAsync(Converter.Get(text), true);
            await Task.Delay(1);
            //SetRawValue(text, true);
            await SetValueAsync(Converter.Get(GetRawValueFromDictionary()));
            //Console.WriteLine($"Paste: {text}");
        }

        public void OnSelect(int start, int end)
        {
            _selection = (start, end);
            Console.WriteLine($"Select: {_selection.Value.Item1}-{_selection.Value.Item2}");
        }

        private async void OnFocused(FocusEventArgs obj)
        {
            _isFocused = true;
            //if (string.IsNullOrEmpty(_rawValue) && !string.IsNullOrEmpty(Placeholder))
            //{

            //}
            //else
            //{
            //    await UltimateImplementMask(false, Mask);
            //    SetCaretPosition(FindFirstCaretLocation());
            //}
            if (!string.IsNullOrEmpty(_rawValue) || string.IsNullOrEmpty(Placeholder))
            {
                await ImplementMask(null, Mask);
                SetCaretPosition(FindFirstCaretLocation());
            }
        }

        protected override void OnBlurred(FocusEventArgs obj)
        {
            base.OnBlurred(obj);
            if (string.IsNullOrEmpty(_rawValue))
            {
                SetTextAsync("", updateValue:false).AndForget();
            }
            _isFocused = false;
        }

        public string GetRawValue()
        {
            if (_rawValue == null)
            {
                return "";
            }
            return _rawValue;
        }

        #region Masking Helpers

        private void UpdateCustomCharacters()
        {
            foreach (var item in CustomCharacterTypes)
            {
                if (!MaskCharacters.ContainsKey(item.Item1))
                    MaskCharacters.Add(item.Item1, CharacterType.Custom);
            }
        }

        private CharacterType GetCharacterType(string character, bool isMaskingCharacter = false)
        {
            if (string.IsNullOrEmpty(character))
            {
                return CharacterType.None;
            }

            if (isMaskingCharacter)
            {
                if (!MaskCharacters.ContainsKey(character[0]))
                    return CharacterType.Other;

                return MaskCharacters[character[0]];
            }
            else
            {
                if (Regex.IsMatch(character, @"\p{L}"))
                {
                    return CharacterType.Letter;
                }
                else if (Regex.IsMatch(character, @"^\d$"))
                {
                    return CharacterType.Digit;
                }
                else
                {
                    return CharacterType.Other;
                }
            }
        }

        private bool IsCharsMatch(char textChar, char maskChar)
        {
            if (GetCharacterType(maskChar.ToString(), true) == GetCharacterType(textChar.ToString()))
                return true;
            if (GetCharacterType(maskChar.ToString(), true) == CharacterType.LetterOrDigit && GetCharacterType(textChar.ToString()) != CharacterType.Other)
                return true;
            if (GetCharacterType(maskChar.ToString(), true) == CharacterType.Custom)
            {
                foreach (var item in CustomCharacterTypes)
                {
                    if (item.Item1 == maskChar && item.Item2.IsMatch(textChar.ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //private void SetRawValueFromText()
        //{
        //    if (Text == null)
        //    {
        //        Text = "";
        //    }
        //    string rawValue = "";
        //    int counterMeter = 0;
        //    foreach (var c in Text)
        //    {
        //        if ((c != Mask[counterMeter] || (GetCharacterType(Mask[counterMeter].ToString(), true) != CharacterType.Other && c == Mask[counterMeter] && GetCharacterType(Mask[counterMeter].ToString(), true) == GetCharacterType(c.ToString()))) && c != PlaceholderCharacter)
        //        {
        //            rawValue += c.ToString();
        //        }
        //        counterMeter++;
        //    }
        //    _rawValue = rawValue;
        //}
        #endregion

        #region Core Masking

        private void UpdateRawValueDictionary(string lastPressedKey = null, string pastedText = null)
        {
            Dictionary<int, char> backUpDictionary = new Dictionary<int, char>();
            for (int i = 0; i < Mask.Length; i++)
            {
                int a = i;
                if (_rawValueDictionary.ContainsKey(a))
                    backUpDictionary.Add(a, _rawValueDictionary[a]);
            }
            _rawValueDictionary.Clear();
            if (pastedText != null)
            {
                int pastedIndex = 0;
                foreach (var c in pastedText)
                {
                    for (int i = 0; i < Mask.Length; i++)
                    {
                        int a = i;
                        if (Mask.Length <= _caretPosition + pastedIndex + a)
                        {
                            _pastedText = null;
                            return;
                        }

                        if (MaskCharacters.ContainsKey(Mask[_caretPosition + pastedIndex + a]) && !_rawValueDictionary.ContainsKey(_caretPosition + pastedIndex + a))
                        {
                            _rawValueDictionary.Add(_caretPosition + pastedIndex + a, c);
                            break;
                        }
                    }
                    pastedIndex++;
                }
                _pastedText = null;
                return;
            }
            if (lastPressedKey != null && !_rawValueDictionary.ContainsKey(_caretPosition) && !string.IsNullOrEmpty(lastPressedKey)
                 && !(lastPressedKey == "Backspace" || lastPressedKey == "Delete"))
            {
                _rawValueDictionary.Add(_caretPosition, lastPressedKey[0]);
            }
            if (string.IsNullOrEmpty(Text))
                return;

            if (KeepCharacterPositions)
            {
                for (int i = 0; i < Mask.Length; i++)
                {
                    int a = i;
                    if (Regex.IsMatch(Text[a].ToString(), @"^(\p{L}|\d)$") && !_rawValueDictionary.ContainsKey(a))
                    {
                        _rawValueDictionary.Add(a, Text[a]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Mask.Length; i++)
                {
                    int a = i;
                    if (backUpDictionary.ContainsKey(a))
                    {
                        if (a < _caretPosition)
                            _rawValueDictionary.Add(a, backUpDictionary[a]);
                        else
                        {
                            for (int i2 = 0; a + i2 < Mask.Length; i2++)
                            {
                                int a2 = i2;
                                if (MaskCharacters.ContainsKey(Mask[a + a2]) && !_rawValueDictionary.ContainsKey(a + a2))
                                {
                                    _rawValueDictionary.Add(a + a2, backUpDictionary[a]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (lastPressedKey == "Backspace")
            {
                if (KeepCharacterPositions)
                {
                    for (int i = 1; i < Text.Length; i++)
                    {
                        int a = i;
                        if (_caretPosition - a < 0)
                            break;
                        if (Text[_caretPosition - a] == PlaceholderCharacter)
                            break;
                        if (_rawValueDictionary.ContainsKey(_caretPosition - a))
                        {
                            _rawValueDictionary.Remove(_caretPosition - a);
                            break;
                        }
                    }
                }
                else
                {
                    int removedIndex = 0;
                    for (int i = 0; i < (Text.Length - (Text.Length - _caretPosition)); i++)
                    {
                        removedIndex++;
                        if (MaskCharacters.ContainsKey(Mask[_caretPosition - removedIndex]))
                        {
                            _rawValueDictionary.Remove(_caretPosition - removedIndex);
                            break;
                        }
                    }

                    Dictionary<int, char> replacedDictionary = new Dictionary<int, char>();
                    foreach (var item in _rawValueDictionary)
                    {
                        if (_caretPosition <= item.Key)
                        {
                            for (int i = 0; i < Text.Length; i++)
                            {
                                int a = i;
                                if (MaskCharacters.ContainsKey(Mask[item.Key - removedIndex - a]))
                                {
                                    replacedDictionary.Add(item.Key - removedIndex - a, item.Value);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            replacedDictionary.Add(item.Key, item.Value);
                        }
                    }
                    _rawValueDictionary = replacedDictionary;
                }
            }

            else if (lastPressedKey == "Delete")
            {
                for (int i = 0; i < Text.Length; i++)
                {
                    int a = i;
                    if (_caretPosition + a < 0)
                        break;
                    if (Text[_caretPosition + a] == PlaceholderCharacter)
                        break;
                    if (_rawValueDictionary.ContainsKey(_caretPosition + a))
                    {
                        _rawValueDictionary.Remove(_caretPosition + a);
                        break;
                    }
                }
            }
        }

        private async Task ImplementMask(string lastPressedKey, string mask, string pastedText = null)
        {
            if (mask == null)
            {
                return;
            }

            string result = "";

            UpdateCustomCharacters();
            UpdateRawValueDictionary(lastPressedKey, pastedText);

            bool hasValue = false;
            for (int i = 0; i < mask.Length; i++)
            {
                int a = i;
                if (!MaskCharacters.ContainsKey(mask[a]))
                {
                    result += mask[a].ToString();
                    if (_rawValueDictionary.ContainsKey(a))
                    {
                        _rawValueDictionary.Remove(a);
                    }
                }
                else if (_rawValueDictionary.ContainsKey(a))
                {
                    if (IsCharsMatch(_rawValueDictionary[a], mask[a]))
                    {
                        result += _rawValueDictionary[a].ToString();
                        if (!hasValue)
                            hasValue = true;
                    }
                    else
                    {
                        _rawValueDictionary.Remove(a);
                        result += PlaceholderCharacter.ToString();
                    }
                }
                else
                {
                    result += PlaceholderCharacter.ToString();
                }
            }

            if (hasValue == false && !string.IsNullOrEmpty(Placeholder))
            {
                await SetTextAsync(null, updateValue: false);
            }
            else
            {
                await SetTextAsync(result, updateValue: false);
            }

            //await _elementReference.SetText(result);
        }

        #endregion

        #region Caret

        private int FindNextCaretLocation(int currentCaretIndex)
        {
            if (Text == null || Text.Length == 0)
                return 0;
            int a = currentCaretIndex;
            for (int i = a; i < Mask.Length; i++)
            {
                if (Text[a] == PlaceholderCharacter)
                {
                    return a;
                }
                a++;
            }
            return a;
        }

        private int FindFirstCaretLocation()
        {
            if (Text == null || Text.Length == 0)
                return 0;
            int a = 0;
            for (int i = 0; i < Mask.Length; i++)
            {
                a = i;
                if (Text[a] == PlaceholderCharacter)
                {
                    return a;
                }
            }
            return 0;
        }

        private int FindPreviousCaretLocation(int currentCaretIndex)
        {
            if (Text == null || Text.Length == 0)
                return 0;
            int a = currentCaretIndex;
            for (int i = 0; i < Text.Length; i++)
            {
                if (a <= 0)
                    return 0;
                if (Text[a - 1] == PlaceholderCharacter)
                {
                    return a - 1;
                }
                a--;
            }
            return a;
        }

        private int FindLastCaretLocation()
        {
            if (Text == null || Text.Length == 0)
                return 0;
            int a = Mask.Length;
            for (int i = 0; i < Mask.Length; i++)
            {
                a -= i;
                if (Text[a] == PlaceholderCharacter)
                {
                    return a;
                }
            }
            return Mask.Length;
        }

        public void SetCaretPosition(int caretPosition)
        {
            _caretPosition = caretPosition;
            if (!_isFocused)
                return;
            _elementReference.SelectRangeAsync(_caretPosition, _caretPosition).AndForget();
            StateHasChanged();
        }

        internal void ArrangeCaretPosition(KeyboardEventArgs obj)
        {
            if (obj.Key == "Backspace")
            {
                SetCaretPosition(FindPreviousCaretLocation(_caretPosition));
            }
            else if (obj.Key == "Delete")
            {
                SetCaretPosition(_caretPosition);
            }
            else
            {
                SetCaretPosition(FindNextCaretLocation(_caretPosition));
            }
        }

        #endregion

        private string GetMaskByType()
        {
            string resultMask = "";

            if (MaskType == MaskType.Telephone)
            {
                resultMask = "(000) 000 0000";
            }
            else if (MaskType == MaskType.Mac)
            {
                resultMask = "cc cc cc cc cc cc";
            }
            else if (Mask == null)
            {
                resultMask = "";
            }
            else
            {
                resultMask = Mask;
            }

            return resultMask;
        }

        protected internal async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            if (obj.CtrlKey == true || (!Regex.IsMatch(obj.Key, @"^(\p{L}|\d)$") &&
                !(obj.Key == "Backspace" || obj.Key == "Delete")))
                return;
            Console.WriteLine($"HandleKeyDown: '{obj.Key}'");
            await ImplementMask(obj.Key, Mask);
            string val = GetRawValueFromDictionary();
            await SetValueAsync(Converter.Get(val), false);

            OnKeyDown.InvokeAsync(obj).AndForget();
            ArrangeCaretPosition(obj);
        }
    }
}
