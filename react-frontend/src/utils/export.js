import { Entities } from '../utils/const.js';

export function buildExportURL(apiUrl, dataType, exportType) {
    const typeSegment = exportType ? exportType.toLowerCase() : 'excel';
    const baseUrl = `${apiUrl}/${dataType}/Export/${typeSegment}`;
    const params = {};

    const getInputValue = (id) => document.getElementById(id)?.value?.trim() || '';

    switch (dataType) {
        case Entities.AIRLINES: {
            const searchName = getInputValue('searchInput');
            if (searchName) params.name = searchName;
            break;
        }
        case Entities.API_USERS: {
            const username = getInputValue('username');
            const password = getInputValue('password');
            const searchRole = getInputValue('roleSelect');
            if (username) params.username = username;
            if (password) params.password = password;
            if (searchRole) params.roles = searchRole;
            break;
        }
        case Entities.DESTINATIONS: {
            const city = getInputValue('city');
            const airport = getInputValue('airport');
            if (city) params.city = city;
            if (airport) params.airport = airport;
            break;
        }
        case Entities.FLIGHTS: {
            const startDate = getInputValue('startDate');
            const endDate = getInputValue('endDate');
            if (startDate) params.startDate = startDate;
            if (endDate) params.endDate = endDate;
            break;
        }
        case Entities.PASSENGERS: {
            const firstName = getInputValue('firstName');
            const lastName = getInputValue('lastName');
            const uprn = getInputValue('uprn');
            const passport = getInputValue('passport');
            const address = getInputValue('address');
            const phone = getInputValue('phone');
            if (firstName) params.firstName = firstName;
            if (lastName) params.lastName = lastName;
            if (uprn) params.uprn = uprn;
            if (passport) params.passport = passport;
            if (address) params.address = address;
            if (phone) params.phone = phone;
            break;
        }
        case Entities.PILOTS: {
            const firstName = getInputValue('firstName');
            const lastName = getInputValue('lastName');
            const uprn = getInputValue('uprn');
            const flyingHours = getInputValue('flyingHours');
            if (firstName) params.firstName = firstName;
            if (lastName) params.lastName = lastName;
            if (uprn) params.uprn = uprn;
            if (flyingHours) params.flyingHours = flyingHours;
            break;
        }
        case Entities.PLANE_TICKETS: {
            const price = getInputValue('price');
            const purchaseDate = getInputValue('purchaseDate');
            const seatNumber = getInputValue('seatNumber');
            if (price) params.price = price;
            if (purchaseDate) params.purchaseDate = purchaseDate;
            if (seatNumber) params.seatNumber = seatNumber;
            break;
        }
        case Entities.HEALTH_CHECKS: {
            break;
        }
        default: {
            break;
        }
    }

    if (Object.keys(params).length === 0) {
        params.getAll = 'true';
    }

    return { baseUrl, params };
}
