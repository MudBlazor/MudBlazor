// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MudBlazor
{
    public class Field
        : IField
    {
        public string Name { get; }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type Type { get; }

        private Field(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            Name = name;
            Type = type;
        }

        public static IField Create(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            return new Field(name, type);
        }

        public static IField Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(FilterDefinition<T> filedDefinition)
        {
            return new Field(filedDefinition.Field, filedDefinition.FieldType);
        }

        public static IField Create<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(Expression<Func<T, TValue>> fieldExpression)
        {
            return new Field(fieldExpression.GetPropertyInfo().Name, typeof(TValue));
        }

        public static IField Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(Expression<Func<TValue>> fieldExpression)
        {
            return new Field(fieldExpression.GetPropertyInfo().Name, typeof(TValue));
        }
    }
}
