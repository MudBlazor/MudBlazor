using System;
using System.Collections.Generic;

namespace MudBlazor.Services
{
    public class ResizeOptions : IEquatable<ResizeOptions>
    {
        /// <summary>
        /// Rate in milliseconds that the browsers `resize()` event should report a change.
        /// Setting this value too low can cause poor application performance.
        /// </summary>
        public int ReportRate { get; set; } = 100;

        /// <summary>
        /// Report resize events and media queries in the browser's console.
        /// </summary>
        public bool EnableLogging { get; set; } = false;

        /// <summary>
        /// Suppress the first OnResized that is invoked when a new event handler is added.
        /// </summary>
        public bool SuppressInitEvent { get; set; } = true;

        /// <summary>
        /// If true, RaiseOnResized is called only when breakpoint has changed.
        /// </summary>
        public bool NotifyOnBreakpointOnly { get; set; } = true;

        /// <summary>
        /// Breakpoint definitions.
        /// </summary>
        public Dictionary<string, int> BreakpointDefinitions { get; set; } = new();

        public static bool operator ==(ResizeOptions l, ResizeOptions r) => l.Equals(r);
        public static bool operator !=(ResizeOptions l, ResizeOptions r) => !l.Equals(r);

        public override bool Equals(object obj)
        {
            if (obj is not ResizeOptions) { return false; }

            return Equals((ResizeOptions)obj);
        }

        public bool Equals(ResizeOptions other)
        {
            if (ReportRate != other.ReportRate ||
               EnableLogging != other.EnableLogging ||
               SuppressInitEvent != other.SuppressInitEvent ||
               NotifyOnBreakpointOnly != other.NotifyOnBreakpointOnly)
            {
                return false;
            }

            if (BreakpointDefinitions is not null)
            {
                if (other.BreakpointDefinitions is null) { return false; }
                else
                {
                    if (BreakpointDefinitions.Count != other.BreakpointDefinitions.Count)
                    {
                        return false;
                    }

                    foreach (var item in BreakpointDefinitions.Keys)
                    {
                        if (other.BreakpointDefinitions.ContainsKey(item) == false)
                        {
                            return false;
                        }

                        if (BreakpointDefinitions[item] != other.BreakpointDefinitions[item])
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                return other.BreakpointDefinitions is null;
            }
        }

        public override int GetHashCode() => ReportRate;
    }
}
