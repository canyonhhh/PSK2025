export interface CartItemDto {
    itemId: string;
    quantity: number;
    productName: string | undefined;
    price: number;
    description?: string;
    imageUrl?: string;
}
