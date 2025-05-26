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
import {
    ASCENDING_OPTIONS,
    ORDER_STATUSES,
    SORT_BY_OPTIONS,
} from "./OrderTable.types";
import { OrdersTableRow } from "./OrdersTableRow";

interface OrdersTableProps {
    paginatedOrdersResponse: PaginatedResponse<OrderDto>;
    setCurrentPage: (page: number) => void;
    status: number | undefined;
    setStatus: (status: number | undefined) => void;
    ascending: boolean;
    setAscending: (value: boolean) => void;
    sortBy: number | undefined;
    setSortBy: (value: number | undefined) => void;
    canChangeStatus?: boolean;
    doNotRenderFilters?: boolean;
}

const OrdersTable = ({
    paginatedOrdersResponse,
    setCurrentPage,
    status,
    setStatus,
    ascending,
    setAscending,
    sortBy,
    setSortBy,
    canChangeStatus,
    doNotRenderFilters,
}: OrdersTableProps) => {
    const handleStatusChange = (event: any) => {
        if (event.target.value !== status) {
            setStatus(
                event.target.value === "all"
                    ? undefined
                    : Number(event.target.value),
            );
        }
    };

    const handleSortByChange = (event: any) => {
        if (event.target.value !== sortBy) {
            setSortBy(
                event.target.value === "none"
                    ? undefined
                    : Number(event.target.value),
            );
        }
    };

    const handleAscendingChange = (event: any) => {
        if (event.target.value !== ascending) {
            setAscending(Number(event.target.value) === 0 ? true : false);
        }
    };

    const renderOrdersTableRow = (order: OrderDto) => {
        return (
            <OrdersTableRow order={order} canChangeSatus={canChangeStatus} />
        );
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

    const renderFilters = () => {
        if (doNotRenderFilters) {
            return null;
        }
        return (
            <Box
                display="flex"
                justifyContent="start"
                flexDirection="column"
                gap={1}
                m={2}
            >
                <FormControl size="small" sx={{ minWidth: 180, maxWidth: 300 }}>
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
                <FormControl size="small" sx={{ minWidth: 180, maxWidth: 300 }}>
                    <InputLabel>Sort by</InputLabel>
                    <Select
                        label="Sort by"
                        value={sortBy !== undefined ? sortBy : "none"}
                        onChange={handleSortByChange}
                    >
                        <MenuItem value="none">None</MenuItem>
                        {SORT_BY_OPTIONS.map((s) => (
                            <MenuItem key={s.value} value={s.value}>
                                {s.label}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
                <FormControl size="small" sx={{ minWidth: 180, maxWidth: 300 }}>
                    <InputLabel>Direction</InputLabel>
                    <Select
                        label="Direction"
                        value={ascending ? 0 : 1}
                        onChange={handleAscendingChange}
                    >
                        {ASCENDING_OPTIONS.map((s) => (
                            <MenuItem key={s.value} value={s.value}>
                                {s.label}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Box>
        );
    };

    return (
        <>
            {renderFilters()}
            {renderOrders()}
        </>
    );
};

export default OrdersTable;
