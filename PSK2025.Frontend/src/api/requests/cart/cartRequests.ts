import api from "../../api";
import { AddCartItemDto } from "./types/AddCartItemDto";
import { CartDto } from "./types/CartDto";
import { UpdateCartItemDto } from "./types/UpdateCartItemDto";

const CONTROLLER = "/Cart";

// TODO: Add error handling
export const fetchCart = async (): Promise<CartDto> => {
    const response = await api.get<CartDto>(`${CONTROLLER}`);
    return response.data;
};

export const createCartItem = async (
    cartItem: AddCartItemDto,
): Promise<void> => {
    await api.post<void>(`${CONTROLLER}/items`, cartItem);
};

export const updateCartItem = async (
    cartItem: UpdateCartItemDto,
): Promise<void> => {
    await api.put<void>(`${CONTROLLER}/items`, cartItem);
};

export const deleteCartItems = async (): Promise<void> => {
    await api.delete<void>(`${CONTROLLER}/items`);
};

export const deleteCartItem = async (id: string): Promise<void> => {
    await api.delete<void>(`${CONTROLLER}/items/${id}`);
};
