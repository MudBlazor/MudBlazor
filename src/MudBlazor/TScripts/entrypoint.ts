// Must stay the first import so the license banner leads the bundle.
import "./mudAAAlicense";

// Legacy side effect imports
import "./mudDragAndDrop";
import "./mudElementReference";
import "./mudExitPrompt";
import "./mudHelpers";
import "./mudHotkeyListener";
import "./mudInput";
import "./mudInputSizing";
import "./mudJsEvent";
import "./mudKeyInterceptor";
import "./mudPointerEventsNone";
import "./mudPopover";
import "./mudResizeListener";
import "./mudResizeObserver";
import "./mudRipple";
import "./mudScrollListener";
import "./mudScrollManager";
import "./mudScrollSpy";
import "./mudSplitPanel";
import "./mudTableCell";
import "./mudThemeProvider";
import "./mudTimePicker";

// Code ported to TypeScript
import MudFileUpload from "./mudFileUpload";
import MudPointerCapture from "./mudPointerCapture";
import MudWindow from "./MudWindow";

declare global {
    interface Window {
        mudFileUpload: MudFileUpload;
        mudPointerCapture: MudPointerCapture;
        mudWindow: MudWindow;
    }
}

window.mudFileUpload = new MudFileUpload();
window.mudPointerCapture = new MudPointerCapture();
window.mudWindow = new MudWindow();
