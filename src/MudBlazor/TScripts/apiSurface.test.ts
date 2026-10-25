import { expect, test } from "bun:test";

// Blazor interop resolves identifiers like "mudInput.resetValue" against window at runtime,
// so the runtime shape of every global is the public API, regardless of how the source is written.
// This snapshot pins that shape while scripts are ported to TypeScript.
// It records types and function arity, never values, so it only changes when the surface does.

type Surface = string | { [member: string]: Surface };

const globalsBefore = new Set(Object.getOwnPropertyNames(globalThis));
await import("./entrypoint");
const addedGlobals = Object.getOwnPropertyNames(globalThis)
    .filter(name => !globalsBefore.has(name))
    .sort();

function isNative(fn: unknown) {
    return typeof fn === "function" && Function.prototype.toString.call(fn).includes("[native code]");
}

// Plain object literals are interop entry points even though their constructor is the built-in Object.
function isBuiltIn(value: object) {
    return value.constructor !== Object && isNative(value.constructor);
}

/**
 * Collects the member names reachable on an object, walking its prototype chain up to the first built-in prototype.
 */
function memberNames(value: object) {
    const names = new Set<string>();
    for (let current: object | null = value; current && current !== Object.prototype && !isBuiltIn(current); current = Object.getPrototypeOf(current)) {
        for (const name of Object.getOwnPropertyNames(current)) {
            if (name !== "constructor") names.add(name);
        }
    }
    return [...names].sort();
}

function describeSurface(value: unknown, seen: Set<unknown>): Surface {
    if (typeof value === "function") {
        return `function(${value.length})`;
    }
    if (value === null || typeof value !== "object") {
        return value === null ? "null" : typeof value;
    }
    // Built-in objects such as MediaQueryList or Map are implementation details, not interop entry points.
    if (isBuiltIn(value)) {
        return `[object ${value.constructor.name}]`;
    }
    if (seen.has(value)) {
        return "[circular]";
    }
    seen.add(value);

    const surface: Record<string, Surface> = {};
    for (const name of memberNames(value)) {
        surface[name] = describeSurface((value as Record<string, unknown>)[name], seen);
    }
    return surface;
}

test("window API surface is unchanged", () => {
    if (addedGlobals.length === 0) {
        throw new Error("entrypoint.ts was already loaded by another test file. Run bun test with --isolate.");
    }

    const surface: Record<string, Surface> = {};
    for (const name of addedGlobals) {
        surface[name] = describeSurface((globalThis as Record<string, unknown>)[name], new Set());
    }
    expect(surface).toMatchSnapshot();
});
