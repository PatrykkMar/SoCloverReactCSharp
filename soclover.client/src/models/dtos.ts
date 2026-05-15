export const GameStatus = {
    Lobby: "Lobby",
    Writing: "Writing",
    Solving: "Solving",
    Finished: "Finished",
} as const;

export interface RoomDataDTO {
    roomCode: string;
    players: string[];
    status: string;
}

export interface CardDTO {
    wordTop: string;
    wordRight: string;
    wordBottom: string;
    wordLeft: string;
    currentRotation: number;
    positionIndex: number;
}

export interface BoardDataDTO {
    cards: CardDTO[];
    topClue: string;
    rightClue: string;
    bottomClue: string;
    leftClue: string;
    isActive: boolean;
}
