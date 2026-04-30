import { useContext} from "react";
import { SocketContext } from "../context/SocketContext";


export default function UsersList() {

    const clientSocket = useContext(SocketContext);

    return (
        <div className="border p-3">
            <h5>Users in lobby</h5>
            <ul className="list-unstyled mb-0">
                {clientSocket?.players.map((nick, i) => (
                    <li key={i}>{nick}</li>
                ))}
            </ul>
        </div>
    );
}