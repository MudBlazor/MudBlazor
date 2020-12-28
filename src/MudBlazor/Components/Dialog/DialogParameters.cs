// Copyright (c) 2019 - Blazored
// Copyright (c) 2020 - Adaptations by Jonny Larsson and Meinrad Recheis

using System.Collections;
using System.Collections.Generic;

namespace MudBlazor
{
    public class DialogParameters : IEnumerable
    {
        internal Dictionary<string, object> _parameters;

        public DialogParameters()
        {
            _parameters = new Dictionary<string, object>();
        }

        public void Add(string parameterName, object value)
        {
            _parameters[parameterName] = value;
        }

        public T Get<T>(string parameterName)
        {
            if (_parameters.TryGetValue(parameterName, out var value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"{parameterName} does not exist in Dialog parameters");
        }

        public T TryGet<T>(string parameterName)
        {
            if (_parameters.TryGetValue(parameterName, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public object this[string parameterName]
        {
            get => Get<object>(parameterName);
            set => _parameters[parameterName] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }
    }
}
