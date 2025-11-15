// noinspection JSUnusedGlobalSymbols
class MudSplitPanel {
    static build(containerId, horizontal, minPanelSize, firstPanelInitialSize) {
        window.splitPanels[containerId] = new MudSplitPanel(containerId, horizontal, minPanelSize, firstPanelInitialSize);
    }

    constructor(containerId, horizontal, minPanelSize, firstPanelInitialSize) {
        this.container = document.getElementById(containerId);
        if (!this.container) {
            console.warn(`MudSplitPanel: Container with id '${containerId}' not found.`);
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

        this.update(horizontal, minPanelSize);
        this.isDragging = false;
        this.startPos = 0;
        this.startFirstSize = 0;

        this._onMouseDown = this._onMouseDown.bind(this);
        this._onMouseMove = this._onMouseMove.bind(this);
        this._onMouseUp = this._onMouseUp.bind(this);

        this.divider.addEventListener("mousedown", this._onMouseDown);
        this.divider.addEventListener("touchstart", this._onMouseDown);

        if (firstPanelInitialSize) {
            const containerSize = this.horizontal
                ? this.container.offsetHeight
                : this.container.offsetWidth;
            this._setPanelSizes(firstPanelInitialSize, containerSize);
        }
    }

    // noinspection JSUnusedGlobalSymbols
    update(horizontal, minPanelSize) {
        this.horizontal = horizontal;
        this.minPanelSize = minPanelSize;

        if (horizontal) {
            this.firstPanel.style.width = "100%";
            this.secondPanel.style.width = "100%";
        } else {
            this.firstPanel.style.height = "100%";
            this.secondPanel.style.height = "100%";
        }
    }

    _onMouseDown(e) {
        e.preventDefault();
        this.isDragging = true;

        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;

        if (this.horizontal) {
            this.startPos = clientY;
            this.startFirstSize = this.firstPanel.offsetHeight;
            document.body.style.cursor = "n-resize";
        } else {
            this.startPos = clientX;
            this.startFirstSize = this.firstPanel.offsetWidth;
            document.body.style.cursor = "ew-resize";
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

        const min = this.minPanelSize;
        const max = containerSize - min;

        if (newFirstSize > min && newFirstSize < max) {
            this._setPanelSizes(newFirstSize, containerSize);
        }
    }

    _setPanelSizes(newFirstSize, containerSize) {
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

    _onMouseUp() {
        if (!this.isDragging) return;
        this.isDragging = false;

        document.body.style.userSelect = "";
        document.body.style.cursor = "";
        document.removeEventListener("mousemove", this._onMouseMove);
        document.removeEventListener("touchmove", this._onMouseMove);
        document.removeEventListener("mouseup", this._onMouseUp);
        document.removeEventListener("touchend", this._onMouseUp);
    }
}

if (!window.mudSplitPanel) {
    window.mudSplitPanel = MudSplitPanel;
    window.splitPanels = {};
}

window.mudSplitPanel_update = function (id, horizontal, minPanelSize) {
    window.splitPanels[id].update(horizontal, minPanelSize);
};