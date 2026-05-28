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

    const [roomCode, setRoomCode] = useState<string | null>(null);
    const [status, setStatus] = useState<string>(GameStatus.Lobby);
    const [players, setPlayers] = useState<string[]>([]);

    useEffect(() => {
        const emitter = socketCont.eventsRef.current;
        if (!emitter) return;

        const handleRoomUpdated = (event: Event) => {
            const customEvent = event as CustomEvent<RoomDataDTO>;
            const data = customEvent.detail;

            setRoomCode(data.roomCode);
            setPlayers(data.players);
            setStatus(data.status);
        };

        emitter.addEventListener("RoomUpdated", handleRoomUpdated);

        return () => {
            emitter.removeEventListener("RoomUpdated", handleRoomUpdated);
        };
    }, [socketCont.eventsRef]);

    const createRoom = async (request: CreateRoomRequest) => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'CreateRoom': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("CreateRoom", request);
    };

    const joinRoom = async (request: JoinRoomRequest) => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'JoinRoom': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("JoinRoom", request);
    };

    const startGame = async () => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'StartGame': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("StartGame");
    };

    return (
        <RoomContext.Provider value={{ roomCode, players, status, createRoom, joinRoom, startGame }}>
            {children}
        </RoomContext.Provider>
    );
};