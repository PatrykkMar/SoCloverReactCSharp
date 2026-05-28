export const GameStatus = {
    Lobby: "Lobby",
    Writing: "Writing",
    Solving: "Solving",
    Finished: "Finished",
} as const;

export const CardLocation = {
    InDeck: "InDeck",
    Discarded: "Discarded",
    InHand: "InHand",
    OnBoard: "OnBoard",
} as const;

export interface RoomDataDTO {
    roomCode: string;
    players: string[];
    status: string;
    numberOfAttempts: number;
}

export interface CardDTO {
    gameRoomCardId: number;
    wordTop: string;
    wordRight: string;
    wordBottom: string;
    wordLeft: string;
    currentRotation: number;
    positionIndex: number | undefined; //position on board
    location: string;
    isCorrect: boolean | undefined;
}

export interface BoardDataDTO {
    cards: CardDTO[];
    topClue: string;
    rightClue: string;
    bottomClue: string;
    leftClue: string;
    inputsActive: boolean;
    cardsActive: boolean;
    isChecked: boolean;
}
