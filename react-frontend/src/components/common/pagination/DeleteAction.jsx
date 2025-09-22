import React from 'react';
import Button from '@mui/material/Button';
import DeleteIcon from '@mui/icons-material/Delete';

export default function DeleteAction({ dataType, dataId, onDelete }) {

    const handleDelete = () => {
        onDelete(dataType, dataId);
    };

    return (
        <Button
            variant="outlined"
            color="error"
            onClick={handleDelete}
            startIcon={<DeleteIcon />}
        >
            Delete
        </Button>
    );
}