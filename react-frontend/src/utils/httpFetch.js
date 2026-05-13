import { getAuthToken } from './auth.js';
import logger from './logger.js'
import { generateErrorMessage, handleNetworkError } from './errorUtils.js';
import { CustomAPIError } from './CustomError.js';
import { ENTITIES } from './const.js';

function buildHeaders() {
    const headers = { 'Content-Type': 'application/json' };
    const authToken = getAuthToken();
    if (authToken) {
        headers['Authorization'] = `Bearer ${authToken}`;
    }
    return headers;
}

function buildURL(apiUrl, dataType, dataId, page, pageSize, searchParams = {}) {
    
    let url = `${apiUrl}/${dataType}`;

    if (dataId !== null) {
        url += `/${dataId}`;
    } else {
        const paginationParams = `page=${page}&pageSize=${pageSize || 10}`;
        url += `?${paginationParams}`;

        switch (dataType) {
            case ENTITIES.AIRLINES: {
                const { name } = searchParams;
                if (name) {
                    url = `${apiUrl}/${ENTITIES.AIRLINES}/search?name=${encodeURIComponent(name)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.API_USERS: {
                const { username = '', role = '' } = searchParams;
                if (username || role) {
                    url = `${apiUrl}/${ENTITIES.API_USERS}/search?userName=${encodeURIComponent(username)}&roles=${encodeURIComponent(role)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.DESTINATIONS: {
                const { city = '', airport = '' } = searchParams;
                if (city || airport) {
                    url = `${apiUrl}/${ENTITIES.DESTINATIONS}/search?city=${encodeURIComponent(city)}&airport=${encodeURIComponent(airport)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.FLIGHTS: {
                const { startDate = '', endDate = '' } = searchParams;
                if (startDate || endDate) {
                    url = `${apiUrl}/${ENTITIES.FLIGHTS}/search?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.PASSENGERS: {
                const { firstName = '', lastName = '', uprn = '', passport = '', address = '', phone = '' } = searchParams;
                if (firstName || lastName || uprn || passport || address || phone) {
                    url = `${apiUrl}/${ENTITIES.PASSENGERS}/search?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&uprn=${encodeURIComponent(uprn)}&passport=${encodeURIComponent(passport)}&address=${encodeURIComponent(address)}&phone=${encodeURIComponent(phone)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.PILOTS: {
                const { firstName = '', lastName = '', uprn = '', flyingHours = '' } = searchParams;
                if (firstName || lastName || uprn || flyingHours) {
                    url = `${apiUrl}/${ENTITIES.PILOTS}/search?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&uprn=${encodeURIComponent(uprn)}&flyingHours=${encodeURIComponent(flyingHours)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.PLANE_TICKETS: {
                const { price = '', purchaseDate = '', seatNumber = '' } = searchParams;
                if (price || purchaseDate || seatNumber) {
                    url = `${apiUrl}/${ENTITIES.PLANE_TICKETS}/search?price=${encodeURIComponent(price)}&purchaseDate=${encodeURIComponent(purchaseDate)}&seatNumber=${encodeURIComponent(seatNumber)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.HEALTH_CHECKS: {
                url = `${apiUrl}/${ENTITIES.HEALTH_CHECKS}`;
                break;
            }

            default:
                url = `${apiUrl}/${dataType}?${paginationParams}`;
                break;
        }
    }
    return url;
}

export async function fetchData(dataType, dataId, apiUrl, page = 1, rowsPerPage, signal, searchParams = {}) {
    try {
        if (!apiUrl) {
            throw new CustomAPIError('CONFIG_ERROR', 'API URL is not available');
        }

        const url = buildURL(apiUrl, dataType, dataId, page, rowsPerPage, searchParams);

        const response = await fetch(url, {
            headers: buildHeaders(),
            signal,
        });

        if (response.ok) {
            if (response.status === 204) {
                return { data: [], dataExist: false };
            }
            const responseData = await response.json();
            return { data: responseData, dataExist: true };
        } else {
            const errorMessage = await generateErrorMessage(response, dataType, dataId);
            throw new CustomAPIError('API_ERROR', errorMessage);
        }
    } catch (error) {
        if (error.name === 'AbortError') {
            throw error;
        }

        const networkErrorObject = handleNetworkError(error);

        if (networkErrorObject) {
            throw new CustomAPIError('NETWORK_ERROR', networkErrorObject.message);
        }

        if (error instanceof CustomAPIError) {
            throw error;
        } else {
            logger.error('Error fetching data:', error);
            const message = error.message || 'An unexpected error occurred during the fetch operation.';
            throw new CustomAPIError('UNEXPECTED_ERROR', message);
        }
    }
}
