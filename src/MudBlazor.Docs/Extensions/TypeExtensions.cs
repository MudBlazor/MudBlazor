using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Extensions
{
    internal static partial class TypeExtensions
    {
        /// <summary>Converts a <see cref="Type"/> into a <see cref="string"/> as it would appear in C# source code.</summary>
        /// <param name="type">The <see cref="Type"/> to convert to a <see cref="string"/>.</param>
        /// <param name="showGenericParameters">If the generic parameters are the generic types, whether they should be shown or not.</param>
        /// <returns>The <see cref="string"/> as the <see cref="Type"/> would appear in C# source code.</returns>
        public static string ConvertToCSharpSource(this Type type, bool showGenericParameters = false)
        {
            var genericParameters = new Queue<Type>();
            foreach (var x in type.GetGenericArguments())
                genericParameters.Enqueue(x);
            return ConvertToCsharpSource(type);

            string ConvertToCsharpSource(Type type)
            {
                _ = type ?? throw new ArgumentNullException(nameof(type));
                var result = type.IsNested
                    ? ConvertToCsharpSource(type.DeclaringType) + "."
                    : ""; //: type.Namespace + ".";
                result += BacktickRegularExpression().Replace(type.Name, string.Empty);
                if (type.IsGenericType)
                {
                    result += "<";
                    var firstIteration = true;
                    foreach (var generic in type.GetGenericArguments())
                    {
                        if (genericParameters.Count <= 0)
                        {
                            break;
                        }
                        var correctGeneric = genericParameters.Dequeue();
                        result += (firstIteration ? string.Empty : ",") +
                                  (correctGeneric.IsGenericParameter
                                      ? (showGenericParameters ? (firstIteration ? string.Empty : " ") + correctGeneric.Name : string.Empty)
                                      : (firstIteration ? string.Empty : " ") + ConvertToCSharpSource(correctGeneric));
                        firstIteration = false;
                    }
                    result += ">";
                }
                return result;
            }
        }

        [GeneratedRegex("`.*")]
        private static partial Regex BacktickRegularExpression();
    }
}
