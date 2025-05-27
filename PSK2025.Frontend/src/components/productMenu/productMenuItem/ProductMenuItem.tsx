import { Box, Button, Snackbar, Typography } from "@mui/material";
import noProductImage from "../../../no-photos.png";
import { useState } from "react";
import ItemEditModal from "./ItemEditModal";
import { ProductDto } from "../../../api/types/Product";
import {
    changeProductAvailability,
    deleteProduct,
} from "../../../api/requests/product/ProductRequests";
import ConfirmationModal from "../../confirmationModal/ConfirmationModal";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { keys } from "../../../api/queryKeyFactory";
import { useAuthContext } from "../../../context/AuthContext";
import { Role } from "../../../routing/roles";
import { createCartItem } from "../../../api/requests/cart/cartRequests";

interface ShopMenuItemProps {
    product: ProductDto;
}

const ProductMenuItem = ({ product }: ShopMenuItemProps) => {
    const queryClient = useQueryClient();
    const { role } = useAuthContext();
    const [open, setOpen] = useState(false);
    const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
    const [inStock, setInStock] = useState(true);
    const handleOpenEditModal = () => setOpen(true);
    const handleCloseEditModal = () => setOpen(false);
    const [toastMessage, setToastMessage] = useState<string | null>();
    const { mutate } = useMutation({
        mutationFn: (id: string) => deleteProduct(id),
        onError: () => setToastMessage("Failed to delete product"),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.allProductsAll }),
    });

    const { mutate: changeInStockMutation } = useMutation({
        mutationFn: () => changeProductAvailability(!inStock, product.id),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.allProductsAll }),
    });

    const markItemOutOfStock = () => {
        changeInStockMutation();
        setInStock((isInStock) => !isInStock);
    };

    const { mutate: addToCartMutation } = useMutation({
        mutationFn: () =>
            createCartItem({
                productId: product.id,
                quantity: 1,
            }),
        onError: () => setToastMessage("Failed to add item to cart"),
        onSuccess: () => {
            setToastMessage(`${product.title} added to cart`);
            queryClient.invalidateQueries({ queryKey: keys.activeCart });
        },
    });

    const handleConfirmDelete = () => {
        mutate(product.id);
        setOpen(false);
    };

    const handleDeclineDelete = () => {
        setIsConfirmationOpen(false);
    };

    const renderActionButtons = () => {
        if (role === Role.CUSTOMER) {
            return (
                <Button
                    variant="outlined"
                    onClick={() => {
                        addToCartMutation();
                    }}
                >
                    Add to cart
                </Button>
            );
        } else if (role === Role.MANAGER) {
            return (
                <>
                    <Button variant="outlined" onClick={handleOpenEditModal}>
                        Edit
                    </Button>
                    <Button
                        variant="contained"
                        color="error"
                        onClick={() => setIsConfirmationOpen(true)}
                    >
                        Delete
                    </Button>
                </>
            );
        } else if (role == Role.BARISTA) {
            return (
                <Button variant="outlined" onClick={markItemOutOfStock}>
                    {inStock ? "Mark out of stock" : "Mark in stock"}
                </Button>
            );
        }
    };

    return (
        <Box
            sx={{
                borderRadius: 4,
                boxShadow: 3,
                p: 3,
                backgroundColor: "background.paper",
                margin: "auto",
            }}
        >
            <Box display="flex" flexDirection="column" alignItems="start">
                <Box maxWidth="300px" marginRight="2rem" flexShrink={0}>
                    <Box>
                        <Box
                            component="img"
                            borderRadius="0.5rem"
                            sx={{
                                height: 233,
                                width: 300,
                                maxHeight: { xs: 233, md: 167 },
                                maxWidth: { xs: 300, md: 250 },
                            }}
                            src={product.photoUrl ?? noProductImage}
                        />
                    </Box>
                </Box>
                <Box flexGrow={1}>
                    <Box marginBottom="3rem">
                        <Box marginBottom="1rem">
                            <Typography variant="h5">
                                {product.title}
                            </Typography>
                            <Typography variant="subtitle2">
                                {product.price} €
                            </Typography>
                        </Box>
                        <Typography variant="body1">
                            {product.description}
                        </Typography>
                    </Box>
                    <Box display="flex" gap="1rem">
                        {renderActionButtons()}
                    </Box>
                </Box>
            </Box>
            <ItemEditModal
                open={open}
                product={product}
                handleCloseEditModal={handleCloseEditModal}
            />
            <ConfirmationModal
                isOpen={isConfirmationOpen}
                handleConfirm={handleConfirmDelete}
                handleDecline={handleDeclineDelete}
                handleClose={handleDeclineDelete}
                confirmationText={
                    "Are you sure you want to delete this menu item?"
                }
            />
            <Snackbar
                open={!!toastMessage}
                autoHideDuration={6000}
                onClose={() => setToastMessage(null)}
                message={toastMessage}
            />
        </Box>
    );
};

export default ProductMenuItem;
