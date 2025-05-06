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
import noProductImage from "../../no-photos.png";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { keys } from "../../api/queryKeyFactory";
import { CreateProductDto } from "../../api/requests/product/types/CreateProductDto";
import { createProduct } from "../../api/requests/product/ProductRequests";

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
  handleCloseCreateModal: () => void;
}

const CreateProductModal = ({ open, handleCloseCreateModal }: Props) => {
  const queryClient = useQueryClient();
  const [image, setImage] = useState("");
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [price, setPrice] = useState(0);
  const [toastMessage, setToastMessage] = useState<string | null>();
  const { mutate } = useMutation({
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: keys.allProducts });
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
            <TextField
              type="url"
              label="Image url"
              value={image}
              fullWidth
              onChange={(e) => setImage(e.target.value)}
            />
            <Box
              component="img"
              sx={{
                height: 400,
                maxHeight: { xs: 233, md: 167 },
              }}
              src={image ?? noProductImage}
            />
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
