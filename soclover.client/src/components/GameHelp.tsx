import React from 'react';
import { useTranslation } from 'react-i18next';

interface GameHelpProps {
    currentGameState: string;
}

interface InstructionObject {
    title: string;
    description: string;
    tips?: string[];
}

export default function GameHelp({ currentGameState }: GameHelpProps) {
    const { t } = useTranslation();

    const stateInstructions = t("gameHelp.states", { returnObjects: true });

    const currentInstruction = (
        stateInstructions &&
        typeof stateInstructions === 'object' &&
        (stateInstructions as Record<string, InstructionObject>)[currentGameState]
    ) || {
        title: t("gameHelp.fallbackTitle"),
        description: t("gameHelp.fallbackDescription"),
        tips: []
    };

    return (
        <div className="card shadow-sm h-100 border-0" style={{ borderRadius: '12px', overflow: 'hidden' }}>
            <div className="card-header text-white px-4 py-3 border-0 d-flex align-items-center justify-content-between"
                style={{ backgroundColor: '#198754' }}>
                <h5 className="m-0 font-weight-bold" style={{ fontSize: '1.05rem', letterSpacing: '0.5px' }}>
                    📖 {t("gameHelp.guideTitle")}
                </h5>
            </div>

            <div className="card-body p-4 bg-white d-flex flex-column h-100">
                <div className="mb-4">
                    <h6 className="font-weight-bold mb-3" style={{ fontSize: '1.15rem', color: '#2c3e50' }}>
                        {currentInstruction.title}
                    </h6>
                    <p className="text-muted" style={{ fontSize: '0.92rem', lineHeight: '1.6' }}>
                        {currentInstruction.description}
                    </p>
                </div>

                {currentInstruction.tips && Array.isArray(currentInstruction.tips) && currentInstruction.tips.length > 0 && (
                    <div className="p-3 rounded mt-auto" style={{ backgroundColor: '#f8f9fa', borderRadius: '8px', borderLeft: '4px solid #198754' }}>
                        <span className="d-block text-uppercase font-weight-bold mb-2 text-success" style={{ fontSize: '0.72rem', letterSpacing: '1px' }}>
                            💡 {t("gameHelp.tips")}
                        </span>
                        <ul className="list-unstyled m-0">
                            {currentInstruction.tips.map((tip, idx) => (
                                <li key={idx} className="text-secondary d-flex align-items-start mb-2" style={{ fontSize: '0.85rem' }}>
                                    <span>{tip}</span>
                                </li>
                            ))}
                        </ul>
                    </div>
                )}
            </div>
        </div>
    );
};