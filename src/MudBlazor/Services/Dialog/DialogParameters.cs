// Copyright (c) 2019 - Blazored
// Copyright (c) 2020 - Adaptations by Jonny Larsson and Meinrad Recheis

using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// The parameters passed into a <see cref="MudDialog"/> instance.
    /// </summary>
    public class DialogParameters : IEnumerable<KeyValuePair<string, object?>>
    {
        /// <summary>
        /// The default dialog parameters.
        /// This field is only intended for parameters that do not differ from their default values.
        /// </summary>
        internal static readonly DialogParameters Default = new();

        internal Dictionary<string, object?> _parameters = new();

        /// <summary>
        /// Adds or updates a parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value to add or update.</param>
        public void Add(string parameterName, object? value)
        {
            _parameters[parameterName] = value;
        }

        /// <summary>
        /// Gets an existing parameter.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="parameterName">The name of the parameter to find.</param>
        /// <returns>The parameter value, if it exists.</returns>
        public T? Get<T>(string parameterName)
        {
            if (_parameters.TryGetValue(parameterName, out var value))
            {
                return (T?)value;
            }

            throw new KeyNotFoundException($"{parameterName} does not exist in Dialog parameters");
        }

        /// <summary>
        /// Gets an existing parameter or a default value if nothing was found.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="parameterName">The name of the parameter to find.</param>
        /// <returns>The parameter value, if it exists.</returns>
        public T? TryGet<T>(string parameterName)
        {
            if (_parameters.TryGetValue(parameterName, out var value))
            {
                return (T?)value;
            }

            return default;
        }

        /// <summary>
        /// The number of parameters.
        /// </summary>
        public int Count => _parameters.Count;

        /// <summary>
        /// Gets or sets a parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to find.</param>
        /// <returns>The parameter value.</returns>
        public object? this[string parameterName]
        {
            get => Get<object?>(parameterName);
            set => _parameters[parameterName] = value;
        }

        /// <summary>
        /// Gets an enumerator for all parameters.
        /// </summary>
        /// <returns>An enumerator of <see cref="KeyValuePair{TKey, TValue}"/> values.</returns>
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }
    }
}
