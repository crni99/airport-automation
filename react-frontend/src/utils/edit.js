import { getAuthToken } from '../utils/auth.js';
import { generateErrorMessage, handleNetworkError } from '../utils/errorUtils.js';

/*
Input Validation: 
Ensure proper validation of input parameters (dataType, dataId, and data) \
before constructing the request URL and body to prevent potential security vulnerabilities or data corruption.
*/

export async function editData(data, dataType, dataId, apiUrl, navigate) {
    try {

        const authToken = getAuthToken();
        const headers = {
            'Content-Type': 'application/json'
        };
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`${apiUrl}/${dataType}/${dataId}`, {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(data)
        });
        if (response.ok || response.status === 204) {
            navigate(`/${dataType}/${dataId}`);
        } else {
            const errorMessage = await generateErrorMessage(response, dataType, dataId);
            throw new Error(errorMessage);
        }
    } catch (error) {
        const networkErrorMessage = handleNetworkError(error);
        if (networkErrorMessage) {
            return networkErrorMessage;
        } else {
            console.error('Error creating data:', error);
        }
        return error;
    }
}

