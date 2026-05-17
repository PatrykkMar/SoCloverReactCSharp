import type { CardDTO } from "../../models/dtos";
import styles from "./card.module.css";
interface Props {
    slot: CardDTO;
}

export default function Card({ slot }: Props) {


    const rotationDegrees = slot.currentRotation * 90;

    return (
        <div
            className={styles.slot}
            style={{ transform: `rotate(${rotationDegrees}deg)` }}
        >
            {slot ? (
                <div className={styles.card}>
                    <div className={styles.wordTop}>{slot.wordTop}</div>
                    <div className={styles.wordRight}>{slot.wordRight}</div>
                    <div className={styles.wordBottom}>{slot.wordBottom}</div>
                    <div className={styles.wordLeft}>{slot.wordLeft}</div>
                </div>
            ) : (
                <div className={styles.emptySlot}>Empty</div>
            )}
        </div>
    );
}