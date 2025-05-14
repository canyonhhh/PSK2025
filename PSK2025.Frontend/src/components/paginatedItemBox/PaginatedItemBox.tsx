import { Box, Grid, Pagination, Typography } from '@mui/material';
import { PaginatedResponse } from '../../api/types/PaginatedResposeDto';

interface PaginatedItemsProps<T> {
  content: PaginatedResponse<T>;
  itemRenderer: (item: T, index: number) => React.ReactNode;
  onPageChange: (page: number) => void;
}

export function PaginatedItems<T>({
  content,
  itemRenderer,
  onPageChange,
}: PaginatedItemsProps<T>) {
  const totalPages = Math.ceil(content.totalCount / content.pageSize);

  const handleChange = (_event: React.ChangeEvent<unknown>, value: number) => {
    onPageChange(value);
  };

  return (
    <Box>
      {content.items.length === 0 ? (
        <Typography variant="body1" align="center">No items found.</Typography>
      ) : (
        <Grid container spacing={2}>
          {content.items.map((item, index) => (
            <Grid item xs={12} sm={6} md={4} key={index}>
              {itemRenderer(item, index)}
            </Grid>
          ))}
        </Grid>
      )}
      
      {totalPages > 1 && (
        <Box display="flex" justifyContent="center" mt={4}>
          <Pagination
            count={totalPages}
            page={content.currentPage}
            onChange={handleChange}
            color="primary"
            shape="rounded"
          />
        </Box>
      )}
    </Box>
  );
}