import { getAuthToken } from './auth.js';
import { generateErrorMessage, handleNetworkError } from './errorUtils.js';
import { CustomAPIError } from './CustomError.js';

export async function deleteData(dataType, dataId, apiUrl) {
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

        if (response.ok || response.status === 204) {
            return {
                success: true,
                message: 'Data successfully deleted.'
            };
        } else {
            const errorMessage = await generateErrorMessage(response, dataType, dataId);
            throw new CustomAPIError('API_ERROR', errorMessage);
        }
    } catch (error) {
        const networkErrorObject = handleNetworkError(error);

        if (networkErrorObject) {
            throw new CustomAPIError('NETWORK_ERROR', networkErrorObject.message);
        }
        if (error instanceof CustomAPIError) {
            throw error;
        }
        else {
            console.error('Error deleting data:', error);
            const message = error.message || 'An unexpected error occurred during the delete operation.';
            throw new CustomAPIError('UNEXPECTED_ERROR', message);
        }
    }
}