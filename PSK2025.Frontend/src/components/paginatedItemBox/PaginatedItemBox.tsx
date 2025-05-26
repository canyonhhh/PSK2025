import { Box, Pagination, Typography } from "@mui/material";
import Grid from "@mui/material/Grid";
import { PaginatedResponse } from "../../api/types/PaginatedResposeDto";

interface PaginatedItemsProps<T> {
    content: PaginatedResponse<T>;
    itemRenderer: (item: T, index: number) => React.ReactNode;
    onPageChange: (page: number) => void;
    containerRenderer?: (elements: React.ReactNode[]) => React.ReactNode;
}

export function PaginatedItems<T>({
    content,
    itemRenderer,
    onPageChange,
    containerRenderer,
}: PaginatedItemsProps<T>) {
    const totalPages = Math.ceil(content.totalCount / content.pageSize);

    const handleChange = (
        _event: React.ChangeEvent<unknown>,
        value: number,
    ) => {
        onPageChange(value);
    };

    const renderContainer = () => {
        if (containerRenderer) {
            return containerRenderer(content.items.map(itemRenderer));
        }

        return (
            <Grid container spacing={2}>
                {content.items.map((item, index) => (
                    <Grid size={{ xs: 12, sm: 6, md: 4 }} key={index}>
                        {itemRenderer(item, index)}
                    </Grid>
                ))}
            </Grid>
        );
    };

    return (
        <Box>
            {content.items.length === 0 ? (
                <Typography variant="body1" align="center">
                    No items found.
                </Typography>
            ) : (
                renderContainer()
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
