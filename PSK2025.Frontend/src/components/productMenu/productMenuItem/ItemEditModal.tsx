import Box from "@mui/material/Box";
import Modal from "@mui/material/Modal";
import Typography from "@mui/material/Typography";
import { useState } from "react";
import {
  Button,
  FormControl,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  Snackbar,
  TextField,
} from "@mui/material";
import noProductImage from "../../../no-photos.png";
import { ProductDto } from "../../../api/types/Product";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateProduct } from "../../../api/requests/product/ProductRequests";
import { UpdateProductDto } from "../../../api/requests/product/types/UpdateProductDto";
import { keys } from "../../../api/queryKeyFactory";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 600,
  bgcolor: "background.paper",
  boxShadow: 24,
  p: 4,
};

interface Props {
  open: boolean;
  product: ProductDto;
  handleCloseEditModal: () => void;
}

const ItemEditModal = ({ open, handleCloseEditModal, product }: Props) => {
  const queryClient = useQueryClient();
  const [image, setImage] = useState(product.PhotoUrl ?? "");
  const [name, setName] = useState(product.Title ?? "");
  const [description, setDescription] = useState(product.Description ?? "");
  const [price, setPrice] = useState(product.Price);
  const [toastMessage, setToastMessage] = useState<string | null>();
  const { mutate } = useMutation({
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: keys.allProducts }),
    onError: () => {
      setToastMessage("Failed to update product");
    },
    mutationFn: (params: { product: UpdateProductDto; id: string }) =>
      updateProduct(params.product, params.id),
  });

  const handleCloseToast = () => {
    setToastMessage(null);
  };
  const onUpdate = () => {
    mutate({
      product: {
        Title: name,
        Price: price,
        IsAvailable: product.IsAvailable,
        Description: description,
        PhotoUrl: image,
      },
      id: product.Id,
    });
  };
  return (
    <div>
      <Modal
        open={open}
        onClose={handleCloseEditModal}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          <Typography
            id="modal-modal-title"
            marginBottom="3rem"
            variant="h6"
            component="h2"
          >
            Edit product
          </Typography>
          <Box display="flex" flexDirection="column" gap="1rem">
            <TextField
              required
              type="text"
              label="Name"
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
            <Box display="flex" gap="1rem">
              <Box
                component="img"
                sx={{
                  height: 400,
                  maxHeight: { xs: 233, md: 167 },
                }}
                src={image ?? noProductImage}
              />
              <TextField
                type="url"
                label="Image url"
                value={image}
                fullWidth
                onChange={(e) => setImage(e.target.value)}
              />
            </Box>
            <TextField
              label="Description"
              multiline
              value={description}
              type="text"
              onChange={(e) => setDescription(e.target.value)}
            />
            <FormControl fullWidth sx={{ m: 1 }}>
              <InputLabel htmlFor="outlined-adornment-amount">
                Amount
              </InputLabel>
              <OutlinedInput
                required
                id="outlined-adornment-amount"
                startAdornment={
                  <InputAdornment position="start">€</InputAdornment>
                }
                label="Price"
                value={price}
                onChange={(e) => setPrice(Number(e.target.value))}
                type="number"
              />
            </FormControl>
          </Box>
          <Box marginTop="2rem">
            <Button variant="contained" onClick={onUpdate}>
              Update
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

export default ItemEditModal;
