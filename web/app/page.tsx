import Link from "next/link";
import { ExampleCard } from "@/components/example-card";
import { SectionHeading } from "@/components/section-heading";
import { SiteFooter } from "@/components/site-footer";
import { SiteHeader } from "@/components/site-header";
import { TerminalPreview } from "@/components/terminal-preview";
import { docsHref } from "@/lib/site";
import styles from "./page.module.css";

const starterExamples = [
    {
        eyebrow: "Starter",
        title: "HelloWorld",
        body: "Centered first screen, first action button, first polished terminal surface.",
        command: "dotnet run --project examples/HelloWorld/HelloWorld.csproj",
        href: docsHref("/getting-started/"),
        tags: ["layout", "button flow"],
    },
    {
        eyebrow: "Interactive",
        title: "CounterForm",
        body: "Inputs, selection, progress, and updates without leaving the small public API.",
        command: "dotnet run --project examples/CounterForm/CounterForm.csproj",
        href: docsHref("/examples/"),
        tags: ["state", "controls"],
    },
    {
        eyebrow: "Multi-pane",
        title: "WorkspaceApp",
        body: "Navigation, editing, preview, and actions in one cohesive workflow shell.",
        command: "dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj",
        href: docsHref("/showcase/"),
        tags: ["composition", "workflow"],
    },
];

const flagshipExamples = [
    {
        eyebrow: "Workflow",
        title: "GitConsole",
        body: "Command-driven workflow surface with editing, diff review, and action history.",
        command: "dotnet run --project examples/GitConsole/GitConsole.csproj",
        href: docsHref("/showcase/"),
    },
    {
        eyebrow: "Dashboard",
        title: "OpsWatch",
        body: "Telemetry rails, alerts, chips, and operator actions in a dense operations shell.",
        command: "dotnet run --project examples/OpsWatch/OpsWatch.csproj",
        href: docsHref("/showcase/"),
    },
    {
        eyebrow: "Workbench",
        title: "DataWorkbench",
        body: "Multi-pane composition, richer state orchestration, and pointer-ready configuration.",
        command: "dotnet run --project examples/DataWorkbench/DataWorkbench.csproj",
        href: docsHref("/showcase/"),
    },
];

export default function HomePage() {
    return (
        <div className={styles.page}>
            <SiteHeader />
            <main className={styles.main}>
                <section className={styles.hero}>
                    <div className={styles.heroCopy}>
                        <span className={styles.heroBadge}>Public alpha · .NET 10 · C# first</span>
                        <h1>
                            Build terminal software with
                            <span> real product taste.</span>
                        </h1>
                        <p>
                            Ship operator dashboards, workflow shells, and multi-pane tools
                            with a small public app model, semantic theming, and examples
                            that already look serious.
                        </p>
                        <div className={styles.heroActions}>
                            <Link className={styles.primaryButton} href={docsHref("/getting-started/")}>
                                Get started
                            </Link>
                            <Link className={styles.secondaryButton} href={docsHref("/showcase/")}>
                                See the showcase
                            </Link>
                        </div>
                        <ul className={styles.heroProof}>
                            <li>library-first startup path</li>
                            <li>semantic theming built in</li>
                            <li>real examples, not widget dumps</li>
                        </ul>
                    </div>
                    <TerminalPreview />
                </section>

                <section className={styles.valueBand}>
                    <article>
                        <strong>Small public model</strong>
                        <p>Explicit screens, layouts, controls, and messages. No nested DSL maze.</p>
                    </article>
                    <article>
                        <strong>Theme it intentionally</strong>
                        <p>Tokens and overrides are first-class API, not last-mile hacks.</p>
                    </article>
                    <article>
                        <strong>Scale into richer shells</strong>
                        <p>Move from starter examples into dashboards and workbenches without changing direction.</p>
                    </article>
                </section>

                <section className={styles.section}>
                    <SectionHeading
                        kicker="Starter ladder"
                        title="Three examples. One deliberate learning curve."
                        body="Learn the public path in sequence, then open the denser showcases when you want the fuller picture."
                    />
                    <div className={styles.cardGrid}>
                        {starterExamples.map((example) => (
                            <ExampleCard key={example.title} {...example} />
                        ))}
                    </div>
                </section>

                <section className={styles.section}>
                    <SectionHeading
                        kicker="Flagship examples"
                        title="See how the same model scales up."
                        body="Use the flagships to evaluate denser dashboards, command-heavy flows, and workbench-style shells."
                    />
                    <div className={styles.cardGrid}>
                        {flagshipExamples.map((example) => (
                            <ExampleCard key={example.title} {...example} />
                        ))}
                    </div>
                </section>

                <section className={styles.finalCta}>
                    <SectionHeading
                        kicker="Public path"
                        title="Start with the docs. Then run something real."
                        body="The best way to evaluate Tessera is to follow the starter ladder, then judge the flagship shells for yourself."
                    />
                    <div className={styles.heroActions}>
                        <Link className={styles.primaryButton} href={docsHref("/getting-started/")}>
                            Read the docs
                        </Link>
                        <Link className={styles.secondaryButton} href={docsHref("/overview/")}>
                            See the overview
                        </Link>
                    </div>
                </section>
            </main>
            <SiteFooter />
        </div>
    );
}
