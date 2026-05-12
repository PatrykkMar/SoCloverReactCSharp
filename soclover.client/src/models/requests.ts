export interface CreateRoomRequest {
    playerName: string;
    playerGuid: string;
}

export interface JoinRoomRequest {
    roomCode: string;
    playerName: string;
    playerGuid: string;
}

export interface SubmitCluesRequest {
    words: string[];
}
