export interface UpdateProductDto {
    title: string;
    price: number;
    photoUrl?: string;
    description?: string;
    isAvailable: boolean;
}
