using System;
using System.Collections.Generic;

namespace MudBlazor.Services
{
#nullable enable
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
        public Dictionary<Breakpoint, int>? BreakpointDefinitions { get; set; } = new();

        public static bool operator ==(ResizeOptions? l, ResizeOptions? r)
        {
            if (ReferenceEquals(l, r))
                return true;
            if (ReferenceEquals(l, null))
                return false;
            if (ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }

        public static bool operator !=(ResizeOptions l, ResizeOptions r) => !(l == r);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ResizeOptions options && Equals(options);

        /// <inheritdoc />
        public bool Equals(ResizeOptions? other)
        {
            if (other is null)
                return false;
            if (ReportRate != other.ReportRate ||
               EnableLogging != other.EnableLogging ||
               SuppressInitEvent != other.SuppressInitEvent ||
               NotifyOnBreakpointOnly != other.NotifyOnBreakpointOnly)
            {
                return false;
            }
            if (BreakpointDefinitions is null)
                return other.BreakpointDefinitions is null;
            if (other.BreakpointDefinitions is null)
                return false;
            if (BreakpointDefinitions.Count != other.BreakpointDefinitions.Count)
                return false;
            foreach (var item in BreakpointDefinitions.Keys)
            {
                if (other.BreakpointDefinitions.ContainsKey(item) == false)
                    return false;
                if (BreakpointDefinitions[item] != other.BreakpointDefinitions[item])
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            // ReSharper disable NonReadonlyMemberInGetHashCode
            hashCode.Add(ReportRate);
            hashCode.Add(EnableLogging);
            hashCode.Add(SuppressInitEvent);
            hashCode.Add(NotifyOnBreakpointOnly);
            hashCode.Add(ReportRate);
            if (BreakpointDefinitions is not null)
            {
                foreach (var pair in BreakpointDefinitions)
                {
                    hashCode.Add(pair.Key);
                    hashCode.Add(pair.Value);
                }
            }
            // ReSharper restore NonReadonlyMemberInGetHashCode

            return hashCode.ToHashCode();
        }
    }
}
