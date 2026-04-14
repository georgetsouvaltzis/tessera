import styles from "@/app/page.module.css";

export function TerminalPreview() {
    return (
        <div className={styles.terminalFrame}>
            <div className={styles.terminalChrome}>
                <span />
                <span />
                <span />
                <em className={styles.terminalTitle}>~/starter-path</em>
            </div>
            <div className={styles.terminalBody}>
                <div className={styles.terminalSnippet}>
                    <div><span>$</span> dotnet run --project examples/HelloWorld</div>
                    <div><span>✓</span> first screen online</div>
                    <div><span>$</span> dotnet run --project examples/WorkspaceApp</div>
                </div>
                <div className={styles.terminalStatus}>
                    <strong>Tessera starter path</strong>
                    <span>Ready in 3 examples</span>
                </div>
            </div>
        </div>
    );
}
