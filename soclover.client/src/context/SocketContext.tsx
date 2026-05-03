import { createContext, useEffect, useState, type ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';

interface SocketContextType {
    connection: signalR.HubConnection | null;
    isConnected: boolean;
}

// eslint-disable-next-line react-refresh/only-export-components
export const SocketContext = createContext<SocketContextType | undefined>(undefined);

export const SocketProvider = ({ children }: { children: ReactNode }) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7048/socloverhub")
            .withAutomaticReconnect()
            .build();

        const start = async () => {
            try {
                await conn.start();
                setConnection(conn);
                setIsConnected(true);
            } catch (err) {
                console.error(err);
            }
        };

        start();

        return () => {
            conn.stop();
            setConnection(null);
            setIsConnected(false);
        };
    }, []);

    return (
        <SocketContext.Provider value={{ connection, isConnected }}>
            {children}
        </SocketContext.Provider>
    );
};