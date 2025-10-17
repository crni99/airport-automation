import { getAuthToken } from './auth.js';
import { generateErrorMessage, handleNetworkError } from './errorUtils.js';
import { CustomAPIError } from './CustomError.js';

export async function createData(data, dataType, apiUrl) {
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
            return {
                success: true,
                message: 'Data successfully created.',
                newId: responseData.id
            };
        } else {
            const errorMessage = await generateErrorMessage(response, dataType);
            throw new CustomAPIError('API_ERROR', errorMessage);
        }
    } catch (error) {
        const networkErrorObject = handleNetworkError(error);

        if (networkErrorObject) {
            throw new CustomAPIError('NETWORK_ERROR', networkErrorObject.message);
        }
        if (error instanceof CustomAPIError) {
            throw error;
        } else {
            console.error('Error creating data:', error);
            const message = error.message || 'An unexpected error occurred during the create operation.';
            throw new CustomAPIError('UNEXPECTED_ERROR', message);
        }
    }
}
