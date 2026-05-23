import { useContext, useState } from "react";
import { BoardContext } from "../../context/BoardContext";
import type { CardDTO } from "../../models/dtos";
import Card from "./Card";
import styles from "./boardslot.module.css";

interface BoardSlotProps {
    index: number;
    card: CardDTO | undefined;
}

export default function BoardSlot({ index, card }: BoardSlotProps) {
    const boardContext = useContext(BoardContext);
    const [isDragOver, setIsDragOver] = useState(false);

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
        if (!card) {
            setIsDragOver(true);
            e.dataTransfer.dropEffect = "move";
        }
    };

    const handleDragLeave = () => {
        setIsDragOver(false);
    };

    const handleDrop = async (e: React.DragEvent) => {
        e.preventDefault();
        setIsDragOver(false);

        if (card) return;

        const cardIdStr = e.dataTransfer.getData("text/plain");
        const cardId = parseInt(cardIdStr, 10);

        if (!isNaN(cardId) && boardContext?.moveCardToSlot) {
            await boardContext.moveCardToSlot({
                gameRoomCardId: cardId,
                positionIndex: index
            });
        }
    };

    return (
        <div 
            className={`${styles.slotContainer} ${!card ? styles.emptySlot : ""} ${isDragOver ? styles.dragOverActive : ""}`}
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            onDrop={handleDrop}
        >
            {card ? (
                <Card slot={card} />
            ) : (
                <div className={styles.slotPlaceholder}>
                    <span>+</span>
                </div>
            )}
        </div>
    );
}