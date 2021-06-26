function setStylePositionInParent(element, position) {
    if (!element) return;
    if (window.getComputedStyle(element.parentElement).position === "static") {
        element.parentElement.style.position = position;
    }
}