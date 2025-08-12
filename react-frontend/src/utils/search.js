import { getAuthToken } from '../utils/auth.js';
import { generateErrorMessage, handleNetworkError } from '../utils/errorUtils.js';

async function searchAirlinesByName(searchTerm, apiUrl) {
    const headers = buildHeaders();
    const url = apiUrl + searchTerm;
    
    try {
        const response = await fetch(url, { headers });
        if (response.ok) {
            return await handleResponse(response);
        } else {
            throw new Error('Failed to fetch airlines by name');
        }
    } catch (error) {
        throw new Error('Failed to fetch airlines by name: ' + error.message);
    }
}

function buildHeaders() {
    const headers = { 'Content-Type': 'application/json' };
    const authToken = getAuthToken();
    if (authToken) {
        headers['Authorization'] = `Bearer ${authToken}`;
    }
    return headers;
}

async function handleResponse(response) {
    try {
        if (response.status === 204) {
            return { data: [], dataExist: false };
        } else {
            const responseData = await response.json();
            return { data: responseData, dataExist: true };
        }
    } catch (error) {
        throw new Error('Error handling response: ' + error.message);
    }
}

export default searchAirlinesByName;
