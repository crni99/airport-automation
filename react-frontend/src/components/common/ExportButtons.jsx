import React, { useState } from 'react';
import { useExport } from '../../hooks/useExport';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import PictureAsPdfIcon from '@mui/icons-material/PictureAsPdf';
import DescriptionIcon from '@mui/icons-material/Description';
import FileDownloadIcon from '@mui/icons-material/FileDownload';

export default function ExportButton({ dataType }) {
    const { triggerExport, isLoading } = useExport();

    const [anchorEl, setAnchorEl] = useState(null);
    const open = Boolean(anchorEl);

    const handleClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleExport = (format) => {
        triggerExport(dataType, format);
        handleClose();
    };

    return (
        <Stack direction="row" spacing={2} alignItems="center">
            <Button
                variant="contained"
                onClick={handleClick}
                disabled={isLoading}
                color="secondary"
                startIcon={<FileDownloadIcon />}
            >
                Export
            </Button>
            <Menu
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
            >
                {/* 
                <MenuItem onClick={() => handleExport('pdf')} disabled={isLoading}>
                    <PictureAsPdfIcon sx={{ mr: 1, mb: 1 }} color="error" />
                    PDF
                </MenuItem>
                */}
                <MenuItem onClick={() => handleExport('excel')} disabled={isLoading}>
                    <DescriptionIcon sx={{ mr: 1 }} color="success" />
                    Excel
                </MenuItem>
            </Menu>

            {isLoading && <CircularProgress size={24} />}
        </Stack>
    );
}