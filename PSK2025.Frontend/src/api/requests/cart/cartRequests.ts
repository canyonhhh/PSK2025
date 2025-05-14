import api from "../../api";
import { AddCartItemDto } from "./types/AddCartItemDto";
import { CartDto } from "./types/CartDto";
import { UpdateCartDto } from "./types/UpdateCartDto";
import { UpdateCartItemDto } from "./types/UpdateCartItemDto";

const CONTROLLER = "/cart";

// TODO: Add error handling
export const fetchAllCarts = async (): Promise<CartDto[]> => {
    const response = await api.get<CartDto[]>(CONTROLLER);
    return response.data;
};

export const fetchCart = async (): Promise<CartDto> => {
    const response = await api.get<CartDto>(`${CONTROLLER}/active`);
    return response.data;
};

export const createCartItem = async (
    cartItem: AddCartItemDto,
): Promise<void> => {
    await api.post<void>(`${CONTROLLER}/active/items`, cartItem);
};

export const updateCart = async (cart: UpdateCartDto): Promise<void> => {
    await api.put<void>(`${CONTROLLER}/active`, cart);
};

export const updateCartItem = async (
    cartItem: UpdateCartItemDto,
): Promise<void> => {
    await api.put<void>(
        `${CONTROLLER}/active/items/${cartItem.itemId}`,
        cartItem,
    );
};

export const deleteCartItem = async (id: string): Promise<void> => {
    await api.delete<void>(`${CONTROLLER}/active/items/${id}`);
};
