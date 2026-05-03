import { useContext} from "react";
import { RoomContext } from "../context/RoomContext";


export default function UsersList() {
    const room = useContext(RoomContext);

    return (
        <div className="mt-2">
            <h5 className="mb-3">Players ({room?.players.length}/6)</h5>
            <ul className="list-group">
                {room?.players.map((nick, i) => (
                    <li key={i} className="list-group-item d-flex align-items-center">
                        <div
                            className="bg-primary rounded-circle me-2"
                            style={{ width: '10px', height: '10px' }}
                        ></div>
                        {nick}
                    </li>
                ))}
            </ul>
        </div>
    );
}