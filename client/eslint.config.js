import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'
import importPlugin from 'eslint-plugin-import'

export default tseslint.config(
  { ignores: ['dist'] },
  {
    extends: [js.configs.recommended, ...tseslint.configs.recommended],
    files: ['**/*.{ts,tsx}'],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
    },
    plugins: {
      'react-hooks': reactHooks,
      'react-refresh': reactRefresh,
      'import': importPlugin,
    },
    rules: {
      ...reactHooks.configs.recommended.rules,
      'react-hooks/exhaustive-deps': 'off',
      'react-refresh/only-export-components': [
        'warn',
        { allowConstantExport: true },
      ],
      'import/no-restricted-paths': [
        'error',
        {
          zones: [
            {
              target: './src/models/**/*',
              from: ['./src/atoms/**/*', './src/functions/**/*', './src/hooks/**/*', './src/components/**/*', './src/pages/**/*']
            },
            {
              target: './src/atoms/**/*', 
              from: ['./src/functions/**/*', './src/hooks/**/*', './src/components/**/*', './src/pages/**/*']
            },
            {
              target: './src/functions/**/*',
              from: ['./src/hooks/**/*', './src/components/**/*', './src/pages/**/*']
            },
            {
              target: './src/hooks/**/*',
              from: ['./src/components/**/*', './src/pages/**/*']
            },
            {
              target: './src/components/**/*',
              from: ['./src/pages/**/*']
            }
          ]
        }
      ],
      'no-restricted-imports': [
        'error',
        {
          patterns: [
            {
              group: ['../models/*', '../atoms/*', '../functions/*', '../hooks/*', '../components/*', '../pages/*'],
              message: 'Use absolute imports through index files instead'
            },
            {
              group: ['./models/*', './atoms/*', './functions/*', './hooks/*', './components/*', './pages/*'],
              message: 'Import through the module index file'
            }
          ]
        }
      ]
    },
  },
)