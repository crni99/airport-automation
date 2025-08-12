import React from 'react';

export default function EditAction({ dataType, dataId, onEdit }) {
    return (
        <li className="page-item">
            <a href={`/${dataType}/Edit/${dataId}`} className="page-link text-success">Edit</a>
        </li>
    )
}