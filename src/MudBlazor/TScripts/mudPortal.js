/**
 *
 * @param {{id:string, isVisible:boolean, cssPosition:string}} portalModel
 * @param {RectInfo} portalAnchor
 * @returns
 */
function mudHandlePortal(portalModel, portalAnchor) {
    let portalledElement = document.getElementById(portalModel.id);
    if (!portalledElement || !portalAnchor) return;

    let action = portalModel.isVisible ? 'add' : 'remove';
    portalledElement.firstElementChild.classList[action](
        'mud-popover-open',
        'mud-tooltip-visible'
    );

    if (!portalModel.isVisible) return;

    let anchorRect = window.mudElementRef.getBoundingClientRect(
        portalAnchor.parentElement
    );

    let fragmentRect = window.mudElementRef.getBoundingClientRect(
        portalAnchor.firstElementChild
    );

    let correctedAnchorRect = mudCorrectAnchorBoundaries(anchorRect, fragmentRect);

    let style = mudGetAnchorStyle(correctedAnchorRect, portalModel);

    portalledElement.style.cssText = style;
}

function mudGetAnchorStyle(anchorRect, portalModel) {
    let top =
        portalModel.cssPosition == 'fixed'
            ? anchorRect?.top
            : anchorRect?.absoluteTop;

    let left =
        portalModel.cssPosition == 'fixed'
            ? anchorRect?.left
            : anchorRect?.absoluteLeft;

    let height = anchorRect?.height;
    let width = anchorRect?.width;
    let position = !portalModel.isVisible ? 'fixed' : portalModel.cssPosition;
    let zIndex = portalModel.cssPosition == 'fixed' && 1400;

    return `top:${top}px;left:${left}px;height:${height}px;width:${width}px;position:${position};z-index:${zIndex}`;
}

class RectInfo {
    /**
     *
     * @param {RectInfo} rect
     */
    constructor(rect) {
        this.left = rect.left;
        this.top = rect.top;
        this.width = rect.width;
        this.height = rect.height;
        this.windowHeight = rect.windowHeight;
        this.windowWidth = rect.windowWidth;
        this.scrollX = rect.scrollX;
        this.scrollY = rect.scrollY;
    }

    get right() {
        return this.left + this.width;
    }

    get bottom() {
        return this.top + this.height;
    }

    get absoluteLeft() {
        return this.left + this.scrollX;
    }

    get absoluteTop() {
        return this.top + this.scrollY;
    }

    get absoluteRight() {
        return this.right + this.scrollX;
    }

    get absoluteBottom() {
        return this.bottom + this.scrollY;
    }

    //check if the rect is outside of the viewport
    get isOutsideBottom() {
        return this.bottom > this.windowHeight;
    }

    get isOutsideLeft() {
        return this.left < 0;
    }

    get isOutsideTop() {
        return this.top < 0;
    }

    get isOutsideRight() {
        return this.right > this.windowWidth;
    }
}

/**
 *
 * @param {RectInfo} anchorRect
 * @param {RectInfo} fragmentRect
 * @returns
 */

function mudCorrectAnchorBoundaries(anchorRect, fragmentRect) {
    if (!fragmentRect || !anchorRect) return;
    anchorRect = new RectInfo(anchorRect);
    fragmentRect = new RectInfo(fragmentRect);

    let rectified = mudShallowClone(anchorRect);
    
    let fragmentIsAboveorBelowAnchor =
        fragmentRect.top > anchorRect.bottom ||
        fragmentRect.bottom < anchorRect.top;

    // comes out at the bottom
    if (fragmentRect.isOutsideBottom) {
        rectified.top -=
            2 * (fragmentRect.top - anchorRect.bottom) +
            anchorRect.height +
            fragmentRect.height;
    }

    // comes out at the top
    if (fragmentRect.isOutsideTop) {
        rectified.top +=
            2 * Math.abs(anchorRect.top - fragmentRect.bottom) +
            anchorRect.height +
            fragmentRect.height;
    }

    // comes out at the left
    if (fragmentRect.isOutsideLeft) {
        rectified.left += fragmentIsAboveorBelowAnchor
            ? anchorRect.left - fragmentRect.left
            : 2 * Math.abs(anchorRect.left - fragmentRect.right) +
            fragmentRect.width +
            anchorRect.width;
    }

    // comes out at the right
    if (fragmentRect.isOutsideRight) {
        rectified.left -= fragmentIsAboveorBelowAnchor
            ? fragmentRect.right - anchorRect.right
            : 2 * Math.abs(fragmentRect.left - anchorRect.right) +
            fragmentRect.width +
            anchorRect.width;
    }

    return rectified;
}

function mudShallowClone(obj) {
    return Object.create(
        Object.getPrototypeOf(obj),
        Object.getOwnPropertyDescriptors(obj)
    );
}
