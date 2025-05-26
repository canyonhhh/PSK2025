import { useQuery } from "@tanstack/react-query";
import { getItemOverTime } from "../../api/requests/statistics/statisticsRequests";
import { keys } from "../../api/queryKeyFactory";
import { useState, useMemo } from "react";
import dayjs, { Dayjs } from "dayjs";
import {
    Box,
    CircularProgress,
    Typography,
    Grid,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    TextField,
    Autocomplete,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LineChart } from "@mui/x-charts";
import debounce from "lodash.debounce";
import { fetchAllProducts } from "../../api/requests/product/ProductRequests";

const ProductOverTime = () => {
    const [ordersOverTimeFrom, setOrdersOverTimeFrom] = useState<Dayjs>(
        dayjs().subtract(30, "days"),
    );
    const [productsOverTimeTo, setProductsOverTimeTo] =
        useState<Dayjs>(dayjs());
    const [productsOverTimeGrouping, setProductsOverTimeGrouping] = useState(0);
    const [searchTerm, setSearchTerm] = useState("");
    const [productId, setProductId] = useState<string | null>(null);

    const { data: products, isLoading: isProductsLoading } = useQuery({
        queryKey: ["products", searchTerm],
        queryFn: () => fetchAllProducts(1, 10, searchTerm),
    });

    const {
        data: productsOverTime,
        isError,
        isLoading,
    } = useQuery({
        queryKey: productId
            ? keys.ordersOverTime
                  .byParams(
                      ordersOverTimeFrom.toISOString(),
                      productsOverTimeTo.toISOString(),
                      productsOverTimeGrouping,
                  )
                  .concat(productId)
            : ["no-product-selected"],
        queryFn: () =>
            getItemOverTime(
                productId!,
                ordersOverTimeFrom.toISOString(),
                productsOverTimeTo.toISOString(),
                productsOverTimeGrouping,
            ),
        enabled: !!productId,
    });

    // debounce search input
    const debouncedSetSearch = useMemo(() => debounce(setSearchTerm, 300), []);

    return (
        <Box mt={4} mx="auto" maxWidth="900px">
            <Typography variant="h5" gutterBottom>
                Product Orders Over Time
            </Typography>

            <Grid container spacing={2} mb={3}>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <DatePicker
                        label="From Date"
                        value={ordersOverTimeFrom}
                        onChange={(newValue) =>
                            newValue && setOrdersOverTimeFrom(newValue)
                        }
                        slotProps={{ textField: { fullWidth: true } }}
                    />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <DatePicker
                        label="To Date"
                        value={productsOverTimeTo}
                        onChange={(newValue) =>
                            newValue && setProductsOverTimeTo(newValue)
                        }
                        slotProps={{ textField: { fullWidth: true } }}
                    />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <FormControl fullWidth>
                        <InputLabel id="grouping-select-label">
                            Grouping
                        </InputLabel>
                        <Select
                            labelId="grouping-select-label"
                            value={productsOverTimeGrouping}
                            label="Grouping"
                            onChange={(e) =>
                                setProductsOverTimeGrouping(
                                    Number(e.target.value),
                                )
                            }
                        >
                            <MenuItem value={0}>Daily</MenuItem>
                            <MenuItem value={1}>Weekly</MenuItem>
                            <MenuItem value={2}>Monthly</MenuItem>
                        </Select>
                    </FormControl>
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <Autocomplete
                        fullWidth
                        options={products?.items || []}
                        loading={isProductsLoading}
                        getOptionLabel={(option) => option.title}
                        onInputChange={(_, value) => debouncedSetSearch(value)}
                        onChange={(_, value) => setProductId(value?.id ?? null)}
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                label="Search Product"
                                InputProps={{
                                    ...params.InputProps,
                                    endAdornment: (
                                        <>
                                            {isProductsLoading ? (
                                                <CircularProgress size={20} />
                                            ) : null}
                                            {params.InputProps.endAdornment}
                                        </>
                                    ),
                                }}
                            />
                        )}
                    />
                </Grid>
            </Grid>

            {isLoading ? (
                <Box display="flex" justifyContent="center" mt={4}>
                    <CircularProgress />
                </Box>
            ) : isError || !productsOverTime ? (
                productId && (
                    <Typography color="error" align="center" mt={4}>
                        Failed to load data.
                    </Typography>
                )
            ) : (
                <LineChart
                    xAxis={[
                        {
                            data: productsOverTime.map((point) =>
                                dayjs(point.period).format("YYYY-MM-DD"),
                            ),
                            scaleType: "band",
                            label: "Date",
                        },
                    ]}
                    series={[
                        {
                            data: productsOverTime.map((point) => point.count),
                            label: "Orders",
                            color: "#1976d2",
                        },
                    ]}
                    height={400}
                />
            )}
        </Box>
    );
};

export default ProductOverTime;
