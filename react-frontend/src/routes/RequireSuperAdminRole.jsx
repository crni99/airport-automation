import { Navigate } from 'react-router-dom';
import { getRole } from '../utils/auth';
import { ROLES } from '../utils/const';

const RequireSuperAdminRole = ({ element }) => {
    const role = getRole();
    return role === ROLES.SUPER_ADMIN ? element : <Navigate to="/unauthorized" />;
};

export default RequireSuperAdminRole;
