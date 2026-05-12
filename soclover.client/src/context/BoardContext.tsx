import { createContext, type ReactNode, useContext, useState, useEffect } from "react";
import type { BoardDataDTO } from "../models/dtos";
import type { SubmitClueRequest} from "../models/requests";
import { SocketContext } from "./SocketContext";

export interface BoardContextType {
    board: BoardDataDTO | null;
    submitClues: (request: SubmitClueRequest) => Promise<void>;
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

    const submitClues = async (request: SubmitClueRequest) => {
        if (!con) return;
        await con.invoke("SubmitClue", request);
    };

    return (
        <BoardContext.Provider value={{ board, submitClues }}>
            {children}
        </BoardContext.Provider>
    );
};
