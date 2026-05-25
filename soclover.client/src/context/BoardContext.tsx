import { createContext, type ReactNode, useContext, useState, useEffect } from "react";
import type { BoardDataDTO } from "../models/dtos";
import type { MoveCardRequest, RotateCardRequest, SubmitCluesRequest} from "../models/requests";
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

    const con = socketCont?.connection;
    const isConnected = socketCont?.isConnected;

    const [board, setBoard] = useState<BoardDataDTO | null>(null);

    useEffect(() => {
        if (!isConnected || !con) return;

        const handleBoardUpdated = (data: BoardDataDTO) => {
            console.log("New board:", data);
            setBoard(data);
        };

        con.on("BoardUpdated", handleBoardUpdated);

        return () => {
            con.off("BoardUpdated", handleBoardUpdated);
        };
    }, [isConnected, con]);

    const submitClues = async (request: SubmitCluesRequest) => {
        if (!con) return;
        await con.invoke("SubmitClues", request);
    };

    const rotateCard = async (request: RotateCardRequest) => {
        if (!con) return;
        await con.invoke("RotateCard", request);
    };

    const moveCardToSlot = async (request: MoveCardRequest) => {
        if (!con) return;
        await con.invoke("MoveCardToSlot", request);
    };

    const check = async () => {
        if (!con) return;
        await con.invoke("Check");
    };

    const returnToWriting = async () => {
        if (!con) return;
        await con.invoke("ReturnToWriting");
    };

    return (
        <BoardContext.Provider value={{ board, submitClues, rotateCard, moveCardToSlot, check, returnToWriting }}>
            {children}
        </BoardContext.Provider>
    );
};
