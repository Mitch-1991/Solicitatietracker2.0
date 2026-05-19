import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { ViewOnlyProvider, useViewOnly } from './ViewOnlyContext';
import { AuthContext } from './authContextValue';
import type { AuthContextType } from '../types/auth';

function makeAuthValue(isViewOnly: boolean): AuthContextType {
    return {
        user: isViewOnly
            ? { id: 1, firstName: 'Recruiter', lastName: 'Demo', email: 'recruiter.demo@sollicitatietracker.be' }
            : { id: 2, firstName: 'Regular', lastName: 'User', email: 'user@example.com' },
        token: 'test-token',
        isAuthenticated: true,
        isViewOnly,
        isLoading: false,
        login: vi.fn(),
        register: vi.fn(),
        logout: vi.fn(),
    };
}

function TestConsumer() {
    const { requestWrite, isOpen } = useViewOnly();
    return (
        <>
            <span data-testid="modal-open">{isOpen ? 'open' : 'closed'}</span>
            <button onClick={() => requestWrite(() => { /* write action */ })}>
                Write
            </button>
        </>
    );
}

function renderWithAuth(isViewOnly: boolean) {
    return render(
        <AuthContext.Provider value={makeAuthValue(isViewOnly)}>
            <ViewOnlyProvider>
                <TestConsumer />
            </ViewOnlyProvider>
        </AuthContext.Provider>
    );
}

describe('ViewOnlyContext', () => {
    it('executes the action for a regular user', () => {
        const action = vi.fn();
        function TestAction() {
            const { requestWrite } = useViewOnly();
            return <button onClick={() => requestWrite(action)}>Go</button>;
        }
        render(
            <AuthContext.Provider value={makeAuthValue(false)}>
                <ViewOnlyProvider>
                    <TestAction />
                </ViewOnlyProvider>
            </AuthContext.Provider>
        );
        fireEvent.click(screen.getByText('Go'));
        expect(action).toHaveBeenCalledOnce();
    });

    it('does NOT execute the action for a view-only user', () => {
        const action = vi.fn();
        function TestAction() {
            const { requestWrite } = useViewOnly();
            return <button onClick={() => requestWrite(action)}>Go</button>;
        }
        render(
            <AuthContext.Provider value={makeAuthValue(true)}>
                <ViewOnlyProvider>
                    <TestAction />
                </ViewOnlyProvider>
            </AuthContext.Provider>
        );
        fireEvent.click(screen.getByText('Go'));
        expect(action).not.toHaveBeenCalled();
    });

    it('opens the modal for a view-only user', () => {
        renderWithAuth(true);
        expect(screen.getByTestId('modal-open').textContent).toBe('closed');
        fireEvent.click(screen.getByText('Write'));
        expect(screen.getByTestId('modal-open').textContent).toBe('open');
    });

    it('does NOT open the modal for a regular user', () => {
        renderWithAuth(false);
        fireEvent.click(screen.getByText('Write'));
        expect(screen.getByTestId('modal-open').textContent).toBe('closed');
    });

    it('throws when used outside the provider', () => {
        const spy = vi.spyOn(console, 'error').mockImplementation(() => {});
        expect(() => render(
            <AuthContext.Provider value={makeAuthValue(false)}>
                <TestConsumer />
            </AuthContext.Provider>
        )).toThrow('useViewOnly must be used within a ViewOnlyProvider');
        spy.mockRestore();
    });
});
