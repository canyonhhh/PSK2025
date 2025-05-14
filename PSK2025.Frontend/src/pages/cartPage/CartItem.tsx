import {
    Button,
    Card,
    CardActions,
    CardContent,
    CardMedia,
    IconButton,
    InputLabel,
    OutlinedInput,
    Typography,
} from "@mui/material";
import { CartItemDto } from "../../api/requests/cart/types/CartItemDto";
import { AddCircle, RemoveCircle } from "@mui/icons-material";

interface CartItemProps {
    cartItem: CartItemDto;
}

const CartItem = ({
    cartItem: { quantity, price, productName, imageUrl, description },
}: CartItemProps) => {
    return (
        <Card sx={{ maxWidth: 345 }}>
            <CardMedia
                sx={{ height: 140 }}
                image={imageUrl}
                title="green iguana"
            />
            <CardContent>
                <Typography gutterBottom variant="h5" component="div">
                    {productName} - <strong>{price}</strong>
                </Typography>
                <Typography gutterBottom variant="h5" component="div">
                    {productName}
                </Typography>
                <Typography variant="body2" sx={{ color: "text.secondary" }}>
                    {description}
                </Typography>
            </CardContent>
            <CardActions>
                <IconButton>
                    <AddCircle />
                </IconButton>
                <InputLabel htmlFor="amount">Amount</InputLabel>
                <OutlinedInput id="amount" value={quantity} type="number" />
                <IconButton>
                    <RemoveCircle />
                </IconButton>
                <Button size="small"></Button>
                <Button size="small">Share</Button>
            </CardActions>
        </Card>
    );
};

export default CartItem;
