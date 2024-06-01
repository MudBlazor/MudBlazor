// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/MudBlazor/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored

using System;

namespace MudBlazor
{
    /// <summary>
    /// Represents the result of a user's interaction with a <see cref="MudDialog"/>.
    /// </summary>
    public class DialogResult
    {
        /// <summary>
        /// The data includes with the result.
        /// </summary>
        /// <remarks>
        /// This value is typically a custom object related to the dialog, such as the object which will be affect by the user's response.
        /// </remarks>
        public object Data { get; }

        /// <summary>
        /// The type of object in the <see cref="Data"/> property.
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// Indicates whether the user clicked a <c>Cancel</c> button.
        /// </summary>
        public bool Canceled { get; }

        protected internal DialogResult(object data, Type resultType, bool canceled)
        {
            Data = data;
            DataType = resultType;
            Canceled = canceled;
        }

        /// <summary>
        /// The result when the user clicks the <c>Ok</c> button.
        /// </summary>
        /// <typeparam name="T">The type of result included.</typeparam>
        /// <param name="result">The value included.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Ok<T>(T result) => Ok(result, default);

        /// <summary>
        /// The result when the user clicks the <c>Ok</c> button.
        /// </summary>
        /// <typeparam name="T">The type of result included.</typeparam>
        /// <param name="result">The value included.</param>
        /// <param name="dialogType">The type of dialog which the user responded to.</param>
        /// <returns>The dialog result.</returns>
        public static DialogResult Ok<T>(T result, Type dialogType) => new(result, dialogType, false);

        /// <summary>
        /// The result when the user clicks the <c>Cancel</c> button.
        /// </summary>
        /// <returns>The dialog result.</returns>
        public static DialogResult Cancel() => new(default, typeof(object), true);
    }
}
