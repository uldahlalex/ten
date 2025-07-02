module.exports = {
  forbidden: [
    {
      name: 'no-circular',
      severity: 'error',
      from: {},
      to: { circular: true }
    },
    {
      name: 'models-cannot-import-upper-layers',
      severity: 'error', 
      from: { path: '^src/models' },
      to: { path: '^src/(atoms|functions|hooks|components|pages)' }
    },
    {
      name: 'atoms-cannot-import-upper-layers',
      severity: 'error',
      from: { path: '^src/atoms' },
      to: { path: '^src/(functions|hooks|components|pages)' }
    },
    {
      name: 'functions-cannot-import-upper-layers', 
      severity: 'error',
      from: { path: '^src/functions' },
      to: { path: '^src/(hooks|components|pages)' }
    },
    {
      name: 'hooks-cannot-import-upper-layers',
      severity: 'error',
      from: { path: '^src/hooks' },
      to: { path: '^src/(components|pages)' }
    },
    {
      name: 'components-cannot-import-pages',
      severity: 'error',
      from: { path: '^src/components' },
      to: { path: '^src/pages' }
    }
  ],
  options: {
    doNotFollow: {
      path: 'node_modules'
    },
    exclude: {
      path: '\\.(test|spec)\\.(js|ts|tsx)$'
    },
    tsPreCompilationDeps: true,
    tsConfig: {
      fileName: './tsconfig.json'
    }
  }
};