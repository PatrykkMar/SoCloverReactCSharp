import type { CardDTO } from "../../models/dtos";
import type { RotateCardRequest } from "../../models/requests";
import { BoardContext } from "../../context/BoardContext";
import styles from "./card.module.css";
import { useContext } from "react";
interface Props {
    slot: CardDTO;
}

export default function Card({ slot }: Props) {

    const boardContext = useContext(BoardContext);
    const rotationDegrees = slot.currentRotation * 90;

    const handleRotateClick = () => {

        if (boardContext?.rotateCard && slot.gameRoomCardId) {
            boardContext.rotateCard({
                gameRoomCardId: slot.gameRoomCardId
            } as RotateCardRequest);
        }
    };

    const handleDragStart = (e: React.DragEvent) => {
        e.dataTransfer.setData("text/plain", slot.gameRoomCardId.toString());
        e.dataTransfer.effectAllowed = "move";
    };



    return (
        <div
            className={styles.slot}
        >
            {slot ? (
                <div className={styles.card} style={{ transform: `rotate(${rotationDegrees}deg)` }} draggable={slot.gameRoomCardId !== 0} onDragStart={handleDragStart}>
                    <div className={styles.wordTop}>{slot.wordTop}</div>
                    <div className={styles.wordRight}>{slot.wordRight}</div>
                    <div className={styles.wordBottom}>{slot.wordBottom}</div>
                    <div className={styles.wordLeft}>{slot.wordLeft}</div>

                    {boardContext?.board?.cardsActive && (
                        <button
                            className={styles.rotateBtn}
                            onClick={handleRotateClick}
                            style={{ transform: `translate(-50%, -50%)` }}
                            title="Rotate card"
                        >
                            ↻
                        </button>
                    )}
                </div>
            ) : (
                <div className={styles.emptySlot}>Empty</div>
            )}
        </div>
    );
}