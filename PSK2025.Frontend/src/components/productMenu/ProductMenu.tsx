import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert, Box, Button, CircularProgress } from "@mui/material";
import { keys } from "../../api/queryKeyFactory";
import { fetchAllProducts } from "../../api/requests/product/ProductRequests";
import ProductMenuItem from "./productMenuItem/ProductMenuItem";
import { Fragment, useState } from "react";
import { useAuthContext } from "../../context/AuthContext";
import { Role } from "../../routing/roles";
import CreateProductModal from "./CreateProductModal";
import { PaginatedItems } from "../paginatedItemBox/PaginatedItemBox";
import {
    getAppStatus,
    pauseOrders,
    resumeOrders,
} from "../../api/requests/application/applicationRequests";

const SIZE_PER_PAGE = 12;

export function ProductMenu() {
    const queryClient = useQueryClient();
    const { role } = useAuthContext();
    const [pageCount, setPageCount] = useState(1);
    let { data: paginatedRows, isFetching } = useQuery({
        queryKey: keys.allProducts(pageCount),
        queryFn: () => fetchAllProducts(pageCount, SIZE_PER_PAGE),
    });

    let { data: statusResponse, isFetching: isAreOrdersStopedFetching } =
        useQuery({
            queryKey: keys.appStatus,
            queryFn: getAppStatus,
        });

    const { mutate: stopOrdres } = useMutation({
        mutationFn: pauseOrders,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: keys.appStatus });
        },
    });

    const { mutate: continueOrders } = useMutation({
        mutationFn: resumeOrders,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: keys.appStatus });
        },
    });

    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

    if (!isFetching && !paginatedRows) {
        return <Alert severity="error">Failed to retrieve Menu Items</Alert>;
    }

    if (isFetching || !paginatedRows || isAreOrdersStopedFetching) {
        return <CircularProgress />;
    }

    const forbidMakingOrders = () => {
        if (statusResponse?.orderingPaused) {
            continueOrders();
        } else {
            stopOrdres();
        }
    };

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
                    <Fragment>
                        <Button
                            variant="outlined"
                            onClick={() => {
                                setIsCreateModalOpen((prevState) => !prevState);
                            }}
                        >
                            Create Product
                        </Button>
                        <Button
                            variant="contained"
                            onClick={forbidMakingOrders}
                        >
                            {statusResponse?.orderingPaused
                                ? "Allow making of orders"
                                : "Disallow making of new orders"}
                        </Button>
                    </Fragment>
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
