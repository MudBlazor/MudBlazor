window.getMudBoundingClientRect = (element) => {
    return element.getBoundingClientRect();
};

window.addTranstionEndListener = (element, dotnet) => {
    element.addEventListener("transitionend", function () {
        dotnet.invokeMethodAsync("TransitionEnd");
    });
};