// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

type PointerLikeEvent = MouseEvent | TouchEvent;

function clientPosition(e: PointerLikeEvent) {
    const touch = (e as TouchEvent).touches ? (e as TouchEvent).touches[0]! : e as MouseEvent;
    return { clientX: touch.clientX, clientY: touch.clientY };
}

// noinspection JSUnusedGlobalSymbols
/**
 * SplitPanel resize behavior for the MudSplitPanel component.
 * Owns pointer/keyboard interaction and ARIA value updates for smooth resizing.
 */
export default class MudSplitPanel {
    // The constructor can return early for an invalid container, so these are declared without emitting fields.
    declare container: HTMLElement | null;
    declare firstPanel: HTMLElement;
    declare divider: HTMLElement;
    declare secondPanel: HTMLElement;
    declare isDragging: boolean;
    declare dividerHasMoved: boolean;
    declare startPos: number;
    declare startFirstSize: number;
    declare lastTap: number;
    declare lastDragEndDate: number;
    declare firstPanelInitialSize: number | null;
    declare keyboardStep: number;
    declare horizontal: boolean;
    declare minPanelSize: number;
    declare panelGap: number;
    declare resetOnDoubleClick: boolean;

    /**
     * Creates and stores a split panel runtime instance for a container ID.
     */
    static build(containerId: string, horizontal: boolean, resetOnDoubleClick: boolean, minPanelSize: number, firstPanelInitialSize: number | null, panelGap: number) {
        window.splitPanels[containerId] = new MudSplitPanel(containerId, horizontal, resetOnDoubleClick, minPanelSize, firstPanelInitialSize, panelGap);
    }

