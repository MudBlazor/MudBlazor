function setRippleOffset(event, target) {
    // calculate click position relative to the center of the target element
    const rect = target.getBoundingClientRect();
    const x = event.clientX - rect.left - rect.width / 2;
    const y = event.clientY - rect.top - rect.height / 2;
    target.style.setProperty("--mud-ripple-offset-x", `${x}px`);
    target.style.setProperty("--mud-ripple-offset-y", `${y}px`);
}

let rippleTimeout;

function startRipple(event) {
    const target = event.target.closest(".mud-ripple");
    if (!target) return;
    setRippleOffset(event, target);
    target.classList.add("mud-ripple-animating");
    target.classList.remove("mud-ripple-fade");
    // Store the start time
    target._rippleStart = Date.now();
}

function endRipple(event) {
    const target = event.target.closest(".mud-ripple");
    if (!target) return;
    const minDuration = 300; // ms
    const now = Date.now();
    const start = target._rippleStart || now;
    const elapsed = now - start;
    const remaining = Math.max(0, minDuration - elapsed);
    clearTimeout(target._rippleTimeout);
    target._rippleTimeout = setTimeout(() => {
        target.classList.add("mud-ripple-fade");
        setTimeout(() => {
            target.classList.remove("mud-ripple-animating", "mud-ripple-fade");
        }, 300); // match fade transition duration in SCSS
    }, remaining);
}

document.addEventListener("pointerdown", startRipple);
document.addEventListener("pointerup", endRipple);
document.addEventListener("pointercancel", endRipple);