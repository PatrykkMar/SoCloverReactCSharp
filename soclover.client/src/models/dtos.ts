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
}

export interface BoardSlotDTO {
    card: CardDTO | null;
    currentRotation: number;
    positionIndex: number;
}

export interface BoardDataDTO {
    boardSlots: BoardSlotDTO[];
    wordTop: string;
    wordRight: string;
    wordBottom: string;
    wordLeft: string;
}
