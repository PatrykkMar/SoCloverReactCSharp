import { BoardContext } from "../../context/BoardContext";
import BoardSlot from "./BoardSlot";
import styles from "./Board.module.css";
import { useContext } from "react";

export default function Board() {
    const boardContext = useContext(BoardContext);

    if (!boardContext || !boardContext.board)
        return <div>Board loading...</div>;

    const { board } = boardContext;

    return (
        <div className={styles.boardContainer}>
            <div className={styles.inputRow}>
                <input
                    className={`${styles.clueInput} ${styles.top}`}
                />
            </div>

            <div className={styles.middleRow}>
                <input
                    className={`${styles.clueInput} ${styles.left}`}
                />

                <div className={styles.grid}>
                    {board.boardSlots
                        .sort((a, b) => a.positionIndex - b.positionIndex)
                        .map((slot) => (
                            <BoardSlot key={slot.positionIndex} slot={slot} />
                        ))}
                </div>

                <input
                    className={`${styles.clueInput} ${styles.right}`}
                />
            </div>

            <div className={styles.inputRow}>
                <input
                    className={`${styles.clueInput} ${styles.bottom}`}
                />
            </div>
        </div>
    );
}