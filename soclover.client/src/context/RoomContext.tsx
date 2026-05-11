import { createContext, type ReactNode, useContext, useState, useEffect } from "react";
import type { RoomDataDTO } from "../models/dtos";
import { GameStatus } from "../models/dtos";
import type { CreateRoomRequest, JoinRoomRequest } from "../models/requests";
import { SocketContext } from "./SocketContext";

export interface RoomContextType {
    roomCode: string | null;
    players: string[];
    status: string;
    createRoom: (request: CreateRoomRequest) => Promise<void>;
    joinRoom: (request: JoinRoomRequest) => Promise<void>;
    startGame: () => Promise<void>;
}

// eslint-disable-next-line react-refresh/only-export-components
export const RoomContext = createContext<RoomContextType | undefined>(undefined);


export const RoomProvider = ({ children }: { children: ReactNode }) => {
    const socketCont = useContext(SocketContext);

    if (!socketCont)
        throw new Error("RoomProvider must be used within a SocketProvider");

    const con = socketCont.connection;

    const [roomCode, setRoomCode] = useState<string | null>(null);
    const [status, setStatus] = useState<string>(GameStatus.Lobby);
    const [players, setPlayers] = useState<string[]>([]);

    useEffect(() => {
        if (!socketCont.isConnected || !con) return;

        const handleRoomUpdated = (data: RoomDataDTO) => {
            setRoomCode(data.roomCode);
            setPlayers(data.players);
            setStatus(data.status);
        };

        const handlePlayerLeft = (name: string) => {
            setPlayers(prev => prev.filter(p => p !== name));
        };

        con.on("RoomUpdated", handleRoomUpdated);
        con.on("PlayerLeft", handlePlayerLeft);

        return () => {
            con.off("RoomUpdated", handleRoomUpdated);
            con.off("PlayerLeft", handlePlayerLeft);
        };
    }, [socketCont.isConnected, con]);

    const createRoom = async (request: CreateRoomRequest) => {
        if (!con) return;
        await con.invoke("CreateRoom", request);
    };

    const joinRoom = async (request: JoinRoomRequest) => {
        if (!con) return;
        await con.invoke("JoinRoom", request);
    };

    const startGame = async () => {
        if (!con) return;
        await con.invoke("StartGame");
    };

    return (
        <RoomContext.Provider value={{ roomCode, players, status, createRoom, joinRoom, startGame }}>
            {children}
        </RoomContext.Provider>
    );
};
