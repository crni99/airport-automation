import React from 'react';

export default function DeleteAction({ dataType, dataId, onDelete }) {

    const handleDelete = () => {
        onDelete(dataType, dataId);
    };
    
    return (
        <li className="page-item">
            <button onClick={handleDelete} className="page-link text-danger" type="button">Delete</button>
        </li>
    );
}