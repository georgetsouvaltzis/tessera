import Link from "next/link";
import { docsHref } from "@/lib/site";
import styles from "@/app/page.module.css";

export function SiteFooter() {
    return (
        <footer className={styles.footer}>
            <div className={styles.footerShell}>
                <div>
                    <strong>Tessera</strong>
                    <span>C#-first terminal UI for real software surfaces.</span>
                </div>
                <nav className={styles.footerLinks} aria-label="Footer">
                    <Link href={docsHref("/getting-started/")}>Getting started</Link>
                    <Link href={docsHref("/showcase/")}>Showcase</Link>
                    <Link href={docsHref("/public-api-guidelines/")}>Public API</Link>
                </nav>
            </div>
        </footer>
    );
}
