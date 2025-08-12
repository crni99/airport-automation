import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { deleteData } from '../../../utils/delete.js';
import { DataContext } from '../../../store/data-context.jsx';

const TableActions = ({ entity, id, entityType, currentUserRole }) => {
    const [isDeleting, setIsDeleting] = useState(false);
    const [deleteError, setDeleteError] = useState(null);

    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();

    const handleDelete = async () => {
        setIsDeleting(true);
        setDeleteError(null);
        try {
            const result = await deleteData(entity, id, dataCtx.apiUrl, navigate);
            if (result?.message) {
                setDeleteError(result.message);
            }
        } catch (error) {
            setDeleteError(error.message);
        } finally {
            setIsDeleting(false);
        }
    };

    const basePath = `/${entity}`;
    const openUrl = `${basePath}/${id}`;
    const editUrl = `${basePath}/Edit/${id}`;

    return (
        <td>
            <div className="btn-group btn-group-sm" role="group">
                <a
                    href={openUrl}
                    className="btn btn-icon-open me-3"
                    target="_blank"
                    rel="noopener noreferrer"
                    data-bs-toggle="tooltip"
                    data-bs-placement="right"
                    title="Open"
                >
                    <i className="fa-solid fa-square-up-right fa-xl"></i>
                </a>

                {currentUserRole !== 'User' && (
                    <>
                        <a
                            href={editUrl}
                            className="btn btn-icon-edit me-3"
                            target="_blank"
                            rel="noopener noreferrer"
                            data-bs-toggle="tooltip"
                            data-bs-placement="right"
                            title="Edit"
                        >
                            <i className="fa-solid fa-square-pen fa-xl"></i>
                        </a>

                        {entityType !== 'ApiUser' && (
                            <button
                                onClick={handleDelete}
                                className="btn btn-icon-delete"
                                type="button"
                                disabled={isDeleting}
                                data-bs-toggle="tooltip"
                                data-bs-placement="right"
                                title="Delete"
                            >
                                <i className="fa-solid fa-trash-can fa-xl"></i>
                            </button>
                        )}
                    </>
                )}
            </div>
            {deleteError && (
                <div className="text-danger small mt-1">
                    {deleteError}
                </div>
            )}
        </td>
    );
};

export default TableActions;
