import React from 'react';
import { Box, Typography, TablePagination } from '@mui/material';

const Pagination = ({
    data,
    pageNumber,
    totalPages,
    rowsPerPage,
    handlePageChange,
    handleRowsPerPageChange,
}) => {
    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                mt: 3,
                mb: 2
            }}
        >
            <Typography variant="body2" color="text.secondary">
                &nbsp;&nbsp;&nbsp;&nbsp;Page {pageNumber} of {totalPages}
            </Typography>
            <TablePagination
                component="div"
                count={data?.totalCount ?? 0}
                page={pageNumber - 1}
                onPageChange={handlePageChange}
                rowsPerPage={rowsPerPage}
                onRowsPerPageChange={handleRowsPerPageChange}
                rowsPerPageOptions={[5, 10, 20]}
                showFirstButton
                showLastButton
            />
        </Box>
    );
};

export default Pagination;