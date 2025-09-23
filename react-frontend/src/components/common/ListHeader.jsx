import React from 'react';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import CreateButton from "../common/CreateButton";
import SearchInputWithButton from "../common/SearchInputWithButton";
import { getRole } from "../../utils/auth";
import { ENTITIES } from '../../utils/const.js';

export default function ListHeader({ dataExist, dataType, createButtonTitle, setTriggerFetch }) {

    const isUser = getRole();

    const disableCreateButton = isUser === 'User' || dataType === ENTITIES.API_USERS || dataType === ENTITIES.TRAVEL_CLASSES;

    return (
        <Box sx={{ mt: 5, mb: 4 }}>
            <Stack
                direction={{ xs: 'column', md: 'row' }}
                justifyContent="space-between"
                alignItems="center"
                spacing={2}
            >
                {!disableCreateButton && (
                    <CreateButton destination={`/${dataType}/Create`} title={createButtonTitle} />
                )}
                {dataExist && (
                    <Box sx={{ width: { xs: '100%', md: 'auto' } }}>
                        <SearchInputWithButton type={dataType} setTriggerFetch={setTriggerFetch} isUser={isUser} />
                    </Box>
                )}
            </Stack>
        </Box>
    );
}