// Must stay the first import so the license banner leads the bundle.
import "./mudAAAlicense";

// Legacy side effect imports
import "./mudElementReference";
import "./mudHelpers";
import "./mudInputSizing";
import "./mudJsEvent";
import "./mudKeyInterceptor";
import "./mudPointerEventsNone";
import "./mudPopover";
import "./mudResizeListener";
import "./mudResizeObserver";
import "./mudSplitPanel";

// Code ported to TypeScript
import MudDragAndDrop from "./mudDragAndDrop";
import MudExitPrompt from "./mudExitPrompt";
import MudFileUpload from "./mudFileUpload";
import MudHotkeyListener from "./mudHotkeyListener";
import MudInput from "./mudInput";
import MudPointerCapture from "./mudPointerCapture";
import { registerRippleHandlers } from "./mudRipple";
import MudScrollListener from "./mudScrollListener";
import MudScrollManager from "./mudScrollManager";
import MudScrollSpy from "./mudScrollSpy";
import MudTableCell from "./mudTableCell";
import MudThemeProvider from "./mudThemeProvider";
import MudTimePicker from "./mudTimePicker";
import MudWindow from "./MudWindow";

declare global {
    interface Window {
        mudDragAndDrop: MudDragAndDrop;
        mudExitPrompt: MudExitPrompt;
        mudFileUpload: MudFileUpload;
        mudHotkeyListener: MudHotkeyListener;
        mudInput: MudInput;
        mudPointerCapture: MudPointerCapture;
        mudScrollListener: MudScrollListener;
        mudScrollManager: MudScrollManager;
        mudScrollSpy: MudScrollSpy;
        mudTableCell: MudTableCell;
        mudThemeProvider: MudThemeProvider;
        mudTimePicker: MudTimePicker;
        mudWindow: MudWindow;
    }
}

window.mudDragAndDrop = new MudDragAndDrop();
// Guard against a second copy of the script replacing the instance that owns the registered prompts.
if (!window.mudExitPrompt) {
    window.mudExitPrompt = new MudExitPrompt();
}
window.mudFileUpload = new MudFileUpload();
if (!window.mudHotkeyListener) {
    window.mudHotkeyListener = new MudHotkeyListener();
}
window.mudInput = new MudInput();
window.mudPointerCapture = new MudPointerCapture();
if (!window.mudScrollListener) {
    window.mudScrollListener = new MudScrollListener();
}
window.mudScrollManager = new MudScrollManager();
window.mudScrollSpy = new MudScrollSpy();
window.mudTableCell = new MudTableCell();
window.mudThemeProvider = new MudThemeProvider();
window.mudTimePicker = new MudTimePicker();
window.mudWindow = new MudWindow();

registerRippleHandlers();
