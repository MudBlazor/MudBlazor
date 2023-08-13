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
            TransitionTo(SnackbarState.Hiding);
        }

        private void TransitionTo(SnackbarState state)
        {
            StopTimer();
            State.SnackbarState = state;
            var options = State.Options;

            if (state.IsShowing())
            {
                if (options.ShowTransitionDuration <= 0) TransitionTo(SnackbarState.Visible);
                else StartTimer(options.ShowTransitionDuration);
            }
            else if (state.IsVisible() && !options.RequireInteraction)
            {
                if (options.VisibleStateDuration <= 0) TransitionTo(SnackbarState.Hiding);
                else StartTimer(options.VisibleStateDuration);
            }
            else if (state.IsHiding())
            {
                if (options.HideTransitionDuration <= 0) OnClose?.Invoke(this);
                else StartTimer(options.HideTransitionDuration);
            }

            OnUpdate?.Invoke();
        }

        private void TimerElapsed(object state)
        {
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

        private void StartTimer(int duration)
        {
            State.Stopwatch.Restart();
            Timer?.Change(duration, Timeout.Infinite);
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
            if (!disposing) return;
            StopTimer();

            var timer = Timer;
            Timer = null;

            timer?.Dispose();
        }
    }
}

