// noinspection JSUnusedGlobalSymbols
class MudSplitPanel {
    static build(containerId, horizontal, resetOnDoubleClick, minPanelSize, firstPanelInitialSize, panelGap) {
        window.splitPanels[containerId] = new MudSplitPanel(containerId, horizontal, resetOnDoubleClick, minPanelSize, firstPanelInitialSize, panelGap);
    }

    constructor(containerId, horizontal, resetOnDoubleClick, minPanelSize, firstPanelInitialSize, panelGap) {
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

        this.isDragging = false;
        this.startPos = 0;
        this.startFirstSize = 0;
        this.lastTap = 0;
        this.firstPanelInitialSize = firstPanelInitialSize;
        this.keyboardStep = 10;

        this._onMouseDown = this._onMouseDown.bind(this);
        this._onMouseMove = this._onMouseMove.bind(this);
        this._onMouseUp = this._onMouseUp.bind(this);
        this._onDoubleClick = this._onDoubleClick.bind(this);
        this._onTouchEnd = this._onTouchEnd.bind(this);
        this._onKeyDown = this._onKeyDown.bind(this);

        this.divider.addEventListener("mousedown", this._onMouseDown);
        this.divider.addEventListener("touchstart", this._onMouseDown);
        this.divider.addEventListener("dblclick", this._onDoubleClick);
        this.divider.addEventListener("touchend", this._onTouchEnd);
        this.divider.addEventListener("keydown", this._onKeyDown);

        this.update(horizontal, resetOnDoubleClick, minPanelSize, panelGap, true);
    }

    destroy() {
        this.divider.removeEventListener("mousedown", this._onMouseDown);
        this.divider.removeEventListener("touchstart", this._onMouseDown);
        this.divider.removeEventListener("dblclick", this._onDoubleClick);
        this.divider.removeEventListener("touchend", this._onTouchEnd);
        this.divider.removeEventListener("keydown", this._onKeyDown);
        this._onMouseUp();

        delete window.splitPanels[this.container.id];
    }

    // noinspection JSUnusedGlobalSymbols
    update(horizontal, resetOnDoubleClick, minPanelSize, panelGap, forceRecalculateSize = false) {
        let shouldRecalculateSize = horizontal !== this.horizontal || forceRecalculateSize;
        this.horizontal = horizontal;
        this.minPanelSize = minPanelSize;
        this.panelGap = panelGap;
        this.resetOnDoubleClick = resetOnDoubleClick;

        this.divider.style.minWidth = this.horizontal ? null : `${panelGap}px`;
        this.divider.style.minHeight = this.horizontal ? `${panelGap}px` : null;

        let containerSize = this._getContainerSize();
        this.divider.ariaValueMin = (this.minPanelSize / containerSize * 100).toFixed(2).toString();
        this.divider.ariaValueMax = ((containerSize - panelGap - this.minPanelSize) / containerSize * 100).toFixed(2).toString();

        if (shouldRecalculateSize) {
            this.resetSizes();
        }
    }

    resetSizes() {
        this.firstPanel.style.width = "100%";
        this.secondPanel.style.width = "100%";
        this.firstPanel.style.height = "100%";
        this.secondPanel.style.height = "100%";

        if (this.firstPanelInitialSize !== null) {
            this._setPanelSizes(this.firstPanelInitialSize, this._getContainerSize());
        } else {
            this.divider.ariaValueNow = "50";
        }
    }

    _getContainerSize() {
        return this.horizontal ? this.container.offsetHeight : this.container.offsetWidth;
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
        const delta = this.horizontal ? clientY - this.startPos : clientX - this.startPos;
        const newFirstSize = this.startFirstSize + delta;

        const containerSize = this._getContainerSize();
        const min = this.minPanelSize;
        const max = containerSize - this.panelGap - min;

        if (newFirstSize >= min && newFirstSize <= max) {
            this._setPanelSizes(newFirstSize, containerSize);
        }
    }

    _setPanelSizes(newFirstSize, containerSize) {
        const newSecondSize = containerSize - newFirstSize - this.panelGap;

        this.firstPanel.style.height = this.horizontal ? `${newFirstSize}px` : "100%";
        this.secondPanel.style.height = this.horizontal ? `${newSecondSize}px` : "100%";
        this.firstPanel.style.width = this.horizontal ? "100%" : `${newFirstSize}px`;
        this.secondPanel.style.width = this.horizontal ? "100%" : `${newSecondSize}px`;

        this.divider.ariaValueNow = (newFirstSize / containerSize * 100).toFixed(2).toString();
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

    _onDoubleClick() {
        if (!this.resetOnDoubleClick) return;

        const containerSize = this.horizontal
            ? this.container.offsetHeight
            : this.container.offsetWidth;

        let firstPanelSize = this.firstPanelInitialSize;
        if (!firstPanelSize) {
            firstPanelSize = containerSize / 2 - this.panelGap / 2;
        }

        this._setPanelSizes(firstPanelSize, containerSize);
    }

    _onTouchEnd() {
        const now = Date.now();
        if (now - this.lastTap < 300) {
            this._onDoubleClick();
        }
        this.lastTap = now;
    }

    _onKeyDown(e) {
        const key = e.key;
        let delta = 0;
        const containerSize = this._getContainerSize();

        if (key.startsWith("Arrow") || key === "Home" || key === "End") {
            e.preventDefault();
        }

        if (this.horizontal) {
            if (key === "ArrowUp") {
                delta = -this.keyboardStep;
            } else if (key === "ArrowDown") {
                delta = this.keyboardStep;
            } else if (key === "Home") {
                this._setPanelSizes(this.minPanelSize, containerSize);
            } else if (key === "End") {
                this._setPanelSizes(containerSize - this.panelGap - this.minPanelSize, containerSize);
            }
        } else {
            if (key === "ArrowLeft") {
                delta = -this.keyboardStep;
            } else if (key === "ArrowRight") {
                delta = this.keyboardStep;
            } else if (key === "Home") {
                this._setPanelSizes(this.minPanelSize, containerSize);
            } else if (key === "End") {
                this._setPanelSizes(containerSize - this.panelGap - this.minPanelSize, containerSize);
            }
        }

        if (delta !== 0) {
            const firstPanelSize = this.horizontal
                ? this.firstPanel.offsetHeight
                : this.firstPanel.offsetWidth;
            const newFirstPanelSize = firstPanelSize + delta;
            const min = this.minPanelSize;
            const max = containerSize - this.panelGap - min;

            if (newFirstPanelSize >= min && newFirstPanelSize <= max) {
                this._setPanelSizes(newFirstPanelSize, containerSize);
            }
        }
    }
}

if (!window.mudSplitPanel) {
    window.mudSplitPanel = MudSplitPanel;
    window.splitPanels = {};
}

window.mudSplitPanel_update = function (id, horizontal, resetOnDoubleClick, minPanelSize, panelGap) {
    window.splitPanels[id].update(horizontal, resetOnDoubleClick, minPanelSize, panelGap);
};

window.mudSplitPanel_resetSizes = function (id) {
    window.splitPanels[id].resetSizes();
};

window.mudSplitPanel_destroy = function (id) {
    window.splitPanels[id].destroy();
};