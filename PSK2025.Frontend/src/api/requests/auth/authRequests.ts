import api from "../../api";
import { ChangePasswordDto } from "./types/ChangePasswordDto";
import { ForgotPasswordDto } from "./types/ForgotPasswordDto";
import { LoginDto } from "./types/LoginDto";
import { LoginResponseDto } from "./types/LoginResponseDto";

const CONTROLLER = "/Auth";

// TODO: Add error handling
export const loginRequest = async (
    credentials: LoginDto,
): Promise<LoginResponseDto> => {
    const response = await api.post<LoginResponseDto>(
        `${CONTROLLER}/login`,
        credentials,
    );
    return response.data;
};

export const forgotPasswordRequest = async (
    credentials: ForgotPasswordDto,
): Promise<void> => {
    await api.post<void>(`${CONTROLLER}/forgot-password`, credentials);
};

export const changePasswordRequest = async (
    credentials: ChangePasswordDto,
): Promise<void> => {
    await api.post<void>(`${CONTROLLER}/change-password`, credentials);
};
