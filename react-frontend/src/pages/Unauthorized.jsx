import React from 'react';
import Box from '@mui/material/Box';
import MuiAlert from '@mui/material/Alert';
import Typography from '@mui/material/Typography';

export default function Unauthorized() {
    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                height: '50vh',
                textAlign: 'center'
            }}
        >
            <Box sx={{ width: { xs: '90%', sm: '75%', md: '50%' } }}>
                <MuiAlert severity="error">
                    <Typography variant="body1">
                        You do not have permission to access this resource.
                    </Typography>
                </MuiAlert>
            </Box>
        </Box>
    );
}