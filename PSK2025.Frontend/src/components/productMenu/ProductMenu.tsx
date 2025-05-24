import { useQuery } from "@tanstack/react-query";
import { Alert, Box, Button, CircularProgress } from "@mui/material";
import { keys } from "../../api/queryKeyFactory";
import { fetchAllProducts } from "../../api/requests/product/ProductRequests";
import ProductMenuItem from "./productMenuItem/ProductMenuItem";
import { useState } from "react";
import { useAuthContext } from "../../context/AuthContext";
import { Role } from "../../routing/roles";
import CreateProductModal from "./CreateProductModal";
import { PaginatedItems } from "../paginatedItemBox/PaginatedItemBox";

const SIZE_PER_PAGE = 12;

export function ProductMenu() {
    const { role } = useAuthContext();
    const [pageCount, setPageCount] = useState(1);
    let { data: paginatedRows, isFetching } = useQuery({
        queryKey: keys.allProducts(pageCount),
        queryFn: () => fetchAllProducts(pageCount, SIZE_PER_PAGE),
    });

    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

    if (!isFetching && !paginatedRows) {
        return <Alert severity="error">Failed to retrieve Menu Items</Alert>;
    }

    if (isFetching || !paginatedRows) {
        return <CircularProgress />;
    }

    return (
        <>
            <Box
                display="flex"
                flexDirection="row"
                justifyContent="space-between"
                alignItems="start"
                pb={2}
            >
                {role == Role.MANAGER && (
                    <Button
                        variant="outlined"
                        onClick={() => {
                            setIsCreateModalOpen((prevState) => !prevState);
                        }}
                    >
                        Create Product
                    </Button>
                )}
            </Box>
            <PaginatedItems
                content={paginatedRows}
                itemRenderer={(item) => (
                    <ProductMenuItem key={item.id} product={item} />
                )}
                onPageChange={setPageCount}
            />
            <CreateProductModal
                open={isCreateModalOpen}
                handleCloseCreateModal={() => setIsCreateModalOpen(false)}
            />
        </>
    );
}
