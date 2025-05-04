export interface CreateProductDto {
  Title: string;
  Price: number;
  PhotoUrl?: string;
  Description?: string;
  IsAvailable: boolean;
}
