//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;

namespace MudBlazor
{
    internal class State
    {
        private string AnimationId { get; }
        public bool UserHasInteracted { get; set; }
        public SnackbarOptions Options { get; }
        public SnackbarState SnackbarState { get; set; }
        public DateTime TransitionStartTime { get; set; }

        public State(SnackbarOptions options)
        {
            Options = options;
            AnimationId = $"snackbar-{Guid.NewGuid()}";
            SnackbarState = SnackbarState.Init;
        }
        private string Opacity => ((decimal)Options.MaximumOpacity / 100).ToPercentage();
        public bool ShowCloseIcon => Options.ShowCloseIcon;

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
                switch (SnackbarState)
                {
                    case SnackbarState.Showing:
                        var showingDuration = RemainingTransitionMilliseconds(Options.ShowTransitionDuration);
                        return string.Format(template, Opacity, showingDuration, AnimationId);
                    case SnackbarState.Hiding:
                        var hidingDuration = RemainingTransitionMilliseconds(Options.HideTransitionDuration);
                        return string.Format(template, 0, hidingDuration, AnimationId);
                    case SnackbarState.Visible:
                        return $"opacity: {Opacity};";
                    default:
                        return string.Empty;
                }
            }
        }

        public string SnackbarClass
        {
            get
            {
                var forceCursor = Options.ShowCloseIcon ? "" : " force-cursor";
                return $"mud-snackbar {Options.SnackbarTypeClass}{forceCursor}";
            }
        }

        public string TransitionClass
        {
            get
            {
                var template = "@keyframes " + AnimationId + " {{from{{ {0}: {1}; }} to{{ {0}: {2}; }}}}";

                switch (SnackbarState)
                {
                    case SnackbarState.Showing:
                        return string.Format(template, "opacity", "0%", Opacity);
                    case SnackbarState.Hiding:
                        return string.Format(template, "opacity", Opacity, "0%");
                    case SnackbarState.Visible:
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
