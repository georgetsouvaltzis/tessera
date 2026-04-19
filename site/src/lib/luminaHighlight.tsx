import React from 'react';

type TokenKind =
  | 'attribute'
  | 'comment'
  | 'flag'
  | 'function'
  | 'ident'
  | 'keyword'
  | 'number'
  | 'prompt'
  | 'punct'
  | 'string'
  | 'tag'
  | 'text'
  | 'type'
  | 'variable'
  | 'ws';

type Token = {
  kind: TokenKind;
  value: string;
};

const CSHARP_KEYWORDS = new Set([
  'abstract', 'as', 'async', 'await', 'base', 'bool', 'break', 'byte', 'case',
  'catch', 'char', 'checked', 'class', 'const', 'continue', 'decimal', 'default',
  'delegate', 'do', 'double', 'else', 'enum', 'event', 'explicit', 'extern',
  'false', 'finally', 'fixed', 'float', 'for', 'foreach', 'goto', 'if',
  'implicit', 'in', 'int', 'interface', 'internal', 'is', 'lock', 'long',
  'namespace', 'new', 'null', 'object', 'operator', 'out', 'override', 'params',
  'private', 'protected', 'public', 'readonly', 'record', 'ref', 'return',
  'sbyte', 'sealed', 'short', 'sizeof', 'stackalloc', 'static', 'string', 'struct',
  'switch', 'this', 'throw', 'true', 'try', 'typeof', 'uint', 'ulong', 'unchecked',
  'unsafe', 'ushort', 'using', 'var', 'virtual', 'void', 'volatile', 'while',
]);

const TOKEN_CLASSES: Record<TokenKind, string> = {
  attribute: 'lumina-token lumina-token--attribute',
  comment: 'lumina-token lumina-token--comment',
  flag: 'lumina-token lumina-token--flag',
  function: 'lumina-token lumina-token--function',
  ident: 'lumina-token lumina-token--ident',
  keyword: 'lumina-token lumina-token--keyword',
  number: 'lumina-token lumina-token--number',
  prompt: 'lumina-token lumina-token--prompt',
  punct: 'lumina-token lumina-token--punct',
  string: 'lumina-token lumina-token--string',
  tag: 'lumina-token lumina-token--tag',
  text: 'lumina-token lumina-token--text',
  type: 'lumina-token lumina-token--type',
  variable: 'lumina-token lumina-token--variable',
  ws: '',
};

function tokenizeCSharp(src: string): Token[] {
  const tokens: Token[] = [];
  let remaining = src;

  while (remaining.length > 0) {
    const comment = remaining.match(/^(\/\/[^\n]*|\/\*[\s\S]*?\*\/)/);
    const str = remaining.match(/^@?"(?:[^"\\]|\\.|"")*"/);
    const attribute = remaining.match(/^\[[A-Za-z_][A-Za-z0-9_]*\]/);
    const number = remaining.match(/^\b\d+(?:\.\d+)?\b/);
    const ident = remaining.match(/^[A-Za-z_][A-Za-z0-9_]*/);
    const punct = remaining.match(/^[{}()[\].,;:+\-*/%&|^!=<>?]/);
    const ws = remaining.match(/^\s+/);

    if (comment) tokens.push({ kind: 'comment', value: comment[0] });
    else if (str) tokens.push({ kind: 'string', value: str[0] });
    else if (attribute) tokens.push({ kind: 'attribute', value: attribute[0] });
    else if (number) tokens.push({ kind: 'number', value: number[0] });
    else if (ident) {
      if (CSHARP_KEYWORDS.has(ident[0])) {
        tokens.push({ kind: 'keyword', value: ident[0] });
      } else if (/^[A-Z]/.test(ident[0])) {
        tokens.push({ kind: 'type', value: ident[0] });
      } else {
        tokens.push({ kind: 'ident', value: ident[0] });
      }
    } else if (punct) tokens.push({ kind: 'punct', value: punct[0] });
    else if (ws) tokens.push({ kind: 'ws', value: ws[0] });
    else tokens.push({ kind: 'text', value: remaining[0] });

    remaining = remaining.slice(tokens[tokens.length - 1].value.length);
  }

  for (let i = 0; i < tokens.length - 1; i++) {
    if (tokens[i].kind === 'ident') {
      let j = i + 1;
      while (j < tokens.length && tokens[j].kind === 'ws') j++;
      if (tokens[j]?.value === '(') {
        tokens[i] = { ...tokens[i], kind: 'function' };
      }
    }
  }

  return tokens;
}

