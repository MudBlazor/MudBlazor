// noinspection JSUnusedGlobalSymbols

class MudFileUpload {
    openFilePicker (id) {
        const element = document.getElementById(id);

        if (!element) {
            return;
        }

        try {
            // only supported starting with Safari 16.4+
            // // checking for user activation because browsers won't execute showPicker() without it
            // if (!navigator.userActivation.isActive)
            // {
            //     return;
            // }

            // more reliable than click() and works in Safari
            element.showPicker();
        } catch (error) {
            // fallback
            element.click();
        }
    }
}

window.mudFileUpload = new MudFileUpload();