// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace MudBlazor
{
    internal class State
    {
        private string AnimationId { get; }
        public bool UserHasInteracted { get; set; }
        public ToastOptions Options { get; }
        public ToastState ToastState { get; set; }
        public DateTime TransitionStartTime { get; set; }

        public State(ToastOptions options)
        {
            Options = options;
            AnimationId = $"toaster-{Guid.NewGuid()}";
            ToastState = ToastState.Init;
        }

        private string Opacity => ((decimal) Options.MaximumOpacity / 100).ToPercentage();
        public bool ShowCloseIcon => Options.ShowCloseIcon;
        public string ProgressBarClass => Options.ProgressBarClass;
        public string CloseIconClass => Options.CloseIconClass;
        public bool ShowProgressBar => Options.ShowProgressBar && ToastState.IsVisible() && !Options.RequireInteraction;

        public bool EscapeHtml => Options.EscapeHtml;
        public string ToastTitleClass => Options.ToastTitleClass;
        public string ToastMessageClass => Options.ToastMessageClass;

        public string ProgressBarStyle
        {
            get
            {
                var duration = RemainingTransitionMilliseconds(Options.VisibleStateDuration);
                return $"width:100;animation:{AnimationId} {duration}ms;";
            }
        }

        public string AnimationStyle
        {
            get
            {
                const string template = "opacity: {0}; animation: {1}ms linear {2};";
                switch (ToastState)
                {
                    case ToastState.Showing:
                        var showingDuration = RemainingTransitionMilliseconds(Options.ShowTransitionDuration);
                        return string.Format(template, Opacity, showingDuration, AnimationId);
                    case ToastState.Hiding:
                        var hidingDuration = RemainingTransitionMilliseconds(Options.HideTransitionDuration);
                        return string.Format(template, 0, hidingDuration, AnimationId);
                    case ToastState.MouseOver:
                        return "opacity: 1;";
                    case ToastState.Visible:
                        return $"opacity: {Opacity};";
                    default:
                        return string.Empty;
                }
            }
        }

        public string ToastClass
        {
            get
            {
                var forceCursor = Options.ShowCloseIcon ? "" : " force-cursor";
                return $"{Options.ToastClass} {Options.ToastTypeClass}{forceCursor}";
            }
        }

        public string TransitionClass
        {
            get
            {
                var template = "@keyframes " + AnimationId + " {{from{{ {0}: {1}; }} to{{ {0}: {2}; }}}}";

                switch (ToastState)
                {
                    case ToastState.Showing:
                        return string.Format(template, "opacity", 0, Opacity);
                    case ToastState.Hiding:
                        return string.Format(template, "opacity", Opacity, 0);
                    case ToastState.Visible:
                        return string.Format(template, "width", "100%", "0%");
                    default:
                        return string.Empty;
                }
            }
        }

        private int RemainingTransitionMilliseconds(int transitionDuration)
        {
            var duration = transitionDuration - (TransitionStartTime - DateTime.Now).Milliseconds;
            return duration >= 0 ? duration : 0;
        }
    }
}
