import React, { useState, useEffect } from 'react';
import { ENTITIES } from '../../utils/const';
import SearchButton from './search/SearchButton';
import ClearInputButton from './search/ClearInputsButton';
import ExportButtons from './ExportButtons';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import CreateButton from "../common/CreateButton";
import { getRole } from "../../utils/auth";

export default function SearchInputWithButton({ type, setTriggerFetch, createButtonTitle }) {
    const [searchTerms, setSearchTerms] = useState({});

    const isUser = getRole();
    const disableCreateButton = isUser === 'User' || type === ENTITIES.API_USERS || type === ENTITIES.TRAVEL_CLASSES;

    useEffect(() => {
        setSearchTerms({});
    }, [type]);

    const handleSearch = () => {
        setTriggerFetch(true);
    };

    const handleInputChange = (e) => {
        const { id, value, name } = e.target;
        setSearchTerms(prev => ({ ...prev, [id || name]: value }));
    };

    const handleClear = () => {
        setSearchTerms({});
    };

    var pathType = type;
    if (type === ENTITIES.PLANE_TICKETS) {
        pathType = 'plane-tickets';
    }
    const createButton = !disableCreateButton && (
        <CreateButton destination={`/${pathType}/Create`} title={createButtonTitle} />
    );

    const exportButtonsOrSpace = isUser !== 'User' && (
        <ExportButtons dataType={type} />
    );

    const renderInput = () => {
        switch (type) {
            case ENTITIES.AIRLINES:
                return (
                    <>
                        {createButton}
                        <Box sx={{ width: { xs: '100%', md: 'auto' } }}>
                            <Stack direction="row" spacing={2} alignItems="center">
                                <TextField
                                    label="Name"
                                    id="searchInput"
                                    placeholder="Air Serbia"
                                    value={searchTerms.searchInput || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <SearchButton onClick={handleSearch} />
                                <ClearInputButton onClear={handleClear} />
                                {exportButtonsOrSpace}
                            </Stack>
                        </Box>
                    </>
                );
            case ENTITIES.API_USERS:
                return (
                    <Box sx={{ width: { xs: '100%', md: 'auto' } }}>
                        <Stack direction="row" spacing={2} alignItems="center">
                            <TextField
                                id="username"
                                label="Username"
                                placeholder="Enter username"
                                value={searchTerms.username || ''}
                                onChange={handleInputChange}
                                size='small'
                            />
                            <FormControl sx={{ minWidth: 120 }}>
                                <InputLabel id="role-select-label" size='small'>Role</InputLabel>
                                <Select
                                    labelId="role"
                                    id="role"
                                    name="role"
                                    value={searchTerms.role || ''}
                                    label="Role"
                                    onChange={handleInputChange}
                                    size='small'
                                >
                                    <MenuItem value="">Select a role</MenuItem>
                                    <MenuItem value="User">User</MenuItem>
                                    <MenuItem value="Admin">Admin</MenuItem>
                                    <MenuItem value="SuperAdmin">Super Admin</MenuItem>
                                </Select>
                            </FormControl>
                            <SearchButton onClick={handleSearch} />
                            <ClearInputButton onClear={handleClear} />
                        </Stack>
                    </Box>
                );
            case ENTITIES.DESTINATIONS:
                return (
                    <>
                        {createButton}
                        <Box sx={{ width: { xs: '100%', md: 'auto' } }}>
                            <Stack direction="row" spacing={2} alignItems="center">
                                <TextField
                                    label="City"
                                    id="city"
                                    placeholder="Belgrade"
                                    value={searchTerms.city || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <TextField
                                    label="Airport"
                                    id="airport"
                                    placeholder="Belgrade Nikola Tesla"
                                    value={searchTerms.airport || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <SearchButton onClick={handleSearch} />
                                <ClearInputButton onClear={handleClear} />
                                {exportButtonsOrSpace}
                            </Stack>
                        </Box>
                    </>
                );
            case ENTITIES.FLIGHTS:
                return (
                    <>
                        {createButton}
                        <Box sx={{ width: { xs: '100%', md: 'auto' } }}>
                            <Stack direction="row" spacing={2} alignItems="center">
                                <TextField
                                    id="startDate"
                                    label="Start Date"
                                    type="date"
                                    slotProps={{ inputLabel: { shrink: true } }}
                                    value={searchTerms.startDate || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <TextField
                                    id="endDate"
                                    label="End Date"
                                    type="date"
                                    slotProps={{ inputLabel: { shrink: true } }}
                                    value={searchTerms.endDate || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <SearchButton onClick={handleSearch} />
                                <ClearInputButton onClear={handleClear} />
                                {exportButtonsOrSpace}
                            </Stack>
                        </Box >
                    </>
                );
            case ENTITIES.PASSENGERS:
                return (
                    <Stack direction="column" spacing={2} sx={{ width: '100%' }}>
                        <Stack
                            direction="row"
                            alignItems="center"
                            justifyContent="space-between"
                            sx={{ width: '100%' }}
                        >
                            {createButton}
                            <Stack direction="row" spacing={2} alignItems="center">
                                <SearchButton onClick={handleSearch} />
                                <ClearInputButton onClear={handleClear} />
                                {exportButtonsOrSpace}
                            </Stack>
                        </Stack>
                        <Stack
                            direction="row"
                            spacing={2}
                            alignItems="center"
                            flexWrap="wrap"
                        >
                            <TextField
                                id="firstName"
                                label="First Name"
                                placeholder="Ognjen"
                                value={searchTerms.firstName || ''}
                                onChange={handleInputChange}
                                size='small'
                                sx={{ width: '15%' }}
                            />
                            <TextField
                                id="lastName"
                                label="Last Name"
                                placeholder="Andjelic"
                                value={searchTerms.lastName || ''}
                                onChange={handleInputChange}
                                size='small'
                                sx={{ width: '15%' }}
                            />
                            <TextField
                                id="uprn"
                                label="UPRN"
                                type="number"
                                placeholder="1234567890123"
                                value={searchTerms.uprn || ''}
                                onChange={handleInputChange}
                                size='small'
                                sx={{ width: '15%' }}
                            />
                            <TextField
                                id="passport"
                                label="Passport"
                                placeholder="P12345678"
                                value={searchTerms.passport || ''}
                                onChange={handleInputChange}
                                size='small'
                                sx={{ width: '10%' }}
                            />
                            <TextField
                                id="address"
                                label="Address"
                                placeholder="123 Main Street, New York, United States"
                                value={searchTerms.address || ''}
                                onChange={handleInputChange}
                                size='small'
                                sx={{ width: '23%' }}
                            />
                            <TextField
                                id="phone"
                                label="Phone"
                                type="tel"
                                placeholder="123-456-7890"
                                value={searchTerms.phone || ''}
                                onChange={handleInputChange}
                                size='small'
                                sx={{ width: '15%' }}
                            />
                        </Stack>
                    </Stack>
                );
            case ENTITIES.PILOTS:
                return (
                    <>
                        {createButton}
                        <Box sx={{ width: { xs: '100%', md: 'auto' } }}>
                            <Stack direction="row" spacing={2} alignItems="center">
                                <TextField
                                    id="firstName"
                                    label="First Name"
                                    placeholder="Ognjen"
                                    value={searchTerms.firstName || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <TextField
                                    id="lastName"
                                    label="Last Name"
                                    placeholder="Andjelic"
                                    value={searchTerms.lastName || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <TextField
                                    id="uprn"
                                    label="UPRN"
                                    type="number"
                                    placeholder="1234567890123"
                                    value={searchTerms.uprn || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <TextField
                                    id="flyingHours"
                                    label="Flying Hours"
                                    type="number"
                                    placeholder="10"
                                    value={searchTerms.flyingHours || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                    sx={{ width: '15%' }}
                                />
                                <SearchButton onClick={handleSearch} />
                                <ClearInputButton onClear={handleClear} />
                                {exportButtonsOrSpace}
                            </Stack>
                        </Box>
                    </>
                );
            case ENTITIES.PLANE_TICKETS:
                return (
                    <>
                        {createButton}
                        <Box sx={{ width: { xs: '100%', md: 'auto' } }}>
                            <Stack direction="row" spacing={2} alignItems="center">
                                <TextField
                                    id="price"
                                    label="Price (€)"
                                    type="number"
                                    placeholder="600"
                                    value={searchTerms.price || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <TextField
                                    id="purchaseDate"
                                    label="Purchase Date"
                                    type="date"
                                    slotProps={{ inputLabel: { shrink: true } }}
                                    value={searchTerms.purchaseDate || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <TextField
                                    id="seatNumber"
                                    label="Seat Number"
                                    type="number"
                                    placeholder="160"
                                    value={searchTerms.seatNumber || ''}
                                    onChange={handleInputChange}
                                    size='small'
                                />
                                <SearchButton onClick={handleSearch} />
                                <ClearInputButton onClear={handleClear} />
                                {exportButtonsOrSpace}
                            </Stack>
                        </Box>
                    </>
                );
            case ENTITIES.TRAVEL_CLASSES:
                return (
                    <Box sx={{ mb: 3 }}>
                        {exportButtonsOrSpace}
                    </Box>
                );
            default:
                return null;
        }
    };

    return renderInput();
}