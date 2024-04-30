// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal;

[Flags]
public enum DiagnosticFieldReportOptions
{
    None = 0x0,
    ReportOnReturnType = 0x1,
}

[Flags]
public enum DiagnosticInvocationReportOptions
{
    None = 0x0,
    ReportOnMember = 0x1,
}

[Flags]
public enum DiagnosticMethodReportOptions
{
    None = 0x0,
    ReportOnMethodName = 0x1,
    ReportOnReturnType = 0x2,
}

[Flags]
public enum DiagnosticParameterReportOptions
{
    None = 0x0,
    ReportOnType = 0x1,
}

[Flags]
public enum DiagnosticPropertyReportOptions
{
    None = 0x0,
    ReportOnReturnType = 0x1,
}
