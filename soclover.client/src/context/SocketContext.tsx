import { createContext, useEffect, useRef, useState, type ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';

export interface Player {
    id: number;
    name: string;
    isReady: boolean;
    score: number;
}

export interface SocketContextType {
    isConnected: boolean;
    roomCode: string | null;
    players: Player[];
    createRoom: () => Promise<void>;
    joinRoom: (code: string, name: string) => Promise<void>;
}

// eslint-disable-next-line react-refresh/only-export-components
export const SocketContext = createContext<SocketContextType | undefined>(undefined);

export const SocketProvider = ({ children }: { children: ReactNode }) => {
    const connectionRef = useRef<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [roomCode, setRoomCode] = useState<string | null>(null);
    const [players, setPlayers] = useState<Player[]>([]);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7048/socloverhub")
            .withAutomaticReconnect()
            .build();

        newConnection.on("RoomCreated", (code: string) => setRoomCode(code));
        newConnection.on("PlayerJoined", (newPlayer: Player) => setPlayers(prev => [...prev, newPlayer]));
        newConnection.on("PlayerLeft", (id: string) => setPlayers(prev => prev.filter(p => p.id.toString() !== id)));
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
    }, []);

    const createRoom = async () => {
        if (connectionRef.current) await connectionRef.current.invoke("CreateRoom");
    };

    const joinRoom = async (code: string, name: string) => {
        if (connectionRef.current) await connectionRef.current.invoke("JoinRoom", code, name);
    };

    return (
        <SocketContext.Provider value={{ isConnected, roomCode, players, createRoom, joinRoom }}>
            {children}
        </SocketContext.Provider>
    );
};