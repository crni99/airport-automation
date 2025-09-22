import React from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';

const MapEmbed = ({ address }) => {
    if (!address) {
        return (
            <Typography variant="body1" color="text.secondary" sx={{ p: 2, textAlign: 'center' }}>
                No address provided.
            </Typography>
        );
    }

    const encodedAddress = encodeURIComponent(address);
    const mapSrc = `https://maps.google.com/maps?q=${encodedAddress}&output=embed`;

    return (
        <Box sx={{ width: '100%', height: 320, borderRadius: 1, overflow: 'hidden' }}>
            <iframe
                src={mapSrc}
                width="100%"
                height="100%"
                style={{ border: 0 }}
                allowFullScreen=""
                loading="lazy"
                referrerPolicy="no-referrer-when-downgrade"
                title="Google Map"
            ></iframe>
        </Box>
    );
};

export default MapEmbed;