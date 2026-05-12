import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { getAuthToken } from '../utils/auth';

const RequireAuth = () => {
    
    const token = getAuthToken();
    const location = useLocation();

    if (!token || token === 'EXPIRED') {
        return <Navigate to="/" state={{ from: location, authErrorMessage: 'You must be logged in to access this resource. Please log in and try again.' }} replace />;
    }

    return <Outlet />;
};

export default RequireAuth;