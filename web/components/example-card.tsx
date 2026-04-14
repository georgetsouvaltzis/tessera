import Link from "next/link";
import styles from "@/app/page.module.css";

type ExampleCardProps = {
    eyebrow: string;
    title: string;
    body: string;
    href: string;
};

export function ExampleCard({
    eyebrow,
    title,
    body,
    href,
}: ExampleCardProps) {
    return (
        <article className={styles.exampleCard}>
            <span className={styles.exampleEyebrow}>{eyebrow}</span>
            <h3>{title}</h3>
            <p>{body}</p>
            <Link className={styles.cardLink} href={href}>
                Open path
            </Link>
        </article>
    );
}
