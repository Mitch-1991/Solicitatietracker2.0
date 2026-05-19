import { createContext, useContext, useState } from 'react';
import type { ReactNode } from 'react';
import { useAuth } from './authContextValue';

interface ViewOnlyContextType {
    isOpen: boolean;
    show: () => void;
    hide: () => void;
    requestWrite: (action: () => void | Promise<void>) => void;
}

const ViewOnlyContext = createContext<ViewOnlyContextType | undefined>(undefined);

export function ViewOnlyProvider({ children }: { children: ReactNode }) {
    const [isOpen, setIsOpen] = useState(false);
    const { isViewOnly } = useAuth();

    const show = () => setIsOpen(true);
    const hide = () => setIsOpen(false);

    const requestWrite = (action: () => void | Promise<void>): void => {
        if (isViewOnly) {
            show();
            return;
        }
        Promise.resolve(action()).catch((err: unknown) => {
            console.error('[requestWrite] Unhandled error in write action:', err);
        });
    };

    return (
        <ViewOnlyContext.Provider value={{ isOpen, show, hide, requestWrite }}>
            {children}
        </ViewOnlyContext.Provider>
    );
}

export function useViewOnly(): ViewOnlyContextType {
    const context = useContext(ViewOnlyContext);
    if (!context) {
        throw new Error('useViewOnly must be used within a ViewOnlyProvider');
    }
    return context;
}
