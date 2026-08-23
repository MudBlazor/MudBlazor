// Legacy side effect imports
import "./mudAAAlicense";
import "./mudDragAndDrop";
import "./mudElementReference";
import "./mudExitPrompt";
import "./mudFileUpload";
import "./mudHelpers";
import "./mudHotkeyListener";
import "./mudInput";
import "./mudInputSizing";
import "./mudJsEvent";
import "./mudKeyInterceptor";
import "./mudPointerEventsNone";
import "./mudPointerCapture";
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
import MudWindow from "./MudWindow";

declare global {
    interface Window {
        mudWindow: MudWindow;
    }
}

window.mudWindow = new MudWindow();
