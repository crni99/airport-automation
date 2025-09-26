import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { deleteData } from '../../../utils/delete.js';
import { DataContext } from '../../../store/DataContext.jsx';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import IconButton from '@mui/material/IconButton';
import Tooltip from '@mui/material/Tooltip';
import Alert from '@mui/material/Alert';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

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
        <Box>
            <Stack direction="row" spacing={1} alignItems="center" sx={{ width: '100%', justifyContent: 'center' }} >
                <Tooltip title="Open" arrow>
                    <IconButton
                        component="a"
                        href={openUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        size="small"
                        color="primary"
                    >
                        <OpenInNewIcon />
                    </IconButton>
                </Tooltip>

                {currentUserRole !== 'User' && (
                    <>
                        <Tooltip title="Edit" arrow>
                            <IconButton
                                component="a"
                                href={editUrl}
                                target="_blank"
                                rel="noopener noreferrer"
                                size="small"
                                color="success"
                            >
                                <EditIcon />
                            </IconButton>
                        </Tooltip>

                        {entityType !== 'ApiUser' && (
                            <Tooltip title="Delete" arrow>
                                <IconButton
                                    onClick={handleDelete}
                                    disabled={isDeleting}
                                    size="small"
                                    color="error"
                                >
                                    <DeleteIcon />
                                </IconButton>
                            </Tooltip>
                        )}
                    </>
                )}
            </Stack>
            {deleteError && (
                <Alert severity="error" sx={{ mt: 1 }}>
                    {deleteError}
                </Alert>
            )}
        </Box>
    );
};

export default TableActions;