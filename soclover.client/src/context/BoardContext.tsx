import { createContext, type ReactNode, useContext, useState, useEffect } from "react";
import type { BoardDataDTO } from "../models/dtos";
import type { MoveCardRequest, RotateCardRequest, SubmitCluesRequest } from "../models/requests";
import { SocketContext } from "./SocketContext";

export interface BoardContextType {
    board: BoardDataDTO | null;
    submitClues: (request: SubmitCluesRequest) => Promise<void>;
    rotateCard: (request: RotateCardRequest) => Promise<void>;
    moveCardToSlot: (request: MoveCardRequest) => Promise<void>;
    check: () => Promise<void>;
    returnToWriting: () => Promise<void>;
}

// eslint-disable-next-line react-refresh/only-export-components
export const BoardContext = createContext<BoardContextType | undefined>(undefined);

export const BoardProvider = ({ children }: { children: ReactNode }) => {
    const socketCont = useContext(SocketContext);

    if (!socketCont)
        throw new Error("BoardProvider must be used within a SocketProvider");

    const [board, setBoard] = useState<BoardDataDTO | null>(null);

    useEffect(() => {
        const emitter = socketCont.eventsRef.current;
        if (!emitter) return;

        const handleBoardUpdated = (event: Event) => {
            const customEvent = event as CustomEvent<BoardDataDTO>;
            const data = customEvent.detail;

            console.log("New board received from event bus:", data);
            setBoard(data);
        };

        emitter.addEventListener("BoardUpdated", handleBoardUpdated);

        return () => {
            emitter.removeEventListener("BoardUpdated", handleBoardUpdated);
        };
    }, [socketCont.eventsRef]);

    const submitClues = async (request: SubmitCluesRequest) => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'SubmitClues': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("SubmitClues", request);
    };

    const rotateCard = async (request: RotateCardRequest) => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'RotateCard': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("RotateCard", request);
    };

    const moveCardToSlot = async (request: MoveCardRequest) => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'MoveCardToSlot': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("MoveCardToSlot", request);
    };

    const check = async () => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'Check': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("Check");
    };

    const returnToWriting = async () => {
        const activeConnection = socketCont.connectionRef.current;
        if (!activeConnection) {
            console.error("Cannot invoke 'ReturnToWriting': SignalR connection is not established.");
            return;
        }
        await activeConnection.invoke("ReturnToWriting");
    };

    return (
        <BoardContext.Provider value={{ board, submitClues, rotateCard, moveCardToSlot, check, returnToWriting }}>
            {children}
        </BoardContext.Provider>
    );
};