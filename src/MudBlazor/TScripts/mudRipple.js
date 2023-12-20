function setRippleOffset(event, target) {
    // calculate click position relative to the center of the target element
    const rect = target.getBoundingClientRect();
    const x = event.clientX - rect.left - rect.width / 2;
    const y = event.clientY - rect.top - rect.height / 2;
    
    target.style.setProperty("--mud-ripple-offset-x", `${x}px`);
    target.style.setProperty("--mud-ripple-offset-y", `${y}px`);
}

document.addEventListener("click", function (event) {
    const target = event.target.closest(".mud-ripple");

    if (target) {
        setRippleOffset(event, target);
    }
});