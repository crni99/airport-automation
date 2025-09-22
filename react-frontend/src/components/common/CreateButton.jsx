import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';

export default function CreateButton({ destination, title }) {
    const navigate = useNavigate();

    const handleCreateClick = () => {
        navigate(destination);
    };

    return (
        <Button
            variant="contained"
            color="success"
            onClick={handleCreateClick}
            startIcon={<AddIcon />}
            sx={{ flexShrink: 0 }}
        >
            {title}
        </Button>
    );
}