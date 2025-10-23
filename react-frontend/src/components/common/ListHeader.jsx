import React from 'react';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import SearchInputWithButton from "../common/SearchInputWithButton";

export default function ListHeader({ dataType, createButtonTitle, setTriggerFetch }) {

    const isPassenger = dataType === 'Passengers';
    const marginTop = isPassenger ? 3 : 5;
    const marginBottom = isPassenger ? 2 : 4;

    return (
        <Box sx={{
            mt: marginTop,
            mb: marginBottom
        }}>
            <Stack
                direction={{ xs: 'column', md: 'row' }}
                justifyContent="space-between"
                alignItems="center"
                spacing={2}
            >
                <SearchInputWithButton
                    type={dataType}
                    setTriggerFetch={setTriggerFetch}
                    createButtonTitle={createButtonTitle}
                />
            </Stack>
        </Box>
    );
}