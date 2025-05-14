import { Alert, Box, CircularProgress } from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { keys } from "../../api/queryKeyFactory";
import { fetchCart } from "../../api/requests/cart/cartRequests";
import CartItem from "./CartItem";
import { Fragment } from "react/jsx-runtime";

const CartPage = () => {
    const {
        data: cart,
        isLoading,
        isFetching,
        isError,
    } = useQuery({ queryKey: keys.activeCart, queryFn: fetchCart });
    if (isLoading || isFetching) {
        return <CircularProgress />;
    }

    if (!isFetching || isError || !cart) {
        return <Alert severity="error">Failed to retrieve Menu Items</Alert>;
    }
    return (
        <Box display="flex" gap="4">
            <Box flex="4">
                {cart.items.map((item, idx) => (
                    <Fragment key={idx}>
                        <CartItem cartItem={item} />
                    </Fragment>
                ))}
            </Box>
            <Box flex="3">This is text also</Box>
        </Box>
    );
};

export default CartPage;
