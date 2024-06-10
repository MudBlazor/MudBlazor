class MudFileUpload {
    openFilePicker (id) {
        const element = document.getElementById(id);

        if (!element) {
            return;
        }

        // checking for user activation because Safari won't do anything without it
        if (!navigator.userActivation.isActive)
        {
            return;
        }

        try {
            // showPicker is more reliable than click() and works in Safari
            element.showPicker();
        } catch {
            // fallback
            element.click();
        }
    }
}

window.mudFileUpload = new MudFileUpload();