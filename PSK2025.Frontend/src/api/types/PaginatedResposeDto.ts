export interface PaginatedResponse<T> {
    items: T[];
    totalCount: number;
    pageSize: number;
    currentPage: number;
    hasPrevious: boolean;
    hasNext: boolean;
}
