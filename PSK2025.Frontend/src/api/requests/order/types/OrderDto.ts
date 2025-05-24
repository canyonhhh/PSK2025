import { OrderItemDto } from "./OrderItemDto";

export interface OrderDto {
    id: string;
    userId: string;
    totalPrice: number;
    createdAt: string;
    completedAt?: string;
    expectedCompletionTime: string;
    status: number;
    items: OrderItemDto[];
}