function tokenizeBash(src: string): Token[] {
  const tokens: Token[] = [];
  let remaining = src;
  let atLineStart = true;

  while (remaining.length > 0) {
    const comment = remaining.match(/^#[^\n]*/);
    const str = remaining.match(/^(?:"(?:[^"\\]|\\.)*"|'(?:[^'\\]|\\.)*')/);
    const flag = remaining.match(/^--?[A-Za-z0-9-]+/);
    const variable = remaining.match(/^\$[A-Za-z_][A-Za-z0-9_]*/);
    const prompt = atLineStart ? remaining.match(/^\$/) : null;
    const number = remaining.match(/^\b\d+(?:\.\d+)?\b/);
    const ident = remaining.match(/^[A-Za-z_./][A-Za-z0-9_./-]*/);
    const punct = remaining.match(/^[|&;()[\]{}<>]/);
    const ws = remaining.match(/^\s+/);

    if (comment) tokens.push({ kind: 'comment', value: comment[0] });
    else if (str) tokens.push({ kind: 'string', value: str[0] });
    else if (flag) tokens.push({ kind: 'flag', value: flag[0] });
    else if (variable) tokens.push({ kind: 'variable', value: variable[0] });
    else if (prompt) tokens.push({ kind: 'prompt', value: prompt[0] });
    else if (number) tokens.push({ kind: 'number', value: number[0] });
    else if (ident) tokens.push({ kind: atLineStart ? 'function' : 'ident', value: ident[0] });
    else if (punct) tokens.push({ kind: 'punct', value: punct[0] });
    else if (ws) tokens.push({ kind: 'ws', value: ws[0] });
    else tokens.push({ kind: 'text', value: remaining[0] });

    atLineStart = /\n$/.test(tokens[tokens.length - 1].value);
    remaining = remaining.slice(tokens[tokens.length - 1].value.length);
  }

  return tokens;
}

function tokenizeXml(src: string): Token[] {
  const tokens: Token[] = [];
  let remaining = src;

  while (remaining.length > 0) {
    const comment = remaining.match(/^<!--[\s\S]*?-->/);
    const tag = remaining.match(/^<\/?[A-Za-z_:][A-Za-z0-9:._-]*/);
    const attr = remaining.match(/^[A-Za-z_:][A-Za-z0-9:._-]*(?==)/);
    const str = remaining.match(/^(?:"[^"]*"|'[^']*')/);
    const close = remaining.match(/^\/?>/);
    const punct = remaining.match(/^=/);
    const ws = remaining.match(/^\s+/);

    if (comment) tokens.push({ kind: 'comment', value: comment[0] });
    else if (tag) tokens.push({ kind: 'tag', value: tag[0] });
    else if (attr) tokens.push({ kind: 'attribute', value: attr[0] });
    else if (str) tokens.push({ kind: 'string', value: str[0] });
    else if (close) tokens.push({ kind: 'tag', value: close[0] });
    else if (punct) tokens.push({ kind: 'punct', value: punct[0] });
    else if (ws) tokens.push({ kind: 'ws', value: ws[0] });
    else tokens.push({ kind: 'text', value: remaining[0] });

    remaining = remaining.slice(tokens[tokens.length - 1].value.length);
  }

  return tokens;
}

function highlightTokens(code: string, lang: string): Token[] {
  const normalized = lang.toLowerCase();

  if (normalized === 'csharp' || normalized === 'cs' || normalized === 'c#') {
    return tokenizeCSharp(code);
  }

  if (normalized === 'bash' || normalized === 'sh' || normalized === 'shell' || normalized === 'powershell' || normalized === 'ps1' || normalized === 'pwsh') {
    return tokenizeBash(code);
  }

  if (normalized === 'xml' || normalized === 'html') {
    return tokenizeXml(code);
  }

  return [{ kind: 'text', value: code }];
}

export function highlightCode(code: string, lang: string): React.ReactNode {
  return highlightTokens(code, lang).map((token, index) => {
    const className = TOKEN_CLASSES[token.kind];

    if (!className) {
      return <React.Fragment key={index}>{token.value}</React.Fragment>;
    }

    return (
      <span key={index} className={className}>
        {token.value}
      </span>
    );
  });
}
