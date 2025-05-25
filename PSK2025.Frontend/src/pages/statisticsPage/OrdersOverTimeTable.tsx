import { useQuery } from "@tanstack/react-query";
import { getOrdersOverTime } from "../../api/requests/statistics/statisticsRequests";
import { keys } from "../../api/queryKeyFactory";
import { useState } from "react";
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
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LineChart } from "@mui/x-charts";

const OrdersOverTimeChart = () => {
    const [ordersOverTimeFrom, setOrdersOverTimeFrom] = useState<Dayjs>(
        dayjs().subtract(30, "days"),
    );
    const [ordersOverTimeTo, setOrdersOverTimeTo] = useState<Dayjs>(dayjs());
    const [ordersOverTimeGrouping, setOrdersOverTimeGrouping] = useState(0);

    const {
        data: ordersOverTime,
        isError: isOrdersOverTimeError,
        isLoading: isOrdersOverTimeLoading,
    } = useQuery({
        queryFn: () =>
            getOrdersOverTime(
                ordersOverTimeFrom.toISOString(),
                ordersOverTimeTo.toISOString(),
                ordersOverTimeGrouping,
            ),
        queryKey: keys.ordersOverTime.byParams(
            ordersOverTimeFrom.toISOString(),
            ordersOverTimeTo.toISOString(),
            ordersOverTimeGrouping,
        ),
    });

    return (
        <Box mt={4} mx="auto" maxWidth="900px">
            <Typography variant="h5" gutterBottom>
                Orders Over Time
            </Typography>

            {/* Date Pickers */}
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
                        value={ordersOverTimeTo}
                        onChange={(newValue) =>
                            newValue && setOrdersOverTimeTo(newValue)
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
                            value={ordersOverTimeGrouping}
                            label="Grouping"
                            onChange={(e) =>
                                setOrdersOverTimeGrouping(
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
            </Grid>

            {isOrdersOverTimeLoading ? (
                <Box display="flex" justifyContent="center" mt={4}>
                    <CircularProgress />
                </Box>
            ) : isOrdersOverTimeError || !ordersOverTime ? (
                <Typography color="error" align="center" mt={4}>
                    Failed to load data.
                </Typography>
            ) : (
                <LineChart
                    xAxis={[
                        {
                            data: ordersOverTime.map((point) =>
                                dayjs(point.period).format("YYYY-MM-DD"),
                            ),
                            scaleType: "band",
                            label: "Date",
                        },
                    ]}
                    series={[
                        {
                            data: ordersOverTime.map((point) => point.count),
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

export default OrdersOverTimeChart;
