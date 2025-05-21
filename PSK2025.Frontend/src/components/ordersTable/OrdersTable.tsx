import {
    Box,
    Alert,
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
import { PaginatedResponse } from "../../api/types/PaginatedResposeDto";
import { OrderDto } from "../../api/requests/order/types/OrderDto";
import { PaginatedItems } from "../paginatedItemBox/PaginatedItemBox";
import { ORDER_STATUSES } from "./OrderTable.types";
import { OrdersTableRow } from "./OrdersTableRow";

interface OrdersTableProps {
    paginatedOrdersResponse: PaginatedResponse<OrderDto>;
    setCurrentPage: (page: number) => void;
    status: number | undefined;
    setStatus: (status: number | undefined) => void;
}

const OrdersTable = ({
    paginatedOrdersResponse,
    setCurrentPage,
    status,
    setStatus,
}: OrdersTableProps) => {
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

    const renderOrdersTableRow = (order: OrderDto) => {
        return <OrdersTableRow order={order} />;
    };

    const renderContainer = (items: React.ReactNode[]) => {
        return (
            <TableContainer component={Paper} sx={{ mt: 2 }}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell />
                            <TableCell>ID</TableCell>
                            <TableCell>User ID</TableCell>
                            <TableCell>Total Price</TableCell>
                            <TableCell>Status</TableCell>
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
        if (
            !paginatedOrdersResponse ||
            paginatedOrdersResponse.items.length === 0
        ) {
            return <Alert severity="info">No orders yet...</Alert>;
        }

        return (
            <PaginatedItems
                content={paginatedOrdersResponse}
                containerRenderer={renderContainer}
                itemRenderer={renderOrdersTableRow}
                onPageChange={setCurrentPage}
            />
        );
        return;
    };

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
        </>
    );
};

export default OrdersTable;
