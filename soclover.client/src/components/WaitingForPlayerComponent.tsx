export default function WaitingForPlayerComponent() {

    return (
        <div className="text-center">
            <div className="spinner-border text-primary mb-3" role="status"></div>
            <h3>Waiting for start...</h3>
            <p className="text-secondary">Someone will start the game soon.</p>
        </div>
    );
}