import { useContext } from "react";
import { RoomContext } from "../context/RoomContext";
import { GameStatus } from "../models/dtos";
import { useTranslation } from "react-i18next";

export default function UsersList() {
    const { t } = useTranslation();
    const room = useContext(RoomContext);

    return (
        <div className="mt-2">
            <h5 className="mb-3">{t("usersList.players")} ({room?.players.length}/6)</h5>
            <h5>{t("usersList.state")}: {room?.status}</h5>
            {
                room?.status === GameStatus.Solving && <h6>{t("usersList.numberOfAttempts")}: {room?.numberOfAttempts}</h6>
            }
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