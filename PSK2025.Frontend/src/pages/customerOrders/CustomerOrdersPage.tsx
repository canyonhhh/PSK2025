import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { fetchOrders } from "../../api/requests/order/orderRequests";
import { keys } from "../../api/queryKeyFactory";
import { Alert, Box, CircularProgress, Typography } from "@mui/material";
import OrdersTable from "../../components/ordersTable/OrdersTable";
import { useAuthContext } from "../../context/AuthContext";

const CustomerOrdersPage = () => {
    const queryClient = useQueryClient();
    const { id: userId } = useAuthContext();
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
            fetchOrders(
                currentPage,
                16,
                userId ?? undefined,
                status,
                sortBy,
                ascending,
            ),
    });

    useEffect(() => {
        queryClient.invalidateQueries({ queryKey: keys.orders.all });
    }, [status, ascending, sortBy, status, queryClient]);

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
                Your order history
            </Typography>
            <OrdersTable
                paginatedOrdersResponse={orders}
                setCurrentPage={setCurrentPage}
                status={status}
                setStatus={setStatus}
                ascending={ascending}
                setAscending={setAscending}
                sortBy={sortBy}
                setSortBy={setSortBy}
            />
        </Box>
    );
};

export default CustomerOrdersPage;
