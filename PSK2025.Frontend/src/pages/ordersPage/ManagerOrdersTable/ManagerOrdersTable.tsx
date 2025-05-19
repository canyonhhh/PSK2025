import {
    Box,
    CircularProgress,
    Alert,
    Snackbar,
    Typography,
    Select,
    MenuItem,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    FormControl,
    InputLabel,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { fetchOrders } from "../../../api/requests/order/orderRequests";
import { keys } from "../../../api/queryKeyFactory";
import { PaginatedItems } from "../../../components/paginatedItemBox/PaginatedItemBox";
import { OrderDto } from "../../../api/requests/order/types/OrderDto";

const ORDER_STATUSES = [
    { value: 0, label: "Pending" },
    { value: 1, label: "Preparing" },
    { value: 2, label: "Completed" },
    { value: 3, label: "Cancelled" },
];

const ManagerOrdersTable = () => {
    const queryClient = useQueryClient();
    const [currentPage, setCurrentPage] = useState(1);
    // TODO: use these fields for sorting
    const [ascending, setAscending] = useState(true);
    const [sortBy, setSortBy] = useState<number | undefined>(undefined);
    const [status, setStatus] = useState<number | undefined>(undefined);
    const [toastMessage, setToastMessage] = useState<string | null>(null);

    const {
        data: cart,
        isLoading,
        isFetching,
        isError,
    } = useQuery({
        queryKey: keys.orders.page(currentPage),
        queryFn: () =>
            fetchOrders(currentPage, 16, undefined, status, sortBy, ascending),
    });

    const handleStatusChange = (event: any) => {
        console.log(Number(event.target.value));

        if (event.target.value !== status) {
            setStatus(
                event.target.value === "all"
                    ? undefined
                    : Number(event.target.value),
            );
        }
    };

    const renderTableRow = (order: OrderDto) => {
        return (
            <TableRow key={order.id}>
                <TableCell>{order.id}</TableCell>
                <TableCell>{order.userId}</TableCell>
                <TableCell>{order.totalPrice}€</TableCell>
                <TableCell>
                    {ORDER_STATUSES.find((s) => s.value === order.status)
                        ?.label ?? "Unknown"}
                </TableCell>
                <TableCell>
                    {order.items.map((item) => (
                        <Typography key={item.id}>
                            {item.productName} x{item.quantity}
                        </Typography>
                    ))}
                </TableCell>
                <TableCell>
                    {new Date(order.createdAt).toLocaleString()}
                </TableCell>
                <TableCell>
                    {new Date(order.expectedCompletionTime).toLocaleString()}
                </TableCell>
                <TableCell>
                    {order.completedAt
                        ? new Date(order.completedAt).toLocaleString()
                        : "To be completed"}
                </TableCell>
            </TableRow>
        );
    };

    const renderContainer = (items: React.ReactNode[]) => {
        return (
            <TableContainer component={Paper} sx={{ mt: 2 }}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>ID</TableCell>
                            <TableCell>User ID</TableCell>
                            <TableCell>Total Price</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Items</TableCell>
                            <TableCell>Created At</TableCell>
                            <TableCell>Expected To Be Completed At</TableCell>
                            <TableCell>Completed At</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>{items}</TableBody>
                </Table>
            </TableContainer>
        );
    };

    const renderOrders = () => {
        if (!cart || cart.items.length === 0) {
            return <Alert severity="info">No orders yet...</Alert>;
        }

        return (
            <PaginatedItems
                content={cart}
                containerRenderer={renderContainer}
                itemRenderer={renderTableRow}
                onPageChange={setCurrentPage}
            />
        );
        return;
    };

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

    if ((!isFetching && (isError || !cart)) || !cart) {
        return <Alert severity="error">Failed to retrieve orders</Alert>;
    }

    return (
        <>
            <Box
                display="flex"
                justifyContent="space-between"
                alignItems="center"
                mb={2}
            >
                <FormControl size="small" sx={{ minWidth: 180 }}>
                    <InputLabel>Status</InputLabel>
                    <Select
                        label="Status"
                        value={status !== undefined ? status : "all"}
                        onChange={handleStatusChange}
                    >
                        <MenuItem value="all">All</MenuItem>
                        {ORDER_STATUSES.map((s) => (
                            <MenuItem key={s.value} value={s.value}>
                                {s.label}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Box>

            {renderOrders()}

            <Snackbar
                open={!!toastMessage}
                autoHideDuration={6000}
                onClose={() => setToastMessage(null)}
                message={toastMessage}
            />
        </>
    );
};

export default ManagerOrdersTable;
