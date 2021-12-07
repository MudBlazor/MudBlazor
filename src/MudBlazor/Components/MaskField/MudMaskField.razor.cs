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
    public partial class MudMaskField<T> : MudDebouncedInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        private MudInput<string> _elementReference;

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }

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
        public Dictionary<char, CharacterType> MaskCharacters { get; set; } = new() { 
            ['a'] = CharacterType.Letter,
            ['0'] = CharacterType.Digit,
            ['*'] = CharacterType.LetterOrDigit,
            
        };

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)] 
        public Regex CustomCharacterType { get; set; } = new Regex(@"^[a-zA-Z0-9]$");

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

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public string RawValue
        {
            get => _rawValue;
            set
            {
                if (_rawValue == value)
                    return;
                _rawValue = value;
                //UltimateImplementMask(_rawValue, Mask).AndForget();
                BindingValueChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public EventCallback<string> BindingValueChanged { get; set; }

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
                        new KeyOptions { Key="Escape" },
                        new KeyOptions { Key="/^[a-zA-Z0-9]$/", PreventDown = "key+none" },
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                        new KeyOptions { Key="Backspace", PreventDown = "key+none" },
                        new KeyOptions { Key="Delete", PreventDown = "key+none" },
                        new KeyOptions { Key="Shift", PreventDown = "key+none" },
                    },
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            await _elementReference.SetText(null);
        }

        //string val = "";

        private void GetCustomCharacters()
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

            RawValue = rawValue;
        }

        public string GetRawValue()
        {
            if (RawValue == null)
            {
                return "";
            }
            return RawValue;
        }

        private string GetSemiRawText(string value)
        {
            for (int i = 0; i < Mask.Length; i++)
            {
                int a = i;
                if (!MaskCharacters.ContainsKey(Mask[a]) && a <= value.Length)
                {
                    value = value.Insert(a, Mask[a].ToString());
                }
            }

            //UpdateMaskSymbols();
            //foreach (var item in MaskSymbols)
            //{
            //    if (item.Key <= value.Length)
            //    {
            //        value = value.Insert(item.Key, item.Value.ToString());
            //    }
            //}

            return value;
        }

        private async Task UltimateImplementMask(string RawText, string Mask)
        {
            if (Mask == null)
            {
                return;
            }

            if (RawText == null)
            {
                RawText = "";
            }

            GetCustomCharacters();
            string semiRawText = GetSemiRawText(RawText);
            string maskedText = "";

            for (int i = 0; i < Mask.Length; i++)
            {
                int a = i;
                if ((!MaskCharacters.ContainsKey(Mask[a])))
                {
                    maskedText += Mask[a];
                }
                else if (semiRawText.Length < a + 1)
                {
                    maskedText += PlaceholderCharacter.ToString();
                }
                else if (IsCharsMatch(semiRawText[a], Mask[a]))
                {
                    maskedText += semiRawText[a].ToString();
                }
                else
                {
                    maskedText += PlaceholderCharacter.ToString();
                }
            }
            await _elementReference.SetText(maskedText);
        }

        private bool IsCharsMatch(char TextChar, char MaskChar)
        {
            if (GetCharacterType(MaskChar.ToString(), true) == GetCharacterType(TextChar.ToString()))
                return true;
            if (GetCharacterType(MaskChar.ToString(), true) == CharacterType.LetterOrDigit && GetCharacterType(TextChar.ToString()) != CharacterType.Other)
                return true;
            //foreach (var item in CustomCharacterTypes)
            //{
            //    //if (GetCharacterType(item.Item1.ToString(), true) == CharacterType.Custom && item.Item2.IsMatch(TextChar.ToString()))
            //    //    return true;
            //    if (item.Item1 == MaskChar && item.Item2.IsMatch(TextChar.ToString()))
            //        return true;
            //}
            if (GetCharacterType(MaskChar.ToString(), true) == CharacterType.Custom)
            {
                foreach (var item in CustomCharacterTypes)
                {
                    if (item.Item1 == MaskChar && item.Item2.IsMatch(TextChar.ToString()))
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }

        private Dictionary<int, char> MaskSymbols = new Dictionary<int, char>();

        private void UpdateMaskSymbols()
        {
            MaskSymbols.Clear();

            for (int i = 0; i < Mask.Length; i++)
            {
                int a = i;
                if (GetCharacterType(Mask[a].ToString()) == CharacterType.Other)
                {
                    MaskSymbols.Add(a, Mask[a]);
                }
            }
        }

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
            _elementReference.SelectRangeAsync(_caretPosition, _caretPosition);
        }

        protected async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            _lastKeyDownCharacter = obj.Key;

            if (obj.Key == "ArrowUp" || obj.Key == "ArrowDown" || obj.Key == "ArrowLeft" || obj.Key == "ArrowRight" || obj.Key == "Shift" || obj.Key == "CapsLock")
                return;

            SetRawValueFromText();
            string _text = GetRawValue();

            string toBeMaskedText = "";
            if (obj.Key == "Backspace")
            {
                if (1 <= RawValue.Length)
                {
                    toBeMaskedText = _text.Substring(0, _text.Length - 1);
                }
            }
            else
            {
                toBeMaskedText = _text + _lastKeyDownCharacter;
            }
            //await ImplementMask(Text, Mask);
            await UltimateImplementMask(toBeMaskedText, GetMaskByType());
            SetRawValueFromText();

            OnKeyDown.InvokeAsync(obj).AndForget();
            await Task.Delay(1);
            SetCaretPosition(Text.Length);
        }

        private string _lastKeyDownCharacter = "";
    }
}
