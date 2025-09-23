import React from 'react';
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';

export default function CustomAlert({ alertType, type, message }) {

    return (
        <Alert severity={alertType} variant="outlined" sx={{ mb: 3}}>
            <AlertTitle>{type}</AlertTitle>
            {message}
        </Alert>
    );
}