import api from "../../api";
import { LoginResponseDto } from "../auth/types/LoginResponseDto";
import { RegisterDto } from "./types/RegisterDto";
import { UpdateUserDto } from "./types/UpdateUserDto";
import { UserDto } from "./types/UserDto";
import { UserRole } from "./types/UserRoles";

const CONTROLLER = "/User";

// TODO: Add error handling
export const fetchAllUsers = async (): Promise<UserDto[]> => {
    const response = await api.get<UserDto[]>(CONTROLLER);
    return response.data;
};

export const fetchUser = async (id: string): Promise<UserDto> => {
    const response = await api.get<UserDto>(`${CONTROLLER}/${id}`);
    return response.data;
};

export const fetchUserByEmail = async (email: string): Promise<UserDto> => {
    const response = await api.get<UserDto>(`${CONTROLLER}/email/${email}`);
    return response.data;
};

export const registerRequest = async (
    credentials: RegisterDto,
): Promise<LoginResponseDto> => {
    const response = await api.post<LoginResponseDto>(CONTROLLER, credentials);
    return response.data;
};

export const updateUserRequest = async (
    user: UpdateUserDto,
    id: string,
): Promise<UserDto> => {
    const response = await api.put<UserDto>(`${CONTROLLER}/${id}`, user);
    return response.data;
};

export const deleteUserRequest = async (id: string): Promise<void> => {
    await api.delete<void>(`${CONTROLLER}/${id}`);
};

export const changeRoleReuqest = async (
    id: string,
    newRole: UserRole,
): Promise<UserDto> => {
    const response = await api.put<UserDto>(
        `${CONTROLLER}/${id}/change-role`,
        newRole,
    );
    return response.data;
};
