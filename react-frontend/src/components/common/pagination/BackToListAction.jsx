import React from 'react';

export default function BackToListAction({ dataType }) {
    return (
        <li className="page-item">
            <a href={`/${dataType}`} className="page-link">Back to List</a>
        </li>
    )
}