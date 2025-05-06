export interface CreateProductDto {
  title: string;
  price: number;
  photoUrl?: string;
  description?: string;
  isAvailable: boolean;
}
