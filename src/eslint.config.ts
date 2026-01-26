import { defineConfig } from "eslint/config";

export default defineConfig({
    languageOptions: {
        ecmaVersion: 2024,
        sourceType: "module",
    },
    rules: {
        //"curly": "error",
        //"eqeqeq": ["error", "always"],
        "no-debugger": "error",
        //"no-undef": "error",
        "no-unused-vars": ["error", {
            // Allow marking unused variables with an underscore, like in C#
            "varsIgnorePattern": "^_",
            "argsIgnorePattern": "^_",
            "caughtErrorsIgnorePattern": "^_"
        }],
        "no-var": "error",
        "prefer-const": "error",
        "semi": ["error", "always"],
    },
});
