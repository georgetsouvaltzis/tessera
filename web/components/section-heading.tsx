import styles from "@/app/page.module.css";

type SectionHeadingProps = {
    kicker: string;
    title: string;
    body: string;
};

export function SectionHeading({ kicker, title, body }: SectionHeadingProps) {
    return (
        <div className={styles.sectionHeading}>
            <span className={styles.sectionKicker}>{kicker}</span>
            <h2>{title}</h2>
            <p>{body}</p>
        </div>
    );
}
