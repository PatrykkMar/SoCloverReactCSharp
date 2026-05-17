import { createContext, type ReactNode, useContext, useState, useEffect } from "react";
import type { BoardDataDTO } from "../models/dtos";
import type { SubmitCluesRequest} from "../models/requests";
import { SocketContext } from "./SocketContext";

export interface BoardContextType {
    board: BoardDataDTO | null;
    submitClues: (request: SubmitCluesRequest) => Promise<void>;
    rotateCard: (positionIndex: number) => Promise<void>;
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

    const rotateCard = async (positionIndex: number) => {
        if (!con) return;
        await con.invoke("RotateCard", { positionIndex });
    };

    return (
        <BoardContext.Provider value={{ board, submitClues, rotateCard }}>
            {children}
        </BoardContext.Provider>
    );
};
