module.exports = {
  parserOptions: {
    ecmaVersion: 2021,
    sourceType: 'module',
  },
  plugins: ['react', 'react-hooks'],
  extends: [
    'eslint:recommended',
    'plugin:react/recommended',
    'plugin:react-hooks/recommended',
  ],
  env: {
    browser: true,
  },
  settings: {
    react: {
      version: 'detect',
    },
  },
  rules: {
    'react/react-in-jsx-scope': 'off',
    'react/jsx-uses-vars': 'error',
    'react-hooks/rules-of-hooks': 'error',
    'react-hooks/exhaustive-deps': 'warn',
    'react/prop-types': 'off',
    'array-callback-return': 'error',
  },

  overrides: [
    {
      files: ['**/service-worker.js'],
      env: {
        worker: true,
        browser: false,
        es2021: true,
      },
      globals: {
        self: 'readonly',
      },
      rules: {
        'array-callback-return': 'error',
      },
    },
  ],
};