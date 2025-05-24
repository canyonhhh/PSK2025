import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import {
    IconButton,
    Collapse,
    TableRow,
    TableCell,
    Box,
    Typography,
    Table,
    TableHead,
    TableBody,
} from "@mui/material";
import React, { useState } from "react";
import { OrderDto } from "../../api/requests/order/types/OrderDto";
import { ORDER_STATUSES } from "./OrderTable.types";

interface OrderTableRowProps {
    order: OrderDto;
}

export const OrdersTableRow = ({ order }: OrderTableRowProps) => {
    const [open, setOpen] = useState(false);

    return (
        <React.Fragment key={order.id}>
            <TableRow>
                <TableCell>
                    <IconButton
                        aria-label="expand row"
                        size="small"
                        onClick={() => setOpen(!open)}
                    >
                        {open ? (
                            <KeyboardArrowUpIcon />
                        ) : (
                            <KeyboardArrowDownIcon />
                        )}
                    </IconButton>
                </TableCell>
                <TableCell>{order.id}</TableCell>
                <TableCell>{order.userId}</TableCell>
                <TableCell>{order.totalPrice}€</TableCell>
                <TableCell>
                    {ORDER_STATUSES.find((s) => s.value === order.status)
                        ?.label ?? "Unknown"}
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

            <TableRow>
                <TableCell
                    style={{ paddingBottom: 0, paddingTop: 0 }}
                    colSpan={8}
                >
                    <Collapse in={open} timeout="auto" unmountOnExit>
                        <Box sx={{ margin: 1 }}>
                            <Typography
                                variant="subtitle1"
                                gutterBottom
                                component="div"
                            >
                                Order Items
                            </Typography>
                            <Table size="small" aria-label="order-items">
                                <TableHead>
                                    <TableRow>
                                        <TableCell>Product</TableCell>
                                        <TableCell>Quantity</TableCell>
                                        <TableCell>Unit Price</TableCell>
                                        <TableCell>Total</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {order.items.map((item) => (
                                        <TableRow key={item.id}>
                                            <TableCell>
                                                {item.productName}
                                            </TableCell>
                                            <TableCell>
                                                {item.quantity}
                                            </TableCell>
                                            <TableCell>
                                                {item.productPrice}€
                                            </TableCell>
                                            <TableCell>
                                                {(
                                                    item.productPrice *
                                                    item.quantity
                                                ).toFixed(2)}
                                                €
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </Box>
                    </Collapse>
                </TableCell>
            </TableRow>
        </React.Fragment>
    );
};
