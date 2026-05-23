import { BoardContext } from "../../context/BoardContext";
import type { SubmitCluesRequest } from "../../models/requests";
import BoardSlot from "./BoardSlot";
import styles from "./Board.module.css";
import { useContext, useState, useEffect } from "react";
import { RoomContext } from "../../context/RoomContext";
import { GameStatus } from "../../models/dtos";
import Hand from "./Hand";

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


    const { board, submitClues, check } = boardContext;

    const slotIndexes = [0, 1, 2, 3];
    const handCards = board.cards.filter(card => card.location === "InHand");

    const handleInputChange = (direction: keyof typeof clues, value: string) => {
        if (!board.inputsActive) return;
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

    const handleCheck = () => {
        const cardsOnBoard = board.cards.filter(card => card.location === "OnBoard");

        if (cardsOnBoard.length < 4) {
            alert("Place all cards on the board before checking!");
            return;
        }
        check();
    };

    return (
        <div className={styles.boardContainer}>
            <div className={styles.inputRow}>
                <input
                    value={clues.top}
                    onChange={(e) => handleInputChange("top", e.target.value)}
                    className={`${styles.clueInput} ${styles.top}`}
                    readOnly={!board.inputsActive}
                    placeholder="Top clue..."
                />
            </div>

            <div className={styles.middleRow}>
                <input
                    value={clues.left}
                    onChange={(e) => handleInputChange("left", e.target.value)}
                    className={`${styles.clueInput} ${styles.left}`}
                    readOnly={!board.inputsActive}
                    placeholder="Left..."
                />

                <div className={styles.grid}>
                    {slotIndexes.map((index) => {
                        const cardAtSlot = board.cards.find(
                            (card) => card.location === "OnBoard" && card.positionIndex === index
                        );

                        return (
                            <BoardSlot
                                key={index}
                                index={index}
                                card={cardAtSlot}
                            />
                        );
                    })}
                </div>

                <input
                    value={clues.right}
                    onChange={(e) => handleInputChange("right", e.target.value)}
                    className={`${styles.clueInput} ${styles.right}`}
                    readOnly={!board.inputsActive}
                    placeholder="Right..."
                />
            </div>

            <div className={styles.inputRow}>
                <input
                    value={clues.bottom}
                    onChange={(e) => handleInputChange("bottom", e.target.value)}
                    className={`${styles.clueInput} ${styles.bottom}`}
                    readOnly={!board.inputsActive}
                    placeholder="Bottom clue..."
                />
            </div>

            {roomContext?.status === GameStatus.Writing && (
                <button className="btn btn-primary mt-3" onClick={handleSubmitClues}>
                    Submit clues
                </button>
            )}
            {roomContext.status === GameStatus.Solving &&  (
                <>
                    <Hand cards={handCards} />
                    {!boardContext.board.isChecked && (
                    <button className="btn btn-primary mt-3" onClick={handleCheck}>
                        Check
                    </button>)}
                </>
            )}
        </div>
    );
}
