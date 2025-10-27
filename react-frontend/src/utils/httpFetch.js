import { getAuthToken } from './auth.js';
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

function buildURL(apiUrl, dataType, dataId, page, pageSize) {
    let url = `${apiUrl}/${dataType}`;

    if (dataId !== null) {
        url += `/${dataId}`;
    } else {
        let paginationParams = `page=${page}&pageSize=${pageSize || 10}`;
        url += `?${paginationParams}`;

        switch (dataType) {
            case ENTITIES.AIRLINES: {
                const searchName = document.getElementById('name')?.value?.trim();

                if (searchName) {
                    url = `${apiUrl}/${ENTITIES.AIRLINES}/search?name=${encodeURIComponent(searchName)}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.API_USERS: {
                const username = document.getElementById('username')?.value?.trim();
                const searchRole = document.querySelector('input[name="role"]')?.value?.trim();

                if (username || searchRole) {
                    url = `${apiUrl}/${ENTITIES.API_USERS}/search?userName=${encodeURIComponent(username || '')}&roles=${encodeURIComponent(searchRole || '')}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.DESTINATIONS: {
                const city = document.getElementById('city')?.value?.trim();
                const airport = document.getElementById('airport')?.value?.trim();

                if (city || airport) {
                    url = `${apiUrl}/${ENTITIES.DESTINATIONS}/search?city=${encodeURIComponent(city || '')}&airport=${encodeURIComponent(airport || '')}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.FLIGHTS: {
                const startDate = document.getElementById('startDate')?.value?.trim();
                const endDate = document.getElementById('endDate')?.value?.trim();

                if (startDate || endDate) {
                    url = `${apiUrl}/${ENTITIES.FLIGHTS}/search?startDate=${encodeURIComponent(startDate || '')}&endDate=${encodeURIComponent(endDate || '')}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.PASSENGERS: {
                const firstName = document.getElementById('firstName')?.value?.trim();
                const lastName = document.getElementById('lastName')?.value?.trim();
                const uprn = document.getElementById('uprn')?.value?.trim();
                const passport = document.getElementById('passport')?.value?.trim();
                const address = document.getElementById('address')?.value?.trim();
                const phone = document.getElementById('phone')?.value?.trim();

                if (firstName || lastName || uprn || passport || address || phone) {
                    url = `${apiUrl}/${ENTITIES.PASSENGERS}/search?firstName=${encodeURIComponent(firstName || '')}&lastName=${encodeURIComponent(lastName || '')}&uprn=${encodeURIComponent(uprn || '')}&passport=${encodeURIComponent(passport || '')}&address=${encodeURIComponent(address || '')}&phone=${encodeURIComponent(phone || '')}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.PILOTS: {
                const firstName = document.getElementById('firstName')?.value?.trim();
                const lastName = document.getElementById('lastName')?.value?.trim();
                const uprn = document.getElementById('uprn')?.value?.trim();
                const flyingHours = document.getElementById('flyingHours')?.value?.trim();

                if (firstName || lastName || uprn || flyingHours) {
                    url = `${apiUrl}/${ENTITIES.PILOTS}/search?firstName=${encodeURIComponent(firstName || '')}&lastName=${encodeURIComponent(lastName || '')}&uprn=${encodeURIComponent(uprn || '')}&flyingHours=${encodeURIComponent(flyingHours || '')}&${paginationParams}`;
                }
                break;
            }

            case ENTITIES.PLANE_TICKETS: {
                const price = document.getElementById('price')?.value?.trim();
                const purchaseDate = document.getElementById('purchaseDate')?.value?.trim();
                const seatNumber = document.getElementById('seatNumber')?.value?.trim();

                if (price || purchaseDate || seatNumber) {
                    url = `${apiUrl}/${ENTITIES.PLANE_TICKETS}/search?price=${encodeURIComponent(price || '')}&purchaseDate=${encodeURIComponent(purchaseDate || '')}&seatNumber=${encodeURIComponent(seatNumber || '')}&${paginationParams}`;
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

export async function fetchData(dataType, dataId, apiUrl, page = 1, rowsPerPage, signal) {
    try {
        if (!apiUrl) {
            throw new CustomAPIError('CONFIG_ERROR', 'API URL is not available');
        }

        const url = buildURL(apiUrl, dataType, dataId, page, rowsPerPage);

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
            console.error('Error fetching data:', error);
            const message = error.message || 'An unexpected error occurred during the fetch operation.';
            throw new CustomAPIError('UNEXPECTED_ERROR', message);
        }
    }
}