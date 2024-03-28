// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MudBlazor.Docs.WasmHost.Controllers
{
    public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is not OperationCanceledException)
            {
                return;
            }

            context.Result = new StatusCodeResult(499);
            context.ExceptionHandled = true;
        }
    }
}
