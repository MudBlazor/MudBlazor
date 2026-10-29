import { afterEach, beforeEach, expect, mock, spyOn, test } from "bun:test";
import type { DotNet } from "@microsoft/dotnet-js-interop";
import MudThemeProvider from "./mudThemeProvider";

// A controllable media query, because happy-dom cannot emulate a change of the OS color scheme.
class TestMediaQueryList extends EventTarget {
    constructor(public matches: boolean) {
        super();
    }

    change(matches: boolean) {
        this.matches = matches;
        this.dispatchEvent(Object.assign(new Event("change"), { matches }));
    }
}

let darkModeQuery: TestMediaQueryList;

function createDotNetHelper() {
    const invokeMethodAsync = mock(async (_method: string, _isDarkMode: boolean) => { });
    return { helper: { invokeMethodAsync } as unknown as DotNet.DotNetObject, invokeMethodAsync };
}

beforeEach(() => {
    darkModeQuery = new TestMediaQueryList(false);
    spyOn(window, "matchMedia").mockImplementation(() => darkModeQuery as unknown as MediaQueryList);
});

afterEach(() => {
    mock.restore();
});

test("queries the system color scheme preference", () => {
    new MudThemeProvider();

    expect(window.matchMedia).toHaveBeenCalledWith("(prefers-color-scheme: dark)");
});

test("isDarkMode reflects the current system preference", () => {
    const themeProvider = new MudThemeProvider();
    expect(themeProvider.isDarkMode()).toBe(false);

    darkModeQuery.matches = true;
    expect(themeProvider.isDarkMode()).toBe(true);
});

test("watchDarkMode forwards system preference changes to .NET", () => {
    const themeProvider = new MudThemeProvider();
    const { helper, invokeMethodAsync } = createDotNetHelper();

    themeProvider.watchDarkMode(helper);
    darkModeQuery.change(true);
    darkModeQuery.change(false);

    expect(invokeMethodAsync.mock.calls).toEqual([
        ["SystemDarkModeChangedAsync", true],
        ["SystemDarkModeChangedAsync", false],
    ]);
});

test("watching again replaces the .NET target without duplicating notifications", () => {
    const themeProvider = new MudThemeProvider();
    const first = createDotNetHelper();
    const second = createDotNetHelper();

    themeProvider.watchDarkMode(first.helper);
    themeProvider.watchDarkMode(second.helper);
    darkModeQuery.change(true);

    expect(first.invokeMethodAsync).not.toHaveBeenCalled();
    expect(second.invokeMethodAsync).toHaveBeenCalledTimes(1);
});

test("stopWatchingDarkMode stops forwarding changes", () => {
    const themeProvider = new MudThemeProvider();
    const { helper, invokeMethodAsync } = createDotNetHelper();

    themeProvider.watchDarkMode(helper);
    themeProvider.stopWatchingDarkMode();
    darkModeQuery.change(true);

    expect(invokeMethodAsync).not.toHaveBeenCalled();
});
