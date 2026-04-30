import { useContext } from "react";
import { SocketContext } from "../context/SocketContext";

export default function Game() {


    const socket = useContext(SocketContext);

    if (!socket) {
        return <div>SocketProvider not found!</div>;
    }

    return (
        <div className="container mt-5">
            <h1>Yay, you joined the game. Lobby code: {socket.roomCode}</h1>
        </div>
    );
}