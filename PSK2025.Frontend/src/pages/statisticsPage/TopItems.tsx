import { useQuery } from "@tanstack/react-query";
import { getOrderedItems } from "../../api/requests/statistics/statisticsRequests";
import { useState } from "react";
import { keys } from "../../api/queryKeyFactory";
import {
    Box,
    CircularProgress,
    Typography,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
} from "@mui/material";

const TopItems = () => {
    const [ascending, setAscending] = useState<"ascending" | "descending">(
        "ascending",
    );

    const {
        data: orderedProducts,
        isError,
        isLoading,
    } = useQuery({
        queryFn: () => getOrderedItems(10, ascending),
        queryKey:
            ascending === "ascending"
                ? keys.topItems.top
                : keys.topItems.bottom,
    });

    return (
        <Box mt={4} mx="auto" maxWidth="800px">
            <Typography variant="h5" gutterBottom>
                Top 10 Ordered Items
            </Typography>

            <FormControl sx={{ marginBottom: 8 }} fullWidth margin="normal">
                <InputLabel id="direction-select-label">Order</InputLabel>
                <Select
                    labelId="direction-select-label"
                    value={ascending}
                    label="Order"
                    onChange={(e) =>
                        setAscending(
                            e.target.value as "ascending" | "descending",
                        )
                    }
                >
                    <MenuItem value="ascending">Ascending</MenuItem>
                    <MenuItem value="descending">Descending</MenuItem>
                </Select>
            </FormControl>

            {isLoading ? (
                <Box display="flex" justifyContent="center" mt={4}>
                    <CircularProgress />
                </Box>
            ) : isError || !orderedProducts ? (
                <Typography color="error" align="center" mt={4}>
                    Failed to load data.
                </Typography>
            ) : (
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>#</TableCell>
                                <TableCell>Product Name</TableCell>
                                <TableCell align="right">
                                    Total Quantity
                                </TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {orderedProducts.map((item, index) => (
                                <TableRow
                                    key={item.productId}
                                    sx={{
                                        backgroundColor:
                                            index === 0
                                                ? "rgba(25, 118, 210, 0.15)" // Highlight top 3
                                                : index % 2 === 0
                                                  ? "rgba(0, 0, 0, 0.02)" // Even rows
                                                  : "transparent", // Odd rows
                                    }}
                                >
                                    <TableCell>{index + 1}</TableCell>
                                    <TableCell>{item.productName}</TableCell>
                                    <TableCell align="right">
                                        {item.totalQuantity}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}
        </Box>
    );
};

export default TopItems;
