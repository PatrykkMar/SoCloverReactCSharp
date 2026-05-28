import type { CardDTO } from "../../models/dtos";
import Card from "./Card";
import styles from "./Hand.module.css";

interface HandProps {
    cards: CardDTO[];
}

export default function Hand({ cards }: HandProps) {
    return (
        <div className={styles.handContainer}>
            <h3 className={styles.handTitle}>Your cards</h3>
            <div className={styles.cardsBelt}>
                {cards.map((card, index) => (
                    <div key={index} className={styles.handCardWrapper}>
                        <Card card={card} />
                    </div>
                ))}
                {cards.length === 0 && (
                    <div className={styles.emptyHand}>No cards in hand</div>
                )}
            </div>
        </div>
    );
}