import { useState, useEffect, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { SocketContext } from "../context/SocketContext";
import 'bootstrap/dist/css/bootstrap.min.css';
export default function Home() {


    const [lobbyId, setLobbyId] = useState("");
    const [nick, setNick] = useState("");
    
    const navigate = useNavigate();


    const socket = useContext(SocketContext);

    useEffect(() => {
    if (socket?.roomCode) {
        navigate(`/game`);
    }
    }, [socket?.roomCode, navigate]);

    if (!socket) {
    return <div>SocketProvider not found!</div>;
    }


    const handleCreate = async () => {
        if (!nick) return alert("Enter your nick!");
        await socket.createRoom();
    };

    const handleJoin = async () => {
        if (!nick || !lobbyId) return alert("Uzupełnij dane!");
        await socket.joinRoom(lobbyId, nick);
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
                        value={lobbyId}
                        onChange={e => setLobbyId(e.target.value)}
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
                        disabled={!socket.isConnected || !lobbyId}
                    >
                        Join existing room
                    </button>
                </div>
            </div>
        </div>
    );
}