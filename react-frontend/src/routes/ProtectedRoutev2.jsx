import { Navigate } from 'react-router-dom';
import { getRole } from '../utils/auth';

const ProtectedRoutev2 = ({ element }) => {
    const role = getRole();
    return role === 'SuperAdmin' ? element : <Navigate to="/unauthorized" />;
};

export default ProtectedRoutev2;
