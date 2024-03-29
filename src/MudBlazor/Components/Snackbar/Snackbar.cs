//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using System.Threading;
using MudBlazor.Components.Snackbar;
using MudBlazor.Components.Snackbar.InternalComponents;

namespace MudBlazor
{
    public class Snackbar : IDisposable
    {
        private bool _paused = false;
        private bool _transitionCancellable = true;
        private bool _hideOnResume = false;
        private Timer Timer { get; set; }
        internal SnackBarMessageState State { get; }
        public string Message => SnackbarMessage.Text;
        internal SnackbarMessage SnackbarMessage { get; }
        public event Action<Snackbar> OnClose;
        public event Action OnUpdate;
        public Severity Severity => State.Options.Severity;

        internal Snackbar(SnackbarMessage message, SnackbarOptions options)
        {
            SnackbarMessage = message;
            State = new SnackBarMessageState(options);
            Timer = new Timer(TimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
        }

        internal void Init() => TransitionTo(SnackbarState.Showing);

        internal void Clicked(bool fromCloseIcon)
        {
            if (State.UserHasInteracted)
                return; // You should only be able to interact with the snackbar once.

            if (!fromCloseIcon)
            {
                // Do not start the hiding transition if no click action
                if (State.Options.Onclick == null)
                    return;

                // Click action is executed only if it's not from the close icon
                State.Options.Onclick.Invoke(this);
            }

            State.UserHasInteracted = true;
            TransitionTo(SnackbarState.Hiding, cancellable: false);
        }

        /// <summary>
        /// Forcibly close the snackbar without performing any animations.
        /// </summary>
        public void ForceClose()
        {
            TransitionTo(SnackbarState.Hiding, false, false);
        }

        /// <summary>
        /// Transitions the snackbar to the specified state.
        /// </summary>
        /// <param name="state">The state to transition to</param>
        /// <param name="animate">Whether the transition should be animated or instant</param>
        /// <param name="cancellable">Whether the transition, if animated, can be cancelled</param>
        private void TransitionTo(SnackbarState state, bool animate = true, bool cancellable = true)
        {
            // A new non-cancellable transition takes priority and will force a resume.
            if (!cancellable)
            {
                _paused = false;
            }
            else if (!_transitionCancellable)
            {
                // The current transition can't be cancelled.
                return;
            }

            StopTimer();

            State.SnackbarState = state;
            _transitionCancellable = cancellable;
            var options = State.Options;

            if (state.IsShowing())
            {
                if (!animate || !StartTimer(options.ShowTransitionDuration))
                    TransitionTo(SnackbarState.Visible);
            }
            else if (state.IsVisible() && !options.RequireInteraction)
            {
                if (!animate || !StartTimer(options.VisibleStateDuration))
                    TransitionTo(SnackbarState.Hiding);
            }
            else if (state.IsHiding())
            {
                if (!animate || !StartTimer(options.HideTransitionDuration))
                    OnClose?.Invoke(this);
            }

            OnUpdate?.Invoke();
        }

        public void PauseTransitions(bool pause)
        {
            // Some transitions, like from the close button, can't be canceled or it would restart the transition when the user leaves the snackbar.
            if (!_transitionCancellable)
            {
                _paused = false;
                return;
            }

            // Pause any transitions and stay visible.
            _paused = pause;

            if (pause)
            {
                switch (State.SnackbarState)
                {
                    case SnackbarState.Showing:
                        // Skip the Showing animation and go straight to Visible.
                        TransitionTo(SnackbarState.Visible);
                        break;
                    case SnackbarState.Hiding:
                        // Stop the Hiding transition and go to a Visible state with no duration.
                        // As soon as we resume we will trigger the Hiding transition again.
                        StopTimer();
                        State.SnackbarState = SnackbarState.Visible;
                        _hideOnResume = true;
                        OnUpdate?.Invoke();
                        break;
                }
            }
            else if (_hideOnResume)
            {
                // The Hiding transition has been pending and we can now execute it.
                _hideOnResume = false;
                TransitionTo(SnackbarState.Hiding);
            }
        }

        private void TimerElapsed(object _)
        {
            // Let the transition be triggered after the pause is ended.
            if (_paused)
            {
                if (State.SnackbarState.IsVisible() || State.SnackbarState.IsHiding())
                {
                    _hideOnResume = true;
                }

                return;
            }

            // Take the next step after the current state has transitioned.
            switch (State.SnackbarState)
            {
                case SnackbarState.Showing:
                    TransitionTo(SnackbarState.Visible);
                    break;
                case SnackbarState.Visible:
                    TransitionTo(SnackbarState.Hiding);
                    break;
                case SnackbarState.Hiding:
                    OnClose?.Invoke(this);
                    break;
            }
        }

        /// <summary>
        /// Starts the transition timer that elapses after the specified duration; or return <c>false</c> if the period would be instantaneous.
        /// </summary>
        private bool StartTimer(int duration)
        {
            if (duration <= 0)
                return false;

            State.Stopwatch.Restart();
            Timer?.Change(duration, Timeout.Infinite);

            return true;
        }

        private void StopTimer()
        {
            State.Stopwatch.Stop();
            Timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            StopTimer();

            var timer = Timer;
            Timer = null;

            timer?.Dispose();
        }
    }
}
