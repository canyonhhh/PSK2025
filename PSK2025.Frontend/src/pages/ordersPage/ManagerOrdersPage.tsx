import { Box, Typography } from "@mui/material";
import ManagerOrdersTable from "./ManagerOrdersTable/ManagerOrdersTable";

const ManagerOrdersPage = () => {
    return (
        <Box>
            <Typography variant="h4" gutterBottom>
                Orders Table
            </Typography>
            <ManagerOrdersTable />
        </Box>
    );
};

export default ManagerOrdersPage;
