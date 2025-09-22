import React from 'react';
import Box from '@mui/material/Box';

export function DD({ children }) {
    return (
        <Box component="dd" sx={{ flexGrow: 1, ml: { sm: 2 }, mt: 2 }}>
            {children}
        </Box>
    );
}