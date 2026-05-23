import type { CardDTO } from "../../models/dtos";
import Card from "./Card";
import styles from "./boardslot.module.css";

interface BoardSlotProps {
    index: number;
    card: CardDTO | undefined;
}

export default function BoardSlot({ index, card }: BoardSlotProps) {
    const hasCard = card && card.gameRoomCardId !== 0;

    return (
        <div
            data-position-index={index}
        >
            {hasCard ? (
                <Card slot={card} />
            ) : (
                <div className={styles.slotPlaceholder}>
                    <span>+</span>
                </div>
            )}
        </div>
    );
}