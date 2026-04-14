import Link from "next/link";
import styles from "@/app/page.module.css";

type ExampleCardProps = {
    eyebrow: string;
    title: string;
    body: string;
    command: string;
    href: string;
    tags?: string[];
};

export function ExampleCard({
    eyebrow,
    title,
    body,
    command,
    href,
    tags = [],
}: ExampleCardProps) {
    return (
        <article className={styles.exampleCard}>
            <span className={styles.exampleEyebrow}>{eyebrow}</span>
            <h3>{title}</h3>
            <p>{body}</p>
            {tags.length > 0 ? (
                <div className={styles.cardTags}>
                    {tags.map((tag) => (
                        <span key={tag} className={styles.cardTag}>
                            {tag}
                        </span>
                    ))}
                </div>
            ) : null}
            <code className={styles.commandLine}>{command}</code>
            <Link className={styles.cardLink} href={href}>
                Learn more
            </Link>
        </article>
    );
}
