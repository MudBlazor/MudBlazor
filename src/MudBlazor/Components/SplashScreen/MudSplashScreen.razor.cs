﻿#nullable enable

using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace MudBlazor;

/// <summary>
/// Represents a component which displays a splash screen during the initial page load.
/// </summary>
/// <remarks>This component works by displaying a full-screen overlay (often an SVG path) for a few
/// seconds, then fading into the actual page content.  The actual page content will initialize and render
/// in the background, allowing for a smooth transition.</remarks>
public partial class MudSplashScreen
{
    /* To use this component, wrap your existing <MudLayout> element with the following code in your 
     * MainLayout.razor file:

     <MudSplashScreen DelaySeconds="1">
        <SplashContent>
            <svg height="80%" viewBox="0 0 1404 1404" class="docs-mudblazor-logo" xmlns="http://www.w3.org/2000/svg" >
            <path d="M406.89,219.14c-51.85,6.86-134.7-55.37-142.65-115.43-7.95-60.07,62.01-95.23,113.86-102.09,51.85-6.86,112.62,11.94,120.57,72.01,7.95,60.07-39.93,138.66-91.78,145.52Z" style="fill:#7e6fff;"></path>
            <path d="M1351.61,490.53c-29.26-77.07-70.13-149.81-121.87-214.05-34.85-43.28-74.24-83.06-118.5-116.66-51.13-38.82-107.18-74.24-169.52-89.6C659.86,.78,682.24,202.58,537.88,264.05c-144.35,61.47-316.15,4.62-409.08,131.03-38.73,52.69-76.68,109.42-100.97,170.37C5.41,621.7-2.93,684,1.05,744.27c7.77,117.64,64.46,245.62,162.39,314.92,34.64,24.51,91.9,55.27,131.4,25.76,19.29-14.41,31.2-37.27,39.96-59.15,25.29-63.2,31.63-134.4,35.72-201.73,4.09-67.39-8.86-285.66,4.07-323.91,10.1-29.89,34.98-51.3,65.12-59.55,37.46-10.25,84.85,1.44,112.4,28.99,11.98,11.98,21.72,26.39,29.24,43.17,7.49,16.78,16.01,38.08,25.62,63.86l55.76,156.52c4.8,13.19,9.9,26.68,15.31,40.45,5.38,13.81,11.24,26.42,17.55,37.79,6.28,11.4,12.88,20.69,19.76,27.9,6.89,7.17,13.96,10.79,21.14,10.79,5.99,0,12.01-3.17,18-9.45,5.99-6.31,11.85-14.7,17.55-25.21,5.7-10.47,11.37-22.61,17.07-36.42,5.7-13.8,11.24-28.47,16.65-44.07l60.28-162.83c8.39-22.77,16.33-42.57,23.83-59.35,7.5-16.78,16.21-30.59,26.07-41.38,9.9-10.79,21.75-18.9,35.55-24.31,13.77-5.38,31.48-8.07,53.07-8.07,16.78,0,31.32,1.63,43.62,4.93,12.3,3.3,22.32,9.77,30.14,19.34,14.5,17.77,17.58,45.82,20.66,68.01,6.54,46.97,1.82,95.87,1.82,143.32v272.45c0,23.37,1.14,47.05,0,70.39-.98,20.03-.76,40.4-10.45,58.74-8.84,16.71-25.19,30.07-43.97,32.23-24.29,2.79-43.71-15.61-52.61-36.6-4.2-9.9-7.05-19.79-8.55-29.69-1.5-9.9-2.24-17.84-2.24-23.83,0,0,10.01-267.01-33.46-261.48-8.83,1.12-28.92,49.45-34.66,61.45-30.1,60.21-107.26,230.27-122.13,251.82-12.03,17.43-28.81,35.72-50.55,39.92-15.19,2.94-29.82-2.71-41.65-12.12-29.45-23.43-42.28-66.81-56.1-100.93-11.3-27.91-84.23-310.38-138.19-317.77-7.14-.98-12.3,12.27-15.31,36.86-3.01,24.6-15.8,456.32,106.49,577.77,174.23,173.02,487.57,157.97,720.78-196.79,113.7-172.96,103.22-422.6,33.4-606.5Z" style="fill:#7e6fff;"></path>
            <path d="M369.84,537.14s-27.13,411.28-182.45,417.33c-116.75,4.55-168.93-122.4-176.64-144.99,20.77,97.3,72.69,193.09,152.7,249.7,34.64,24.51,91.9,55.26,131.4,25.76,19.29-14.41,31.2-37.27,39.96-59.15,25.29-63.2,31.63-134.4,35.72-201.73,3.34-55.09-4.69-210.9-.67-286.88v-.04Z" style="fill:#5a47ff;"></path></svg>
        </SplashContent>
        <ChildContent>
            <MudLayout>
                [etc.]
            </MudLayout>
        </ChildContent>
     </MudSplashScreen>

    */

    /// <summary>
    /// Gets or sets the content shown when the loading screen completes.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Slider.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the content shown in the splash screen.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Slider.Behavior)]
    public RenderFragment? SplashContent { get; set; }

    /// <summary>
    /// Gets or sets the minimum time the splash screen is shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Slider.Behavior)]
    public int DelaySeconds { get; set; } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether the splash screen is displayed.
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        // Is this the first render?
        if (firstRender)
        {
#pragma warning disable VSTHRD110 // By NOT awaiting the code below, the page can initialize fully while the loading screen is displayed
            // Yes.  Start a new task which...
            Task.Run(async () =>
            {
                // ... waits for the specified delay
                await Task.Delay(TimeSpan.FromSeconds(DelaySeconds)).ConfigureAwait(true);
                // Then fades out the splash screen
                await InvokeAsync(async () =>
                {
                    // We're no longer loading
                    IsLoading = false;
                    // Update the UI
                    await InvokeAsync(() => StateHasChanged()).ConfigureAwait(true);
                }).ConfigureAwait(true);
            });
#pragma warning restore VSTHRD110
        }
    }
}
