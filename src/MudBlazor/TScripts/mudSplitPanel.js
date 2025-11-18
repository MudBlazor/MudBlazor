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
        this.firstPanelInitialSize = firstPanelInitialSize;

        this._onMouseDown = this._onMouseDown.bind(this);
        this._onMouseMove = this._onMouseMove.bind(this);
        this._onMouseUp = this._onMouseUp.bind(this);
        this._onDoubleClick = this._onDoubleClick.bind(this);

        this.divider.addEventListener("mousedown", this._onMouseDown);
        this.divider.addEventListener("touchstart", this._onMouseDown);
        this.divider.addEventListener("dblclick", this._onDoubleClick);
        let lastTap = 0;
        this.divider.addEventListener("touchend", (_) => {
            const now = Date.now();
            if (now - lastTap < 300) {
                this._onDoubleClick();
            }
            lastTap = now;
        });

        this.update(horizontal, resetOnDoubleClick, minPanelSize, panelGap);
    }

    // noinspection JSUnusedGlobalSymbols
    update(horizontal, resetOnDoubleClick, minPanelSize, panelGap) {
        let shouldRecalculateSize = horizontal !== this.horizontal;
        this.horizontal = horizontal;
        this.minPanelSize = minPanelSize;
        this.panelGap = panelGap;
        this.resetOnDoubleClick = resetOnDoubleClick;

        this.divider.style.minWidth = this.horizontal ? null : `${panelGap}px`;
        this.divider.style.minHeight = this.horizontal ? `${panelGap}px` : null;

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
            const containerSize = this.horizontal
                ? this.container.offsetHeight
                : this.container.offsetWidth;
            this._setPanelSizes(this.firstPanelInitialSize, containerSize);
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
        const max = containerSize - this.panelGap - min;

        if (newFirstSize > min && newFirstSize < max) {
            this._setPanelSizes(newFirstSize, containerSize);
        }
    }

    _setPanelSizes(newFirstSize, containerSize) {
        const newSecondSize = containerSize - newFirstSize - this.panelGap;

        this.firstPanel.style.height = this.horizontal ? `${newFirstSize}px` : "100%";
        this.secondPanel.style.height = this.horizontal ? `${newSecondSize}px` : "100%";
        this.firstPanel.style.width = this.horizontal ? "100%" : `${newFirstSize}px`;
        this.secondPanel.style.width = this.horizontal ? "100%" : `${newSecondSize}px`;
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
            firstPanelSize = containerSize / 2;
        }

        this._setPanelSizes(firstPanelSize, containerSize);
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