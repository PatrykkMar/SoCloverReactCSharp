import type { BoardSlotDTO } from "../../models/dtos";
import styles from "./Board.module.css";

interface Props {
    slot: BoardSlotDTO;
}

export default function BoardSlot({ slot }: Props) {

    const rotationDegrees = slot.currentRotation * 90;

    return (
        <div
            className={styles.slot}
            style={{ transform: `rotate(${rotationDegrees}deg)` }}
        >
            {slot.card ? (
                <div className={styles.card}>
                    <div className={styles.wordTop}>{slot.card.wordTop}</div>
                    <div className={styles.wordRight}>{slot.card.wordRight}</div>
                    <div className={styles.wordBottom}>{slot.card.wordBottom}</div>
                    <div className={styles.wordLeft}>{slot.card.wordLeft}</div>
                </div>
            ) : (
                <div className={styles.emptySlot}>Empty</div>
            )}
        </div>
    );
}