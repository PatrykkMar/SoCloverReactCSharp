import { useContext } from "react";
import { RoomContext } from "../context/RoomContext";
import { GameStatus } from "../models/dtos";
import UsersList from "../components/UsersList";
import Board from "../components/board/Board";
import WaitingForPlayerComponent from "../components/WaitingForPlayerComponent";

export default function Game() {
    const room = useContext(RoomContext);

    if (!room) {
        return <div>RoomProvider not found!</div>;
    }

    return (
        <div className="container-fluid mt-4">
            <div className="row">
                <div className="col-md-3">
                    <div className="card shadow-sm">
                        <div className="card-body">
                            <h6 className="text-muted">Room: {room.roomCode}</h6>
                            <UsersList />
                            {room.status === GameStatus.Lobby && (
                                <button className="btn btn-success w-100 mt-3" onClick={room.startGame}>
                                    Start Game
                                </button>
                            )}
                        </div>
                    </div>
                </div>

                <div className="col-md-9">
                    <div
                        className="border rounded d-flex align-items-center justify-content-center bg-light"
                        style={{ minHeight: '500px' }}>
                        {room.status === GameStatus.Lobby ? (<WaitingForPlayerComponent/>) : (<Board />)}
                    </div>
                </div>
            </div>
        </div>
    );
}