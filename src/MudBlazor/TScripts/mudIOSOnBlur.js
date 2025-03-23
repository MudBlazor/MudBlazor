class MudIosBlurHandler extends HTMLElement {
    constructor() {
        super();

        // hides the element from the user (and css) but keeps it in the DOM for the blur event to work
        this.style.display = "contents";
    }
    connectedCallback() {
        this.inputElement = this.querySelector('input, textarea');
        if (this.inputElement) {
            this.handleBlur = this.handleBlur.bind(this);
            this.inputElement.addEventListener('blur', this.handleBlur);
        }
    }

    disconnectedCallBack() {
        if (this.inputElement) {
            this.inputElement.removeEventListener('blur', this.handleBlur);
        }
    }

    handleBlur(event) {
        const dotnetRef = this.getAttribute('dotnet-ref');

        if (dotnetRef) {
            DotNet.invokeMethodAsync(this.getAttribute('assembly'), 'CallOnBlurredAsync', dotnetRef);
        }
    }
}

customElements.define('mud-ios-blur-handler', MudIosBlurHandler);
// Hides the element from the user and CSS
document.head.insertAdjacentHTML('beforeend', `
    <style>
        mud-ios-blur-handler {
            display: contents;
        }
    </style>
`);