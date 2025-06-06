function setRippleOffset(event, target) {
    // calculate click position relative to the center of the target element
    const rect = target.getBoundingClientRect();
    const x = event.clientX - rect.left - rect.width / 2;
    const y = event.clientY - rect.top - rect.height / 2;
    target.style.setProperty("--mud-ripple-offset-x", `${x}px`);
    target.style.setProperty("--mud-ripple-offset-y", `${y}px`);
}

function startRipple(event) {
    const target = event.target.closest(".mud-ripple");
    if (!target) return;
    setRippleOffset(event, target);
    target.classList.add("mud-ripple-animating");
}

function endRipple(event) {
    const target = event.target.closest(".mud-ripple");
    if (!target) return;
    target.classList.remove("mud-ripple-animating");
}

document.addEventListener("pointerdown", startRipple);
document.addEventListener("pointerup", endRipple);
document.addEventListener("pointercancel", endRipple);