class MudWindow {
    copyToClipboard (text) {
        navigator.clipboard.writeText(text);
    }

    changeCssById (id, css) {
        var element = document.getElementById(id);
        if (element) {
            element.className = css;
        }
    }

    changeGlobalCssVariable (name, newValue) {
        document.documentElement.style.setProperty(name, newValue);
    }

    // Needed as per https://stackoverflow.com/questions/62769031/how-can-i-open-a-new-window-without-using-js
    open (args) {
        window.open(args);
    }

   
};
window.mudWindow = new MudWindow();