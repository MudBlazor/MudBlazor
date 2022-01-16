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
    public class PlaceholderMask
    {
        public PlaceholderMask() 
        {
            _maskDict = _maskChars.ToDictionary(x => x.Char);
        }

        private Dictionary<char, MaskChar> _maskDict = new Dictionary<char, MaskChar>();
        private MaskChar[] _maskChars = new MaskChar[]
        {
            MaskChar.Letter('a'),
            MaskChar.Digit('0'),
            MaskChar.LetterOrDigit('*'),
            new MaskChar { Char = 'l', AddToValue = false, Regex = "^[a-zıöüşçğ]$" },
            new MaskChar { Char = 'u', AddToValue = false, Regex = "^[A-ZİÖÜŞÇĞ]$" },
        };

        /// <summary>
        /// The input mask, for instance aa-000.00
        /// </summary>
        public string Mask{ get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Set your new MaskChar[] definition. MaskChar's can be used for masking characters. By default 'a' for letter, '0' for digit, '*' for letter or digit characters. You can also add custom parameters and Regex patterns.
        /// </summary>
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
        /// Sets the empty-mask character.
        /// </summary>
        public char PlaceholderCharacter { get; set; } = '_';

        /// <summary>
        /// If false, characters move previous or next location when insert or delete a character.
        /// </summary>
        public bool KeepCharacterPositions { get; set; } = false;

        public int CaretPosition { get; set; } = 0;
        public (int, int)? Selection { get; set; } = null;

        private Dictionary<int, char> _rawValueDictionary = new();

        public string GetRawValueFromDictionary()
        {
            string rawValue = "";
            if (_rawValueDictionary.Count == 0)
                return "";
            for (int i = 0; i < Mask.Length; i++)
            {
                int a = i;
                if (_rawValueDictionary.ContainsKey(a))
                {
                    rawValue += _rawValueDictionary[a];
                }
                else if (_maskDict.ContainsKey(Mask[a]) && _maskDict[Mask[a]].AddToValue == true)
                {
                    rawValue += Mask[a];
                }
            }
            return rawValue;
        }

        //Create main dictionary with given string (not affect value directly)
        public void SetRawValueDictionary(string value)
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

        public string GetRawValueFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string result = "";
            for (int i = 0; i < text.Length; i++)
            {
                int a = i;
                if (Regex.IsMatch(text[a].ToString(), @"^(\p{L}|\d)$") && Mask[a] != text[a])
                {
                    result += text[a].ToString();
                }
            }
            return result;
        }

        public async void Paste(string text, int caretPosition)
        {
            var val = GetRawValueFromDictionary();
            int lastAddedIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                for (int i2 = caretPosition; i + i2 < Mask.Length; i2++)
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
            await ImplementMask(null, false);
        }

        public bool IsCharsMatch(char textChar, char maskChar)
        {
            if (_maskDict.ContainsKey(maskChar) && Regex.IsMatch(textChar.ToString(), _maskDict[maskChar].Regex))
                return true;
            return false;
        }

        public void UpdateRawValueDictionary(string lastPressedKey = null)
        {
            //If there is a selection, first delete the selected characters and move the after ones if keep position is false
            //If we have selection, we think the text cannot be null, so we didn't have null check
            #region Selection
            if (Selection != null && Selection.Value.Item1 != Selection.Value.Item2)
            {
                for (int i = Selection.Value.Item1; i < Selection.Value.Item2; i++)
                {
                    if (_rawValueDictionary.ContainsKey(i))
                    {
                        _rawValueDictionary.Remove(i);
                    }
                }
                if (KeepCharacterPositions == false && Text != null)
                {
                    for (int i = 0; i < Text.Length - Selection.Value.Item2; i++)
                    {
                        if (_rawValueDictionary.ContainsKey(Selection.Value.Item2 + i))
                        {
                            for (int i2 = 0; Selection.Value.Item1 + i2 < Text.Length; i2++)
                            {
                                if (_maskDict.ContainsKey(Mask[Selection.Value.Item1 + i2]) && !_rawValueDictionary.ContainsKey(Selection.Value.Item1 + i2))
                                {
                                    _rawValueDictionary.Add(Selection.Value.Item1 + i2, _rawValueDictionary[Selection.Value.Item2 + i]);
                                    _rawValueDictionary.Remove(Selection.Value.Item2 + i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

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
            if (lastPressedKey != null && !_rawValueDictionary.ContainsKey(CaretPosition) && !string.IsNullOrEmpty(lastPressedKey)
                 && !(lastPressedKey == "Backspace" || lastPressedKey == "Delete"))
            {
                if (string.IsNullOrEmpty(Text))
                {
                    _rawValueDictionary.Add(CaretPosition, lastPressedKey[0]);
                    return;
                }

                for (int i = 0; i < Text.Length - CaretPosition; i++)
                {
                    if (KeepCharacterPositions)
                    {
                        if (Text[CaretPosition + i] == PlaceholderCharacter)
                        {
                            _rawValueDictionary.Add(CaretPosition + i, lastPressedKey[0]);
                            break;
                        }
                    }
                    else
                    {
                        if (_maskDict.ContainsKey(Mask[CaretPosition + i]))
                        {
                            _rawValueDictionary.Add(CaretPosition + i, lastPressedKey[0]);
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
                        if (i < CaretPosition)
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
            if (lastPressedKey == "Backspace" && Selection == null)
            {
                if (KeepCharacterPositions)
                {
                    for (int i = 1; i < Text.Length; i++)
                    {
                        if (CaretPosition - i < 0)
                            break;
                        if (Text[CaretPosition - i] == PlaceholderCharacter)
                            break;
                        if (_rawValueDictionary.ContainsKey(CaretPosition - i))
                        {
                            _rawValueDictionary.Remove(CaretPosition - i);
                            break;
                        }
                    }
                }
                else
                {
                    //Check is there any remaining value, otherwise it always removes the first char
                    bool hasValueBefore = false;
                    int removedIndex = 0;
                    for (int i = 0; i < (Text.Length - (Text.Length - CaretPosition)); i++)
                    {
                        removedIndex++;
                        if (_maskDict.ContainsKey(Mask[CaretPosition - removedIndex]) && _maskDict[Mask[CaretPosition - removedIndex]].AddToValue == false)
                        {
                            hasValueBefore = true;
                            _rawValueDictionary.Remove(CaretPosition - removedIndex);
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
                        if (CaretPosition - removedIndex <= item.Key)
                        {
                            for (int i = 1; i < Text.Length; i++)
                            {
                                if (item.Key - i < 0)
                                {
                                    break;
                                }
                                if (_maskDict.ContainsKey(Mask[item.Key - i]) && _maskDict[Mask[item.Key - i]].AddToValue == false)
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
            else if (lastPressedKey == "Delete" && Selection == null)
            {
                if (KeepCharacterPositions == true)
                {
                    for (int i = 0; i < Text.Length; i++)
                    {
                        int a = i;
                        if (CaretPosition + a < 0)
                            break;
                        if (Text[CaretPosition + a] == PlaceholderCharacter)
                            break;
                        if (_rawValueDictionary.ContainsKey(CaretPosition + a))
                        {
                            _rawValueDictionary.Remove(CaretPosition + a);
                            break;
                        }
                    }
                }
                else
                {
                    int? removedPosition = null;
                    for (int i = 0; i < Text.Length; i++)
                    {
                        if (Text.Length <= CaretPosition + i)
                            break;
                        if (Text[CaretPosition + i] == PlaceholderCharacter)
                            break;
                        if (_rawValueDictionary.ContainsKey(CaretPosition + i))
                        {
                            _rawValueDictionary.Remove(CaretPosition + i);
                            removedPosition = CaretPosition + i;
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
                                for (int i2 = 0; 0 < removedPosition + i - i2; i2++)
                                {
                                    if (_maskDict.ContainsKey(Mask[(int)removedPosition + i - i2 - 1]) && _maskDict[Mask[(int)removedPosition + i - i2 - 1]].AddToValue == false)
                                    {
                                        _rawValueDictionary.Remove((int)removedPosition + i);
                                        _rawValueDictionary.Add((int)removedPosition + i - i2 - 1, c);
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

        internal async void HandleKeyDown(string key, bool shiftKey, bool ctrlKey)
        {
            if ((key == "ArrowLeft" || key == "ArrowRight") && shiftKey == false)
            {
                ArrangeCaretPosition(key);
                return;
            }
            if (ctrlKey == true || (!Regex.IsMatch(key, @"^(\p{L}|\d)$") &&
                !(key == "Backspace" || key == "Delete")))
                return;
            if (CaretPosition == Mask.Length && !(key == "Backspace" || key == "Delete"))
                return;
            Console.WriteLine($"HandleKeyDown: '{key}'");
            await ImplementMask(key);
            await SetValueAsync?.Invoke(GetRawValueFromDictionary());

            if (key == "Backspace" || key == "Delete")
            {
                ArrangeCaretPosition(key);
            }
            else
            {
                if (Text == null)
                    return;
                for (int i = CaretPosition; i < Text.Length; i++)
                {
                    if (IsMatchAt(i, Text))
                    {
                        ArrangeCaretPosition(key);
                        break;
                    }
                }
            }
        }

        public bool IsMatchAt(int i, string text)
        {            
            return _rawValueDictionary.ContainsKey(i) && IsCharsMatch(text[i], Mask[i]);
        }

        public async Task ImplementMask(string lastPressedKey, bool updateCharactersOrDictionary=true)
        {
            var mask = Mask;
            if (mask == null)
                return;

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
                if (!_maskDict.ContainsKey(mask[i]) || (_maskDict.ContainsKey(mask[i]) && _maskDict[mask[i]].AddToValue == true))
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

            await SetTextAsync?.Invoke(result);
        }

        public Func<string, Task> SetTextAsync { get; set; }
        public Func<string, Task> SetValueAsync { get; set; }

        public void ArrangeCaretPosition(string key)
        {
            switch (key)
            {
                case "ArrowLeft":
                    if (0 < CaretPosition)
                    {
                        SetCaretPosition?.Invoke(FindPreviousCaretLocation(CaretPosition, false));
                    }
                    break;
                case "ArrowRight":
                    if (CaretPosition < Text.Length)
                    {
                        SetCaretPosition?.Invoke(FindNextCaretLocation(CaretPosition, false));
                    }
                    break;
                case "Backspace":
                    SetCaretPosition?.Invoke(FindPreviousCaretLocation(CaretPosition, false));
                    break;
                case "Delete":
                    SetCaretPosition?.Invoke(CaretPosition);
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
                        SetCaretPosition?.Invoke(FindNextCaretLocation(CaretPosition));
                    }
                    else
                    {
                        SetCaretPosition?.Invoke(FindNextCaretLocation(CaretPosition, false));
                    }
                    break;
            }
        }

        public Action<int> SetCaretPosition { get; set; }

        internal int FindNextCaretLocation(int currentCaretIndex, bool onlyPlaceholderCharacter = true)
        {
            if (Text == null || Text.Length == 0)
                return 0;
            for (int i = currentCaretIndex; i < Mask.Length; i++)
            {
                if (Text.Length <= i + 1)
                    return Mask.Length;
                if (onlyPlaceholderCharacter && Text[i + 1] == PlaceholderCharacter ||
                            !onlyPlaceholderCharacter && _maskDict.ContainsKey(Mask[i + 1]))
                    return i + 1;
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
                    if (_maskDict.ContainsKey(Mask[i - 1]) && _maskDict[Mask[i - 1]].AddToValue == false)
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
    }
}
