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

        private string _rawValue;

        private Dictionary<int, char> _rawValueDictionary = new ();

        private string GetRawValueFromDictionary()
        {
            string rawValue = "";
            foreach (var item in _rawValueDictionary)
            {
                rawValue += item.Value.ToString();
            }
            return rawValue;
        }
        //internal string RawValue
        //{
        //    get => _rawValue;
        //    set
        //    {
        //        if (_rawValue == value)
        //            return;
        //        SetValueAsync(Converter.Get(value), updateText: false).AndForget();
        //        _rawValue = value;
        //        //UltimateImplementMask(_rawValue, Mask).AndForget();
        //    }
        //}

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
                if (Regex.IsMatch(Text[a].ToString(), "^[a-zA-Z0-9]$") && Mask[a] != Text[a])
                {
                    result += Text[a].ToString();
                }
            }
            return result;
        }

        private string _pastedText;

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            // never update text directly. we do it below
            await base.SetValueAsync(value, updateText: false);
            _rawValue = Converter.Set(value);
            if (updateText)
                await UltimateImplementMask(true, Mask, _pastedText);
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
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead hilight previous item
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead hilight next item
                        new KeyOptions { Key="Home", PreventDown = "key+none" },
                        new KeyOptions { Key="End", PreventDown = "key+none" },
                        new KeyOptions { Key="Escape" },
                        new KeyOptions { Key="/^[a-zA-Z0-9]$/", PreventDown = "key+none|key+shift" },
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
            Console.WriteLine($"Caret position: {pos}");
            _caretPosition = pos;
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

        private async void OnFocussed(FocusEventArgs obj)
        {
            _isFocused = true;
            if (string.IsNullOrEmpty(_rawValue) && !string.IsNullOrEmpty(Placeholder))
            {

            }
            else
            {
                await UltimateImplementMask(false, Mask);
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

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            return SetTextAsync(null, updateValue: true);
        }

        //string val = "";

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
                if (Regex.IsMatch(character, @"^[a-zA-Z]$"))
                {
                    return CharacterType.Letter;
                }
                else if (Regex.IsMatch(character, @"^[0-9]$"))
                {
                    return CharacterType.Digit;
                }
                else
                {
                    return CharacterType.Other;
                }
            }
        }

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

        private void SetRawValueFromText()
        {
            if (Text == null)
            {
                Text = "";
            }
            string rawValue = "";
            int counterMeter = 0;
            foreach (var c in Text)
            {
                if ((c != Mask[counterMeter] || (GetCharacterType(Mask[counterMeter].ToString(), true) != CharacterType.Other && c == Mask[counterMeter] && GetCharacterType(Mask[counterMeter].ToString(), true) == GetCharacterType(c.ToString()))) && c != PlaceholderCharacter)
                {
                    rawValue += c.ToString();
                }
                counterMeter++;
            }

            //foreach (var c in Text)
            //{
            //    var type = GetCharacterType(c.ToString());
            //    if (type == CharacterType.Letter || type == CharacterType.Digit)
            //    {
            //        rawValue += c.ToString();
            //    }
            //}

            _rawValue = rawValue;
        }

        public string GetRawValue()
        {
            if (_rawValue == null)
            {
                return "";
            }
            return _rawValue;
        }

        //private string GetSemiRawText(string value)
        //{
        //    for (int i = 0; i < Mask.Length; i++)
        //    {
        //        int a = i;
        //        if (!MaskCharacters.ContainsKey(Mask[a]) && a <= value.Length)
        //        {
        //            value = value.Insert(a, Mask[a].ToString());
        //        }
        //    }

        //    //UpdateMaskSymbols();
        //    //foreach (var item in MaskSymbols)
        //    //{
        //    //    if (item.Key <= value.Length)
        //    //    {
        //    //        value = value.Insert(item.Key, item.Value.ToString());
        //    //    }
        //    //}

        //    return value;
        //}
        private void UpdateRawValueDictionary(bool insertLastPressedKey = false, string pastedText = null)
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
            if (insertLastPressedKey && !_rawValueDictionary.ContainsKey(_caretPosition) && !string.IsNullOrEmpty(_lastKeyDownCharacter)
                 && !(_lastKeyDownCharacter == "Backspace" || _lastKeyDownCharacter == "Delete"))
            {
                _rawValueDictionary.Add(_caretPosition, _lastKeyDownCharacter[0]);
            }
            if (string.IsNullOrEmpty(Text))
                return;

            if (KeepCharacterPositions)
            {
                for (int i = 0; i < Mask.Length; i++)
                {
                    int a = i;
                    if (Regex.IsMatch(Text[a].ToString(), "^[a-zA-Z0-9]$") && !_rawValueDictionary.ContainsKey(a))
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
                //foreach (var item in backUpDictionary)
                //{
                //    if (item.Key < _caretPosition)
                //        _rawValueDictionary.Add(item.Key, item.Value);
                //    else
                //        _rawValueDictionary.Add(item.Key + 1, item.Value);
                //}
                //for (int i = 0; i < Mask.Length; i++)
                //{
                //    int a = i;
                //    if (Regex.IsMatch(Text[a].ToString(), "^[a-zA-Z0-9]$") && !_rawValueDictionary.ContainsKey(a))
                //    {
                //        if (a < _caretPosition)
                //        {
                //            _rawValueDictionary.Add(a, Text[a]);
                //        }
                //        else
                //        {
                //            for (int i2 = 0; i + i2 < Text.Length - _caretPosition; i2++)
                //            {
                //                int a2 = i2;
                //                if (Text[_caretPosition + a + a2] == PlaceholderCharacter)
                //                    _rawValueDictionary.Add(a + a2, Text[a]);
                //            }         
                //        }
                //    }
                //}
            }

            if (_lastKeyDownCharacter == "Backspace")
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

            else if (_lastKeyDownCharacter == "Delete")
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

        private string GetRawValueFromRawMask(string rawText, string rawMask, bool addPlaceholderCharacters)
        {
            string result = "";
            for (int i = 0; i < rawMask.Length; i++)
            {
                int a = i;
                if (rawText.Length <= a)
                {
                    if (addPlaceholderCharacters)
                        result += PlaceholderCharacter;
                }
                else if (rawText[a] == PlaceholderCharacter)
                {
                    if (addPlaceholderCharacters)
                        result += PlaceholderCharacter;
                }
                else if (IsCharsMatch(rawText[a], rawMask[a]))
                {
                    result += rawText[a];
                }
                else
                {
                    if (addPlaceholderCharacters)
                        result += PlaceholderCharacter;
                }
            }
            return result;
        }

        private async Task UltimateImplementMask(bool insertLastPressedKey, string mask, string pastedText = null)
        {
            if (mask == null)
            {
                return;
            }

            //if (_rawValue == null)
            //{
            //    rawText = "";
            //}
            //else
            //{
            //    rawText = _rawValue;
            //}
            string result = "";

            UpdateCustomCharacters();
            UpdateRawValueDictionary(insertLastPressedKey, pastedText);
            string rawMask = GetRawMask();

            //result = GetRawValueFromRawMask(rawText, rawMask, false);
            //for (int i = 0; i < rawMask.Length; i++)
            //{
            //    int a = i;
            //    if (rawText.Length <= a)
            //    {
            //        result += PlaceholderCharacter;
            //    }
            //    else if (rawText[a] == PlaceholderCharacter)
            //    {
            //        result += PlaceholderCharacter;
            //    }
            //    else if (IsCharsMatch(rawText[a], rawMask[a]))
            //    {
            //        result += rawText[a];
            //    }
            //    else
            //    {
            //        result += PlaceholderCharacter;
            //    }
            //}

            //for (int i = 0; i < mask.Length; i++)
            //{
            //    else if (!MaskCharacters.ContainsKey(mask[a]))
            //    {
            //        result = result.Insert(a, mask[a].ToString());
            //    }
            //}
            bool hasValue = false;
            for (int i = 0; i < mask.Length; i++)
            {
                int a = i;
                if (!MaskCharacters.ContainsKey(mask[a]))
                {
                    result += mask[a].ToString();
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

        //private async Task ImplementMask(string rawText, string mask)
        //{
        //    if (mask == null)
        //    {
        //        return;
        //    }

        //    if (rawText == null)
        //    {
        //        rawText = "";
        //    }
        //    string maskedText = "";

        //    UpdateCustomCharacters();
        //    //Find raw mask by removing not masking characters(so remains isLetter, isDigit or custom(regex) key chars)
        //    string rawMask = GetRawMask();
        //    //Find raw text(we use reverse masking for all text)
        //    rawText = GetRawText(rawText);
        //    //Insert last pressed character into correct place(or pasted etc.)
        //    //Check raw mask and raw text char by char, and place underscore(or custom) character if its not match
        //    for (int i = 0; i < rawMask.Length; i++)
        //    {
        //        int a = i;
        //        if (rawText.Length < a)
        //        {
        //            maskedText += PlaceholderCharacter;
        //        }
        //        else if (rawText[a] == PlaceholderCharacter)
        //        {
        //            maskedText += PlaceholderCharacter;
        //        }
        //        else if (IsCharsMatch(rawText[a], rawMask[a]))
        //        {
        //            maskedText += rawText[a];
        //        }
        //        else
        //        {
        //            maskedText += PlaceholderCharacter;
        //        }
        //    }
        //    //Insert mask symbols on raw text
        //    for (int i = 0; i < mask.Length; i++)
        //    {
        //        int a = i;
        //        if (!MaskCharacters.ContainsKey(mask[a]))
        //        {
        //            maskedText = maskedText.Insert(a, mask[a].ToString());
        //        }
        //    }

        //    await SetTextAsync("", updateValue: false);
        //    //await _elementReference.SetText(maskedText);
        //}

        //private string _rawMask = "";

        private string GetRawText(string t)
        {
            if (t == null)
            {
                return null;
            }

            string rawText = "";
            foreach (var item in t)
            {
                if (item == PlaceholderCharacter)
                {
                    rawText += PlaceholderCharacter;
                }
                else if (Regex.IsMatch(item.ToString(), "^[a-zA-Z0-9]$"))
                {
                    rawText += item;
                }
            }
            for (int i = 0; i < GetRawMask().Length - t.Length; i++)
            {
                rawText += PlaceholderCharacter;
            }
            return rawText;
        }

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

        private string GetToBeMaskedText(KeyboardEventArgs obj)
        {
            string result = "";
            int insertPosition = 0;
            for (int i = 0; i < _caretPosition; i++)
            {
                int a = i;
                if (Regex.IsMatch(Text[a].ToString(), "^[a-zA-Z0-9]$"))
                {
                    insertPosition++;
                }
            }

            if (obj.Key == "Backspace")
            {
                //for (int i = 1; i < Text.Length; i++)
                //{
                //    if (_caretPosition - i < 0 || Text[_caretPosition - i] == PlaceholderCharacter)
                //        return Text;
                //    if (Regex.IsMatch(Text[_caretPosition - i].ToString(), "^[a-zA-Z0-9]$"))
                //    {
                //        result = Text.Remove(_caretPosition - i, 1).Insert(_caretPosition - i, PlaceholderCharacter.ToString());
                //        break;
                //    }
                //}
                if (0 < _rawValue.ToString().Length)
                {
                    result = _rawValue.Remove(insertPosition == 0 ? 0 : insertPosition - 1, 1);
                }
                else
                {
                    result = "";
                }
            }
            else if (obj.Key == "Delete")
            {
                if (Regex.IsMatch(Text[_caretPosition].ToString(), "^[a-zA-Z0-9]$"))
                {
                    result = Text.Remove(_caretPosition, 1).Insert(Text.Length - 1, PlaceholderCharacter.ToString());
                }
                else
                {
                    result = Text;
                }
            }
            else
            {
                //if (Text.Length <= _caretPosition)
                //{
                //    _caretPosition--;
                //}
                //result = Text.Remove(_caretPosition, 1).Insert(_caretPosition, _lastKeyDownCharacter);
                

                if (Text.Length <= _caretPosition)
                {
                    _caretPosition--;
                }

                if (_rawValue == null)
                {
                    result = _lastKeyDownCharacter;
                }
                else
                {
                    result = _rawValue.Insert(insertPosition, _lastKeyDownCharacter);
                }
                
            }

            return result;
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

        //private Dictionary<int, char> _maskSymbols = new Dictionary<int, char>();

        //private void UpdateMaskSymbols()
        //{
        //    _maskSymbols.Clear();

        //    for (int i = 0; i < Mask.Length; i++)
        //    {
        //        int a = i;
        //        if (GetCharacterType(Mask[a].ToString()) == CharacterType.Other)
        //        {
        //            _maskSymbols.Add(a, Mask[a]);
        //        }
        //    }
        //}

        private string GetRawMask()
        {
            //string rawMask = "";

            //foreach (var m in Mask)
            //{
            //    bool isMatch = false;

            //    foreach (var item in MaskCharacters)
            //    {
            //        if (m == item.Key)
            //        {
            //            isMatch = true;
            //            //break;
            //        }
            //    }

            //    if (isMatch == true)
            //    {
            //        rawMask += m.ToString();
            //    }
            //}

            //return rawMask;

            var rawMask = "";
            foreach (var c in from c in Mask
                              let isMatch = MaskCharacters.Any(m => m.Key == c)
                              where isMatch
                              select c)
            {
                rawMask += c.ToString();
            }

            return rawMask;
        }

        private int _caretPosition = 0;

        public void SetCaretPosition(int caretPosition)
        {
            _caretPosition = caretPosition;
            if (!_isFocused)
                return;
            _elementReference.SelectRangeAsync(_caretPosition, _caretPosition).AndForget();
            StateHasChanged();
        }

        protected internal async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            //if (obj.Key == "ArrowUp" || obj.Key == "ArrowDown" || obj.Key == "ArrowLeft" || obj.Key == "ArrowRight" || 
            //        obj.Key == "Shift" || obj.Key == "Ctrl" || obj.Key == "CapsLock" || obj.Key == "Tab" || obj.CtrlKey == true)
            //    return;
            if (obj.CtrlKey == true || (!Regex.IsMatch(obj.Key, "^[a-zA-Z0-9]$") && !(obj.Key == "Backspace" || obj.Key == "Delete")))
                return;
            _lastKeyDownCharacter = obj.Key;
            //SetRawValueFromText();
            //string _text = GetRawValue();

            //string toBeMaskedText = GetToBeMaskedText(obj);
            //await ImplementMask(toBeMaskedText, Mask);

            await SetValueAsync(Converter.Get(Text), true);

            //_rawValue = GetRawValueFromRawMask(_rawValue, GetRawMask(), false);
            SetRawValueFromText();
            //await UltimateImplementMask(toBeMaskedText, GetMaskByType());
            //_rawValue = GetRawValueFromText();
            //SetRawValueFromText();
            OnKeyDown.InvokeAsync(obj).AndForget();
            //await Task.Delay(1);
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

        private string _lastKeyDownCharacter = "";

    }
}
