import { createContext, useEffect, useRef, useState, type ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';
import type { RoomDataDto } from '../models/responses';
import type { CreateRoomRequest, JoinRoomRequest } from '../models/requests';

export interface Player {
    id: number;
    name: string;
    isReady: boolean;
    score: number;
}

export interface SocketContextType {
    isConnected: boolean;
    roomCode: string | null;
    players: string[];
    createRoom: (request: CreateRoomRequest) => Promise<void>;
    joinRoom: (request: JoinRoomRequest) => Promise<void>;
}

// eslint-disable-next-line react-refresh/only-export-components
export const SocketContext = createContext<SocketContextType | undefined>(undefined);

export const SocketProvider = ({ children }: { children: ReactNode }) => {
    const connectionRef = useRef<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [roomCode, setRoomCode] = useState<string | null>(null);
    const [players, setPlayers] = useState<string[]>([]);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7048/socloverhub")
            .withAutomaticReconnect()
            .build();

        newConnection.on("RoomUpdated", (data: RoomDataDto) => {
            setRoomCode(data.roomCode);
            setPlayers(data.players);
        });

        newConnection.on("PlayerLeft", (name: string) => setPlayers(prev => prev.filter(p => p !== name)));
        newConnection.on("Error", (msg: string) => console.error(msg));

        const startConnection = async () => {
            try {
                if(!isConnected)
                {
                    await newConnection.start();
                    console.log("SignalR Connected");
                    connectionRef.current = newConnection;
                    setIsConnected(true);               
                }
            } catch (err) {
                console.error("SignalR Connection Error: ", err);
            }
        };

        startConnection();

        return () => {
            newConnection.stop();
            connectionRef.current = null;
        };
    },[]);

    const createRoom = async (request: CreateRoomRequest) => {
        if (connectionRef.current) await connectionRef.current.invoke("CreateRoom", request);
    };

    const joinRoom = async (request: JoinRoomRequest) => {
        if (connectionRef.current) await connectionRef.current.invoke("JoinRoom", request);
    };

    return (
        <SocketContext.Provider value={{ isConnected, roomCode, players, createRoom, joinRoom }}>
            {children}
        </SocketContext.Provider>
    );
};