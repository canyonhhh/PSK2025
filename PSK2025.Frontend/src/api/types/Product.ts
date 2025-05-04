export interface ProductDto {
  Id: string;
  Title: string;
  Price: number;
  PhotoUrl: string | undefined;
  Description: string | undefined;
  IsAvailable: boolean;
  CreatedAt: string;
  UpdatedAt: string;
}
