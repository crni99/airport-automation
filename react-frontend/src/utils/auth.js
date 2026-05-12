import { generateErrorMessage, handleNetworkError } from './errorUtils.js';
import logger from './logger.js'

export async function authenticateUser(userName, password, apiUrl) {
    const userCredentials = {
        UserName: userName,
        Password: password
    };

    try {
        if (!apiUrl) {
            throw new Error('API URL is not available');
        }
        const response = await fetch(`${apiUrl}/Authentication`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userCredentials)
        });

        if (!response.ok) {
            const networkErrorMessage = handleNetworkError(response);
            if (networkErrorMessage) {
                return networkErrorMessage;
            } else {
                const errorMessage = await generateErrorMessage(response, 'Authentication');
                throw new Error(errorMessage);
            }
        }

        const token = await response.text();
        localStorage.setItem('token', token);

        const role = getRoleFromToken(token);
        localStorage.setItem('role', role);

        const expiration = new Date();
        expiration.setHours(expiration.getHours() + 1);
        localStorage.setItem('expiration', expiration.toISOString());

        window.location.href = '/';
    } catch (error) {
        const networkErrorMessage = handleNetworkError(error);
        if (networkErrorMessage) {
            logger.error('Auth network error:', networkErrorMessage);
            return networkErrorMessage;
        } else {
            logger.error('Auth error:', error);
            return error;
        }
    }
}

export function handleSignOut() {
    try {
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        localStorage.removeItem('expiration');
        window.location.href = '/';
    } catch (error) {
        const errorMessage = handleNetworkError(error);
        if (errorMessage) {
            logger.error('Error while signing out:', errorMessage);
        } else {
            logger.error('Error while signing out:', error);
        }
        return error;
    }
}

export function getTokenDuration() {
    const storedExpirationDate = localStorage.getItem('expiration');
    if (!storedExpirationDate) {
        return 0;
    }
    const expirationDate = new Date(storedExpirationDate);
    const now = new Date();
    const duration = expirationDate.getTime() - now.getTime();
    return duration;
}

export function getAuthToken() {
    const token = localStorage.getItem('token');
    if (!token) {
        return null;
    }

    const tokenDuration = getTokenDuration();
    if (tokenDuration < 0) {
        return 'EXPIRED';
    }
    return token;
}

export function getRole() {
    const role = localStorage.getItem('role');
    if (!role) {
        return null;
    }
    return role;
}


function getRoleFromToken(token) {
    if (!token) return null;

    try {
        const parts = token.split('.');
        if (parts.length !== 3) return null;

        const payload = parts[1];
        const decodedPayload = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
        const roleClaim = decodedPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        return roleClaim || null;
    } catch {
        return null;
    }
}