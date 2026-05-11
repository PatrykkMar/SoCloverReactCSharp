import { BoardContext } from "../../context/BoardContext";
import BoardSlot from "./BoardSlot";
import styles from "./Board.module.css";
import { useContext } from "react";

export default function Board() {
    const boardContext = useContext(BoardContext);

    if (!boardContext)
        return <div>Board loading...</div>;

    const { board } = boardContext;

    return (
        <div className={styles.boardWrapper}>
            <div className={styles.middleRow}>
                <div className={styles.grid}>
                    {board?.boardSlots.sort((a, b) => a.positionIndex - b.positionIndex).map((slot) => (
                        <BoardSlot key={slot.positionIndex} slot={slot} />
                    ))}
                </div>
            </div>
        </div>
    );
}
