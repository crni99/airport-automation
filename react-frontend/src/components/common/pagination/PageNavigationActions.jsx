import React from 'react';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { getRole } from "../../../utils/auth";
import BackToListAction from './BackToListAction';
import EditAction from './EditAction';
import DeleteAction from './DeleteAction';

const isUser = getRole();

export default function PageNavigationActions({ dataType, dataId, onEdit, onDelete }) {
    return (
        <Box>
            {isUser !== 'User' && (
                <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
                    <BackToListAction dataType={dataType} />
                    <EditAction dataType={dataType} dataId={dataId} onEdit={onEdit} />
                    <DeleteAction dataType={dataType} dataId={dataId} onDelete={onDelete} />
                </Stack>
            )}
        </Box>
    );
}