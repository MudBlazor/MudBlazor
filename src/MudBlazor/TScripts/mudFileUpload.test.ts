import { afterEach, expect, test } from "bun:test";
import MudFileUpload from "./mudFileUpload";

const mudFileUpload = new MudFileUpload();

// happy-dom can't select a folder, so the input gets a stand-in FileList with the given relative paths.
function renderFolderInput(id: string, paths: string[]) {
    const input = document.createElement("input");
    input.type = "file";
    input.id = id;
    const files = {
        length: paths.length,
        item: (index: number) => (index < paths.length ? { webkitRelativePath: paths[index] } : null),
    };
    Object.defineProperty(input, "files", { value: files });
    document.body.append(input);
}

afterEach(() => {
    document.body.replaceChildren();
});

test("getRelativePaths returns the paths from the start index", () => {
    renderFolderInput("upload", ["Photos/a.jpg", "Photos/2025/b.jpg", "Photos/2025/c.jpg"]);

    expect(mudFileUpload.getRelativePaths("upload", 0)).toEqual(["Photos/a.jpg", "Photos/2025/b.jpg", "Photos/2025/c.jpg"]);
    expect(mudFileUpload.getRelativePaths("upload", 2)).toEqual(["Photos/2025/c.jpg"]);
});

test("getRelativePaths splits a large folder into results under 16 KB", () => {
    const paths = Array.from({ length: 1000 }, (_, i) => `Folder/${"ü".repeat(100)}/${i}.txt`);
    renderFolderInput("upload", paths);

    const read: string[] = [];
    let calls = 0;
    while (read.length < paths.length) {
        const chunk = mudFileUpload.getRelativePaths("upload", read.length);
        expect(chunk.length).toBeGreaterThan(0);
        expect(new TextEncoder().encode(JSON.stringify(chunk)).length).toBeLessThan(16 * 1024);
        read.push(...chunk);
        calls++;
    }

    expect(read).toEqual(paths);
    expect(calls).toBeGreaterThan(1);
});

test("getRelativePaths returns an empty list for an unknown input", () => {
    expect(mudFileUpload.getRelativePaths("missing", 0)).toEqual([]);
});
