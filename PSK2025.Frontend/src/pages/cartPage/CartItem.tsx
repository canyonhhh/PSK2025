import React from "react";
import {
    Box,
    Typography,
    IconButton,
    Button,
    Card,
    CardMedia,
} from "@mui/material";
import RemoveIcon from "@mui/icons-material/Remove";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import { CartItemDto } from "../../api/requests/cart/types/CartItemDto";

interface CartItemCardProps {
    item: CartItemDto;
    onIncrease: (id: string) => void;
    onDecrease: (id: string) => void;
    onRemove: (id: string) => void;
}

const CartItem: React.FC<CartItemCardProps> = ({
    item,
    onIncrease,
    onDecrease,
    onRemove,
}) => {
    return (
        <Card
            sx={{
                display: "flex",
                alignItems: "center",
                p: 2,
                borderRadius: 2,
                boxShadow: 1,
                mb: 2,
            }}
        >
            <CardMedia
                component="img"
                image={item.imageUrl || "https://via.placeholder.com/80"}
                alt={item.productName || "Product"}
                sx={{
                    width: 80,
                    height: 80,
                    borderRadius: 1,
                    objectFit: "cover",
                    mr: 2,
                }}
            />

            <Box flexGrow={1}>
                <Typography fontWeight="bold">
                    {item.productName || "Name"}
                </Typography>
                <Typography color="text.secondary">
                    {item.productPrice.toFixed(2)} €
                </Typography>

                <Box mt={1} display="flex" alignItems="center" gap={1}>
                    <IconButton
                        size="small"
                        onClick={() => onDecrease(item.productId)}
                        disabled={item.quantity <= 1}
                        sx={{ border: "1px solid #ccc" }}
                    >
                        <RemoveIcon fontSize="small" />
                    </IconButton>
                    <Typography>{item.quantity}</Typography>
                    <IconButton
                        size="small"
                        onClick={() => onIncrease(item.productId)}
                        sx={{ border: "1px solid #ccc" }}
                    >
                        <AddIcon fontSize="small" />
                    </IconButton>
                    <Button
                        variant="contained"
                        color="inherit"
                        size="small"
                        onClick={() => onRemove(item.productId)}
                        startIcon={<DeleteIcon />}
                        sx={{
                            backgroundColor: "#212121",
                            color: "#fff",
                            textTransform: "none",
                            borderRadius: 2,
                            ml: 1,
                            "&:hover": {
                                backgroundColor: "#000",
                            },
                        }}
                    >
                        Remove
                    </Button>
                </Box>
            </Box>
        </Card>
    );
};

export default CartItem;
