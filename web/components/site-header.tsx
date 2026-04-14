import Link from "next/link";
import { appHref, docsHref } from "@/lib/site";
import styles from "@/app/page.module.css";

const primaryLinks = [
    { label: "Getting started", href: docsHref("/getting-started/") },
    { label: "Showcase", href: docsHref("/showcase/") },
    { label: "Theming", href: docsHref("/theme-system/") },
    { label: "Docs", href: docsHref("/") },
];

export function SiteHeader() {
    return (
        <header className={styles.header}>
            <div className={styles.headerShell}>
                <Link className={styles.brand} href={appHref("/")}>
                    <span className={styles.brandMark} aria-hidden="true">
                        ✦
                    </span>
                    <span className={styles.brandWordmark}>Tessera</span>
                </Link>
                <nav aria-label="Primary" className={styles.primaryNav}>
                    {primaryLinks.map((link) => (
                        <Link key={link.label} href={link.href}>
                            {link.label}
                        </Link>
                    ))}
                </nav>
                <div className={styles.headerActions}>
                    <Link className={styles.headerGhost} href={docsHref("/overview/")}>
                        Overview
                    </Link>
                    <Link className={styles.headerCta} href={docsHref("/getting-started/")}>
                        Get started
                    </Link>
                </div>
            </div>
        </header>
    );
}
