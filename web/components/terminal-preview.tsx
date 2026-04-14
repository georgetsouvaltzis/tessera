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
                <span className={styles.terminalLabel}>Starter ladder</span>
                <div className={styles.terminalSnippet}>
                    <div><span>$</span> dotnet run --project examples/HelloWorld</div>
                    <div><span>$</span> dotnet run --project examples/CounterForm</div>
                    <div><span>$</span> dotnet run --project examples/WorkspaceApp</div>
                </div>
                <p>Run them in order. Stop after WorkspaceApp if the public path already clicks.</p>
            </div>
        </div>
    );
}
