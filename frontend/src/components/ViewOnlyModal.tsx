import { useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { EyeOff } from 'lucide-react';
import { useViewOnly } from '../context/ViewOnlyContext';

export default function ViewOnlyModal() {
    const { isOpen, hide } = useViewOnly();
    const closeButtonRef = useRef<HTMLButtonElement>(null);

    useEffect(() => {
        if (!isOpen) return;

        closeButtonRef.current?.focus();

        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape') hide();
        };
        document.addEventListener('keydown', handleKeyDown);
        return () => document.removeEventListener('keydown', handleKeyDown);
    }, [isOpen, hide]);

    if (!isOpen) return null;

    return (
        <section
            className="application-modal-overlay"
            role="dialog"
            aria-modal="true"
            aria-labelledby="view-only-title"
            onClick={(e) => e.target === e.currentTarget && hide()}
        >
            <div className="application-modal view-only-modal">
                <div className="application-modal-header">
                    <div className="view-only-header-title">
                        <EyeOff size={20} className="view-only-icon" aria-hidden="true" />
                        <h2 id="view-only-title" className="application-modal-title">
                            View Only profiel
                        </h2>
                    </div>
                    <button
                        ref={closeButtonRef}
                        type="button"
                        className="application-modal-close"
                        onClick={hide}
                        aria-label="Sluit melding"
                    >
                        ×
                    </button>
                </div>

                <div className="application-modal-form">
                    <p className="view-only-message">
                        Dit is een <strong>View Only</strong> profiel. Je kunt alle functionaliteiten bekijken,
                        maar geen wijzigingen opslaan.
                    </p>
                    <p className="view-only-hint">
                        Maak een persoonlijk account aan om de volledige functionaliteit te gebruiken.
                    </p>

                    <div className="view-only-footer">
                        <button
                            type="button"
                            className="application-modal-button application-modal-button-secondary"
                            onClick={hide}
                        >
                            Sluiten
                        </button>
                        <Link
                            to="/register"
                            className="application-modal-button application-modal-button-primary"
                            onClick={hide}
                        >
                            Account aanmaken
                        </Link>
                    </div>
                </div>
            </div>
        </section>
    );
}
