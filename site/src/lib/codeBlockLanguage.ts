type CodeBlockLanguageMeta = {
  extension: string;
  label: string;
};

const languageMeta = new Map<string, CodeBlockLanguageMeta>([
  ['bash', { label: 'bash', extension: '.sh' }],
  ['c#', { label: 'csharp', extension: '.cs' }],
  ['cs', { label: 'csharp', extension: '.cs' }],
  ['csharp', { label: 'csharp', extension: '.cs' }],
  ['html', { label: 'html', extension: '.html' }],
  ['json', { label: 'json', extension: '.json' }],
  ['md', { label: 'markdown', extension: '.md' }],
  ['markdown', { label: 'markdown', extension: '.md' }],
  ['ps1', { label: 'powershell', extension: '.ps1' }],
  ['pwsh', { label: 'powershell', extension: '.ps1' }],
  ['shell', { label: 'bash', extension: '.sh' }],
  ['sh', { label: 'bash', extension: '.sh' }],
  ['tsx', { label: 'tsx', extension: '.tsx' }],
  ['ts', { label: 'ts', extension: '.ts' }],
  ['xml', { label: 'xml', extension: '.xml' }],
  ['yaml', { label: 'yaml', extension: '.yaml' }],
  ['yml', { label: 'yaml', extension: '.yml' }],
]);

export function getCodeBlockLanguageMeta(language?: string): CodeBlockLanguageMeta {
  const normalized = language?.trim().toLowerCase();

  if (!normalized) {
    return { label: 'code', extension: '.txt' };
  }

  return languageMeta.get(normalized) ?? { label: normalized, extension: `.${normalized}` };
}
