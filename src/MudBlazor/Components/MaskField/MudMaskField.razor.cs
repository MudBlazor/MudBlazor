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
        [Parameter] public InputType InputType { get; set; } = InputType.Text;

        [Parameter] public Dictionary<char, CharacterType> MaskCharacters { get; set; } = new() { 
            ['a'] = CharacterType.Letter,
            ['0'] = CharacterType.Digit,
            ['*'] = CharacterType.LetterOrDigit,
        };

        internal string RawValue { get; set; }

        internal override InputType GetInputType() => InputType;

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter] public bool Clearable { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        [Parameter] public string Mask { get; set; } = "";

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
                    },
                });
            }
            //await ImplementMask(Text, Mask);
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

        string val = "";

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

        private void SetRawValue()
        {
            string rawValue = "";

            foreach (var c in Text)
            {
                var type = GetCharacterType(c.ToString());
                if (type == CharacterType.Letter || type == CharacterType.Digit)
                {
                    rawValue += c.ToString();
                }
            }

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

        private async Task ImplementMask(string RawText, string Mask)
        {
            if (RawText == null)
            {
                RawText = "";
            }

            int rawMaskLength = GetRawMask().Length;
            int rawTextLength = RawText.Length;

            UpdateMaskSymbols();
            string remainingChars = "";
            for (int i = 0; i < (rawMaskLength - rawTextLength) ; i++)
            {
                remainingChars += "_";
            }
            string midTermText = RawText + remainingChars;

            foreach (var item in MaskSymbols)
            {
                string s = item.Value.ToString();
                midTermText = midTermText.Insert(item.Key, s);
            }

            //string returnedText = "";
            //char[] textArray = RawText.ToCharArray();
            //char[] maskArray = Mask.ToCharArray();
            //int timer = 0;
            //foreach (char c in maskArray)
            //{
            //    if (GetCharacterType(c.ToString(), true) == CharacterType.Other)
            //    {
            //        returnedText += c.ToString();
            //    }
            //    else
            //    {
            //        if (timer < RawText.Length)
            //        {
            //            returnedText += textArray[timer].ToString();
            //        }
            //        else
            //        {
            //            returnedText += "_";
            //        }
            //    }
            //    timer++;
            //}
            await _elementReference.SetText(midTermText);
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
            string rawMask = "";

            foreach (var m in Mask)
            {
                bool isMatch = false;

                foreach (var item in MaskCharacters)
                {
                    if (m == item.Key)
                    {
                        isMatch = true;
                        //break;
                    }
                }

                if (isMatch == true)
                {
                    rawMask += m.ToString();
                }
            }

            return rawMask;
        }

        private void SetTextFromRawText(string rawText)
        {
            UpdateMaskSymbols();

            foreach (var item in MaskSymbols)
            {

            }
        }

        public void SetCaretPosition()
        {
            char[] textArray = RawValue.ToCharArray();
            char[] maskArray = Mask.ToCharArray();
            int timer = 0;
            int findingNum = 0;
            foreach (char c in maskArray)
            {
                if (GetCharacterType(c.ToString(), true) == CharacterType.Other)
                {
                    
                }
                else
                {
                    if (timer < RawValue.Length)
                    {
                        
                    }
                    else
                    {
                        findingNum = timer;
                        break;
                    }
                }
                timer++;
            }
            _elementReference.SelectRangeAsync(findingNum, findingNum);
        }

        private string CheckCharacterType(int i)
        {
            string _text = "";
            if (Text != null)
            {
                _text = Text;
            }
            char[] maskArray = Mask.ToCharArray();
            List<char> textArray = new List<char>();

            if (0 < _text.Length && _text.Length <= Mask.Length && textArray.Count + i <= maskArray.Length)
            {
                if (maskArray[textArray.Count + i] == 's')
                {
                    return "letter";
                }
                else if (maskArray[textArray.Count + i] == 'n')
                {
                    return "numeric";
                }
                else if (maskArray[textArray.Count + i] == 'c')
                {
                    return "both";
                }
                else
                {
                    return "none";
                }
            }
            else
            {
                return null;
            }
        }

        public void MaskTheText(KeyboardEventArgs e)
        {
            char[] maskCharArray = Mask.ToCharArray();
            //find key char is numeric or letter
            string keyNumericOrLetter = "numeric";
            string valNumericOrLetter = "numeric";
            if (Regex.IsMatch(e.Key, @"[a-z][A-Z]"))
            {
                keyNumericOrLetter = "letter";
            }
            else if (Regex.IsMatch(e.Key, @"[0-9]"))
            {
                keyNumericOrLetter = "numeric";
            }
            else
            {
                keyNumericOrLetter = "none";
            }

            //find raw text
            string rawTextInMask = "";
            char[] maskedCharArray = Text.ToCharArray();

            //prevent key if not match type
            int textLength = maskedCharArray.Length;
            if (maskCharArray[maskedCharArray.Length - 1] == 's')
            {
                valNumericOrLetter = "letter";
            }
            else if (maskCharArray[maskedCharArray.Length - 1] == 'n')
            {
                valNumericOrLetter = "numeric";
            }
            else if (maskCharArray[maskedCharArray.Length - 1] == 'c')
            {
                valNumericOrLetter = "both";
            }

            if (keyNumericOrLetter == valNumericOrLetter)
            {
                foreach (char c in maskedCharArray)
                {
                    if (c != ' ' && c != '(' && c != ')' && c != '-')
                    {
                        rawTextInMask += c.ToString();
                    }
                }
                val = "";
                string rawText = rawTextInMask;

                char[] rawTextChar = rawText.ToCharArray();
                string resultMaskedString = rawText;

                //find special char numbers
                List<int> allSpecialNo = new List<int>();
                List<int> whiteSpaceNo = new List<int>();
                List<int> parantheseOpenNo = new List<int>();
                List<int> parantheseCloseNo = new List<int>();
                List<int> hyphenNo = new List<int>();
                for (int i = 0; i < maskCharArray.Length; i++)
                {
                    if (maskCharArray[i] == '.')
                    {
                        allSpecialNo.Add(i);
                        whiteSpaceNo.Add(i);
                    }
                    else if (maskCharArray[i] == '(')
                    {
                        allSpecialNo.Add(i);
                        parantheseOpenNo.Add(i);
                    }
                    else if (maskCharArray[i] == ')')
                    {
                        allSpecialNo.Add(i);
                        parantheseCloseNo.Add(i);
                    }
                    else if (maskCharArray[i] == '-')
                    {
                        allSpecialNo.Add(i);
                        hyphenNo.Add(i);
                    }

                }

                //insert special characters
                foreach (int i in allSpecialNo)
                {
                    if (whiteSpaceNo.Contains<int>(i) && i <= resultMaskedString.Length)
                    {
                        resultMaskedString = resultMaskedString.Insert(i, " ");
                    }
                    else if (parantheseOpenNo.Contains<int>(i) && i <= resultMaskedString.Length)
                    {
                        resultMaskedString = resultMaskedString.Insert(i, "(");
                    }
                    else if (parantheseCloseNo.Contains<int>(i) && i <= resultMaskedString.Length)
                    {
                        resultMaskedString = resultMaskedString.Insert(i, ")");
                    }
                    else if (hyphenNo.Contains<int>(i) && i <= resultMaskedString.Length)
                    {
                        resultMaskedString = resultMaskedString.Insert(i, "-");
                    }
                }

                val = resultMaskedString;
            }
            else
            {
                //backup
            }



            //need lost focus to show masked value (it dont show automatically)
            //focus again component to continuous keypress
        }

        int _key = 0;

        protected async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            _lastKeyDownCharacter = obj.Key;

            if (obj.Key == "ArrowUp" || obj.Key == "ArrowDown" || obj.Key == "ArrowLeft" || obj.Key == "ArrowRight")
                return;

            SetRawValue();
            string _text = GetRawValue();

            string _semiRawText = "";
            if (Text != null)
            {
                char[] rawArray = Text.ToCharArray();
                foreach (char c in rawArray)
                {
                    if (c == '_')
                    {
                        break;
                    }
                    else
                    {
                        _semiRawText += c.ToString();
                    }
                }
                //_text = Text;
            }

            //string specialMaskCharacters = "";
            char[] maskArray = Mask.ToCharArray();
            List<char> textArray = _semiRawText.ToCharArray().ToList();
            CharacterType keyType = GetCharacterType(obj.Key);
            try
            {
                CharacterType maskType = GetCharacterType(maskArray[textArray.Count].ToString(), true);
                if (keyType == maskType)
                {
                    _isCharacterTypeMatch = true;
                }
            }
            catch (Exception)
            {
                
            }

            //implement mask
            if (_isCharacterTypeMatch == true && obj.Key != "Backspace")
            {
                //await _elementReference.SetText(_text + _lastKeyDownCharacter);
                Text = _text +_lastKeyDownCharacter;
            }
            else
            {
                Text = _text;
            }
            if (obj.Key == "Backspace")
            {
                if (1 <= RawValue.Length)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
            }
            await ImplementMask(Text, Mask);
            SetRawValue();

            OnKeyDown.InvokeAsync(obj).AndForget();
            _isCharacterTypeMatch = false;
            SetCaretPosition();
        }

        private string _lastKeyDownCharacter = "";
        private bool _isCharacterTypeMatch = false;
    }
}
