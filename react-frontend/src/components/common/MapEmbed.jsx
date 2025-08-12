import React from 'react';

const MapEmbed = ({ address }) => {
    if (!address) return <div>No address provided</div>;

    return (
        <iframe
            src={`https://maps.google.com/maps?q=${encodeURIComponent(address)}&output=embed`}
            width="100%"
            height="320px"
            style={{ border: 0, borderRadius: '0.375rem' }}
            allowFullScreen=""
            loading="lazy"
            referrerPolicy="no-referrer-when-downgrade"
            title="Google Map"
        ></iframe>
    );
};

export default MapEmbed;