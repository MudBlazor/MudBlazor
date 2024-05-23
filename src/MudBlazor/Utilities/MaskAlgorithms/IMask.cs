// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public interface IMask
    {
        /// <summary>
        /// The mask defining the structure of the accepted input. 
        /// Its format depends on the implementation.
        /// </summary>
        string Mask { get; }

        /// <summary>
        /// The current text as it is displayed in the component
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Get the Text without delimiters or placeholders. Depends on the implementation entirely.
        /// Clean text will usually be used for the Value property of a mask field. 
        /// </summary>
        string GetCleanText() => Text;

        /// <summary>
        /// The current caret position
        /// </summary>
        int CaretPos { get; set; }

        /// <summary>
        /// The currently selected sub-section of the Text
        /// </summary>
        (int, int)? Selection { get; set; }

        /// <summary>
        /// Implements user input at the current caret position (single key strokes or pasting longer text)
        /// </summary>
        /// <param name="input"></param>
        void Insert(string input);

        /// <summary>
        /// Implements the effect of the Del key at the current cursor position
        /// </summary>
        void Delete();

        /// <summary>
        /// Implements the effect of the Backspace key at the current cursor position
        /// </summary>
        void Backspace();

        /// <summary>
        /// Reset the mask as if the whole textfield was cleared
        /// </summary>
        void Clear();

        /// <summary>
        /// Overwrite the mask text without losing caret position
        /// </summary>
        /// <param name="text"></param>
        void SetText(string text);

        /// <summary>
        /// Copy config from other mask but preserve own state.
        /// </summary>
        /// <param name="other"></param>
        void UpdateFrom(IMask other);

    }
}
