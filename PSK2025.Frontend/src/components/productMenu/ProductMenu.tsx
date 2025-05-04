import { useQuery } from "@tanstack/react-query";
import {
  Alert,
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

interface ManagerMenuProps {
  columnCount: number;
}

export function ProductMenu({ columnCount }: ManagerMenuProps) {
  let { data: rows, isFetching } = useQuery({
    queryKey: keys.allProducts,
    queryFn: fetchAllProducts,
  });
  const [filterString, setFilterString] = useState<string>("");

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
        row.Title.toLowerCase().includes(filterString.toLowerCase()) ||
        row.Description?.toLowerCase().includes(filterString.toLowerCase()),
    );
  }

  return (
    <>
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
      <Grid container spacing={4} columns={columnCount}>
        {filteredProducts.map((row) => (
          <ProductMenuItem key={row.Id} product={row} />
        ))}
      </Grid>
    </>
  );
}
