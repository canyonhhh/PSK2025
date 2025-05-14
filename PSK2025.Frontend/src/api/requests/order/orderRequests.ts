import api from "../../api";
import { PaginatedResponse } from "../../types/PaginatedResposeDto";
import { CreateOrderDto } from "./types/CreateOrderDto";
import { OrderDto } from "./types/OrderDto";
import { UpdateOrderStatusDto } from "./types/UpdateOrderStatusDto";

const CONTROLLER = "/Order";

// TODO: Add error handling
export const fetchOrders = async (
    userId: string,
    status: number,
    sortBy: number,
    ascending: boolean,
    page: number,
    pageSize: number,
): Promise<PaginatedResponse<OrderDto>> => {
    const response = await api.get<PaginatedResponse<OrderDto>>(
        `${CONTROLLER}/?userId=${userId}&status=${status}&sortBy=${sortBy}&ascending=${ascending}&page=${page}&pageSize=${pageSize}`,
    );
    return response.data;
};

export const fetchOrder = async (orderId: string): Promise<OrderDto> => {
    const response = await api.get<OrderDto>(`${CONTROLLER}/${orderId}`);
    return response.data;
};

export const createOrder = async (cartItem: CreateOrderDto): Promise<void> => {
    await api.post<void>(`${CONTROLLER}`, cartItem);
};

export const updateOrderStatus = async (
    orderId: string,
    updateDto: UpdateOrderStatusDto,
): Promise<void> => {
    await api.put<void>(`${CONTROLLER}/${orderId}/status`, updateDto);
};

export const deleteOrder = async (orderId: string): Promise<void> => {
    await api.delete<void>(`${CONTROLLER}/${orderId}`);
};

export const deleteCartItem = async (id: string): Promise<void> => {
    await api.delete<void>(`${CONTROLLER}/items/${id}`);
};
