import { useQuery } from "@tanstack/react-query";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Grid,
  IconButton,
  InputBase,
  Paper,
} from "@mui/material";
import { ProductDto } from "../../api/types/Product";
import { keys } from "../../api/queryKeyFactory";
import { fetchAllProducts } from "../../api/requests/product/ProductRequests";
import ProductMenuItem from "./productMenuItem/ProductMenuItem";
import { useState } from "react";
import SearchIcon from "@mui/icons-material/Search";
import { useAuthContext } from "../../context/AuthContext";
import { Role } from "../../routing/roles";
import CreateProductModal from "./CreateProductModal";

interface ManagerMenuProps {
  columnCount: number;
}

export function ProductMenu({ columnCount }: ManagerMenuProps) {
  const { role } = useAuthContext();
  let { data: rows, isFetching } = useQuery({
    queryKey: keys.allProducts,
    queryFn: fetchAllProducts,
  });

  const [filterString, setFilterString] = useState<string>("");
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  if (!isFetching && !rows) {
    return <Alert severity="error">Failed to retrieve Menu Items</Alert>;
  }

  if (isFetching || !rows) {
    return <CircularProgress />;
  }

  let filteredProducts: ProductDto[];
  if (!filterString) {
    filteredProducts = rows;
  } else {
    filteredProducts = rows.filter(
      (row) =>
        row.title.toLowerCase().includes(filterString.toLowerCase()) ||
        row.description?.toLowerCase().includes(filterString.toLowerCase()),
    );
  }

  return (
    <>
      <Box
        display="flex"
        flexDirection="row"
        justifyContent="space-between"
        alignItems="start"
        pb={2}
      >
        <Paper
          sx={{
            p: "2px 4px",
            mb: "1rem",
            display: "flex",
            alignItems: "center",
            maxWidth: "500px",
          }}
        >
          <InputBase
            sx={{ ml: 1, flex: 1 }}
            placeholder="Search in products"
            value={filterString}
            onChange={(e) => setFilterString(e.target.value)}
            inputProps={{ "aria-label": "search in products" }}
          />
          <IconButton type="button" sx={{ p: "10px" }} aria-label="search">
            <SearchIcon />
          </IconButton>
        </Paper>
        {role == Role.MANAGER && (
          <Button
            variant="outlined"
            onClick={() => {
              setIsCreateModalOpen((prevState) => !prevState);
            }}
          >
            Create Product
          </Button>
        )}
      </Box>
      <Grid container spacing={4} columns={columnCount}>
        {filteredProducts.map((row) => (
          <ProductMenuItem key={row.id} product={row} />
        ))}
      </Grid>
      <CreateProductModal
        open={isCreateModalOpen}
        handleCloseCreateModal={() => setIsCreateModalOpen(false)}
      />
    </>
  );
}
