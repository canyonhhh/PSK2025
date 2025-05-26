import { Box, Typography, Divider, Button } from "@mui/material";
import { CartItemDto } from "../../api/requests/cart/types/CartItemDto";
import { useState } from "react";
import StripePayment from "../payment/StripePayment";

interface OrderSummaryProps {
    items: CartItemDto[];
    expectedCompletionTime: string;
    onPaymentSuccess: (orderId: string) => void;
    onPaymentError: (message: string) => void;
}

const SubtotalBox: React.FC<OrderSummaryProps> = ({ 
    items, 
    expectedCompletionTime,
    onPaymentSuccess,
    onPaymentError
}) => {
    const [showPayment, setShowPayment] = useState(false);
    
    const subtotal = items.reduce(
        (sum, item) => sum + item.productPrice * item.quantity,
        0,
    );

    if (showPayment) {
        return (
            <Box sx={{ width: '100%' }}>
                <StripePayment
                    amount={subtotal}
                    expectedCompletionTime={expectedCompletionTime}
                    onSuccess={onPaymentSuccess}
                    onError={onPaymentError}
                />
                <Button
                    variant="text"
                    onClick={() => setShowPayment(false)}
                    sx={{ mt: 1 }}
                >
                    ← Back to Order Summary
                </Button>
            </Box>
        );
    }

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
                    ${subtotal.toFixed(2)}
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
                onClick={() => setShowPayment(true)}
            >
                Proceed to Payment
            </Button>
        </Box>
    );
};

export default SubtotalBox;