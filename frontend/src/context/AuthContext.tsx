import {
    useState,
    useEffect,
} from 'react';
import {
    login as loginRequest,
    register as registerRequest,
    getCurrentUser,
    getStoredToken,
    storeToken,
    clearToken,
} from '../services/authService';

import type {
    CurrentUser,
    LoginRequest,
    RegisterRequest,
} from '../types/auth';
import { AuthContext } from './authContextValue';

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<CurrentUser | null>(null);
    const [token, setToken] = useState<string | null>(getStoredToken());
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        const restoreSession = async (): Promise<void> => {
            const storedToken = getStoredToken();
            if (!storedToken) {
                setIsLoading(false);
                return;
            }
            try {
                const currentUser = await getCurrentUser(storedToken);
                setUser(currentUser);
                setToken(storedToken);
            } catch {
                clearToken();
                setUser(null);
                setToken(null);
            } finally {
                setIsLoading(false);
            }
        }
        restoreSession();
    }, []);

    const login = async (data: LoginRequest): Promise<void> => {
        const response = await loginRequest(data);
        storeToken(response.token, data.rememberMe);
        setToken(response.token);
        setUser(response.user);
    }

    const register = async (data: RegisterRequest): Promise<void> => {
        const response = await registerRequest(data);
        storeToken(response.token, true);
        setToken(response.token);
        setUser(response.user);
    }

    const logout = (): void => {
        clearToken();
        setUser(null);
        setToken(null);
    }


    return (
        <AuthContext.Provider
            value={{
                user,
                token,
                isAuthenticated: Boolean(user && token),
                isLoading,
                login,
                register,
                logout
            }}>
            {children}
        </AuthContext.Provider>
    );
}
