import { BoardContext } from "../../context/BoardContext";
import type { SubmitCluesRequest } from "../../models/requests";
import Card from "./Card";
import styles from "./Board.module.css";
import { useContext, useState, useEffect } from "react";
import { RoomContext } from "../../context/RoomContext";
import { GameStatus } from "../../models/dtos";

export default function Board() {
    const boardContext = useContext(BoardContext);
    const roomContext = useContext(RoomContext);

    const [clues, setClues] = useState({
        top: "",
        right: "",
        bottom: "",
        left: ""
    });


    useEffect(() => {
        if (boardContext?.board) {
            const setCluesAsync = async () => {
                setClues({
                    top: boardContext?.board?.topClue || "",
                    right: boardContext?.board?.rightClue || "",
                    bottom: boardContext?.board?.bottomClue || "",
                    left: boardContext?.board?.leftClue || ""
                })
            };
            setCluesAsync();
        }
    }, [boardContext?.board]);


    if (!boardContext || !boardContext.board)
        return <div>Board loading...</div>;

    if (!roomContext)
        return <div>Room loading...</div>;


    const { board, submitClues } = boardContext;




    const handleInputChange = (direction: keyof typeof clues, value: string) => {
        if (!board.isActive) return;
        setClues(prev => ({ ...prev, [direction]: value }));
    };

    const handleSubmitClues = () => {
        const words = [clues.top, clues.right, clues.bottom, clues.left];

        if (words.some(x => x.trim() === "")) {
            alert("There are empty inputs");
            return;
        }

        const request: SubmitCluesRequest = { words };
        submitClues(request);
    };

    return (
        <div className={styles.boardContainer}>
            <div className={styles.inputRow}>
                <input
                    value={clues.top}
                    onChange={(e) => handleInputChange("top", e.target.value)}
                    className={`${styles.clueInput} ${styles.top}`}
                    readOnly={!board.isActive}
                    placeholder="Top clue..."
                />
            </div>

            <div className={styles.middleRow}>
                <input
                    value={clues.left}
                    onChange={(e) => handleInputChange("left", e.target.value)}
                    className={`${styles.clueInput} ${styles.left}`}
                    readOnly={!board.isActive}
                    placeholder="Left..."
                />

                <div className={styles.grid}>
                    {board.cards
                        .sort((a, b) => (a.positionIndex ?? 0) - (b.positionIndex ?? 0))
                        .map((slot) => (
                            <Card key={slot.positionIndex} slot={slot} />
                        ))}
                </div>

                <input
                    value={clues.right}
                    onChange={(e) => handleInputChange("right", e.target.value)}
                    className={`${styles.clueInput} ${styles.right}`}
                    readOnly={!board.isActive}
                    placeholder="Right..."
                />
            </div>

            <div className={styles.inputRow}>
                <input
                    value={clues.bottom}
                    onChange={(e) => handleInputChange("bottom", e.target.value)}
                    className={`${styles.clueInput} ${styles.bottom}`}
                    readOnly={!board.isActive}
                    placeholder="Bottom clue..."
                />
            </div>

            {roomContext?.status === GameStatus.Writing && (
                <button className="btn btn-primary mt-3" onClick={handleSubmitClues}>
                    Submit clues
                </button>
            )}
        </div>
    );
}
