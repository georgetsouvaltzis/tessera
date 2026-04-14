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
        eyebrow: "01 · Starter",
        title: "HelloWorld",
        body: "First polished screen. First action button. First feel for the public API.",
        href: docsHref("/getting-started/"),
    },
    {
        eyebrow: "02 · Interactive",
        title: "CounterForm",
        body: "Inputs, progress, selection, and updates without leaving the small public path.",
        href: docsHref("/examples/"),
    },
    {
        eyebrow: "03 · Multi-pane",
        title: "WorkspaceApp",
        body: "Navigation, preview, and actions inside the first shell that already feels like a real product.",
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
                        <span className={styles.heroBadge}>
                            <span className={styles.heroBadgeDot} aria-hidden="true" />
                            v1.0.0-alpha.1 — Public Preview
                        </span>
                        <h1>
                            <span className={styles.heroLeadLine}>C# Terminal UI.</span>
                            Small public path.
                            <span> Serious product feel.</span>
                        </h1>
                        <p>
                            Start with HelloWorld, then CounterForm, then WorkspaceApp. That is
                            the intended public path into Tessera before you open any denser
                            dashboards or workbenches.
                        </p>
                        <div className={styles.heroActions}>
                            <Link className={styles.primaryButton} href={docsHref("/getting-started/")}>
                                Start here
                            </Link>
                            <a className={styles.secondaryButton} href="https://github.com/georgetsouvaltzis/teasharp">
                                GitHub
                            </a>
                        </div>
                        <div className={styles.heroNote}>
                            <span>Public path</span>
                            <p>HelloWorld → CounterForm → WorkspaceApp</p>
                        </div>
                        <div className={styles.quickLinks}>
                            <Link href={docsHref("/getting-started/")}>Getting started</Link>
                            <Link href={docsHref("/examples/")}>Example guide</Link>
                            <Link href={docsHref("/overview/")}>Overview</Link>
                            <Link href={docsHref("/showcase/")}>Showcase</Link>
                        </div>
                    </div>
                    <div className={styles.heroPanel}>
                        <TerminalPreview />
                        <div className={styles.heroPanelNote}>
                            <strong>Start with the starter ladder.</strong>
                            <p>Open the showcase only after the first three examples feel obvious.</p>
                        </div>
                    </div>
                </section>

                <section className={styles.section}>
                    <SectionHeading
                        kicker="Start here"
                        title="The starter ladder."
                        body="These are the only three examples a new user should open first."
                    />
                    <div className={styles.cardGrid}>
                        {starterExamples.map((example) => (
                            <ExampleCard key={example.title} {...example} />
                        ))}
                    </div>
                </section>
            </main>
            <SiteFooter />
        </div>
    );
}
