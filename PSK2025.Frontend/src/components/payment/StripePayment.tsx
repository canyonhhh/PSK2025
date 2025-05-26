import React, { useState, useEffect } from 'react';
import { loadStripe } from '@stripe/stripe-js';
import {
    Elements,
    CardElement,
    useStripe,
    useElements
} from '@stripe/react-stripe-js';
import { 
    Box, 
    Button, 
    Typography, 
    Alert, 
    CircularProgress,
    Paper
} from '@mui/material';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { createPaymentIntent, confirmPayment } from '../../api/requests/payment/paymentRequests';
import { keys } from '../../api/queryKeyFactory';
import { useAuthContext } from '../../context/AuthContext';

const stripePromise = loadStripe('pk_test_51QUWBYGUMof1EohPLwH4KQZj1wbLTJf8o8kzruKACLFgDeiH7SLxAAGN8dYUxP0Ku85bqSow0hEHGthPuPATzfCP00sDDvnD8L');

const cardElementOptions = {
    style: {
        base: {
            fontSize: '16px',
            color: '#424770',
            '::placeholder': {
                color: '#aab7c4',
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
    onError
}) => {
    const stripe = useStripe();
    const elements = useElements();
    const queryClient = useQueryClient();
    const authContext = useAuthContext();
    
    const [clientSecret, setClientSecret] = useState<string>('');
    const [paymentIntentId, setPaymentIntentId] = useState<string>('');
    const [isProcessing, setIsProcessing] = useState(false);
    const [errorMessage, setErrorMessage] = useState<string>('');

    const createIntentMutation = useMutation({
        mutationFn: createPaymentIntent,
        onSuccess: (data) => {
            setClientSecret(data.clientSecret);
            setPaymentIntentId(data.paymentIntentId);
        },
        onError: () => {
            onError('Failed to initialize payment');
        }
    });

    const confirmPaymentMutation = useMutation({
        mutationFn: confirmPayment,
        onSuccess: (data) => {
            if (data.success && data.orderId) {
                onSuccess(data.orderId);
                queryClient.invalidateQueries({ queryKey: keys.activeCart });
                queryClient.invalidateQueries({ 
                    queryKey: keys.ordersByUser(authContext.id ?? '') 
                });
            } else {
                onError(data.message || 'Payment failed');
            }
        },
        onError: () => {
            onError('Payment confirmation failed');
        }
    });

    useEffect(() => {
        if (amount > 0 && expectedCompletionTime) {
            createIntentMutation.mutate({
                currency: 'usd',
                expectedCompletionTime
            });
        }
    }, [amount, expectedCompletionTime]);

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        
        if (!stripe || !elements || !clientSecret) {
            return;
        }

        setIsProcessing(true);
        setErrorMessage('');

        const cardElement = elements.getElement(CardElement);
        if (!cardElement) {
            setErrorMessage('Card element not found');
            setIsProcessing(false);
            return;
        }

        try {
            const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
                payment_method: {
                    card: cardElement,
                }
            });

            if (error) {
                setErrorMessage(error.message || 'Payment failed');
            } else if (paymentIntent && paymentIntent.status === 'succeeded') {
                // Confirm payment with backend
                confirmPaymentMutation.mutate({
                    paymentIntentId: paymentIntentId
                });
            }
        } catch (err) {
            setErrorMessage('An unexpected error occurred');
        } finally {
            setIsProcessing(false);
        }
    };

    if (createIntentMutation.isPending) {
        return (
            <Box display="flex" justifyContent="center" p={2}>
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Paper sx={{ p: 3, mt: 2 }}>
            <Typography variant="h6" gutterBottom>
                Payment Details
            </Typography>
            <Typography variant="body2" color="text.secondary" gutterBottom>
                Total: ${amount.toFixed(2)}
            </Typography>
            
            <form onSubmit={handleSubmit}>
                <Box sx={{ my: 2, p: 2, border: '1px solid #ddd', borderRadius: 1 }}>
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
                    disabled={!stripe || isProcessing || confirmPaymentMutation.isPending}
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