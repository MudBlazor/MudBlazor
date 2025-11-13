class MudSplitPanel {
    _minPanelSize = 50;

    // noinspection JSUnusedGlobalSymbols
    startListening(containerId, horizontal) {
        this.container = document.getElementById(containerId);
        this.horizontal = horizontal;

        if (!this.container) {
            console.warn(`MudSplitPanel: Container '${containerId}' not found.`);
            return;
        }

        const children = this.container.children;
        if (children.length !== 3) {
            console.warn(`MudSplitPanel: Invalid child count '${children.length}'.`);
            return;
        }

        this.firstPanel = children[0];
        this.divider = children[1];
        this.secondPanel = children[2];

        this.isDragging = false;
        this.startPos = 0;
        this.startFirstSize = 0;

        this._onMouseDown = this._onMouseDown.bind(this);
        this._onMouseMove = this._onMouseMove.bind(this);
        this._onMouseUp = this._onMouseUp.bind(this);

        this.divider.addEventListener("mousedown", this._onMouseDown);
        this.divider.addEventListener("touchstart", this._onMouseDown);
    }

    // noinspection JSUnusedGlobalSymbols
    setHorizontal(horizontal) {
        this.horizontal = horizontal;
        this.firstPanel.style.height = "100%";
        this.secondPanel.style.height = "100%";
        this.firstPanel.style.width = "100%";
        this.secondPanel.style.width = "100%";
    }

    _onMouseDown(e) {
        e.preventDefault();
        this.isDragging = true;
        
        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;

        if (this.horizontal) {
            this.startPos = clientY;
            this.startFirstSize = this.firstPanel.offsetHeight;
        } else {
            this.startPos = clientX;
            this.startFirstSize = this.firstPanel.offsetWidth;
        }

        document.body.style.userSelect = "none";
        document.addEventListener("mousemove", this._onMouseMove);
        document.addEventListener("touchmove", this._onMouseMove);
        document.addEventListener("mouseup", this._onMouseUp);
        document.addEventListener("touchend", this._onMouseUp);
    }

    _onMouseMove(e) {
        if (!this.isDragging) return;

        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;

        const containerSize = this.horizontal
            ? this.container.offsetHeight
            : this.container.offsetWidth;
        
        const delta = this.horizontal ? clientY - this.startPos : clientX - this.startPos;
        const newFirstSize = this.startFirstSize + delta;

        const min = this._minPanelSize;
        const max = containerSize - min;

        if (newFirstSize > min && newFirstSize < max) {
            if (this.horizontal) {
                this.firstPanel.style.height = `${newFirstSize}px`;
                this.secondPanel.style.height = `${containerSize - newFirstSize - this.divider.offsetHeight}px`;
                this.firstPanel.style.width = "100%";
                this.secondPanel.style.width = "100%";
            } else {
                this.firstPanel.style.width = `${newFirstSize}px`;
                this.secondPanel.style.width = `${containerSize - newFirstSize - this.divider.offsetWidth}px`;
                this.firstPanel.style.height = "100%";
                this.secondPanel.style.height = "100%";
            }
        }
    }

    _onMouseUp() {
        if (!this.isDragging) return;
        this.isDragging = false;
        
        document.body.style.userSelect = "";
        document.removeEventListener("mousemove", this._onMouseMove);
        document.removeEventListener("touchmove", this._onMouseMove);
        document.removeEventListener("mouseup", this._onMouseUp);
        document.removeEventListener("touchend", this._onMouseUp);
    }
}

window.mudSplitPanel = new MudSplitPanel();
