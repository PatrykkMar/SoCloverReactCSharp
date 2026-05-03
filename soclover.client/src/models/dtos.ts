export type GameStatus = "Lobby" | "Writing" | "Solving" | "Finished";

export interface RoomDataDTO {
    roomCode: string;
    players: string[];
    status: GameStatus;
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