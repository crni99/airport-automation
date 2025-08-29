import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { getAuthToken } from '../utils/auth';

const ProtectedRouteV3 = () => {
    const token = getAuthToken();
    const location = useLocation();

    if (!token || token === 'EXPIRED') {
        localStorage.setItem('authErrorMessage', 'You must be logged in to access this resource. Please log in and try again..');
        return <Navigate to="/" state={{ from: location }} replace />;
    }

    return <Outlet />;
};

export default ProtectedRouteV3;
