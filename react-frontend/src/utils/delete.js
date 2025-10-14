import { getAuthToken } from '../utils/auth.js';
import { generateErrorMessage, handleNetworkError } from '../utils/errorUtils.js';

export async function deleteData(dataType, dataId, apiUrl, navigate) {
    try {
        const authToken = getAuthToken();
        const headers = {
            'Content-Type': 'application/json'
        };
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`${apiUrl}/${dataType}/${dataId}`, {
            method: 'DELETE',
            headers: headers
        });

        console.log('RESPONSE: ', response);
        if (response.ok || response.status === 204) {
            navigate(`/${dataType}`);
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

