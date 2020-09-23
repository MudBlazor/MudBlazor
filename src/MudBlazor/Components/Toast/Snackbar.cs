// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;

namespace MudBlazor
{
    /// <summary>
    /// Represents an instance of a Toast
    /// It handles the user interactions and orchestrates the the state transitions
    /// </summary>
    public class Snackbar : IDisposable
    {
        private Timer Timer { get; }
        internal State State { get; }

        public string Title { get; }
        public string Message { get; }
        public event Action<Snackbar> OnClose;
        public event Action OnUpdate;
        public SnackbarType Type => State.Options.Type;

        internal Snackbar(string title, string message, SnackbarOptions options)
        {
            Title = title;
            Message = message;
            State = new State(options);
            Timer = new Timer(TimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
        }

        internal void Init() => TransitionTo(SnackbarState.Showing);

        internal void MouseEnter() => TransitionTo(SnackbarState.MouseOver);

        internal void MouseLeave()
        {
            if (State.ToastState.IsHiding()) return;
            if (State.Options.RequireInteraction && !State.UserHasInteracted) 
                TransitionTo(SnackbarState.Visible);
            else
                TransitionTo(SnackbarState.Hiding);
        }

        internal void Clicked(bool fromCloseIcon)
        {
            if (!fromCloseIcon)
            {
                // Execute the click action only if it's not from the close icon
                State.Options.Onclick?.Invoke(this);
                // If the close icon is show do not start the hiding transition
                if (State.Options.ShowCloseIcon) return;
            }

            State.UserHasInteracted = true;
            TransitionTo(SnackbarState.Hiding);
        }

        private void TransitionTo(SnackbarState state)
        {
            StopTimer();
            State.ToastState = state;
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
            switch (State.ToastState)
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
            State.TransitionStartTime = DateTime.Now;
            Timer?.Change(duration, Timeout.Infinite);
        }

        private void StopTimer()
        {
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
            Timer.Dispose();
        }
    }
}

