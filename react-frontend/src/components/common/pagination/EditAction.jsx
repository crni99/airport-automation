import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import EditIcon from '@mui/icons-material/Edit';

export default function EditAction({ dataType, dataId }) {
    const navigate = useNavigate();

    const handleEdit = () => {
        if (dataType === 'ApiUsers') {
            navigate(`/api-users/Edit/${dataId}`);
        }
        else if (dataType === 'PlaneTickets') {
            navigate(`/plane-tickets/Edit/${dataId}`);
        }
        else {
            navigate(`/${dataType}/Edit/${dataId}`);
        }
    };

    return (
        <Button
            variant="outlined"
            color="success"
            onClick={handleEdit}
            startIcon={<EditIcon />}
        >
            Edit
        </Button>
    );
}