import React from 'react';
import Box from '@mui/material/Box';

export function DT({ children }) {
    return (
        <Box component="dt" sx={{ width: { sm: 140 }, mr: { sm: 2 }, mt: 2 }}>
            {children}
        </Box>
    );
}