import React, { type ReactNode } from 'react';
import clsx from 'clsx';
import { useCodeBlockContext } from '@docusaurus/theme-common/internal';
import Container from '@theme/CodeBlock/Container';
import type { Props } from '@theme/CodeBlock/Layout';
import { getCodeBlockLanguageMeta } from '../../../lib/codeBlockLanguage';
import { highlightCode } from '../../../lib/luminaHighlight';

export default function CodeBlockLayout({ className }: Props): ReactNode {
  const { metadata, wordWrap } = useCodeBlockContext();
  const language = getCodeBlockLanguageMeta(metadata.language);

  return (
    <Container as="div" className={clsx(className, metadata.className, 'lumina-code-block')}>
      <div className="lumina-code-block__header">
        <span className="lumina-code-block__header-label">
          {metadata.title ?? language.label}
        </span>
        <span className="lumina-code-block__header-ext">{language.extension}</span>
      </div>
      <div className="lumina-code-block__body">
        <pre
          ref={wordWrap.codeBlockRef}
          className="lumina-code-block__pre"
          tabIndex={0}>
          <code className="lumina-code-block__code">
            {highlightCode(metadata.code, language.label)}
          </code>
        </pre>
      </div>
    </Container>
  );
}
