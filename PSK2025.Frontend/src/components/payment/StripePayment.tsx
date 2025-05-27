import React, { useEffect, useState } from "react";
import { loadStripe } from "@stripe/stripe-js";
import {
    Elements,
    CardElement,
    useStripe,
    useElements,
} from "@stripe/react-stripe-js";
import {
    Box,
    Button,
    Typography,
    CircularProgress,
    Alert,
    Paper,
} from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
    createPaymentIntent,
    confirmPayment,
} from "../../api/requests/payment/paymentRequests";
import { keys } from "../../api/queryKeyFactory";
import { useAuthContext } from "../../context/AuthContext";

const stripePromise = loadStripe(
    "pk_test_51QUWBYGUMof1EohPLwH4KQZj1wbLTJf8o8kzruKACLFgDeiH7SLxAAGN8dYUxP0Ku85bqSow0hEHGthPuPATzfCP00sDDvnD8L",
);

const cardElementOptions = {
    style: {
        base: {
            fontSize: "16px",
            color: "#424770",
            "::placeholder": {
                color: "#aab7c4",
            },
        },
    },
};

interface PaymentFormProps {
    amount: number;
    expectedCompletionTime: string;
    onSuccess: (orderId: string) => void;
    onError: (message: string) => void;
}

const PaymentForm: React.FC<PaymentFormProps> = ({
    amount,
    expectedCompletionTime,
    onSuccess,
    onError,
}) => {
    const stripe = useStripe();
    const elements = useElements();
    const queryClient = useQueryClient();
    const authContext = useAuthContext();

    const [clientSecret, setClientSecret] = useState("");
    const [paymentIntentId, setPaymentIntentId] = useState("");
    const [errorMessage, setErrorMessage] = useState("");
    const [isProcessing, setIsProcessing] = useState(false);

    const createIntent = useMutation({
        mutationFn: createPaymentIntent,
        onSuccess: (data) => {
            setClientSecret(data.clientSecret);
            setPaymentIntentId(data.paymentIntentId);
        },
        onError: () => onError("Failed to initialize payment"),
    });

    const confirmPaymentMutation = useMutation({
        mutationFn: confirmPayment,
        onSuccess: (data) => {
            if (data.success && data.orderId) {
                queryClient.invalidateQueries({ queryKey: keys.activeCart });
                queryClient.invalidateQueries({
                    queryKey: keys.ordersByUser(authContext.id ?? ""),
                });
                onSuccess(data.orderId);
            } else {
                onError(data.message || "Payment failed");
            }
        },
        onError: () => onError("Payment confirmation failed"),
    });

    useEffect(() => {
        if (
            !clientSecret &&
            amount > 0 &&
            expectedCompletionTime &&
            !createIntent.isPending &&
            !createIntent.isSuccess
        ) {
            createIntent.mutate({ currency: "usd", expectedCompletionTime });
        }
    }, [amount, expectedCompletionTime, createIntent, clientSecret]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!stripe || !elements || !clientSecret) return;

        setIsProcessing(true);
        setErrorMessage("");

        const card = elements.getElement(CardElement);
        if (!card) {
            setErrorMessage("Card information is missing.");
            setIsProcessing(false);
            return;
        }

        try {
            const { error, paymentIntent } = await stripe.confirmCardPayment(
                clientSecret,
                {
                    payment_method: { card },
                },
            );

            if (error) {
                setErrorMessage(error.message || "Payment failed.");
            } else if (paymentIntent?.status === "succeeded") {
                confirmPaymentMutation.mutate({ paymentIntentId });
            }
        } catch {
            setErrorMessage("Unexpected error occurred.");
        } finally {
            setIsProcessing(false);
        }
    };

    if (createIntent.isPending) {
        return (
            <Box
                display="flex"
                justifyContent="center"
                alignItems="center"
                p={4}
            >
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Paper sx={{ p: 3, mt: 2 }}>
            <Typography variant="h6" gutterBottom>
                Payment Details
            </Typography>
            <Typography variant="body1" gutterBottom>
                Total: ${amount.toFixed(2)}
            </Typography>

            <form onSubmit={handleSubmit}>
                <Box
                    sx={{
                        p: 2,
                        border: "1px solid #ccc",
                        borderRadius: 2,
                        mb: 2,
                    }}
                >
                    <CardElement options={cardElementOptions} />
                </Box>

                {errorMessage && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {errorMessage}
                    </Alert>
                )}

                <Button
                    type="submit"
                    variant="contained"
                    fullWidth
                    disabled={
                        !stripe ||
                        isProcessing ||
                        confirmPaymentMutation.isPending
                    }
                    sx={{ py: 1.5, fontSize: "1rem", borderRadius: 2 }}
                >
                    {isProcessing || confirmPaymentMutation.isPending ? (
                        <CircularProgress size={24} color="inherit" />
                    ) : (
                        `Pay $${amount.toFixed(2)}`
                    )}
                </Button>
            </form>
        </Paper>
    );
};

interface StripePaymentProps {
    amount: number;
    expectedCompletionTime: string;
    onSuccess: (orderId: string) => void;
    onError: (message: string) => void;
}

const StripePayment: React.FC<StripePaymentProps> = (props) => {
    return (
        <Elements stripe={stripePromise}>
            <PaymentForm {...props} />
        </Elements>
    );
};

export default StripePayment;
