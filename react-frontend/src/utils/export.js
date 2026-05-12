import { ENTITIES } from '../utils/const.js';

export function buildExportURL(apiUrl, dataType, exportType, searchParams = {}) {
    
    const typeSegment = exportType ? exportType.toLowerCase() : 'excel';
    const baseUrl = `${apiUrl}/${dataType}/Export/${typeSegment}`;
    const params = {};

    switch (dataType) {
        case ENTITIES.AIRLINES: {
            const { name } = searchParams;
            if (name) params.name = name;
            break;
        }
        case ENTITIES.API_USERS: {
            const { username, role } = searchParams;
            if (username) params.username = username;
            if (role) params.roles = role;
            break;
        }
        case ENTITIES.DESTINATIONS: {
            const { city, airport } = searchParams;
            if (city) params.city = city;
            if (airport) params.airport = airport;
            break;
        }
        case ENTITIES.FLIGHTS: {
            const { startDate, endDate } = searchParams;
            if (startDate) params.startDate = startDate;
            if (endDate) params.endDate = endDate;
            break;
        }
        case ENTITIES.PASSENGERS: {
            const { firstName, lastName, uprn, passport, address, phone } = searchParams;
            if (firstName) params.firstName = firstName;
            if (lastName) params.lastName = lastName;
            if (uprn) params.uprn = uprn;
            if (passport) params.passport = passport;
            if (address) params.address = address;
            if (phone) params.phone = phone;
            break;
        }
        case ENTITIES.PILOTS: {
            const { firstName, lastName, uprn, flyingHours } = searchParams;
            if (firstName) params.firstName = firstName;
            if (lastName) params.lastName = lastName;
            if (uprn) params.uprn = uprn;
            if (flyingHours) params.flyingHours = flyingHours;
            break;
        }
        case ENTITIES.PLANE_TICKETS: {
            const { price, purchaseDate, seatNumber } = searchParams;
            if (price) params.price = price;
            if (purchaseDate) params.purchaseDate = purchaseDate;
            if (seatNumber) params.seatNumber = seatNumber;
            break;
        }
        default:
            break;
    }

    if (Object.keys(params).length === 0) {
        params.getAll = 'true';
    }

    return { baseUrl, params };
}