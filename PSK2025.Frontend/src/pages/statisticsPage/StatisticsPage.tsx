import { Box } from "@mui/material";
import OrdersOverTimeChart from "./OrdersOverTimeTable";
import TopItems from "./TopItems";
import ProductsOverTime from "./ProductsOverTime";

const StatisticsPage = () => {
    return (
        <Box>
            <OrdersOverTimeChart />
            <TopItems />
            <ProductsOverTime />
        </Box>
    );
};

export default StatisticsPage;
