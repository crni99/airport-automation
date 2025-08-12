import { getAuthToken } from '../utils/auth.js';
import { generateErrorMessage, handleNetworkError } from '../utils/errorUtils.js';

/*
Input Validation: 
Ensure proper validation of input data before sending it to the server 
to prevent potential security vulnerabilities or data corruption.
*/

export async function createData(data, dataType, apiUrl, navigate) {
    try {

        const authToken = getAuthToken();
        const headers = {
            'Content-Type': 'application/json'
        };
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`${apiUrl}/${dataType}`, {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(data)
        });
        if (response.ok) {
            const responseData = await response.json();
            navigate(`/${dataType}/${responseData.id}`);
        } else {
            const errorMessage = await generateErrorMessage(response, dataType);
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
