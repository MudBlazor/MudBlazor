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
        private bool _transitionsPaused;
        private bool _transitionCancellable;
        private bool _pendingHideTransition;
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
        /// Transitions the snackbar to the specified state.
        /// </summary>
        /// <param name="state">The state to transition to</param>
        /// <param name="animate">Whether the transition should be animated or instant</param>
        /// <param name="cancellable">Whether the transition, if animated, can be cancelled</param>
        private void TransitionTo(SnackbarState state, bool animate = true, bool cancellable = true)
        {
            // Stop any existing transition.
            StopTimer();
            _pendingHideTransition = false;

            State.SnackbarState = state;
            var options = State.Options;
            _transitionCancellable = cancellable;

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

        private void TimerElapsed(object _)
        {
            // Let the transition be triggered after the pause is ended.
            if (_transitionsPaused)
            {
                _pendingHideTransition = true;
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

        public void SetPaused(bool pause)
        {
            // Some transitions, like from the close button, can't be canceled or it would restart the transition when the user leaves the snackbar.
            if (!_transitionCancellable)
            {
                _transitionsPaused = false;
                return;
            }

            // Pause any transitions and stay visible.
            _transitionsPaused = pause;

            if (pause)
            {
                // If in process of Showing or Hiding: immediately go to Visible from Showing without animating.
                switch (State.SnackbarState)
                {
                    case SnackbarState.Showing:
                    case SnackbarState.Hiding:
                        TransitionTo(SnackbarState.Showing, animate: false);
                        break;
                }
            }
            else if (_pendingHideTransition)
            {
                // The Hiding transition has been pending and we can now complete it.
                TransitionTo(SnackbarState.Hiding);
            }
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
