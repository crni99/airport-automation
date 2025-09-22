import { Navigate } from 'react-router-dom';
import { getRole } from '../utils/auth';
import { ROLES } from '../utils/const';

const ProtectedRoute = ({ element }) => {
    const isUser = getRole();

    return (isUser === ROLES.ADMIN || isUser === ROLES.SUPER_ADMIN) ? element : <Navigate to="/unauthorized" />;
};

export default ProtectedRoute;