import api from "../../api";
import { PaginatedResponse } from "../../types/PaginatedResposeDto";
import { ProductDto } from "../../types/Product";
import { CreateProductDto } from "./types/CreateProductDto";
import { UpdateProductDto } from "./types/UpdateProductDto";

const CONTROLLER = "/Product";

// TODO: Add error handling
export const fetchAllProducts = async (
    page: number,
    itemPerPage: number,
): Promise<PaginatedResponse<ProductDto>> => {
    const response = await api.get<PaginatedResponse<ProductDto>>(
        `${CONTROLLER}?page=${page}&pageSize=${itemPerPage}`,
    );
    return response.data;
};

export const fetchProduct = async (id: string): Promise<ProductDto> => {
    const response = await api.get<ProductDto>(`${CONTROLLER}/${id}`);
    return response.data;
};

export const createProduct = async (
    product: CreateProductDto,
): Promise<ProductDto> => {
    const response = await api.post<ProductDto>(CONTROLLER, product);
    return response.data;
};

export const updateProduct = async (
    product: UpdateProductDto,
    id: string,
): Promise<ProductDto> => {
    const response = await api.put<ProductDto>(`${CONTROLLER}/${id}`, product);
    return response.data;
};

export const deleteProduct = async (id: string): Promise<void> => {
    await api.delete<void>(`${CONTROLLER}/${id}`);
};
