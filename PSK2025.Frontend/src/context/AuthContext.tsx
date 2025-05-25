import { createContext, ReactNode, useContext, useState } from "react";
import { Role, roleToRoleEnum } from "../routing/roles";
import { jwtDecode } from "jwt-decode";

export const AUTH_TOKEN_KEY = "api_auth";

interface TokenPayload {
    role: string;
}

interface AuthContextValues {
    token: string | null;
    role: Role | null;
    id: string | null;
    login: (token: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextValues | null>(null);

interface AuthContextProviderProps {
    children: ReactNode;
}

const AuthContextProvider = ({ children }: AuthContextProviderProps) => {
    const [token, setToken] = useState<string | null>(() =>
        localStorage.getItem(AUTH_TOKEN_KEY),
    );

    let role: Role | null = null;
    let id: string | null = null;

    if (token) {
        const decoded = jwtDecode<TokenPayload>(token);
        if (decoded) {
            const smt =
                ((decoded as any)[
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                ] as string) ?? "";
            role = roleToRoleEnum[smt];
            id =
                ((decoded as any)[
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                ] as string) ?? "";
        }
    }

    const logout = () => {
        localStorage.removeItem(AUTH_TOKEN_KEY);
        setToken(null);
    };

    const login = (loginToken: string) => {
        localStorage.setItem(AUTH_TOKEN_KEY, loginToken);
        setToken(loginToken);
        const decoded = jwtDecode<TokenPayload>(loginToken);
        const smt =
            ((decoded as any)[
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            ] as string) ?? "";
        role = roleToRoleEnum[smt];
        id =
            ((decoded as any)[
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            ] as string) ?? "";
    };

    return (
        <AuthContext.Provider value={{ token, logout, login, role, id }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuthContext = (): AuthContextValues => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error(
            "Unable to use auth context outside auth context scope",
        );
    }

    return context;
};

export default AuthContextProvider;
