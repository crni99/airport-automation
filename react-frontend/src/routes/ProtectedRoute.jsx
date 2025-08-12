import { Navigate } from 'react-router-dom';
import { getRole } from '../utils/auth';

const ProtectedRoute = ({ element }) => {
    const isUser = getRole();

    return (isUser === 'Admin' || isUser === 'SuperAdmin') ? element : <Navigate to="/unauthorized" />;
};

export default ProtectedRoute;