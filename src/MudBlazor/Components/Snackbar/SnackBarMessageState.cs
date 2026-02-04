//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using static System.String;

namespace MudBlazor
{
    internal class SnackBarMessageState
    {
        private string AnimationId { get; }
        public bool UserHasInteracted { get; set; }
        public SnackbarOptions Options { get; }
        public SnackbarState SnackbarState { get; set; }
        private readonly TimeProvider _timeProvider;
        private DateTimeOffset _transitionStartTime;
        private long _elapsedMilliseconds;

        public SnackBarMessageState(SnackbarOptions options, TimeProvider timeProvider)
        {
            Options = options;
            _timeProvider = timeProvider;
            AnimationId = Identifier.Create();
            SnackbarState = SnackbarState.Init;
        }

        /// <summary>
        /// Records the start time of the current transition.
        /// </summary>
        internal void StartTransition(DateTimeOffset now)
        {
            _transitionStartTime = now;
            _elapsedMilliseconds = 0;
        }

        /// <summary>
        /// Stops tracking the transition and records the elapsed time.
        /// </summary>
        internal void StopTransition(DateTimeOffset now)
        {
            _elapsedMilliseconds = (long)(now - _transitionStartTime).TotalMilliseconds;
        }

        private long GetElapsedMilliseconds()
        {
            var now = _timeProvider.GetUtcNow();
            return (long)(now - _transitionStartTime).TotalMilliseconds;
        }
        private string Opacity => ((decimal)Options.MaximumOpacity / 100).ToPercentage();

        public bool ShowActionButton => !IsNullOrWhiteSpace(Options.Action);
        public bool ShowCloseIcon => Options.ShowCloseIcon;

        public bool HideIcon => Options.HideIcon;
        public string Icon => Options.Icon;
        public Color IconColor => Options.IconColor;
        public Size IconSize => Options.IconSize;

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
                const string Template = "opacity: {0}; animation: {1}ms linear {2};";

                switch (SnackbarState)
                {
                    case SnackbarState.Showing:
                        var showingDuration = RemainingTransitionMilliseconds(Options.ShowTransitionDuration);
                        return Format(Template, Opacity, showingDuration, AnimationId);

                    case SnackbarState.Hiding:
                        var hidingDuration = RemainingTransitionMilliseconds(Options.HideTransitionDuration);
                        return Format(Template, 0, hidingDuration, AnimationId);

                    case SnackbarState.Visible:
                        return $"opacity: {Opacity};";

                    default:
                        return Empty;
                }
            }
        }

        public string SnackbarClass
        {
            get
            {
                var baseTypeClass = $"mud-alert-{Options.SnackbarVariant.ToStringFast(true)}-{Options.Severity.ToStringFast(true)}";

                if (Options.SnackbarVariant != Variant.Filled)
                {
                    baseTypeClass += Options.BackgroundBlurred ? " mud-snackbar-blurred" : " mud-snackbar-surface";
                }

                var result = $"mud-snackbar {baseTypeClass} {Options.SnackbarTypeClass}";

                if (Options.OnClick != null && !ShowActionButton)
                    result += " force-cursor";

                return result;
            }
        }

        public string TransitionClass
        {
            get
            {
                var template = "@keyframes " + AnimationId + " {{from{{ {0}: {1}; }} to{{ {0}: {2}; }}}}";

                return SnackbarState switch
                {
                    SnackbarState.Showing => Format(template, "opacity", "0%", Opacity),
                    SnackbarState.Hiding => Format(template, "opacity", Opacity, "0%"),
                    SnackbarState.Visible => Format(template, "width", "100%", "0%"),
                    _ => Empty,
                };
            }
        }

        private int RemainingTransitionMilliseconds(int transitionDuration)
        {
            var elapsed = GetElapsedMilliseconds();
            var duration = transitionDuration - (int)elapsed;

            return duration >= 0 ? duration : 0;
        }
    }
}
