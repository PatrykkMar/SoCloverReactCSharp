import { useContext } from "react";
import { RoomContext } from "../context/RoomContext";
import { GameStatus } from "../models/dtos";
import UsersList from "../components/UsersList";
import GameHelp from "../components/GameHelp";
import Board from "../components/board/Board";
import WaitingForPlayerComponent from "../components/WaitingForPlayerComponent";
import { useTranslation } from "react-i18next";

export default function Game() {
    const { t } = useTranslation();
    const room = useContext(RoomContext);

    if (!room) {
        return <div>{t("game.providerNotFound")}</div>;
    }

    const handleCopyCode = () => {
        navigator.clipboard.writeText(room.roomCode ?? "").catch(err =>
            console.error("Failed to copy: ", err)
        );
    };

    return (
        <div className="container-fluid mt-4">
            <div className="row">
                <div className="col-12 col-sm-4 col-lg-2">
                    <div className="card shadow-sm">
                        <div className="card-body">
                            <h6 className="text-muted d-flex align-items-center m-0">
                                {t("game.room")}: {room.roomCode}
                                <button
                                    onClick={handleCopyCode}
                                    className="btn btn-link p-0 ms-2 text-muted"
                                    style={{
                                        fontSize: "0.9em",
                                        lineHeight: 1,
                                        textDecoration: "none",
                                        boxShadow: "none"
                                    }}
                                    title={t("game.copyTooltip")}>
                                    📋
                                </button>
                            </h6>
                            <UsersList />
                            {room.status === GameStatus.Lobby && (
                                <button className="btn btn-success w-100 mt-3" onClick={room.startGame}>
                                    {t("game.startGame")}
                                </button>
                            )}
                        </div>
                    </div>
                </div>

                <div className="col-12 col-sm-8 col-lg-8">
                    <div
                        className="border-0 rounded d-flex align-items-center justify-content-center bg-light"
                        style={{ minHeight: '500px' }}>
                        {room.status === GameStatus.Lobby ? (<WaitingForPlayerComponent />) : (<Board />)}
                    </div>
                </div>
                <div className="col-12 col-lg-2">
                    <GameHelp currentGameState={room.status} />
                </div>
            </div>
        </div>
    );
}