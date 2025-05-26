import { CartItemDto } from "./CartItemDto";

export interface CartDto {
    id: string;
    userId: string;
    date: string;
    items: CartItemDto[];
}
