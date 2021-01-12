using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor
{
    public class Range<T>
    {
        public T Start { get; set; }

        public T End { get; set; }

        public Range()
        {

        }

        public Range(T start, T end)
        {
            Start = start;
            End = end;
        }

        public override bool Equals(object obj)
        {
            return obj is Range<T> r && r.Start.Equals(Start) && r.End.Equals(End);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
