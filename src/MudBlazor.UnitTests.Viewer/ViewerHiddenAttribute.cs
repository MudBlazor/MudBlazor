using System;

namespace MudBlazor.UnitTests;

/// <summary>
/// Marks a viewer test component as routable but hidden from the sidebar listing.
/// Use for helper or sub-components (e.g. dialog content shown via the dialog service)
/// that are not meaningful to open on their own.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewerHiddenAttribute : Attribute
{
}
