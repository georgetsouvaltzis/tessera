import Link from "next/link";
import { docsHref } from "@/lib/site";
import styles from "@/app/page.module.css";

export function SiteFooter() {
    return (
        <footer className={styles.footer}>
            <div className={styles.footerShell}>
                <div>
                    <strong>Tessera</strong>
                    <span>Start with the three public examples. Scale later.</span>
                </div>
                <nav className={styles.footerLinks} aria-label="Footer">
                    <Link href={docsHref("/getting-started/")}>Getting started</Link>
                    <Link href={docsHref("/examples/")}>Example guide</Link>
                    <a href="https://github.com/georgetsouvaltzis/teasharp">GitHub</a>
                </nav>
            </div>
        </footer>
    );
}
