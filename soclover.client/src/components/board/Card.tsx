import { useContext } from "react";
import type { CardDTO } from "../../models/dtos";
import { BoardContext } from "../../context/BoardContext";
import styles from "./card.module.css";
import { useTranslation } from "react-i18next";

interface CardProps {
    card: CardDTO;
}

export default function Card({ card }: CardProps) {
    const { t } = useTranslation();
    const boardContext = useContext(BoardContext);
    const rotationDegrees = (card.currentRotation ?? 0) * 90;
    const counterRotationDegrees = -rotationDegrees;

    const handleDragStart = (e: React.DragEvent) => {
        e.dataTransfer.setData("text/plain", card.gameRoomCardId.toString());
        e.dataTransfer.effectAllowed = "move";
    };

    const handleRotateClick = (e: React.MouseEvent) => {
        console.log("Rotation");
        e.stopPropagation();
        if (boardContext?.rotateCard && card.gameRoomCardId) {
            boardContext.rotateCard({ gameRoomCardId: card.gameRoomCardId });
        }
    };

    return (
        <div
            className={styles.slot}
            draggable={boardContext?.board?.cardsActive && !card.isCorrect}
            onDragStart={handleDragStart}
        >
            {card ? (
                <div
                    className={`${styles.card} ${card.isCorrect ? styles.correctCard : ""}`}
                    style={{ transform: `rotate(${rotationDegrees}deg)` }}
                >
                    <div className={styles.wordTop}>{card.wordTop}</div>
                    <div className={styles.wordRight}>{card.wordRight}</div>
                    <div className={styles.wordBottom}>{card.wordBottom}</div>
                    <div className={styles.wordLeft}>{card.wordLeft}</div>

                    {card.isCorrect ? (
                        <div>
                            {/*TODO: Some icon*/}
                        </div>
                    ) : (
                        boardContext?.board?.cardsActive && (
                            <button
                                className={styles.rotateBtn}
                                onClick={handleRotateClick}
                                style={{ transform: `translate(-50%, -50%) rotate(${counterRotationDegrees}deg)` }}
                                title={t("card.rotateTooltip")}
                            >
                                ↻
                            </button>
                        )
                    )}
                </div>
            ) : (
                <div className={styles.emptySlot}>{t("card.emptySlot")}</div>
            )}
        </div>
    );
}