// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Global ripple effect handler for elements with the `.mud-ripple` class.
 * Uses document-level event delegation so components do not need per-instance JS registration.
 */

(function () {
    // Minimum duration (ms) before ripple can start fading out
    const MIN_RIPPLE_DURATION = 500;

    // Animation duration for the fade out (ms) - should match CSS transition
    const FADE_OUT_DURATION = 400;

    function getRippleTarget(event) {
        if (!event.target || !event.target.closest) {
            return null;
        }

        return event.target.closest('.mud-ripple');
    }

    /**
     * Creates a ripple element at the pointer position
     * @param {PointerEvent} event - The pointer event
     * @param {HTMLElement} target - The ripple container element
     * @returns {HTMLElement} The created ripple element
     */
    function createRipple(event, target) {
        const rect = target.getBoundingClientRect();
        const ripple = document.createElement('span');

        // Calculate ripple size - should be large enough to cover the entire element
        const size = Math.max(rect.width, rect.height) * 2;

        // Calculate position relative to the target element
        const x = event.clientX - rect.left;
        const y = event.clientY - rect.top;

        ripple.className = 'mud-ripple-effect';
        ripple.style.width = ripple.style.height = `${size}px`;
        ripple.style.left = `${x - size / 2}px`;
        ripple.style.top = `${y - size / 2}px`;

        target.appendChild(ripple);

        // Trigger reflow to ensure the initial state is applied before animation
        ripple.getBoundingClientRect();

        // Start the expansion animation
        ripple.classList.add('mud-ripple-effect-expanding');

        return ripple;
    }

    /**
     * Removes a ripple element with fade out animation
     * @param {HTMLElement} ripple - The ripple element to remove
     * @param {number} startTime - When the ripple was created
     */
    function removeRipple(ripple, startTime) {
        if (!ripple || !ripple.parentNode) {
            return;
        }

        const elapsed = Date.now() - startTime;
        const remaining = Math.max(0, MIN_RIPPLE_DURATION - elapsed);

        // Wait for minimum duration before starting fade out
        setTimeout(function () {
            ripple.classList.add('mud-ripple-effect-fading');

            // Remove the element after fade out animation completes
            setTimeout(function () {
                ripple.remove();
            }, FADE_OUT_DURATION);
        }, remaining);
    }

    /**
     * Handles pointer down event - creates ripple
     * @param {PointerEvent} event
     */
    function handlePointerDown(event) {
        // Only handle primary button (left click / touch)
        if (event.button !== 0) {
            return;
        }

        const target = getRippleTarget(event);
        if (!target) {
            return;
        }

        const ripple = createRipple(event, target);
        const startTime = Date.now();

        // Store ripple info on the element for cleanup
        if (!target._mudRipples) {
            target._mudRipples = new Map();
        }
        target._mudRipples.set(event.pointerId, { ripple: ripple, startTime: startTime });
    }

    /**
     * Handles pointer up/leave/cancel events - removes ripple
     * @param {PointerEvent} event
     */
    function handlePointerUp(event) {
        const target = getRippleTarget(event);
        if (!target || !target._mudRipples) {
            return;
        }

        const rippleInfo = target._mudRipples.get(event.pointerId);
        if (rippleInfo) {
            removeRipple(rippleInfo.ripple, rippleInfo.startTime);
            target._mudRipples.delete(event.pointerId);
        }
    }

    /**
     * Handles pointer leave - removes all ripples for that element
     * This ensures ripples are cleaned up when pointer leaves the element
     * @param {PointerEvent} event
     */
    function handlePointerLeave(event) {
        const target = getRippleTarget(event);
        if (!target || !target._mudRipples) {
            return;
        }

        // Remove all active ripples for this element
        target._mudRipples.forEach(function (rippleInfo) {
            removeRipple(rippleInfo.ripple, rippleInfo.startTime);
        });
        target._mudRipples.clear();
    }

    // Register event listeners
    document.addEventListener('pointerdown', handlePointerDown, { passive: true });
    document.addEventListener('pointerup', handlePointerUp, { passive: true });
    document.addEventListener('pointercancel', handlePointerUp, { passive: true });
    document.addEventListener('pointerleave', handlePointerLeave, { passive: true, capture: true });
})();
