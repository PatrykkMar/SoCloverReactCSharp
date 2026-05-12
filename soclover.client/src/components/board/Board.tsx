import { BoardContext } from "../../context/BoardContext";
import type { SubmitCluesRequest } from "../../models/requests";
import BoardSlot from "./BoardSlot";
import styles from "./Board.module.css";
import { useContext, useRef } from "react";

export default function Board() {
    const boardContext = useContext(BoardContext);

    const topRef = useRef<HTMLInputElement>(null);
    const rightRef = useRef<HTMLInputElement>(null);
    const bottomRef = useRef<HTMLInputElement>(null);
    const leftRef = useRef<HTMLInputElement>(null);


    if (!boardContext || !boardContext.board)
        return <div>Board loading...</div>;


    const { board, submitClues } = boardContext;

    const handleReadyClick = () => {
        const request: SubmitCluesRequest = {words: [
            topRef.current?.value || "",
            rightRef.current?.value || "",
            bottomRef.current?.value || "",
            leftRef.current?.value || ""]
        };

        if (request.words.some(x => x === "")) {
            alert("There are empty inputs");
            return;
        }

        submitClues(request);
    };

    return (
        <div className={styles.boardContainer}>
            <div className={styles.inputRow}>
                <input
                    ref={topRef}
                    className={`${styles.clueInput} ${styles.top}`}
                />
            </div>

            <div className={styles.middleRow}>
                <input ref={leftRef}
                    className={`${styles.clueInput} ${styles.left}`}
                />

                <div className={styles.grid}>
                    {board.boardSlots
                        .sort((a, b) => a.positionIndex - b.positionIndex)
                        .map((slot) => (
                            <BoardSlot key={slot.positionIndex} slot={slot} />
                        ))}
                </div>

                <input ref={rightRef}
                    className={`${styles.clueInput} ${styles.right}`}
                />
            </div>

            <div className={styles.inputRow}>
                <input ref={bottomRef}
                    className={`${styles.clueInput} ${styles.bottom}`}
                />
            </div>

            <button className="btn btn-primary mt-3" onClick={handleReadyClick}>
                Ready
            </button>
        </div>
    );
}