    constructor(containerId: string, horizontal: boolean, resetOnDoubleClick: boolean, minPanelSize: number, firstPanelInitialSize: number | null, panelGap: number) {
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

        this.firstPanel = children[0] as HTMLElement;
        this.divider = children[1] as HTMLElement;
        this.secondPanel = children[2] as HTMLElement;

        this.isDragging = false;
        this.dividerHasMoved = false;
        this.startPos = 0;
        this.startFirstSize = 0;
        this.lastTap = 0;
        this.lastDragEndDate = 0;
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

    /**
     * Removes listeners and disposes the split panel runtime instance.
     */
    destroy() {
        this.divider.removeEventListener("mousedown", this._onMouseDown);
        this.divider.removeEventListener("touchstart", this._onMouseDown);
        this.divider.removeEventListener("dblclick", this._onDoubleClick);
        this.divider.removeEventListener("touchend", this._onTouchEnd);
        this.divider.removeEventListener("keydown", this._onKeyDown);
        this._onMouseUp();

        delete window.splitPanels[this.container!.id];
    }

    // noinspection JSUnusedGlobalSymbols
    /**
     * Updates runtime options and recalculates panel sizes when needed.
     */
    update(horizontal: boolean, resetOnDoubleClick: boolean, minPanelSize: number, panelGap: number, forceRecalculateSize = false) {
        const shouldRecalculateSize = horizontal !== this.horizontal || forceRecalculateSize;
        this.horizontal = horizontal;
        this.minPanelSize = minPanelSize;
        this.panelGap = panelGap;
        this.resetOnDoubleClick = resetOnDoubleClick;

        // Browsers treat the null the script used to assign as an empty string, which clears the property.
        this.divider.style.minWidth = this.horizontal ? '' : `${panelGap}px`;
        this.divider.style.minHeight = this.horizontal ? `${panelGap}px` : '';

        const containerSize = this._getContainerSize();
        this.divider.ariaValueMin = (this.minPanelSize / containerSize * 100).toFixed(2).toString();
        this.divider.ariaValueMax = ((containerSize - panelGap - this.minPanelSize) / containerSize * 100).toFixed(2).toString();

        if (shouldRecalculateSize) {
            this.resetSizes();
        }
    }

    /**
     * Resets both panel sizes using either the provided size or initial configuration.
     */
    resetSizes(firstPanelSize: number | null = null) {
        this.firstPanel.style.width = "100%";
        this.secondPanel.style.width = "100%";
        this.firstPanel.style.height = "100%";
        this.secondPanel.style.height = "100%";

        const firstPanelSizeNew = firstPanelSize !== null ? firstPanelSize : this.firstPanelInitialSize;
        if (firstPanelSizeNew === null) {
            this.divider.ariaValueNow = "50";
        } else {
            this._setPanelSizes(firstPanelSizeNew, this._getContainerSize());
        }
    }

    /**
     * Returns the current divider offset in pixels.
     */
    getDividerPosition() {
        return this.horizontal ? this.firstPanel.clientHeight : this.firstPanel.clientWidth;
    }

    /**
     * Sets the divider offset in pixels.
     */
    setDividerPosition(offset: number) {
        this.resetSizes(offset);
    }

    _getContainerSize() {
        return this.horizontal ? this.container!.offsetHeight : this.container!.offsetWidth;
    }

    _onMouseDown(e: PointerLikeEvent) {
        e.preventDefault();
        this.isDragging = true;
        this.dividerHasMoved = false;

        const { clientX, clientY } = clientPosition(e);

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

    _onMouseMove(e: PointerLikeEvent) {
        if (!this.isDragging) return;

        const { clientX, clientY } = clientPosition(e);
        const delta = this.horizontal ? clientY - this.startPos : clientX - this.startPos;
        const newFirstSize = this.startFirstSize + delta;

        const containerSize = this._getContainerSize();
        const min = this.minPanelSize;
        const max = containerSize - this.panelGap - min;

        if (newFirstSize >= min && newFirstSize <= max) {
            this.dividerHasMoved = true;
            this._setPanelSizes(newFirstSize, containerSize);
        }
    }

    _setPanelSizes(newFirstSize: number, containerSize: number) {
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

        if (this.dividerHasMoved) {
            this.lastDragEndDate = Date.now();
        }

        document.body.style.userSelect = "";
        document.body.style.cursor = "";
        document.removeEventListener("mousemove", this._onMouseMove);
        document.removeEventListener("touchmove", this._onMouseMove);
        document.removeEventListener("mouseup", this._onMouseUp);
        document.removeEventListener("touchend", this._onMouseUp);
    }

    _onDoubleClick() {
        if (!this.resetOnDoubleClick) return;

        // Fixes the edge case where the user first clicks and then clicks again and holds to drag
        if (Date.now() - this.lastDragEndDate < 100) return;

        const containerSize = this.horizontal
            ? this.container!.offsetHeight
            : this.container!.offsetWidth;

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

    _onKeyDown(e: KeyboardEvent) {
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
        } else if (key === "ArrowLeft") {
            delta = -this.keyboardStep;
        } else if (key === "ArrowRight") {
            delta = this.keyboardStep;
        } else if (key === "Home") {
            this._setPanelSizes(this.minPanelSize, containerSize);
        } else if (key === "End") {
            this._setPanelSizes(containerSize - this.panelGap - this.minPanelSize, containerSize);
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

/**
 * Updates split panel options for an existing container ID.
 */
export const mudSplitPanel_update = function (id: string, horizontal: boolean, resetOnDoubleClick: boolean, minPanelSize: number, panelGap: number) {
    window.splitPanels[id]!.update(horizontal, resetOnDoubleClick, minPanelSize, panelGap);
};

/**
 * Resets the divider position to the configured initial value.
 */
export const mudSplitPanel_resetDividerPosition = function (id: string) {
    window.splitPanels[id]!.resetSizes();
};

/**
 * Returns the divider position for a split panel container ID.
 */
export const mudSplitPanel_getDividerPosition = function (id: string) {
    return window.splitPanels[id]!.getDividerPosition();
};

/**
 * Sets the divider position for a split panel container ID.
 */
export const mudSplitPanel_setDividerPosition = function (id: string, offset: number) {
    window.splitPanels[id]!.setDividerPosition(offset);
};

/**
 * Disposes the split panel runtime instance for a container ID.
 */
export const mudSplitPanel_destroy = function (id: string) {
    window.splitPanels[id]!.destroy();
};
