import { BoardContext } from "../../context/BoardContext";
import type { SubmitCluesRequest } from "../../models/requests";
import BoardSlot from "./BoardSlot";
import styles from "./Board.module.css";
import { useContext, useState, useEffect } from "react";
import { RoomContext } from "../../context/RoomContext";
import { GameStatus } from "../../models/dtos";
import Hand from "./Hand";
import { useTranslation } from "react-i18next";

export default function Board() {
    const { t } = useTranslation();
    const boardContext = useContext(BoardContext);
    const roomContext = useContext(RoomContext);

    const [clues, setClues] = useState({
        top: "",
        right: "",
        bottom: "",
        left: ""
    });

    useEffect(() => {
        if (boardContext?.board && roomContext?.status) {
            const isWritingPhase = roomContext.status === GameStatus.Writing;

            const savedCluesStr = sessionStorage.getItem(`clues_backup`);
            const savedClues = savedCluesStr ? JSON.parse(savedCluesStr) : null;

            const setCluesAsync = async () => {
                setClues({
                    top: boardContext?.board?.topClue || (isWritingPhase && savedClues?.top) || "",
                    right: boardContext?.board?.rightClue || (isWritingPhase && savedClues?.right) || "",
                    bottom: boardContext?.board?.bottomClue || (isWritingPhase && savedClues?.bottom) || "",
                    left: boardContext?.board?.leftClue || (isWritingPhase && savedClues?.left) || ""
                })
            };
            setCluesAsync();
        }
    }, [boardContext?.board, roomContext?.status]);

    if (!boardContext || !boardContext.board)
        return <div>{t("board.loadingBoard")}</div>;

    if (!roomContext)
        return <div>{t("board.loadingRoom")}</div>;

    const { board, submitClues, check, returnToWriting } = boardContext;

    const slotIndexes = [0, 1, 2, 3];
    const handCards = board.cards.filter(card => card.location === "InHand");

    const handleInputChange = (direction: keyof typeof clues, value: string) => {
        if (!board.inputsActive) return;

        setClues(prev => {
            const updated = { ...prev, [direction]: value };
            sessionStorage.setItem(`clues_backup`, JSON.stringify(updated));
            return updated;
        });
    };

    const handleSubmitClues = () => {
        const words = [clues.top, clues.right, clues.bottom, clues.left];

        if (words.some(x => x.trim() === "")) {
            alert(t("board.alertEmptyInputs"));
            return;
        }

        const request: SubmitCluesRequest = { words };
        submitClues(request);
        sessionStorage.removeItem(`clues_backup`);
    };

    const handleCheck = () => {
        const cardsOnBoard = board.cards.filter(card => card.location === "OnBoard");

        if (cardsOnBoard.length < 4) {
            alert(t("board.alertPlaceAllCards"));
            return;
        }
        check();
    };

    const handleReturnToWriting = () => {
        const allCardsCorrect = board.cards
            .filter(card => card.location === "OnBoard")
            .every(card => card.isCorrect);

        if (allCardsCorrect) {
            returnToWriting();
            return;
        }

        if (window.confirm(t("board.confirmReturnToWriting"))) {
            returnToWriting();
        }
    };

    return (
        <div className={styles.boardContainer}>
            <div className={styles.inputRow}>
                <input
                    value={clues.top}
                    onChange={(e) => handleInputChange("top", e.target.value)}
                    className={`${styles.clueInput} ${styles.top}`}
                    readOnly={!board.inputsActive}
                    placeholder={t("board.placeholderTop")}
                />
            </div>

            <div className={styles.middleRow}>
                <input
                    value={clues.left}
                    onChange={(e) => handleInputChange("left", e.target.value)}
                    className={`${styles.clueInput} ${styles.left}`}
                    readOnly={!board.inputsActive}
                    placeholder={t("board.placeholderLeft")}
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
                    placeholder={t("board.placeholderRight")}
                />
            </div>

            <div className={styles.inputRow}>
                <input
                    value={clues.bottom}
                    onChange={(e) => handleInputChange("bottom", e.target.value)}
                    className={`${styles.clueInput} ${styles.bottom}`}
                    readOnly={!board.inputsActive}
                    placeholder={t("board.placeholderBottom")}
                />
            </div>

            {roomContext?.status === GameStatus.Writing && (
                <button className="btn btn-primary mt-3" onClick={handleSubmitClues}>
                    {t("board.btnSubmit")}
                </button>
            )}
            {roomContext.status === GameStatus.Solving && (
                <>
                    <Hand cards={handCards} />
                    {!boardContext.board.isChecked &&
                        !boardContext.board.cards.filter(card => card.location === "OnBoard").every(card => card.isCorrect) && (
                            <button className="btn btn-primary mt-3" onClick={handleCheck}>
                                {t("board.btnCheck")}
                            </button>
                        )}

                    <button className="btn btn-primary mt-3" onClick={handleReturnToWriting}>
                        {t("board.btnReturn")}
                    </button>
                </>
            )}
        </div>
    );
}