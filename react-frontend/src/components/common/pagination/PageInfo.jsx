import React from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Divider from '@mui/material/Divider';

function PageInfo({ currentPage, totalPages, totalCount }) {
    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                mt: 1,
                p: 2,
                border: '1px solid',
                borderColor: 'divider',
                borderRadius: 1,
                fontSize: '0.875rem',
            }}
        >
            <Grid container spacing={2} alignItems="center" >
                <Grid sx={{ display: 'flex' }}>
                    <Typography variant="body2" component="strong">
                        Page:
                    </Typography>
                    <Typography variant="body2" component="span" sx={{ ml: 0.5 }}>
                        {currentPage}
                    </Typography>
                </Grid>
                <Grid>
                    <Divider orientation="vertical" flexItem />
                </Grid>
                <Grid sx={{ display: 'flex' }}>
                    <Typography variant="body2" component="strong">
                        Total Pages:
                    </Typography>
                    <Typography variant="body2" component="span" sx={{ ml: 0.5 }}>
                        {totalPages}
                    </Typography>
                </Grid>
                <Grid>
                    <Divider orientation="vertical" flexItem />
                </Grid>
                <Grid sx={{ display: 'flex' }}>
                    <Typography variant="body2" component="strong">
                        Total Records:
                    </Typography>
                    <Typography variant="body2" component="span" sx={{ ml: 0.5 }}>
                        {totalCount}
                    </Typography>
                </Grid>
            </Grid>
        </Box>
    );
}

export default PageInfo;