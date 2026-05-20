import type { CardDTO } from "../../models/dtos";
import Card from "./Card";
import styles from "./Board.module.css";

interface BoardSlotProps {
    index: number;
    card: CardDTO | undefined;
}

export default function BoardSlot({ index, card }: BoardSlotProps) {
    return (
        <div
            className={`${styles.slotContainer} ${!card ? styles.emptySlot : ""}`}
            data-position-index={index}
        >
            {card && card.gameRoomCardId !== 0 ? (
                <Card slot={card} />
            ) : (
                <div className={styles.slotPlaceholder}>
                    <span>+</span>
                </div>
            )}
        </div>
    );
}
