import { Alert, Box, CircularProgress, Snackbar } from "@mui/material";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { keys } from "../../api/queryKeyFactory";
import {
    deleteCartItem,
    fetchCart,
    updateCartItem,
} from "../../api/requests/cart/cartRequests";
import CartItem from "./CartItem";
import { Fragment } from "react/jsx-runtime";
import PickupTimeSelector from "../../components/datePicker/PickupTimeSelector";
import dayjs, { Dayjs } from "dayjs";
import { useState } from "react";
import SubtotalBox from "../../components/subtotalBox/SubtotalBox";
import { UpdateCartItemDto } from "../../api/requests/cart/types/UpdateCartItemDto";
import { useMutation } from "@tanstack/react-query";
import { useAuthContext } from "../../context/AuthContext";

const CartPage = () => {
    const queryClient = useQueryClient();
    const authContext = useAuthContext();
    const [date, setDate] = useState<Dayjs | null>(dayjs().add(1, "day"));
    const [toastMessage, setToastMessage] = useState<string | null>();
    
    const {
        data: cart,
        isLoading,
        isFetching,
        isError,
    } = useQuery({ queryKey: keys.activeCart, queryFn: fetchCart });

    const { mutate: updateCartItemMutation } = useMutation({
        mutationFn: (updateDto: UpdateCartItemDto) => updateCartItem(updateDto),
        onError: () => setToastMessage("Failed to update cart item"),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.activeCart }),
    });

    const { mutate: deleteCartItemMutation } = useMutation({
        mutationFn: (id: string) => deleteCartItem(id),
        onError: () => setToastMessage("Failed to update cart item"),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.activeCart }),
    });

    const handlePaymentSuccess = (orderId: string) => {
        setToastMessage(`Order created successfully! Order ID: ${orderId}`);
        queryClient.invalidateQueries({ queryKey: keys.activeCart });
        queryClient.invalidateQueries({
            queryKey: keys.ordersByUser(authContext.id ?? ""),
        });
    };

    const handlePaymentError = (message: string) => {
        setToastMessage(`Payment failed: ${message}`);
    };

    if (isLoading || isFetching) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center">
                <CircularProgress />
            </Box>
        );
    }

    if ((!isFetching && (isError || !cart)) || !cart) {
        return <Alert severity="error">Failed to retrieve Menu Items</Alert>;
    }

    const renderCartItems = () => {
        if (cart.items.length == 0) {
            return <Alert severity="info"> No items in cart</Alert>;
        }

        return cart.items.map((item, idx) => (
            <Fragment key={idx}>
                <CartItem
                    item={item}
                    onIncrease={(id: string) =>
                        updateCartItemMutation({
                            productId: id,
                            quantity: item.quantity + 1,
                        })
                    }
                    onDecrease={(id: string) =>
                        updateCartItemMutation({
                            productId: id,
                            quantity: item.quantity - 1,
                        })
                    }
                    onRemove={(id: string) => deleteCartItemMutation(id)}
                />
            </Fragment>
        ));
    };

    return (
        <Box
            display="flex"
            flexDirection={{ xs: "column", md: "row" }}
            gap={{ xs: 2, md: 4 }}
            alignItems="stretch"
            height={{ xs: "auto", md: "82vh" }}
        >
            <Box
                sx={{
                    flex: 4,
                    display: "flex",
                    flexDirection: "column",
                    height: { xs: "auto", md: "100%" },
                    overflowY: { xs: "visible", md: "auto" },
                    pr: { xs: 0, md: 1 },
                }}
            >
                {renderCartItems()}
            </Box>

            <Box
                sx={{
                    flex: 3,
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    gap: 2,
                    mt: { xs: 2, md: 0 },
                }}
            >
                <PickupTimeSelector date={date} setDate={setDate} />
                <SubtotalBox
                    items={cart.items}
                    expectedCompletionTime={date?.toISOString() || ""}
                    onPaymentSuccess={handlePaymentSuccess}
                    onPaymentError={handlePaymentError}
                />
            </Box>

            <Snackbar
                open={!!toastMessage}
                autoHideDuration={6000}
                onClose={() => setToastMessage(null)}
                message={toastMessage}
            />
        </Box>
    );
};

export default CartPage;