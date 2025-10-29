import { FlatCompat } from "@eslint/eslintrc";
import path from "path";
import globals from "globals";
import { fileURLToPath } from 'url';
import eslintJs from '@eslint/js';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const compat = new FlatCompat({
  baseDirectory: __dirname,
});

const config = [
  {
    ignores: ["**/service-worker.js"],
  },
  ...compat.extends(
    "react-app",
    "react-app/jest",
    "plugin:react/recommended",
    "plugin:react-hooks/recommended",
  ),
  {
    files: ["**/*.{js,jsx,mjs,cjs}"],
    languageOptions: {
      ecmaVersion: 2022,
      sourceType: "module",
      globals: {
        ...compat.env({ browser: true }).globals,
      }
    },
    ...eslintJs.configs.recommended,
    settings: {
      react: {
        version: "detect",
      },
    },
    rules: {
      'no-unused-vars': 'warn',
      'react/react-in-jsx-scope': 'off',
      'react/jsx-uses-vars': 'error',
      'react-hooks/rules-of-hooks': 'error',
      'react-hooks/exhaustive-deps': 'warn',
      'react/prop-types': 'off',
      'array-callback-return': 'error',
      'react-hooks/set-state-in-effect': 'off',
    },
  },
  {
    files: ["**/service-worker.js"],
    languageOptions: {
      ecmaVersion: 2022,
      sourceType: "module",
      globals: {
        ...globals.worker,
        self: 'readonly',
      },
    },
    rules: {
      'array-callback-return': 'error',
    },
  },
];

export default config;