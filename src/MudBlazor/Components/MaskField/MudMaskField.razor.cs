// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;
using MudBlazor.Components.MaskField;

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

        internal int _caretPosition = 0;
        private (int, int)? _selection = null;

        private Dictionary<int, char> _rawValueDictionary = new();

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }
        [Inject] private IJsEvent _jsEvent { get; set; }
        [Inject] private IJsApiService _jsApiService { get; set; }

        private string _elementId = "maskfield_" + Guid.NewGuid().ToString().Substring(0, 8);

        internal Dictionary<char, MaskChar> _maskDict = new Dictionary<char, MaskChar>();
        private MaskChar[] _maskChars = new MaskChar[]
        {
            MaskChar.Letter('a'),
            MaskChar.Digit('0'),
            MaskChar.LetterOrDigit('*'),
            new MaskChar { Char = 'l', Writable = true, AddToValue = false, Regex = "^[a-z]$" },
            new MaskChar { Char = 'u', Writable = true, AddToValue = false, Regex = "^[A-Z]$" },
        };

        [Parameter]
        public MaskChar[] MaskDefinition
        {
            get => _maskChars;
            set
            {
                _maskChars = value;
                _maskDict = value.ToDictionary(x => x.Char);
            }
        }

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
        public char PlaceholderCharacter { get; set; } = '_';

        private string GetRawValueFromDictionary()
        {
            string rawValue = "";
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
            //if (text == "a")
            //    text="a";
            Console.WriteLine($"SetTextAsync: '{text}' updateValue:{updateValue}");
            await base.SetTextAsync(text, updateValue);
        }

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            Console.WriteLine($"SetValueAsync: '{value}' updateText:{updateText}");
            _rawValue = Converter.Set(value);
            if (updateText)
                await ImplementMask(null, Mask, true);
            // never update text directly. we already did it
            await base.SetValueAsync(value, updateText: false);
        }

        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            if (GetRawValueFromText() == Converter.Set(Value))
                return;
            await ImplementMask(null, Mask, true);
        }

        //internal async void SetRawValue(string rawValue, bool updateText = false)
        //{
        //    if (_rawValue == rawValue)
        //        return;
        //    _rawValue = rawValue;
        //    await SetValueAsync(Converter.Get(rawValue), updateText);
        //    _rawValue = rawValue;
        //}

        internal string GetRawValueFromText()
        {
            if (Text == null)
                return null;
            if (Text == "")
                return "";
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
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    TagName = "INPUT"
                });
                _jsEvent.CaretPositionChanged += OnCaretPositionChanged;
                _jsEvent.Copy += OnCopy;
                _jsEvent.Paste += OnPaste;
                _jsEvent.Select += OnSelect;
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page,
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page,
                        new KeyOptions { Key="ArrowLeft", PreventDown = "key+none" },
                        new KeyOptions { Key="ArrowRight", PreventDown = "key+none" },
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
                _maskDict = _maskChars.ToDictionary(x => x.Char);
            }
            if (_isFocused)
            {
                if (_selection == null)
                    await _elementReference.SelectRangeAsync(_caretPosition, _caretPosition);
                //else
                //    await _elementReference.SelectRangeAsync(_selection.Item1, _selection.Item2);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal void OnCaretPositionChanged(int pos)
        {
            _caretPosition = pos;
            _selection = null;
            //Console.WriteLine($"Caret position: {pos}");
        }

        internal void OnCopy()
        {
            var text = Value?.ToString();
            _jsApiService.CopyToClipboardAsync(text);
            //Console.WriteLine($"Copy: {text}");
        }

        internal async void OnPaste(string text)
        {
            ////Console.WriteLine($"Paste: {text}");
            if (Text == null)
                return;
            var val = GetRawValueFromDictionary();
            int lastAddedIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                for (int i2 = _caretPosition; i + i2 < Text.Length; i2++)
                {
                    if (_maskDict.ContainsKey(Mask[i + i2]) && lastAddedIndex < i + i2)
                    {
                        if (_rawValueDictionary.ContainsKey(i + i2))
                        {
                            _rawValueDictionary.Remove(i + i2);
                        }
                        _rawValueDictionary.Add(i + i2, text[i]);
                        lastAddedIndex = i + i2;
                        break;
                    }
                }
            }
            await ImplementMask(null, Mask, false);
            await SetValueAsync(Converter.Get(GetRawValueFromDictionary()), false);
        }

        public void OnSelect(int start, int end)
        {
            _selection = (start, end);
            //Console.WriteLine($"Select: {_selection.Value.Item1}-{_selection.Value.Item2}");
        }

        internal async void OnFocused(FocusEventArgs obj)
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
            if (!string.IsNullOrEmpty(Converter.Set(Value)) || string.IsNullOrEmpty(Placeholder))
            {
                await ImplementMask(null, Mask, true);
                SetCaretPosition(FindFirstCaretLocation());
            }
        }

        protected internal override void OnBlurred(FocusEventArgs obj)
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

        private bool IsCharsMatch(char textChar, char maskChar)
        {
            if (_maskDict.ContainsKey(maskChar) && Regex.IsMatch(textChar.ToString(), _maskDict[maskChar].Regex))
                return true;
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

        //Create main dictionary with given string (not affect value directly)
        internal void SetRawValueDictionary(string value)
        {
            _rawValueDictionary.Clear();

            for (int i = 0; i < value.Length; i++)
            {
                for (int i2 = 0; i + i2 < Mask.Length; i2++)
                {
                    if (_maskDict.ContainsKey(Mask[i + i2]) && !_rawValueDictionary.ContainsKey(i + i2))
                    {
                        _rawValueDictionary.Add(i + i2, value[i]);
                        break;
                    }
                }
            }
        }

        private void UpdateRawValueDictionary(string lastPressedKey = null)
        {
            //If there is a selection, first delete the selected characters and move the after ones if keep position is false
            //If we have selection, we think the text cannot be null, so we didn't have null check
            #region Selection
            if (_selection != null && _selection.Value.Item1 != _selection.Value.Item2)
            {
                for (int i = _selection.Value.Item1; i < _selection.Value.Item2; i++)
                {
                    if (_rawValueDictionary.ContainsKey(i))
                    {
                        _rawValueDictionary.Remove(i);
                    }
                }
                if (KeepCharacterPositions == false && Text != null)
                {
                    for (int i = 0; i < Text.Length - _selection.Value.Item2; i++)
                    {
                        if (_rawValueDictionary.ContainsKey(_selection.Value.Item2 + i))
                        {
                            for (int i2 = 0; _selection.Value.Item1 + i2 < Text.Length; i2++)
                            {
                                if (_maskDict.ContainsKey(Mask[_selection.Value.Item1 + i2]) && !_rawValueDictionary.ContainsKey(_selection.Value.Item1 + i2))
                                {
                                    _rawValueDictionary.Add(_selection.Value.Item1 + i2, _rawValueDictionary[_selection.Value.Item2 + i]);
                                    _rawValueDictionary.Remove(_selection.Value.Item2 + i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            //If value changed by user from outside, create dictionary from _rawValue scratch, if there is a problem with value changing, probably is here
            if (_rawValue != null && Converter.Set(Value) != _rawValue)
            {
                SetRawValueDictionary(_rawValue);
            }

            //BackUp and clear the main dictionary
            #region BackUp and Clear
            Dictionary<int, char> backUpDictionary = new Dictionary<int, char>();
            for (int i = 0; i < Mask.Length; i++)
            {
                if (_rawValueDictionary.ContainsKey(i))
                    backUpDictionary.Add(i, _rawValueDictionary[i]);
            }
            _rawValueDictionary.Clear();
            #endregion

            //Add pressed key as the first item of cleared dictionary (if there is a key)
            #region Insert Key
            if (lastPressedKey != null && !_rawValueDictionary.ContainsKey(_caretPosition) && !string.IsNullOrEmpty(lastPressedKey)
                 && !(lastPressedKey == "Backspace" || lastPressedKey == "Delete"))
            {
                if (string.IsNullOrEmpty(Text))
                {
                    _rawValueDictionary.Add(_caretPosition, lastPressedKey[0]);
                    return;
                }

                for (int i = 0; i < Text.Length - _caretPosition; i++)
                {
                    if (KeepCharacterPositions)
                    {
                        if (Text[_caretPosition + i] == PlaceholderCharacter)
                        {
                            _rawValueDictionary.Add(_caretPosition + i, lastPressedKey[0]);
                            break;
                        }
                    }
                    else
                    {
                        if (_maskDict.ContainsKey(Mask[_caretPosition + i]))
                        {
                            _rawValueDictionary.Add(_caretPosition + i, lastPressedKey[0]);
                            break;
                        }
                    }
                }
            }
            #endregion

            //Create the main dictionary with backup again
            #region Fetch Into BackUp
            if (string.IsNullOrEmpty(Text))
                return;

            if (KeepCharacterPositions)
            {
                for (int i = 0; i < Mask.Length; i++)
                {
                    if (backUpDictionary.ContainsKey(i))
                    {
                        _rawValueDictionary.Add(i, backUpDictionary[i]);
                    }
                    //int a = i;
                    //if (Regex.IsMatch(Text[a].ToString(), @"^(\p{L}|\d)$") && !_rawValueDictionary.ContainsKey(a))
                    //{
                    //    _rawValueDictionary.Add(a, Text[a]);
                    //}
                }
            }
            else
            {
                for (int i = 0; i < Mask.Length; i++)
                {
                    if (backUpDictionary.ContainsKey(i))
                    {
                        if (i < _caretPosition)
                            _rawValueDictionary.Add(i, backUpDictionary[i]);
                        else
                        {
                            for (int i2 = 0; i + i2 < Mask.Length; i2++)
                            {
                                if (_maskDict.ContainsKey(Mask[i + i2]) && !_rawValueDictionary.ContainsKey(i + i2))
                                {
                                    _rawValueDictionary.Add(i + i2, backUpDictionary[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion


            #region Backspace Arrangement
            if (lastPressedKey == "Backspace" && _selection == null)
            {
                if (KeepCharacterPositions)
                {
                    for (int i = 1; i < Text.Length; i++)
                    {
                        if (_caretPosition - i < 0)
                            break;
                        if (Text[_caretPosition - i] == PlaceholderCharacter)
                            break;
                        if (_rawValueDictionary.ContainsKey(_caretPosition - i))
                        {
                            _rawValueDictionary.Remove(_caretPosition - i);
                            break;
                        }
                    }
                }
                else
                {
                    //Check is there any remaining value, otherwise it always removes the first char
                    bool hasValueBefore = false;
                    int removedIndex = 0;
                    for (int i = 0; i < (Text.Length - (Text.Length - _caretPosition)); i++)
                    {
                        removedIndex++;
                        if (_maskDict.ContainsKey(Mask[_caretPosition - removedIndex]))
                        {
                            hasValueBefore = true;
                            _rawValueDictionary.Remove(_caretPosition - removedIndex);
                            break;
                        }
                    }
                    if (!hasValueBefore)
                    {
                        return;
                    }

                    Dictionary<int, char> replacedDictionary = new Dictionary<int, char>();
                    foreach (var item in _rawValueDictionary)
                    {
                        if (_caretPosition - removedIndex <= item.Key)
                        {
                            for (int i = 1; i < Text.Length; i++)
                            {
                                if (item.Key - i < 0)
                                {
                                    break;
                                }
                                if (_maskDict.ContainsKey(Mask[item.Key - i]))
                                {
                                    replacedDictionary.Add(item.Key - i, item.Value);
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
            #endregion

            #region Delete Arrangement
            else if (lastPressedKey == "Delete" && _selection == null)
            {
                if (KeepCharacterPositions == true)
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
                else
                {
                    int? removedPosition = null;
                    for (int i = 0; i < Text.Length; i++)
                    {
                        int a = i;
                        if (Text.Length <= _caretPosition + a)
                            break;
                        if (Text[_caretPosition + a] == PlaceholderCharacter)
                            break;
                        if (_rawValueDictionary.ContainsKey(_caretPosition + a))
                        {
                            _rawValueDictionary.Remove(_caretPosition + a);
                            removedPosition = _caretPosition + a;
                            break;
                        }
                    }
                    if (removedPosition != null)
                    {
                        for (int i = 0; i < Text.Length; i++)
                        {
                            if (_rawValueDictionary.ContainsKey((int)removedPosition + i))
                            {
                                var c = _rawValueDictionary[(int)removedPosition + i];
                                for (int i2 = 1; 0 < removedPosition + i - i2; i2++)
                                {
                                    if (_maskDict.ContainsKey(Mask[(int)removedPosition + i - i2]))
                                    {
                                        _rawValueDictionary.Remove((int)removedPosition + i);
                                        _rawValueDictionary.Add((int)removedPosition + i - i2, c);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    
                }
            }
            #endregion
        }

        internal async Task ImplementMask(string lastPressedKey, string mask, bool updateCharactersOrDictionary = true)
        {
            if (mask == null)
            {
                return;
            }

            //Useful if only need to change text, not the dictionary itself
            if (updateCharactersOrDictionary == true)
            {
                UpdateRawValueDictionary(lastPressedKey);
            }

            //Create masked text from dictionary
            #region Create Masked Text
            string result = "";
            bool hasValue = false;
            for (int i = 0; i < mask.Length; i++)
            {
                if (!_maskDict.ContainsKey(mask[i]))
                {
                    result += mask[i].ToString();
                    if (_rawValueDictionary.ContainsKey(i))
                    {
                        _rawValueDictionary.Remove(i);
                    }
                }
                else if (_rawValueDictionary.ContainsKey(i))
                {
                    if (IsCharsMatch(_rawValueDictionary[i], mask[i]))
                    {
                        result += _rawValueDictionary[i].ToString();
                        if (!hasValue)
                            hasValue = true;
                    }
                    else
                    {
                        _rawValueDictionary.Remove(i);
                        result += PlaceholderCharacter.ToString();
                    }
                }
                else
                {
                    result += PlaceholderCharacter.ToString();
                }
            }
            #endregion

            #region Internal Text Change
            if (hasValue == false && !string.IsNullOrEmpty(Placeholder))
            {
                await SetTextAsync(null, updateValue: false);
            }
            else
            {
                await SetTextAsync(result, updateValue: false);
            }
            #endregion
        }

        #endregion

        #region Caret

        internal int FindNextCaretLocation(int currentCaretIndex, bool onlyPlaceholderCharacter = true)
        {
            if (Text == null || Text.Length == 0)
                return 0;
            for (int i = currentCaretIndex; i < Mask.Length; i++)
            {
                if (onlyPlaceholderCharacter == true)
                {
                    if (Text.Length <= i + 1)
                    {
                        return Mask.Length;
                    }
                    if (Text[i + 1] == PlaceholderCharacter)
                    {
                        return i + 1;
                    }
                }
                else
                {
                    if (Mask.Length <= i + 1)
                    {
                        return Mask.Length;
                    }
                    if (_maskDict.ContainsKey(Mask[i + 1]))
                    {
                        return i + 1;
                    }
                }

            }
            return currentCaretIndex;
        }

        internal int FindPreviousCaretLocation(int currentCaretIndex, bool onlyPlaceholderCharacter = true)
        {
            if (Text == null || Text.Length == 0)
                return 0;
            for (int i = currentCaretIndex; 0 < i; i--)
            {
                if (onlyPlaceholderCharacter == true)
                {
                    if (Text[i - 1] == PlaceholderCharacter)
                    {
                        return i - 1;
                    }
                }
                else
                {
                    if (_maskDict.ContainsKey(Mask[i - 1]))
                    {
                        return i - 1;
                    }
                }
            }
            return currentCaretIndex;
        }

        internal int FindFirstCaretLocation(bool onlyPlaceholderCharacter = true)
        {
            if (Text == null || Text.Length == 0)
                return 0;
            for (int i = 0; i < Mask.Length; i++)
            {
                if (onlyPlaceholderCharacter)
                {
                    if (Text[i] == PlaceholderCharacter)
                    {
                        return i;
                    }
                }
                else
                {
                    if (Text[i] != PlaceholderCharacter && _maskDict.ContainsKey(Mask[i]))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        internal int FindLastCaretLocation(bool onlyPlaceholderCharacter = true)
        {
            if (Text == null || Text.Length == 0)
                return 0;
            for (int i = Mask.Length; 0 < i; i--)
            {
                if (onlyPlaceholderCharacter)
                {
                    if (Text[i - 1] == PlaceholderCharacter)
                    {
                        return i - 1;
                    }
                }
                else
                {
                    if (Text[i - 1] != PlaceholderCharacter && _maskDict.ContainsKey(Mask[i - 1]))
                    {
                        return i - 1;
                    }
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
            switch (obj.Key)
            {
                case "ArrowLeft":
                    if (0 < _caretPosition)
                    {
                        SetCaretPosition(FindPreviousCaretLocation(_caretPosition, false));
                    }
                    break;
                case "ArrowRight":
                    if (_caretPosition < Text.Length)
                    {
                        SetCaretPosition(FindNextCaretLocation(_caretPosition, false));
                    }
                    break;
                case "Backspace":
                    SetCaretPosition(FindPreviousCaretLocation(_caretPosition, false));
                    break;
                case "Delete":
                    SetCaretPosition(_caretPosition);
                    break;
                default:
                    bool hasPlaceholderCharacter = false;
                    foreach (var c in Text)
                    {
                        if (c == PlaceholderCharacter)
                        {
                            hasPlaceholderCharacter = true;
                        }
                    }
                    if (hasPlaceholderCharacter)
                    {
                        SetCaretPosition(FindNextCaretLocation(_caretPosition));
                    }
                    else
                    {
                        SetCaretPosition(FindNextCaretLocation(_caretPosition, false));
                    }
                    break;
            }

            //if (obj.Key == "ArrowLeft")
            //{
            //    if (0 < _caretPosition)
            //    {
            //        SetCaretPosition(_caretPosition - 1);
            //    }
            //}
            //else if (obj.Key == "ArrowRight")
            //{
            //    if (_caretPosition < Text.Length)
            //    {
            //        SetCaretPosition(_caretPosition + 1);
            //    }
            //}
            //else if (obj.Key == "Backspace")
            //{
            //    SetCaretPosition(FindPreviousCaretLocation(_caretPosition));
            //}
            //else if (obj.Key == "Delete")
            //{
            //    SetCaretPosition(_caretPosition);
            //}
            //else
            //{
            //    SetCaretPosition(FindNextCaretLocation(_caretPosition));
            //}
        }

        #endregion

        //private string GetMaskByType()
        //{
        //    string resultMask = "";

        //    if (MaskType == MaskType.Telephone)
        //    {
        //        resultMask = "(000) 000 0000";
        //    }
        //    else if (MaskType == MaskType.Mac)
        //    {
        //        resultMask = "cc cc cc cc cc cc";
        //    }
        //    else if (Mask == null)
        //    {
        //        resultMask = "";
        //    }
        //    else
        //    {
        //        resultMask = Mask;
        //    }

        //    return resultMask;
        //}

        internal async Task SetBothValueAndText(T value)
        {
            await SetValueAsync(value, true);
        }

        protected internal async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            if ((obj.Key == "ArrowLeft" || obj.Key == "ArrowRight") && obj.ShiftKey == false)
            {
                ArrangeCaretPosition(obj);
                return;
            }
            if (obj.CtrlKey == true || (!Regex.IsMatch(obj.Key, @"^(\p{L}|\d)$") &&
                !(obj.Key == "Backspace" || obj.Key == "Delete")))
                return;
            if (_caretPosition == Mask.Length && !(obj.Key == "Backspace" || obj.Key == "Delete"))
                return;
            Console.WriteLine($"HandleKeyDown: '{obj.Key}'");
            await ImplementMask(obj.Key, Mask, true);
            //string val = GetRawValueFromDictionary();
            await SetValueAsync(Converter.Get(GetRawValueFromDictionary()), false);

            OnKeyDown.InvokeAsync(obj).AndForget();

            if (obj.Key == "Backspace" || obj.Key == "Delete")
            {
                ArrangeCaretPosition(obj);
            }
            else
            {
                if (Text == null)
                    return;
                for (int i = _caretPosition; i < Text.Length; i++)
                {
                    if (_rawValueDictionary.ContainsKey(i))
                    {
                        if (IsCharsMatch(Text[i], Mask[i]))
                        {
                            ArrangeCaretPosition(obj);
                            break;
                        }
                    }
                }
            }
        }
    }
}
