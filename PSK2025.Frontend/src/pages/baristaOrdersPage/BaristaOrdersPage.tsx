import { Alert, Box, CircularProgress, Typography } from "@mui/material";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { keys } from "../../api/queryKeyFactory";
import { fetchOrders } from "../../api/requests/order/orderRequests";
import OrdersTable from "../../components/ordersTable/OrdersTable";

const BaristaOrdersPage = () => {
    const queryClient = useQueryClient();
    const [currentPage, setCurrentPage] = useState(1);
    // TODO: use these fields for sorting
    const [ascending, setAscending] = useState(true);
    const [sortBy, setSortBy] = useState<number | undefined>(undefined);
    const [status, setStatus] = useState<number | undefined>(undefined);

    const {
        data: orders,
        isLoading,
        isFetching,
        isError,
    } = useQuery({
        queryKey: keys.orders.page(currentPage),
        queryFn: () =>
            fetchOrders(currentPage, 16, undefined, status, sortBy, ascending),
    });

    useEffect(() => {
        queryClient.invalidateQueries({ queryKey: keys.orders.all });
    }, [status, queryClient]);

    if (isLoading || isFetching) {
        return (
            <Box
                display="flex"
                justifyContent="center"
                alignItems="center"
                mt={4}
            >
                <CircularProgress />
            </Box>
        );
    }

    if ((!isFetching && (isError || !orders)) || !orders) {
        return <Alert severity="error">Failed to retrieve orders</Alert>;
    }

    return (
        <Box>
            <Typography variant="h4" gutterBottom>
                Current Active Orders
            </Typography>
            <OrdersTable
                paginatedOrdersResponse={orders}
                setCurrentPage={setCurrentPage}
                status={status}
                setStatus={setStatus}
            />
        </Box>
    );
};

export default BaristaOrdersPage;
