// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    public class NaturalComparerTest
    {

        Func<string, string> _expand = (s) =>
        {
            int o; while ((o = s.IndexOf('\\')) != -1)
            {
                var p = o + 1;
                var z = 1; while (s[p] == '0') { z++; p++; }
                var c = int.Parse(s.Substring(p, z));
                s = s.Substring(0, o) + new string(s[o - 1], c) + s.Substring(p + z);
            }
            return s;
        };

        private static string s_encodedFileNames =
                "KDEqLW4xMiotbjEzKjAwMDFcMDY2KjAwMlwwMTcqMDA5XDAxNyowMlwwMTcqMDlcMDE3KjEhKjEtISox" +
                "LWEqMS4yNT8xLjI1KjEuNT8xLjUqMSoxXDAxNyoxXDAxOCoxXDAxOSoxXDA2NioxXDA2NyoxYSoyXDAx" +
                "NyoyXDAxOCo5XDAxNyo5XDAxOCo5XDA2Nio9MSphMDAxdGVzdDAxKmEwMDF0ZXN0aW5nYTBcMzEqYTAw" +
                "Mj9hMDAyIGE/YTAwMiBhKmEwMDIqYTAwMmE/YTAwMmEqYTAxdGVzdGluZ2EwMDEqYTAxdnNmcyphMSph" +
                "MWEqYTF6KmEyKmIwMDAzcTYqYjAwM3E0KmIwM3E1KmMtZSpjZCpjZipmIDEqZipnP2cgMT9oLW4qaG8t" +
                "bipJKmljZS1jcmVhbT9pY2VjcmVhbT9pY2VjcmVhbS0/ajBcNDE/ajAwMWE/ajAxP2shKmsnKmstKmsx" +
                "KmthKmxpc3QqbTAwMDNhMDA1YSptMDAzYTAwMDVhKm0wMDNhMDA1Km0wMDNhMDA1YSpuMTIqbjEzKm8t" +
                "bjAxMypvLW4xMipvLW40P28tbjQhP28tbjR6P28tbjlhLWI1Km8tbjlhYjUqb24wMTMqb24xMipvbjQ/" +
                "b240IT9vbjR6P29uOWEtYjUqb245YWI1Km/CrW4wMTMqb8KtbjEyKnAwMCpwMDEqcDAxwr0hKnAwMcK9" +
                "KnAwMcK9YSpwMDHCvcK+KnAwMipwMMK9KnEtbjAxMypxLW4xMipxbjAxMypxbjEyKnItMDAhKnItMDAh" +
                "NSpyLTAwIe+8lSpyLTAwYSpyLe+8kFwxIS01KnIt77yQXDEhLe+8lSpyLe+8kFwxISpyLe+8kFwxITUq" +
                "ci3vvJBcMSHvvJUqci3vvJBcMWEqci3vvJBcMyE1KnIwMCEqcjAwLTUqcjAwLjUqcjAwNSpyMDBhKnIw" +
                "NSpyMDYqcjQqcjUqctmg2aYqctmkKnLZpSpy27Dbtipy27Qqctu1KnLfgN+GKnLfhCpy34UqcuClpuCl" +
                "rCpy4KWqKnLgpasqcuCnpuCnrCpy4KeqKnLgp6sqcuCppuCprCpy4KmqKnLgqasqcuCrpuCrrCpy4Kuq" +
                "KnLgq6sqcuCtpuCtrCpy4K2qKnLgrasqcuCvpuCvrCpy4K+qKnLgr6sqcuCxpuCxrCpy4LGqKnLgsasq" +
                "cuCzpuCzrCpy4LOqKnLgs6sqcuC1puC1rCpy4LWqKnLgtasqcuC5kOC5lipy4LmUKnLguZUqcuC7kOC7" +
                "lipy4LuUKnLgu5UqcuC8oOC8pipy4LykKnLgvKUqcuGBgOGBhipy4YGEKnLhgYUqcuGCkOGClipy4YKU" +
                "KnLhgpUqcuGfoOGfpipy4Z+kKnLhn6UqcuGgkOGglipy4aCUKnLhoJUqcuGlhuGljCpy4aWKKnLhpYsq" +
                "cuGnkOGnlipy4aeUKnLhp5UqcuGtkOGtlipy4a2UKnLhrZUqcuGusOGutipy4a60KnLhrrUqcuGxgOGx" +
                "hipy4bGEKnLhsYUqcuGxkOGxlipy4bGUKnLhsZUqcuqYoFwx6pilKnLqmKDqmKUqcuqYoOqYpipy6pik" +
                "KnLqmKUqcuqjkOqjlipy6qOUKnLqo5UqcuqkgOqkhipy6qSEKnLqpIUqcuqpkOqplipy6qmUKnLqqZUq" +
                "cvCQkqAqcvCQkqUqcvCdn5gqcvCdn50qcu+8kFwxISpy77yQXDEt77yVKnLvvJBcMS7vvJUqcu+8kFwx" +
                "YSpy77yQXDHqmKUqcu+8kFwx77yO77yVKnLvvJBcMe+8lSpy77yQ77yVKnLvvJDvvJYqcu+8lCpy77yV" +
                "KnNpKnPEsSp0ZXN02aIqdGVzdNmi2aAqdGVzdNmjKnVBZS0qdWFlKnViZS0qdUJlKnVjZS0xw6kqdWNl" +
                "McOpLSp1Y2Uxw6kqdWPDqS0xZSp1Y8OpMWUtKnVjw6kxZSp3ZWlhMSp3ZWlhMip3ZWlzczEqd2Vpc3My" +
                "KndlaXoxKndlaXoyKndlacOfMSp3ZWnDnzIqeSBhMyp5IGE0KnknYTMqeSdhNCp5K2EzKnkrYTQqeS1h" +
                "Myp5LWE0KnlhMyp5YTQqej96IDA1MD96IDIxP3ohMjE/ejIwP3oyMj96YTIxP3rCqTIxP1sxKl8xKsKt" +
                "bjEyKsKtbjEzKsSwKg==";


        private static string[] s_orderedFileNames = new string[]
        {
            "_1.txt"
            ,"-n12.txt"
            ,"-n13.txt"
            ,"(1.txt"
            ,"[1.txt"
            ,"=1.txt"
            ,"1-!.txt"
            ,"1-a.txt"
            ,"1!.txt"
            ,"1.5"
            ,"1.5.txt"
            ,"1.25"
            ,"1.25.txt"
            ,"1.txt"
            ,"1a.txt"
            ,"111111111111111111.txt"
            ,"00222222222222222222.txt"
            ,"0222222222222222222.txt"
            ,"222222222222222222.txt"
            ,"00999999999999999999.txt"
            ,"0999999999999999999.txt"
            ,"999999999999999999.txt"
            ,"1111111111111111111.txt"
            ,"2222222222222222222.txt"
            ,"9999999999999999999.txt"
            ,"11111111111111111111.txt"
            ,"0001111111111111111111111111111111111111111111111111111111111111111111.txt"
            ,"1111111111111111111111111111111111111111111111111111111111111111111.txt"
            ,"9999999999999999999999999999999999999999999999999999999999999999999.txt"
            ,"11111111111111111111111111111111111111111111111111111111111111111111.txt"
            ,"a001test01.txt"
            ,"a001testinga00001.txt"
            ,"a01testinga001.txt"
            ,"a01vsfs.txt"
            ,"a1.txt"
            ,"a1a.txt"
            ,"a1z.txt"
            ,"a002"
            ,"a002 a"
            ,"a002 a.txt"
            ,"a002.txt"
            ,"a002a"
            ,"a002a.txt"
            ,"a2.txt"
            ,"b0003q6.txt"
            ,"b003q4.txt"
            ,"b03q5.txt"
            ,"c-e.txt"
            ,"cd.txt"
            ,"cf.txt"
            ,"f 1.txt"
            ,"f.txt"
            ,"g"
            ,"g 1"
            ,"h-n.txt"
            ,"ho-n.txt"
            ,"I.txt"
            ,"İ.txt"
            ,"ice-cream"
            ,"icecream"
            ,"icecream-"
            ,"j000001"
            ,"j001a"
            ,"j01"
            ,"k-.txt"
            ,"k!.txt"
            ,"k'.txt"
            ,"k1.txt"
            ,"ka.txt"
            ,"list.txt"
            ,"m0003a005a.txt"
            ,"m003a0005a.txt"
            ,"m003a005.txt"
            ,"m003a005a.txt"
            ,"n12.txt"
            ,"­n12.txt"
            ,"n13.txt"
            ,"­n13.txt"
            ,"o-n4"
            ,"o-n4!"
            ,"o-n4z"
            ,"o-n9a-b5.txt"
            ,"o-n9ab5.txt"
            ,"o-n12.txt"
            ,"o-n013.txt"
            ,"on4"
            ,"on4!"
            ,"on4z"
            ,"on9a-b5.txt"
            ,"on9ab5.txt"
            ,"on12.txt"
            ,"o­n12.txt"
            ,"on013.txt"
            ,"o­n013.txt"
            ,"p00.txt"
            ,"p0½.txt"
            ,"p01.txt"
            ,"p01½!.txt"
            ,"p01½.txt"
            ,"p01½¾.txt"
            ,"p01½a.txt"
            ,"p02.txt"
            ,"q-n12.txt"
            ,"q-n013.txt"
            ,"qn12.txt"
            ,"qn013.txt"
            ,"r-００００!5.txt"
            ,"r-００!-5.txt"
            ,"r-００!-５.txt"
            ,"r-00!.txt"
            ,"r-００!.txt"
            ,"r-00!5.txt"
            ,"r-00!５.txt"
            ,"r-００!5.txt"
            ,"r-００!５.txt"
            ,"r-00a.txt"
            ,"r-００a.txt"
            ,"r𐒠.txt"
            ,"r𝟘.txt"
            ,"r00-5.txt"
            ,"r００-５.txt"
            ,"r00!.txt"
            ,"r００!.txt"
            ,"r00.5.txt"
            ,"r００.５.txt"
            ,"r００．５.txt"
            ,"r００꘥.txt"
            ,"r00a.txt"
            ,"r００a.txt"
            ,"r4.txt"
            ,"r٤.txt"
            ,"r۴.txt"
            ,"r߄.txt"
            ,"r४.txt"
            ,"r৪.txt"
            ,"r੪.txt"
            ,"r૪.txt"
            ,"r୪.txt"
            ,"r௪.txt"
            ,"r౪.txt"
            ,"r೪.txt"
            ,"r൪.txt"
            ,"r๔.txt"
            ,"r໔.txt"
            ,"r༤.txt"
            ,"r၄.txt"
            ,"r႔.txt"
            ,"r៤.txt"
            ,"r᠔.txt"
            ,"r᥊.txt"
            ,"r᧔.txt"
            ,"r᭔.txt"
            ,"r᮴.txt"
            ,"r᱄.txt"
            ,"r᱔.txt"
            ,"r꘤.txt"
            ,"r꣔.txt"
            ,"r꤄.txt"
            ,"r꩔.txt"
            ,"r４.txt"
            ,"r005.txt"
            ,"r꘠꘠꘥.txt"
            ,"r００５.txt"
            ,"r05.txt"
            ,"r꘠꘥.txt"
            ,"r０５.txt"
            ,"r5.txt"
            ,"r٥.txt"
            ,"r۵.txt"
            ,"r߅.txt"
            ,"r५.txt"
            ,"r৫.txt"
            ,"r੫.txt"
            ,"r૫.txt"
            ,"r୫.txt"
            ,"r௫.txt"
            ,"r౫.txt"
            ,"r೫.txt"
            ,"r൫.txt"
            ,"r๕.txt"
            ,"r໕.txt"
            ,"r༥.txt"
            ,"r၅.txt"
            ,"r႕.txt"
            ,"r៥.txt"
            ,"r᠕.txt"
            ,"r᥋.txt"
            ,"r᧕.txt"
            ,"r᭕.txt"
            ,"r᮵.txt"
            ,"r᱅.txt"
            ,"r᱕.txt"
            ,"r꘥.txt"
            ,"r꣕.txt"
            ,"r꤅.txt"
            ,"r꩕.txt"
            ,"r５.txt"
            ,"r06.txt"
            ,"r٠٦.txt"
            ,"r۰۶.txt"
            ,"r߀߆.txt"
            ,"r०६.txt"
            ,"r০৬.txt"
            ,"r੦੬.txt"
            ,"r૦૬.txt"
            ,"r୦୬.txt"
            ,"r௦௬.txt"
            ,"r౦౬.txt"
            ,"r೦೬.txt"
            ,"r൦൬.txt"
            ,"r๐๖.txt"
            ,"r໐໖.txt"
            ,"r༠༦.txt"
            ,"r၀၆.txt"
            ,"r႐႖.txt"
            ,"r០៦.txt"
            ,"r᠐᠖.txt"
            ,"r᥆᥌.txt"
            ,"r᧐᧖.txt"
            ,"r᭐᭖.txt"
            ,"r᮰᮶.txt"
            ,"r᱀᱆.txt"
            ,"r᱐᱖.txt"
            ,"r꘠꘦.txt"
            ,"r꣐꣖.txt"
            ,"r꤀꤆.txt"
            ,"r꩐꩖.txt"
            ,"r０６.txt"
            ,"r𐒥.txt"
            ,"r𝟝.txt"
            ,"si.txt"
            ,"sı.txt"
            ,"test٢.txt"
            ,"test٣.txt"
            ,"test٢٠.txt"
            ,"uAe-.txt"
            ,"uae.txt"
            ,"ube-.txt"
            ,"uBe.txt"
            ,"uce-1é.txt"
            ,"ucé-1e.txt"
            ,"uce1é-.txt"
            ,"ucé1e-.txt"
            ,"uce1é.txt"
            ,"ucé1e.txt"
            ,"weia1.txt"
            ,"weia2.txt"
            ,"weiss1.txt"
            ,"weiß1.txt"
            ,"weiss2.txt"
            ,"weiß2.txt"
            ,"weiz1.txt"
            ,"weiz2.txt"
            ,"y a3.txt"
            ,"y a4.txt"
            ,"y-a3.txt"
            ,"y-a4.txt"
            ,"y'a3.txt"
            ,"y'a4.txt"
            ,"y+a3.txt"
            ,"y+a4.txt"
            ,"ya3.txt"
            ,"ya4.txt"
            ,"z"
            ,"z 21"
            ,"z 050"
            ,"z!21"
            ,"z©21"
            ,"z20"
            ,"z22"
            ,"za21"
        };

        /// <summary>
        /// Test if comparer works as intended
        /// </summary>
        [Test]
        public void SortFiles()
        {
            var fileNames = Encoding.UTF8.GetString(Convert.FromBase64String(s_encodedFileNames))
                .Replace("*", ".txt?").Split(new[] { "?" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => _expand(n)).ToArray();

            var orderedFiles = fileNames.OrderBy(x => x, new NaturalComparer()).ToArray();

            orderedFiles.Should().NotBeEmpty();
            orderedFiles.Should().ContainInOrder(s_orderedFileNames);
        }

        /// <summary>
        /// Test if Comparer may receive non string, but comparable types
        /// </summary>
        [Test]
        public void SortNonStringButComparableTypes()
        {
            var items = new List<int>() { 1, 8, 7, 9, 63, 4, 0 };
            var orderedItems = items.OrderBy(x => x, new NaturalComparer());

            orderedItems.Should().NotBeNull();
            orderedItems.Should().NotBeEmpty();

            var expectedOrder = new int[] { 0, 1, 4, 7, 8, 9, 63 };
            orderedItems.Should().ContainInOrder(expectedOrder);
        }

        private record NonEquatable(int Index, string Value)
        {
            public override string ToString()
            {
                return Index.ToString() + Value;
            }
        }

        /// <summary>
        /// Test if Comparer may receive non string that do not implement Equals
        /// </summary>
        [Test]
        public void SortNonEquatable()
        {
            var items = new List<NonEquatable>() { new NonEquatable(1, "un"), new NonEquatable(3, "trois"), new NonEquatable(2, "deux") };

            var orderedItems = items.OrderBy(x => x, new NaturalComparer()).Select(x => x.ToString());

            orderedItems.Should().NotBeNull();
            orderedItems.Should().NotBeEmpty();

            var expectedOrders = new string[] { "1un", "2deux", "3trois" };
            orderedItems.Should().ContainInOrder(expectedOrders);
        }

        /// <summary>
        /// Test when there are null values sent to be compared
        /// </summary>
        [Test]
        public void NullTest()
        {
            var naturalComparer = new NaturalComparer();

            naturalComparer.Compare(null, null).Should().Be(0);
            naturalComparer.Compare(null, 1).Should().Be(-1);
            naturalComparer.Compare(1, null).Should().Be(1);
        }

    }
}
