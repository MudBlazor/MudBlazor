// Must stay the first import so the license banner leads the bundle.
import "./mudAAAlicense";

// Legacy side effect imports
import "./mudDragAndDrop";
import "./mudElementReference";
import "./mudExitPrompt";
import "./mudHelpers";
import "./mudHotkeyListener";
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
import "./mudTimePicker";

// Code ported to TypeScript
import MudFileUpload from "./mudFileUpload";
import MudInput from "./mudInput";
import MudPointerCapture from "./mudPointerCapture";
import MudTableCell from "./mudTableCell";
import MudThemeProvider from "./mudThemeProvider";
import MudWindow from "./MudWindow";

declare global {
    interface Window {
        mudFileUpload: MudFileUpload;
        mudInput: MudInput;
        mudPointerCapture: MudPointerCapture;
        mudTableCell: MudTableCell;
        mudThemeProvider: MudThemeProvider;
        mudWindow: MudWindow;
    }
}

window.mudFileUpload = new MudFileUpload();
window.mudInput = new MudInput();
window.mudPointerCapture = new MudPointerCapture();
window.mudTableCell = new MudTableCell();
window.mudThemeProvider = new MudThemeProvider();
window.mudWindow = new MudWindow();
