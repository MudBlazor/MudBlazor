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
                        new KeyOptions { Key="Backspace", PreventDown = "none" },
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
            return RawValue;
        }

        private async Task ImplementMask(string RawText, string Mask)
        {
            if (RawText == null)
            {
                RawText = "";
            }

            string returnedText = "";
            char[] textArray = RawText.ToCharArray();
            char[] maskArray = Mask.ToCharArray();
            int timer = 0;
            foreach (char c in maskArray)
            {
                if (GetCharacterType(c.ToString(), true) == CharacterType.Other)
                {
                    returnedText += c.ToString();
                }
                else
                {
                    if (timer < RawText.Length)
                    {
                        returnedText += textArray[timer].ToString();
                    }
                    else
                    {
                        returnedText += "_";
                    }
                }
                timer++;
            }
            await _elementReference.SetText(returnedText);
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
            string _text = "";
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
                        _text += c.ToString();
                    }
                }
                //_text = Text;
            }

            string specialMaskCharacters = "";
            char[] maskArray = Mask.ToCharArray();
            List<char> textArray = _text.ToCharArray().ToList();
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

            if (textArray.Count + 1 < maskArray.Length)
            {
                if (CharacterType.Other == GetCharacterType(maskArray[textArray.Count + 1].ToString()))
                {
                    specialMaskCharacters += maskArray[textArray.Count + 1].ToString();
                }
            }

            //Prepare for masking: Find Text and Mask values.
            //char[] maskArray = Mask.ToCharArray();
            //List<char> textArray = new List<char>();
            //textArray = _text.ToCharArray().ToList();
            //string keyNumericOrLetter = "numeric";
            //string maskNumericOrLetter = "";
            //if (Regex.IsMatch(obj.Key, @"[a-zA-Z]"))
            //{
            //    keyNumericOrLetter = "letter";
            //}
            //else if (Regex.IsMatch(obj.Key, @"[0-9]"))
            //{
            //    keyNumericOrLetter = "numeric";
            //}
            //else
            //{
            //    keyNumericOrLetter = "none";
            //}

            //int textLength = textArray.Count;
            //if (0 <= _text.Length && _text.Length <= Mask.Length)
            //{
            //    if (maskArray[textArray.Count] == 's')
            //    {
            //        maskNumericOrLetter = "letter";
            //    }
            //    else if (maskArray[textArray.Count] == 'n')
            //    {
            //        maskNumericOrLetter = "numeric";
            //    }
            //    else if (maskArray[textArray.Count] == 'c')
            //    {
            //        maskNumericOrLetter = "both";
            //    }
            //    else
            //    {
            //        maskNumericOrLetter = "none";
            //    }
            //}

            ////Implement mask
            ////First check if the user deletes character
            //if (obj.Key == "Backspace")
            //{
            //    if (0 < Text.Length)
            //    {
            //        int a = 0;
            //        for (int i = 0; i < Text.Length; i++)
            //        {
            //            //if (Text.Length < a + 1)
            //            //{
            //            //    break;
            //            //}
            //            a = i + 1;

            //            string s = CheckCharacterType(-a);
            //            if (s == null)
            //            {
            //                break;
            //            }
            //            else if (s != "none")
            //            {
            //                break;
            //            }
            //        }

            //        int firstNoneCharacterCount = 0;
            //        for (int i = 1; i < Text.Length; i++)
            //        {
            //            string s0 = CheckCharacterType(firstNoneCharacterCount);
            //            if (s0 == "none")
            //            {
            //                firstNoneCharacterCount++;
            //            }
            //            else
            //            {
            //                break;
            //            }
            //        }

            //        if (Text.Length - a <= 0 || Text.Length <= firstNoneCharacterCount)
            //        {
            //            Text = "";
            //        }
            //        else
            //        {
            //            //Text = Text.Substring(0, Text.Length - a + 1);
            //        }
            //        //_key++;
            //        StateHasChanged();
            //        await Task.Delay(1);
            //        await _elementReference.FocusAsync();
            //    }
            //}
            //else
            //{
            //    if (_text.Length <= Mask.Length)
            //    {
            //        if (!string.IsNullOrEmpty(Mask))
            //        {
            //            //Check special characters
            //            bool isSpecialCharacter = false;
            //            if (keyNumericOrLetter == "none")
            //            {
            //                for (int i = 0; i < Text.Length; i++)
            //                {
            //                    int a = i;
            //                    string s = CheckCharacterType(a);
            //                    if (s == null)
            //                    {
            //                        break;
            //                    }
            //                    else if (s != "none")
            //                    {

            //                    }
            //                    else if (s == keyNumericOrLetter)
            //                    {
            //                        isSpecialCharacter = true;
            //                        break;
            //                    }
            //                }
            //            }

            //            string rawText = "";
            //            if (keyNumericOrLetter == maskNumericOrLetter || isSpecialCharacter || (_text.Length == 1 && Regex.IsMatch(maskArray[0].ToString(), @"[()+-.]")))
            //            {
            //                foreach (char c in textArray)
            //                {
            //                    if (!Regex.IsMatch(c.ToString(), @"[.()+-,]"))
            //                    {
            //                        rawText += c.ToString();
            //                    }
            //                }

            //                char[] rawTextChar = rawText.ToCharArray();

            //                //find special char pairs
            //                Dictionary<int, char> specialPair = new Dictionary<int, char>();
            //                for (int i = 0; i < maskArray.Length; i++)
            //                {
            //                    int a = i;
            //                    if (Regex.IsMatch(maskArray[a].ToString(), @"[.()+-,]"))
            //                    {
            //                        specialPair.Add(a, maskArray[a]);
            //                    }

            //                }

            //                //insert special characters
            //                foreach (var i in specialPair)
            //                {
            //                    if (i.Key <= rawText.Length)
            //                    {
            //                        rawText = rawText.Insert(i.Key, i.Value.ToString());
            //                    }
            //                }

            //                //await SetTextAsync(rawText, true);
            //                //_key++;
            //                //StateHasChanged();
            //                //await Task.Delay(1);
            //                //await _elementReference.FocusAsync();
            //            }
            //            else
            //            {
            //                //Text = Text.Substring(0, Text.Length - 1);
            //                //_key++;
            //                StateHasChanged();
            //                await Task.Delay(1);
            //                await _elementReference.FocusAsync();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //Text = Text.Substring(0, Text.Length - 1);
            //        //_key++;
            //        StateHasChanged();
            //        await Task.Delay(1);
            //        await _elementReference.FocusAsync();
            //    }
            //}
            //await SetTextAsync(Text.Substring(0, Text.Length - 1), true);

            //implement mask
            if (_isCharacterTypeMatch == true && obj.Key != "Backspace")
            {
                await _elementReference.SetText(_text + _lastKeyDownCharacter + specialMaskCharacters);
            }
            await ImplementMask(Text, Mask);
            SetRawValue();
            
            OnKeyDown.InvokeAsync(obj).AndForget();
            _isCharacterTypeMatch = false;
            specialMaskCharacters = "";
            SetCaretPosition();
        }

        private string _lastKeyDownCharacter = "";
        private bool _isCharacterTypeMatch = false;
    }
}
