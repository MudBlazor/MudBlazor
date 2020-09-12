using System;
using System.ComponentModel;

namespace MudBlazor
{
    public struct GridSize
    {
        public static bool False = false;
        public static string Auto = "auto";
        public static bool True = true;
        public static int One = 1;
        public static int Two = 2;
        public static int Three = 3;
        public static int Four = 4;
        public static int Five = 5;
        public static int Six = 6;
        public static int Seven = 7;
        public static int Eight = 8;
        public static int Nine = 9;
        public static int Ten = 10;
        public static int Eleven = 11;
        public static int Twelve = 12;

        private GridSize(object value)
        {
            Value = value;
        }

        public object Value;

        public override string ToString()
        {
            if (Value == null)
                return "";
            return Value.ToString();
        }

        public static implicit operator GridSize(bool b) => new GridSize(b);
        public static implicit operator GridSize(int i)
        {
            if (i < 1)
                return new GridSize(1);
            if (i > 12)
                return new GridSize(12);
            return new GridSize(i);
        }

        public static implicit operator GridSize(string s)
        {
            if (s!="auto")
                return new GridSize("auto");
            return new GridSize(s);
        }
    }


}
