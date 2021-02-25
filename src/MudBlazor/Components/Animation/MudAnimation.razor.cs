using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAnimation : MudComponentBase
    {
        protected string Classname =>
                            new CssBuilder()
                                .AddClass(Class)
                                .Build();

        protected string StyleWithAnimation =>
                    new StyleBuilder()
                        .AddStyle(Style)
                        .AddStyle("animation", _animation, _animation != null)
                        .Build();


        string _animation;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        public MudAnimation Parent { get; set; }

        private int _duration = 500;
        /// <summary>
        /// The Animation Duration, int miliseconds
        /// </summary>
        [Parameter]
        public int Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                 if (_trigger == AnimationTrigger.OnRender)
                    TriggerAnimation();
            }
        }

        private int _delay = 0;
        /// <summary>
        /// The Animation Start Delay, int miliseconds
        /// </summary>
        [Parameter]
        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                if (_trigger == AnimationTrigger.OnRender)
                    TriggerAnimation();
            }
        }

        private AnimationTimmingFunc _effect = AnimationTimmingFunc.linear;
        /// <summary>
        /// The timming effect function to be applied to element
        /// </summary>
        [Parameter]
        public AnimationTimmingFunc TimmingFunction
        {
            get => _effect;
            set
            {
                _effect = value;
                 if (_trigger == AnimationTrigger.OnRender)
                    TriggerAnimation();
            }
        }

        private string _keyframe;
        /// <summary>
        /// Keyframe Name. See MudBlazor.Animations class for built-in ones.
        /// Some of then could require aditional values with String.Format or string interpolation
        /// </summary>
        [Parameter]
        public string KeyFrameName
        {
            get => _keyframe;
            set
            {
                _keyframe = value;
                 if (_trigger == AnimationTrigger.OnRender)
                    TriggerAnimation();
            }
        }

        private AnimationDirection _direction = AnimationDirection.normal;
        /// <summary>
        /// animation-direction: normal|reverse|alternate|alternate-reverse;
        /// </summary>
        [Parameter]
        public AnimationDirection Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                 if (_trigger == AnimationTrigger.OnRender)
                    TriggerAnimation();
            }
        }

        private AnimationTrigger _trigger = AnimationTrigger.Explicity;
        /// <summary>
        /// animation-direction: normal|reverse|alternate|alternate-reverse;
        /// </summary>
        [Parameter]
        public AnimationTrigger Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value;
                if (_trigger == AnimationTrigger.OnRender)
                    TriggerAnimation();
            }
        }

        private bool _infinite = false;
        /// <summary>
        /// The Animation Duration, on CSS format (ex: 1s, 0.25s, etc)
        /// </summary>
        [Parameter]
        public bool Infinite
        {
            get => _infinite;
            set
            {
                _infinite = value;
                if (_trigger == AnimationTrigger.OnRender)
                    TriggerAnimation();
            }
        }


        protected override Task OnInitializedAsync()
        {
            if (Trigger == AnimationTrigger.OnRender)
                 TriggerAnimation();
            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_animation != null && Infinite == false)
            {
                await Task.Delay(Duration + Delay + 100);
                RemoveAninmation();
            }
        }

        /// <summary>
        /// Execute the animation
        /// </summary>
        public void TriggerAnimation()
        {
            RemoveAninmation();
            _animation = $"{KeyFrameName} {((double)Duration / 1000).ToString().Replace(",", ".")}s {TimmingFunction.ToDescriptionString()} {((double)Delay / 1000).ToString().Replace(",", ".")}s {(Infinite == true ? "infinite" : "")} {Direction.ToDescriptionString()}"; 
      
            if (Parent != null)
                Parent.TriggerAnimation();

            StateHasChanged();
        }

        private void RemoveAninmation()
        {
            _animation = null;
            StateHasChanged();
        }
    }
}
