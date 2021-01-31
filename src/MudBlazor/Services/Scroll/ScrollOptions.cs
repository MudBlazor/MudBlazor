using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Services.Scroll
{
    public class ScrollOptions
    {
        /// <summary>
        ///the element to be tracked;
        /// </summary>        
        public ElementReference Element { get; set; }

        /// <summary>
        ///the ammount of milliseconds that the event is throttled 
        /// </summary>        
        public int ReportRate { get; set; } = 300;

        /// <summary>
        /// Suppress the first OnScroll that is invoked when a new event handler is added.
        /// </summary>
        public bool SuppressInitEvent { get; set; }
    }
}
