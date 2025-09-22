import { Navigate } from 'react-router-dom';
import { getRole } from '../utils/auth';
import { ROLES } from '../utils/const';

const ProtectedRouteV2 = ({ element }) => {
    const role = getRole();
    return role === ROLES.SUPER_ADMIN ? element : <Navigate to="/unauthorized" />;
};

export default ProtectedRouteV2;
