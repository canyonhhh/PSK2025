import Box from "@mui/material/Box";
import Modal from "@mui/material/Modal";
import Typography from "@mui/material/Typography";
import {
    Button,
    FormControl,
    InputAdornment,
    InputLabel,
    OutlinedInput,
    Snackbar,
    TextField,
    useMediaQuery,
    useTheme,
} from "@mui/material";
import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { keys } from "../../api/queryKeyFactory";
import { CreateProductDto } from "../../api/requests/product/types/CreateProductDto";
import { createProduct } from "../../api/requests/product/ProductRequests";
import noProductImage from "../../no-photos.png";

interface Props {
    open: boolean;
    handleCloseCreateModal: () => void;
}

const CreateProductModal = ({ open, handleCloseCreateModal }: Props) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down("sm"));

    const queryClient = useQueryClient();
    const [image, setImage] = useState("");
    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const [price, setPrice] = useState(0);
    const [toastMessage, setToastMessage] = useState<string | null>();

    const { mutate } = useMutation({
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: keys.allProductsAll });
            handleCloseCreateModal();
        },
        onError: () => {
            setToastMessage("Failed to update product");
        },
        mutationFn: (params: { product: CreateProductDto }) =>
            createProduct(params.product),
    });

    const handleCloseToast = () => {
        setToastMessage(null);
    };

    const onUpdate = () => {
        mutate({
            product: {
                title: name,
                price: price,
                isAvailable: true,
                description: description,
                photoUrl: image,
            },
        });
    };

    return (
        <div>
            <Modal
                open={open}
                onClose={handleCloseCreateModal}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description"
            >
                <Box
                    sx={{
                        position: "absolute",
                        top: "50%",
                        left: "50%",
                        transform: "translate(-50%, -50%)",
                        width: isMobile ? "90%" : 600,
                        maxHeight: "90vh",
                        bgcolor: "background.paper",
                        boxShadow: 24,
                        p: 3,
                        overflowY: "auto",
                        borderRadius: 2,
                    }}
                >
                    <Typography
                        id="modal-modal-title"
                        variant="h6"
                        component="h2"
                        mb={3}
                        textAlign="center"
                    >
                        Create Product
                    </Typography>

                    <Box display="flex" flexDirection="column" gap={2}>
                        <TextField
                            required
                            type="text"
                            label="Name"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                        />

                        <TextField
                            type="url"
                            label="Image URL"
                            value={image}
                            fullWidth
                            onChange={(e) => setImage(e.target.value)}
                        />

                        <Box
                            component="img"
                            src={image || noProductImage}
                            alt="Product preview"
                            sx={{
                                width: "100%",
                                height: "auto",
                                borderRadius: 1,
                                maxHeight: 300,
                                objectFit: "cover",
                            }}
                        />

                        <TextField
                            label="Description"
                            multiline
                            rows={3}
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                        />

                        <FormControl fullWidth>
                            <InputLabel htmlFor="outlined-adornment-amount">
                                Price
                            </InputLabel>
                            <OutlinedInput
                                required
                                id="outlined-adornment-amount"
                                startAdornment={
                                    <InputAdornment position="start">
                                        €
                                    </InputAdornment>
                                }
                                label="Price"
                                value={price}
                                onChange={(e) =>
                                    setPrice(Number(e.target.value))
                                }
                                type="number"
                            />
                        </FormControl>

                        <Button
                            variant="contained"
                            onClick={onUpdate}
                            fullWidth
                            sx={{ mt: 2 }}
                        >
                            Create Product
                        </Button>
                    </Box>
                </Box>
            </Modal>

            <Snackbar
                open={!!toastMessage}
                autoHideDuration={6000}
                onClose={handleCloseToast}
                message={toastMessage}
            />
        </div>
    );
};

export default CreateProductModal;
