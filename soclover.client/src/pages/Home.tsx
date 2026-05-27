import { useState, useEffect, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { SocketContext } from "../context/SocketContext";
import { RoomContext } from "../context/RoomContext";
import type { CreateRoomRequest, JoinRoomRequest } from "../models/requests";
import 'bootstrap/dist/css/bootstrap.min.css';

export default function Home() {
    const [nick, setNick] = useState("");
    const [inputRoomCode, setInputRoomCode] = useState("");
    const [isLoading, setIsLoading] = useState(false);

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

    const authenticateAndConnect = async (username: string): Promise<boolean> => {
        try {
            const response = await fetch("https://localhost:7048/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username: username.trim() })
            });

            if (!response.ok) {
                throw new Error("Authentication failed server-side.");
            }

            const data = await response.json();

            sessionStorage.setItem("jwt_token", data.token);

            if (!socket.isConnected) {
                await socket.connect();
            }

            return true;
        } catch (err) {
            console.error("Authentication or connection lifecycle failure:", err);
            alert("Failed to initialize game secure session. Please try again.");
            return false;
        }
    };

    const handleCreate = async () => {
        if (!nick.trim()) return alert("Enter your nick!");

        setIsLoading(true);
        const success = await authenticateAndConnect(nick);

        if (success) {
            try {
                await room.createRoom({
                    playerName: nick.trim()
                } as CreateRoomRequest);
            } catch (err) {
                console.error("Failed to create room:", err);
            }
        }
        setIsLoading(false);
    };

    const handleJoin = async () => {
        if (!nick.trim()) return alert("Please enter your nickname!");
        if (!inputRoomCode.trim()) return alert("Please enter a room code!");

        setIsLoading(true);
        const success = await authenticateAndConnect(nick);

        if (success) {
            try {
                await room.joinRoom({
                    playerName: nick.trim(),
                    roomCode: inputRoomCode.trim().toUpperCase()
                } as JoinRoomRequest);
            } catch (err) {
                console.error("Failed to join room:", err);
            }
        }
        setIsLoading(false);
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
                        disabled={isLoading}
                        onChange={e => setNick(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <input
                        className="form-control"
                        placeholder="Lobby code"
                        value={inputRoomCode}
                        disabled={isLoading}
                        onChange={e => setInputRoomCode(e.target.value)}
                    />
                </div>

                <div className="d-grid gap-2">
                    <button
                        className="btn btn-success"
                        onClick={handleCreate}
                        disabled={isLoading || !nick.trim()}
                    >
                        {isLoading ? "Processing..." : "Create new room"}
                    </button>

                    <button
                        className="btn btn-outline-primary"
                        onClick={handleJoin}
                        disabled={isLoading || !nick.trim() || !inputRoomCode.trim()}
                    >
                        {isLoading ? "Processing..." : "Join existing room"}
                    </button>
                </div>
            </div>
        </div>
    );
}