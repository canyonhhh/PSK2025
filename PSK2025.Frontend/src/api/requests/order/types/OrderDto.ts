import { OrderItemDto } from "./OrderItemDto";

export interface OrderDto {
    id: string;
    userId: string;
    totalprice: number;
    createdAt: string;
    completedAt: string;
    expectedCompletionTime: string;
    orderStatus: number;
    items: OrderItemDto[];
}
