import { useTranslation } from "react-i18next";

export default function WaitingForPlayerComponent() {
    const { t } = useTranslation();

    return (
        <div className="text-center">
            <div className="spinner-border text-primary mb-3" role="status"></div>
            <h3>{t("waitingForPlayer.title")}</h3>
            <p className="text-secondary">{t("waitingForPlayer.description")}</p>
        </div>
    );
}