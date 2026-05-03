import { useContext} from "react";
import { RoomContext } from "../context/RoomContext";


export default function UsersList() {

    const room = useContext(RoomContext);

    return (
        <div className="border p-3">
            <h5>Users in lobby</h5>
            <ul className="list-unstyled mb-0">
                {room?.players.map((nick, i) => (
                    <li key={i}>{nick}</li>
                ))}
            </ul>
        </div>
    );
}