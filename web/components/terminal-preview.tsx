import styles from "@/app/page.module.css";

export function TerminalPreview() {
    return (
        <div className={styles.terminalFrame}>
            <div className={styles.terminalChrome}>
                <span />
                <span />
                <span />
            </div>
            <div className={styles.terminalBody}>
                <span className={styles.terminalLabel}>WorkspaceApp</span>
                <h2>Focused editor. Preview rail. Action dock.</h2>
                <p>
                    A starter shell that already feels like a serious workflow surface,
                    not a tiny sample app.
                </p>
                <code className={styles.commandLine}>
                    dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj
                </code>
                <div className={styles.terminalChips}>
                    <span>semantic theming</span>
                    <span>typed controls</span>
                    <span>message-driven updates</span>
                </div>
                <ul className={styles.terminalList}>
                    <li>explicit layouts, controls, and messages</li>
                    <li>public path first, deeper seams later</li>
                    <li>examples that read like products</li>
                </ul>
            </div>
        </div>
    );
}
