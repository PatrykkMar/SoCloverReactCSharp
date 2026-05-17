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
    gameRoomCardId: number;
    wordTop: string;
    wordRight: string;
    wordBottom: string;
    wordLeft: string;
    currentRotation: number;
    positionIndex: number | undefined; //position on board
}

export interface BoardDataDTO {
    cards: CardDTO[];
    hand: CardDTO[];
    topClue: string;
    rightClue: string;
    bottomClue: string;
    leftClue: string;
    isActive: boolean;
}
