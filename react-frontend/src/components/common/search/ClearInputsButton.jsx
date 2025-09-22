import React from 'react';
import Button from '@mui/material/Button';
import ClearIcon from '@mui/icons-material/Clear';

export default function ClearInputButton({ onClear }) {
    return (
        <Button
            variant="contained"
            color="warning"
            onClick={onClear}
            startIcon={<ClearIcon />}
        >
            Clear
        </Button>
    );
}