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

export default function SearchInputWithButton({ type, setTriggerFetch, isUser }) {
    const [searchTerms, setSearchTerms] = useState({});

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

    const exportButtonsOrSpace = isUser !== 'User' && (
        <ExportButtons dataType={type} />
    );

    const renderInput = () => {
        switch (type) {
            case ENTITIES.AIRLINES:
                return (
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
                );
            case ENTITIES.API_USERS:
                return (
                    <Stack direction="row" spacing={2} alignItems="center">
                        <TextField
                            id="username"
                            label="Username"
                            placeholder="Enter username"
                            value={searchTerms.username || ''}
                            onChange={handleInputChange}
                            size='small'
                        />
                        <TextField
                            id="password"
                            label="Password"
                            placeholder="Enter password"
                            type="password"
                            value={searchTerms.password || ''}
                            onChange={handleInputChange}
                            size='small'
                        />
                        <FormControl sx={{ minWidth: 120 }}>
                            <InputLabel id="role-select-label" size='small'>Role</InputLabel>
                            <Select
                                labelId="role-select-label"
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
                );
            case ENTITIES.DESTINATIONS:
                return (
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
                );
            case ENTITIES.FLIGHTS:
                return (
                    <Stack direction="row" spacing={2} alignItems="center">
                        <TextField
                            id="startDate"
                            label="Start Date"
                            type="date"
                            InputLabelProps={{ shrink: true }}
                            value={searchTerms.startDate || ''}
                            onChange={handleInputChange}
                            size='small'
                        />
                        <TextField
                            id="endDate"
                            label="End Date"
                            type="date"
                            InputLabelProps={{ shrink: true }}
                            value={searchTerms.endDate || ''}
                            onChange={handleInputChange}
                            size='small'
                        />
                        <SearchButton onClick={handleSearch} />
                        <ClearInputButton onClear={handleClear} />
                        {exportButtonsOrSpace}
                    </Stack>
                );
            case ENTITIES.PASSENGERS:
                return (
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
                            id="passport"
                            label="Passport"
                            placeholder="P12345678"
                            value={searchTerms.passport || ''}
                            onChange={handleInputChange}
                            size='small'
                        />
                        <TextField
                            id="address"
                            label="Address"
                            placeholder="123 Main Street, New York, United States"
                            value={searchTerms.address || ''}
                            onChange={handleInputChange}
                            size='small'
                        />
                        <TextField
                            id="phone"
                            label="Phone"
                            type="tel"
                            placeholder="123-456-7890"
                            value={searchTerms.phone || ''}
                            onChange={handleInputChange}
                            size='small'
                        />
                        <SearchButton onClick={handleSearch} />
                        <ClearInputButton onClear={handleClear} />
                        {exportButtonsOrSpace}
                    </Stack>
                );
            case ENTITIES.PILOTS:
                return (
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
                        />
                        <SearchButton onClick={handleSearch} />
                        <ClearInputButton onClear={handleClear} />
                        {exportButtonsOrSpace}
                    </Stack>
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