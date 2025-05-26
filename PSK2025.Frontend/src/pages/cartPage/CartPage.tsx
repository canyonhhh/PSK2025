import { Alert, Box, CircularProgress, Snackbar } from "@mui/material";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
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
import { createOrder } from "../../api/requests/order/orderRequests";
import { CreateOrderDto } from "../../api/requests/order/types/CreateOrderDto";
import { useAuthContext } from "../../context/AuthContext";
import { getAppStatus } from "../../api/requests/application/applicationRequests";
import { keys } from "../../api/queryKeyFactory";

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

    let { data: statusResponse, isFetching: isAreOrdersStopedFetching } =
        useQuery({
            queryKey: keys.appStatus,
            queryFn: getAppStatus,
        });

    const { mutate: updateCartItemMutation } = useMutation({
        mutationFn: (updateDto: UpdateCartItemDto) => updateCartItem(updateDto),
        onError: () => setToastMessage("Failed to update cart item"),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.activeCart }),
    });

    const { mutate: createOrderMutation } = useMutation({
        mutationFn: (createOrderDto: CreateOrderDto) =>
            createOrder(createOrderDto),
        onError: () => {
            queryClient.invalidateQueries({ queryKey: keys.appStatus });
            setToastMessage("Failed to create order");
        },
        onSuccess: () => {
            setToastMessage("Order created!");
            queryClient.invalidateQueries({
                queryKey: keys.ordersByUser(authContext.token ?? ""),
            });
            queryClient.invalidateQueries({
                queryKey: keys.activeCart,
            });
        },
    });

    const { mutate: deleteCartItemMutation } = useMutation({
        mutationFn: (id: string) => deleteCartItem(id),
        onError: () => setToastMessage("Failed to update cart item"),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.activeCart }),
    });

    if (isLoading || isFetching || isAreOrdersStopedFetching) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center">
                <CircularProgress />
            </Box>
        );
    }

    if ((!isFetching && (isError || !cart)) || !cart) {
        return <Alert severity="error">Failed to retrieve Menu Items</Alert>;
    }

    if (statusResponse?.orderingPaused) {
        return (
            <Alert severity="warning">Ordering is stoped at the moment</Alert>
        );
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
                    onOrderCreate={() =>
                        createOrderMutation({
                            expectedCompletionTime: date?.toISOString() || "",
                        })
                    }
                    items={cart.items}
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
