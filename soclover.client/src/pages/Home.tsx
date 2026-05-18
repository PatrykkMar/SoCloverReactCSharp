import { useState, useEffect, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { SocketContext } from "../context/SocketContext";
import { RoomContext } from "../context/RoomContext";
import { getOrCreatePlayerGuid } from "../utils/socketUtils";
import type { CreateRoomRequest, JoinRoomRequest } from "../models/requests"
import 'bootstrap/dist/css/bootstrap.min.css';
export default function Home() {


    const [nick, setNick] = useState("");
    const [inputRoomCode, setInputRoomCode] = useState("");
    
    const navigate = useNavigate();


    const socket = useContext(SocketContext);
    const room = useContext(RoomContext);

    if (!socket)
        throw new Error("SocketContext not found! Make sure to wrap your app with SocketProvider.");

    if (!room)
        throw new Error("RoomContext not found! Make sure to wrap your app with RoomProvider.");

    useEffect(() => {
        if (room?.roomCode) {
            navigate(`/game`);
        }
    }, [room?.roomCode, navigate]);


    const handleCreate = async () => {
        if (!nick) return alert("Enter your nick!");
        await room?.createRoom({ playerName: nick, playerGuid: getOrCreatePlayerGuid() } as CreateRoomRequest);
    };

    const handleJoin = async () => {
        if (!nick || !inputRoomCode) return alert("Fill room code!");
        await room?.joinRoom({ playerName: nick, playerGuid: getOrCreatePlayerGuid(), roomCode: inputRoomCode } as JoinRoomRequest);
    };

    return (
        <div className="container mt-5" style={{ maxWidth: '400px' }}>
            <div className="card shadow p-4">
                <h1 className="text-center mb-4">So Clover</h1>
                
                <div className="mb-3">
                    <input
                        className="form-control"
                        placeholder="Your nick"
                        value={nick}
                        onChange={e => setNick(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <input
                        className="form-control"
                        placeholder="Lobby code (optional)"
                        value={inputRoomCode}
                        onChange={e => setInputRoomCode(e.target.value)}
                    />
                </div>

                <div className="d-grid gap-2">
                    <button 
                        className="btn btn-success" 
                        onClick={handleCreate}
                        disabled={!socket.isConnected}
                    >
                        {socket.isConnected ? "Create new room" : "Connecting..."}
                    </button>
                    
                    <button 
                        className="btn btn-outline-primary" 
                        onClick={handleJoin}
                        disabled={!socket.isConnected || !inputRoomCode}
                    >
                        Join existing room
                    </button>
                </div>
            </div>
        </div>
    );
}