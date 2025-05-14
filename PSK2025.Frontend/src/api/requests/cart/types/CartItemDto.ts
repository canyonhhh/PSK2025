export interface CartItemDto {
    itemId: string;
    quantity: number;
    productName: string | undefined;
    ProductPrice: number;
    description?: string;
    imageUrl?: string;
}
