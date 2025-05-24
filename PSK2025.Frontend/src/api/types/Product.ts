export interface ProductDto {
    id: string;
    title: string;
    price: number;
    photoUrl: string | undefined;
    description: string | undefined;
    isAvailable: boolean;
    createdAt: string;
    updatedAt: string;
}
