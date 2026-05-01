export interface RoomDataDto {
    roomCode: string
    players: string[]
}

export interface CardDTO {
    wordTop: string;
    wordRight: string;
    wordBottom: string;
    wordLeft: string;
}

export interface BoardSlotDTO {
    card: CardDTO;
    currentRotation: number;
    positionIndex: number;
}

export interface BoardDTO {
    boardSlots: BoardSlotDTO[];
}