import React, { useEffect, useState } from 'react';

export default function Alert({ alertType, children }) {
    const [visible, setVisible] = useState(true);

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            setVisible(false);
        }, 5000); // 5 seconds

        return () => clearTimeout(timeoutId); // Clean up
    }, []);

    if (!visible) return null;

    let alertClasses = "text-center mt-4 alert mb-4 ";

    if (alertType === 'success') {
        alertClasses += "alert-success";
    } else if (alertType === 'info') {
        alertClasses += "alert-info";
    } else if (alertType === 'danger' || alertType === 'error') {
        alertClasses += "alert-danger";
    } else {
        console.warn('Alert type does not exist:', alertType);
    }

    return (
        <div className={alertClasses} role="alert">
            {children}
        </div>
    );
}
