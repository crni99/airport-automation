import React from 'react';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';

export default function PageTitle({ title }) {
    return (
        <Box sx={{ mb: 4 }}>
            <Typography variant="h2" component="h1">
                {title}
            </Typography>
        </Box>
    );
}