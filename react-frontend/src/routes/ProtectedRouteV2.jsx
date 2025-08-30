import { Navigate } from 'react-router-dom';
import { getRole } from '../utils/auth';

const ProtectedRouteV2 = ({ element }) => {
    const role = getRole();
    return role === 'SuperAdmin' ? element : <Navigate to="/unauthorized" />;
};

export default ProtectedRouteV2;
