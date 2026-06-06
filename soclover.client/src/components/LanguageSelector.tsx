import { useTranslation } from 'react-i18next';
export default function LanguageSelector() {
    const { i18n } = useTranslation();

    const changeLanguage = (lng: string) => {
        i18n.changeLanguage(lng);
    };

    const currentLang = i18n.resolvedLanguage || i18n.language || 'en';
    const isPl = currentLang.startsWith('pl');
    const isEn = currentLang.startsWith('en');

    return (
        <div className="bg-light border-bottom px-4 py-2 d-flex justify-content-end align-items-center">
            <div className="d-flex gap-2 bg-white p-1 rounded border shadow-sm">
                <button
                    onClick={() => changeLanguage('pl')}
                    className={`btn btn-sm py-1 px-3 transition-all d-flex align-items-center ${isPl ? 'btn-success text-white' : 'btn-outline-secondary text-muted'
                        }`}
                    style={{ borderRadius: '6px', fontSize: '0.82rem', fontWeight: 'bold' }}
                    title="Polski"
                >
                    <img
                        src="https://flagcdn.com/w20/pl.png"
                        alt="PL"
                        className="me-1.5 shadow-sm"
                        style={{ width: '18px', height: 'auto', borderRadius: '2px' }}
                    />
                    <span>PL</span>
                </button>

                <button
                    onClick={() => changeLanguage('en')}
                    className={`btn btn-sm py-1 px-3 transition-all d-flex align-items-center ${isEn ? 'btn-success text-white' : 'btn-outline-secondary text-muted'
                        }`}
                    style={{ borderRadius: '6px', fontSize: '0.82rem', fontWeight: 'bold' }}
                    title="English"
                >
                    <img
                        src="https://flagcdn.com/w20/gb.png"
                        alt="EN"
                        className="me-1.5 shadow-sm"
                        style={{ width: '18px', height: 'auto', borderRadius: '2px' }}
                    />
                    <span>EN</span>
                </button>
            </div>
        </div>
    );
};