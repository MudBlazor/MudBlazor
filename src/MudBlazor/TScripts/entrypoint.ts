// Must stay the first import so the license banner leads the bundle.
import "./mudAAAlicense";

// Legacy side effect imports
import "./mudKeyInterceptor";
import "./mudPopover";
import "./mudSplitPanel";

// Code ported to TypeScript
import MudDragAndDrop from "./mudDragAndDrop";
import MudElementReference from "./mudElementReference";
import MudExitPrompt from "./mudExitPrompt";
import MudFileUpload from "./mudFileUpload";
import { getTabbableElements, hasDefinedParentHeight, mudGetSvgBBox, mudObserveElementSize, serializeParameter } from "./mudHelpers";
import MudHotkeyListener from "./mudHotkeyListener";
import MudInput from "./mudInput";
import MudInputSizing from "./mudInputSizing";
import MudJsEventFactory from "./mudJsEvent";
import MudPointerCapture from "./mudPointerCapture";
import { registerRippleHandlers } from "./mudRipple";
import { MudResizeListener, MudResizeListenerFactory } from "./mudResizeListener";
import MudPointerEventsNone from "./mudPointerEventsNone";
import MudResizeObserverFactory from "./mudResizeObserver";
import MudScrollListener from "./mudScrollListener";
import MudScrollManager from "./mudScrollManager";
import MudScrollSpy from "./mudScrollSpy";
import MudTableCell from "./mudTableCell";
import MudThemeProvider from "./mudThemeProvider";
import MudTimePicker from "./mudTimePicker";
import MudWindow from "./MudWindow";

declare global {
    interface Window {
        getTabbableElements: typeof getTabbableElements;
        hasDefinedParentHeight: typeof hasDefinedParentHeight;
        mudDragAndDrop: MudDragAndDrop;
        mudElementRef: MudElementReference;
        mudExitPrompt: MudExitPrompt;
        mudFileUpload: MudFileUpload;
        mudGetSvgBBox: typeof mudGetSvgBBox;
        mudHotkeyListener: MudHotkeyListener;
        mudInput: MudInput;
        mudInputSizing: MudInputSizing;
        mudObserveElementSize: typeof mudObserveElementSize;
        mudJsEvent: MudJsEventFactory;
        mudPointerCapture: MudPointerCapture;
        mudPointerEventsNone: MudPointerEventsNone;
        mudResizeListener: MudResizeListener;
        mudResizeListenerFactory: MudResizeListenerFactory;
        mudResizeObserver: MudResizeObserverFactory;
        mudScrollListener: MudScrollListener;
        mudScrollManager: MudScrollManager;
        mudScrollSpy: MudScrollSpy;
        mudTableCell: MudTableCell;
        mudThemeProvider: MudThemeProvider;
        mudTimePicker: MudTimePicker;
        mudWindow: MudWindow;
        serializeParameter: typeof serializeParameter;
    }
}

window.getTabbableElements = getTabbableElements;
window.hasDefinedParentHeight = hasDefinedParentHeight;
window.mudDragAndDrop = new MudDragAndDrop();
window.mudElementRef = new MudElementReference();
// Guard against a second copy of the script replacing the instance that owns the registered prompts.
if (!window.mudExitPrompt) {
    window.mudExitPrompt = new MudExitPrompt();
}
window.mudFileUpload = new MudFileUpload();
window.mudGetSvgBBox = mudGetSvgBBox;
if (!window.mudHotkeyListener) {
    window.mudHotkeyListener = new MudHotkeyListener();
}
window.mudInput = new MudInput();
window.mudInputSizing = new MudInputSizing();
window.mudJsEvent = new MudJsEventFactory();
window.mudObserveElementSize = mudObserveElementSize;
window.mudPointerCapture = new MudPointerCapture();
window.mudPointerEventsNone = new MudPointerEventsNone();
window.mudResizeListener = new MudResizeListener();
window.mudResizeListenerFactory = new MudResizeListenerFactory();
window.mudResizeObserver = new MudResizeObserverFactory();
if (!window.mudScrollListener) {
    window.mudScrollListener = new MudScrollListener();
}
window.mudScrollManager = new MudScrollManager();
window.mudScrollSpy = new MudScrollSpy();
window.mudTableCell = new MudTableCell();
window.mudThemeProvider = new MudThemeProvider();
window.mudTimePicker = new MudTimePicker();
window.mudWindow = new MudWindow();
window.serializeParameter = serializeParameter;

registerRippleHandlers();
