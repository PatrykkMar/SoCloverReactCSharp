import { createContext, useState, type ReactNode, type RefObject, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import type { BoardDataDTO, RoomDataDTO } from '../models/dtos';

export type SocketEventType = "RoomUpdated" | "BoardUpdated";

interface SocketContextType {
    connectionRef: RefObject<signalR.HubConnection | null>;
    isConnected: boolean;
    connect: () => Promise<void>;
    disconnect: () => Promise<void>;
    eventsRef: RefObject<EventTarget>;
}


// eslint-disable-next-line react-refresh/only-export-components
export const SocketContext = createContext<SocketContextType | undefined>(undefined);

export const SocketProvider = ({ children }: { children: ReactNode }) => {
    const connectionRef = useRef<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    const eventsRef = useRef(new EventTarget());

    const connect = async () => {

        if (connectionRef.current || isConnected) return;

        connectionRef.current = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7048/socloverhub", {
                accessTokenFactory: () => sessionStorage.getItem("jwt_token") || ""
            })
            .withAutomaticReconnect()
            .build();


        connectionRef.current.on("RoomUpdated", (data: RoomDataDTO) => {
            eventsRef.current.dispatchEvent(new CustomEvent<RoomDataDTO>("RoomUpdated", { detail: data }));
        });

        connectionRef.current.on("BoardUpdated", (data: BoardDataDTO) => {
            eventsRef.current.dispatchEvent(new CustomEvent<BoardDataDTO>("BoardUpdated", { detail: data }));
        });

        connectionRef.current.on("Error", (errorMessage: string) => {
            console.error("SignalR Error:", errorMessage);
            alert(`${errorMessage}`);
        });

        try {
            await connectionRef.current.start();
            setIsConnected(true);
            console.log("SignalR: Successful.");
        } catch (err) {
            console.error("SignalR Connection Error:", err);
            throw err; 
        }
    };

    const disconnect = async () => {
        if (connectionRef.current) {
            connectionRef.current.off("Error");
            await connectionRef.current.stop();
            connectionRef.current = null;
            setIsConnected(false);
            console.log("SignalR: Disconnected.");
        }
    };

    return (
        <SocketContext.Provider value={{ connectionRef, isConnected, connect, disconnect, eventsRef }}>
            {children}
        </SocketContext.Provider>
    );
};