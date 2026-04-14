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
                        <span className={styles.heroBadge}>Public alpha · .NET 10 · C# first</span>
                        <h1>
                            <span className={styles.heroLeadLine}>C# Terminal UI.</span>
                            One clean way in.
                            <span> Start with three examples.</span>
                        </h1>
                        <p>
                            Tessera is a C#-first terminal UI framework with a deliberate public
                            path. Open HelloWorld, then CounterForm, then WorkspaceApp. Everything
                            else should come after that.
                        </p>
                        <div className={styles.heroActions}>
                            <Link className={styles.primaryButton} href={docsHref("/getting-started/")}>
                                Open the starter path
                            </Link>
                            <Link className={styles.secondaryButton} href={docsHref("/")}>
                                Read the docs
                            </Link>
                        </div>
                        <ul className={styles.heroProof}>
                            <li>HelloWorld → CounterForm → WorkspaceApp</li>
                            <li>library-first startup path</li>
                            <li>examples that already look serious</li>
                        </ul>
                    </div>
                    <TerminalPreview />
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
                    <div className={styles.quickLinks}>
                        <Link href={docsHref("/getting-started/")}>Getting started</Link>
                        <Link href={docsHref("/examples/")}>Example guide</Link>
                        <Link href={docsHref("/overview/")}>Overview</Link>
                    </div>
                </section>
            </main>
            <SiteFooter />
        </div>
    );
}
