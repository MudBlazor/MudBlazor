// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Bundle entrypoint for MudBlazor JS interop.
// Why: every `window.mud*` API used from C# must be imported here so it lands in `wwwroot/MudBlazor.min.js`.
// Keep this list in sync with the TScripts directory (excluding this file).
import "./mudAAAlicense";
import "./mudDragAndDrop";
import "./mudElementReference";
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
import "./mudWindow";
