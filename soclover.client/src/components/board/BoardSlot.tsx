import type { CardDTO } from "../../models/dtos";
import Card from "./Card";
import styles from "./boardslot.module.css";
import { useContext } from "react";
import { BoardContext } from "../../context/BoardContext";
interface BoardSlotProps {
    index: number;
    card: CardDTO | undefined;
}

export default function BoardSlot({ index, card }: BoardSlotProps) {

    const boardContext = useContext(BoardContext);

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
    };

    const handleDrop = (e: React.DragEvent) => {
        e.preventDefault();
        const cardIdStr = e.dataTransfer.getData("text/plain");
        if (!cardIdStr) return;

        const cardId = parseInt(cardIdStr, 10);

        console.log(`Id: ${cardId} na dropped on slot: ${index}`);
        boardContext?.moveCardToSlot({ gameRoomCardId: cardId, positionIndex: index });
    };

    return (
        <div
            className={`${styles.slotContainer} ${!card ? styles.emptySlot : ""}`}
            onDragOver={handleDragOver}
            onDrop={handleDrop}
        >
            {card && card.gameRoomCardId !== 0 ? (
                <Card card={card} />
            ) : (
                <div className={styles.slotPlaceholder}>
                    <span>+</span>
                </div>
            )}
        </div>
    );
}