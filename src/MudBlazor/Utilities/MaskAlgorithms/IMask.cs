// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
    /// <summary>
    /// Implements input masking features for a mask.
    /// </summary>
    public interface IMask
    {
        /// <summary>
        /// The characters which define the accepted input.
        /// </summary>
        string Mask { get; }

        /// <summary>
        /// The current text displayed in the input, including delimiter and placeholder characters.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The current text in the input, excluding delimiter or placeholder characters.
        /// </summary>
        string GetCleanText() => Text;

        /// <summary>
        /// The current cursor position.
        /// </summary>
        int CaretPos { get; set; }

        /// <summary>
        /// The start and end range of selected characters.
        /// </summary>
        (int, int)? Selection { get; set; }

        /// <summary>
        /// Inserts text at the current cursor position.
        /// </summary>
        /// <param name="input">The characters to insert.</param>
        void Insert(string input);

        /// <summary>
        /// Triggers a delete at the current cursor position.
        /// </summary>
        /// <remarks>
        /// Has the same effect as pressing the <c>Delete</c> key.
        /// </remarks>
        void Delete();

        /// <summary>
        /// Triggers a backspace at the current cursor position.
        /// </summary>
        /// <remarks>
        /// Has the same effect as pressing the <c>Backspace</c> key.
        /// </remarks>
        void Backspace();

        /// <summary>
        /// Clears the text and selection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Overwrites the text without losing the cursor position.
        /// </summary>
        /// <param name="text">The text to set.</param>
        void SetText(string text);

        /// <summary>
        /// Copies the mask and mask characters from the specified mask.
        /// </summary>
        /// <param name="other">The mask to copy from.</param>
        void UpdateFrom(IMask other);
    }
}
