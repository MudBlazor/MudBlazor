window.getMudBoundingClientRect = (element) => {
    return element.getBoundingClientRect();
};

window.changeCss = (element, css) => {
    element.className = css;
};

window.changeCssById = (id, css) => {
    var element = document.getElementById(id);
    if (element) {
        element.className = css;
    }
};

window.changeGlobalCssVariable = (name, newValue) => {
    document.documentElement.style.setProperty(name, newValue);
};

window.changeCssVariable = (element, name, newValue) => {
    element.style.setProperty(name, newValue);
};

window.addMudEventListener = (element, dotnet, event, callback) => {
    element.addEventListener(event, function () {
        dotnet.invokeMethodAsync(callback);
    });
};

// Needed as per https://stackoverflow.com/questions/62769031/how-can-i-open-a-new-window-without-using-js
window.mudOpen = (args) => {
    window.open(args);
};