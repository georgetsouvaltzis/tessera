import type { PrismTheme } from 'prism-react-renderer';

const luminaPrismTheme: PrismTheme = {
  plain: {
    color: '#f2dde8',
    backgroundColor: 'transparent',
  },
  styles: [
    {
      types: ['comment', 'prolog', 'doctype', 'cdata'],
      style: {
        color: 'rgba(214, 176, 197, 0.72)',
        fontStyle: 'italic',
      },
    },
    {
      types: ['builtin', 'class-name', 'type', 'namespace'],
      style: {
        color: '#2dd4df',
      },
    },
    {
      types: ['function', 'method'],
      style: {
        color: '#22d3ee',
      },
    },
    {
      types: ['keyword', 'atrule', 'selector'],
      style: {
        color: '#ff7fc5',
      },
    },
    {
      types: ['string', 'char', 'attr-value', 'inserted'],
      style: {
        color: '#86efac',
      },
    },
    {
      types: ['number', 'boolean', 'constant', 'symbol'],
      style: {
        color: '#ffcc80',
      },
    },
    {
      types: ['operator', 'punctuation'],
      style: {
        color: '#b28a9f',
      },
    },
    {
      types: ['property', 'variable', 'parameter', 'attr-name'],
      style: {
        color: '#f2dde8',
      },
    },
    {
      types: ['regex', 'important'],
      style: {
        color: '#c28dff',
      },
    },
    {
      types: ['deleted'],
      style: {
        color: '#fb7185',
      },
    },
    {
      types: ['tag'],
      style: {
        color: '#ff7fc5',
      },
    },
  ],
};

export default luminaPrismTheme;
