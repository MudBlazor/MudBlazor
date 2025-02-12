using System;
using System.Reflection;
using System.Text;

namespace MudBlazor.Docs.Extensions
{
    // Adaptation from : https://stackoverflow.com/questions/1312166/print-full-signature-of-a-method-from-a-methodinfo/1312321
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Return the method signature as a string.
        /// </summary>
        /// <param name="method">The Method</param>
        /// <param name="callable">Return as an callable string(public void a(string b) would return a(b))</param>
        /// <returns>Method signature</returns>
        public static string GetSignature(this MethodInfo method, bool callable = false)
        {
            // Define local variables
            var firstParameter = true;
            var secondParameter = false;
            var stringBuilder = new StringBuilder();

            // Define the method access
            if (callable == false)
            {
                // Append return type
                stringBuilder.Append(RemoveNamespace(TypeName(method.ReturnType)));
                stringBuilder.Append(' ');
            }

            // Add the name of the method
            stringBuilder.Append(method.Name);

            // Add generics method
            if (method.IsGenericMethod)
            {
                stringBuilder.Append('<');

                foreach (var genericArgument in method.GetGenericArguments())
                {
                    if (firstParameter)
                    {
                        firstParameter = false;
                    }
                    else
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(TypeName(genericArgument));
                }

                stringBuilder.Append('>');
            }

            stringBuilder.Append('(');
            firstParameter = true;

            foreach (var parameter in method.GetParameters())
            {
                if (firstParameter)
                {
                    firstParameter = false;

                    if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
                    {
                        if (callable)
                        {
                            secondParameter = true;
                            continue;
                        }
                        stringBuilder.Append("this ");
                    }
                }
                else if (secondParameter)
                {
                    secondParameter = false;
                }
                else
                {
                    stringBuilder.Append(", ");
                }

                if (parameter.ParameterType.IsByRef)
                {
                    stringBuilder.Append("ref ");
                }
                else if (parameter.IsOut)
                {
                    stringBuilder.Append("out ");
                }

                if (!callable)
                {
                    stringBuilder.Append(TypeName(parameter.ParameterType));
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append(parameter.Name);
            }

            stringBuilder.Append(')');

            // Return final result
            return stringBuilder.ToString();
        }

        public static string GetAliases(string value, Type type = null)
        {
            switch (value.ToUpperInvariant())
            {
                case "STRING": return "string";
                case "INT16": return "short";
                case "INT32": return "int";
                case "INT64": return "long";
                case "INTPTR": return "nint";
                case "UINT16": return "ushort";
                case "UINT32": return "uint";
                case "UINT64": return "ulong";
                case "UINTPTR": return "nuint";
                case "DOUBLE": return "double";
                case "DECIMAL": return "decimal";
                case "OBJECT": return "object";
                case "VOID": return string.Empty;
                case "BOOLEAN": return "bool";
                case "SBYTE": return "sbyte";
                case "CHAR": return "char";
                case "FLOAT": return "float";
                default:
                    {
                        if (type != null)
                        {
                            return string.IsNullOrWhiteSpace(type.FullName) ? RemoveNamespace(type.Name) : RemoveNamespace(type.FullName);
                        }
                        else
                        {
                            return RemoveNamespace(value);
                        }
                    }
            }
        }

        /// <summary>
        /// Get full type name with full namespace names
        /// </summary>
        /// <param name="type">Type. May be generic or nullable</param>
        /// <returns>Full type name, fully qualified namespaces</returns>
        private static string TypeName(Type type)
        {
            var first = true;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
            {
                return RemoveNamespace(nullableType.Name + "?");
            }

            if (!(type.IsGenericType && type.Name.Contains('`')))
            {
                return GetAliases(type.Name.ToUpperInvariant(), type);
            }

            var stringBuilder = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
            stringBuilder.Append('<');

            foreach (var t in type.GetGenericArguments())
            {
                if (!first)
                {
                    stringBuilder.Append(',');
                }
                stringBuilder.Append(TypeName(t));
                first = false;
            }
            stringBuilder.Append('>');

            // Return result
            return RemoveNamespace(stringBuilder.ToString());
        }

        private static string RemoveNamespace(string value)
        {
            return value.Split('.')[value.Split('.').Length - 1];
        }
    }
}
