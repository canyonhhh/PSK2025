export interface CartItemDto {
    id: string;
    productId: string;
    quantity: number;
    productName: string | undefined;
    productPrice: number;
    description?: string;
    imageUrl?: string;
}
