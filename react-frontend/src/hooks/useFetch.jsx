import { useState, useEffect, useCallback } from 'react';
import { getAuthToken } from '../utils/auth.js';
import { useContext } from 'react';
import { DataContext } from '../store/data-context.jsx';
import { generateErrorMessage, handleNetworkError } from '../utils/errorUtils.js';
import { Entities } from '../utils/const.js';

export default function useFetch(dataType, dataId, page = 1, triggerFetch, rowsPerPage) {
    const dataCtx = useContext(DataContext);

    const [data, setData] = useState(null);
    const [dataExist, setDataExist] = useState(false);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isError, setIsError] = useState(false);

    const handleResponse = useCallback(async (response) => {
        try {
            if (response.ok) {
                if (response.status === 204) {
                    setData([]);
                    setDataExist(false);
                } else {
                    const responseData = await response.json();
                    setData(responseData);
                    setDataExist(true);
                }
            } else {
                throw new Error(await generateErrorMessage(response, dataType, dataId));
            }
        } catch (error) {
            handleFetchError(error);
        }
    }, [dataType, dataId]);

    useEffect(() => {
        const controller = new AbortController();
        const signal = controller.signal;

        async function fetchData() {
            setIsLoading(true);
            try {
                if (!dataCtx || !dataCtx.apiUrl) {
                    throw new Error('API URL is not available');
                }
                const url = buildURL(dataCtx.apiUrl, dataType, dataId, page, rowsPerPage);
                const response = await fetch(url, {
                    headers: buildHeaders(),
                    signal,
                });

                if (!signal.aborted) {
                    await handleResponse(response);
                }
            } catch (error) {
                if (error.name !== 'AbortError') {
                    handleFetchError(error);
                }
            } finally {
                if (!signal.aborted) {
                    setIsLoading(false);
                }
            }
        }
        fetchData();

        return () => {
            controller.abort();
        };
    }, [dataType, dataId, page, dataCtx, handleResponse, triggerFetch, rowsPerPage]);


    function buildURL(apiUrl, dataType, dataId, page, pageSize) {
        let url = `${apiUrl}/${dataType}`;

        if (dataId !== null) {
            url += `/${dataId}`;
        } else {
            let paginationParams = `page=${page}&pageSize=${pageSize || 10}`;
            url += `?${paginationParams}`;

            switch (dataType) {
                case Entities.AIRLINES: {
                    const searchName = document.getElementById('searchInput')?.value?.trim();
                    if (searchName) {
                        url = `${apiUrl}/${Entities.AIRLINES}/ByName/${encodeURIComponent(searchName)}?${paginationParams}`;
                    }
                    break;
                }

                case Entities.API_USERS: {
                    const username = document.getElementById('username')?.value?.trim();
                    const password = document.getElementById('password')?.value?.trim();
                    const searchRole = document.getElementById('roleSelect')?.value?.trim();

                    if (username || password || searchRole) {
                        url = `${apiUrl}/${Entities.API_USERS}/ByFilter?username=${encodeURIComponent(username || '')}&password=${encodeURIComponent(password || '')}&roles=${encodeURIComponent(searchRole || '')}&${paginationParams}`;
                    }
                    break;
                }

                case Entities.DESTINATIONS: {
                    const city = document.getElementById('city')?.value?.trim();
                    const airport = document.getElementById('airport')?.value?.trim();

                    if (city || airport) {
                        url = `${apiUrl}/${Entities.DESTINATIONS}/search?city=${encodeURIComponent(city || '')}&airport=${encodeURIComponent(airport || '')}&${paginationParams}`;
                    }
                    break;
                }

                case Entities.FLIGHTS: {
                    const startDate = document.getElementById('startDate')?.value?.trim();
                    const endDate = document.getElementById('endDate')?.value?.trim();

                    if (startDate || endDate) {
                        url = `${apiUrl}/${Entities.FLIGHTS}/byDate?startDate=${encodeURIComponent(startDate || '')}&endDate=${encodeURIComponent(endDate || '')}&${paginationParams}`;
                    }
                    break;
                }

                case Entities.PASSENGERS: {
                    const firstName = document.getElementById('firstName')?.value?.trim();
                    const lastName = document.getElementById('lastName')?.value?.trim();
                    const uprn = document.getElementById('uprn')?.value?.trim();
                    const passport = document.getElementById('passport')?.value?.trim();
                    const address = document.getElementById('address')?.value?.trim();
                    const phone = document.getElementById('phone')?.value?.trim();

                    if (firstName || lastName || uprn || passport || address || phone) {
                        url = `${apiUrl}/${Entities.PASSENGERS}/byFilter?firstName=${encodeURIComponent(firstName || '')}&lastName=${encodeURIComponent(lastName || '')}&uprn=${encodeURIComponent(uprn || '')}&passport=${encodeURIComponent(passport || '')}&address=${encodeURIComponent(address || '')}&phone=${encodeURIComponent(phone || '')}&${paginationParams}`;
                    }
                    break;
                }

                case Entities.PILOTS: {
                    const firstName = document.getElementById('firstName')?.value?.trim();
                    const lastName = document.getElementById('lastName')?.value?.trim();
                    const uprn = document.getElementById('uprn')?.value?.trim();
                    const flyingHours = document.getElementById('flyingHours')?.value?.trim();

                    if (firstName || lastName || uprn || flyingHours) {
                        url = `${apiUrl}/${Entities.PILOTS}/byFilter?firstName=${encodeURIComponent(firstName || '')}&lastName=${encodeURIComponent(lastName || '')}&uprn=${encodeURIComponent(uprn || '')}&flyingHours=${encodeURIComponent(flyingHours || '')}&${paginationParams}`;
                    }
                    break;
                }

                case Entities.PLANE_TICKETS: {
                    const price = document.getElementById('price')?.value?.trim();
                    const purchaseDate = document.getElementById('purchaseDate')?.value?.trim();
                    const seatNumber = document.getElementById('seatNumber')?.value?.trim();

                    if (price || purchaseDate || seatNumber) {
                        url = `${apiUrl}/${Entities.PLANE_TICKETS}/byFilter?price=${encodeURIComponent(price || '')}&purchaseDate=${encodeURIComponent(purchaseDate || '')}&seatNumber=${encodeURIComponent(seatNumber || '')}&${paginationParams}`;
                    }
                    break;
                }

                case Entities.HEALTH_CHECKS: {
                    url = `${apiUrl}/${Entities.HEALTH_CHECKS}`;
                    break;
                }

                default:
                    url = `${apiUrl}/${dataType}?${paginationParams}`;
                    break;
            }
        }
        return url;
    }

    function buildHeaders() {
        const headers = { 'Content-Type': 'application/json' };
        const authToken = getAuthToken();
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }
        return headers;
    }

    function handleFetchError(error) {
        const networkError = handleNetworkError(error);
        if (networkError) {
            setError({
                type: networkError.type || 'Network Error',
                message: networkError.message || 'A network issue occurred.'
            });
        } else {
            setError({
                type: error.name || 'Fetch Error',
                message: error.message || 'An unknown error occurred.'
            });
        }
        setIsError(true);
    }

    return { data, dataExist, error, isLoading, isError };
}