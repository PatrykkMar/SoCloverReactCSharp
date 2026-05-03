import { useContext } from "react";
import { RoomContext } from "../context/RoomContext";
import UsersList from "../components/UsersList";

export default function Game() {


    const room = useContext(RoomContext);

    if (!room) {
        return <div>RoomProvider not found!</div>;
    }

    return (
        <div className="container mt-5">
            <h1>Yay, you joined the game. Lobby code: {room.roomCode}</h1>
            <UsersList></UsersList>
        </div>
    );
}