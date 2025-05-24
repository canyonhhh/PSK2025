import { Box, Typography, Divider, Button } from "@mui/material";
import { CartItemDto } from "../../api/requests/cart/types/CartItemDto";

interface OrderSummaryProps {
    items: CartItemDto[];
    onOrderCreate: () => void;
}

const SubtotalBox: React.FC<OrderSummaryProps> = ({ items, onOrderCreate }) => {
    const subtotal = items.reduce(
        (sum, item) => sum + item.productPrice * item.quantity,
        0,
    );
    return (
        <Box
            sx={{
                borderRadius: 2,
                boxShadow: 2,
                p: 3,
                backgroundColor: "background.paper",
                mx: "auto",
            }}
        >
            <Typography variant="h6" gutterBottom>
                Order Summary
            </Typography>

            <Divider sx={{ my: 2 }} />

            <Box display="flex" justifyContent="space-between" mb={2}>
                <Typography fontWeight="bold">Total</Typography>
                <Typography fontWeight="bold">
                    {subtotal.toFixed(2)} €
                </Typography>
            </Box>

            <Button
                variant="contained"
                fullWidth
                sx={{
                    backgroundColor: "#212121",
                    borderRadius: 2,
                    textTransform: "none",
                    fontWeight: 500,
                    fontSize: "1rem",
                    "&:hover": {
                        backgroundColor: "#000",
                    },
                }}
                onClick={onOrderCreate}
            >
                Proceed to Payment
            </Button>
        </Box>
    );
};

export default SubtotalBox;
