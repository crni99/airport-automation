import { Entities } from '../../utils/const.js';

export const validateField = (type, field, value) => {
    switch (type) {
        case Entities.AIRLINES:
            if (field === 'name') {
                if (!value.trim()) {
                    return "Airline name is required.";
                }
                if (value.length > 255) {
                    return "Airline name cannot exceed 255 characters.";
                }
            }
            break;
        case Entities.DESTINATIONS:
            if (field === 'city') {
                if (!value.trim()) {
                    return "Destination city is required.";
                }
                if (value.length > 255) {
                    return "Destination city cannot exceed 255 characters.";
                }
            }
            if (field === 'airport') {
                if (!value.trim()) {
                    return "Destination airport is required.";
                }
                if (value.length > 255) {
                    return "Destination airport cannot exceed 255 characters.";
                }
            }
            break;
        case Entities.PASSENGERS:
            if (field === 'firstName') {
                if (!value.trim()) {
                    return "Passenger first name is required.";
                }
                if (value.length > 50) {
                    return "Passenger first name cannot exceed 50 characters.";
                }
            }
            if (field === 'lastName') {
                if (!value.trim()) {
                    return "Passenger last name is required.";
                }
                if (value.length > 50) {
                    return "Passenger last name cannot exceed 50 characters.";
                }
            }
            if (field === 'uprn') {
                if (!value.trim()) {
                    return "Passenger UPRN is required.";
                }
                if (value.length !== 13) {
                    return "Passenger UPRN must be exactly 13 characters.";
                }
            }
            if (field === 'passport') {
                if (!value.trim()) {
                    return "Passenger passport is required.";
                }
                if (value.length !== 9) {
                    return "Passenger passport must be exactly 9 characters.";
                }
            }
            if (field === 'address') {
                if (!value.trim()) {
                    return "Passenger address is required.";
                }
                if (value.length > 200) {
                    return "Passenger address cannot exceed 200 characters.";
                }
            }
            if (field === 'phone') {
                if (!value.trim()) {
                    return "Passenger phone is required.";
                }
                if (value.length > 30) {
                    return "Passenger phone cannot exceed 30 characters.";
                }
            }
            break;
        case Entities.PILOTS:
            if (field === 'firstName') {
                if (!value.trim()) {
                    return "Pilot first name is required.";
                }
                if (value.length > 50) {
                    return "Pilot first name cannot exceed 50 characters.";
                }
            }
            if (field === 'lastName') {
                if (!value.trim()) {
                    return "Pilot last name is required.";
                }
                if (value.length > 50) {
                    return "Pilot last name cannot exceed 50 characters.";
                }
            }
            if (field === 'uprn') {
                if (!value.trim()) {
                    return "Pilot UPRN is required.";
                }
                if (value.length !== 13) {
                    return "Pilot UPRN must be exactly 13 characters.";
                }
            }
            if (field === 'flyingHours') {
                if (!value.trim() || isNaN(value)) {
                    return "Pilot flying hours is required and must be a number.";
                }
                const flyingHours = parseInt(value, 10);
                if (flyingHours > 40000) {
                    return "Pilot flying hours cannot exceed 40000";
                }
                if (flyingHours < 0) {
                    return "Pilot flying hours cannot be lower than 0.";
                }
            }
            break;
        case Entities.API_USERS:
            if (field === 'userName') {
                if (!value.trim()) {
                    return "ApiUser username is required.";
                }
                if (value.length > 255) {
                    return "ApiUser username cannot exceed 50 characters.";
                }
            }
            if (field === 'password') {
                if (!value.trim()) {
                    return "ApiUser password is required.";
                }
                if (value.length > 255) {
                    return "ApiUser password cannot exceed 50 characters.";
                }
            }
            break;
        case Entities.FLIGHTS:
            if (field === 'departureDate') {
                if (!value.trim()) {
                    return "Flight departure date is required.";
                }
                const datePattern = /^\d{4}-\d{2}-\d{2}$/;
                if (!datePattern.test(value)) {
                    return "Flight departure date must be in the format YYYY-MM-DD.";
                }
            }
            if (field === 'departureTime') {
                if (!value.trim()) {
                    return "Flight departure time is required.";
                }
                const timePattern = /^([01]?[0-9]|2[0-3]):([0-5]?[0-9])$/;
                if (!timePattern.test(value)) {
                    return "Flight departure time must be in the format HH:mm.";
                }
            }
            if (field === 'airlineId') {
                if (!value || isNaN(value)) {
                    return "Airline ID is required and must be a number.";
                }
                if (value === null || value === "") {
                    return "Airline ID cannot be null or empty.";
                }
            }

            if (field === 'destinationId') {
                if (!value || isNaN(value)) {
                    return "Destination ID is required and must be a number.";
                }
                if (value === null || value === "") {
                    return "Destination ID cannot be null or empty.";
                }
            }

            if (field === 'pilotId') {
                if (!value || isNaN(value)) {
                    return "Pilot ID is required and must be a number.";
                }
                if (value === null || value === "") {
                    return "Pilot ID cannot be null or empty.";
                }
            }
            break;
        case Entities.PLANE_TICKETS:
            if (field === 'price') {
                const priceValue = String(value).trim();
                if (!priceValue || isNaN(priceValue)) {
                    return "Price is required and must be a number.";
                }
                const price = parseInt(priceValue, 10);
                if (price < 0) {
                    return "Price cannot be lower than 0.";
                }
            }
            if (field === 'purchaseDate') {
                const purchaseDateValue = String(value).trim();
                if (!purchaseDateValue) {
                    return "Purchase date is required.";
                }
                const datePattern = /^\d{4}-\d{2}-\d{2}$/;
                if (!datePattern.test(purchaseDateValue)) {
                    return "Purchase date must be in the format YYYY-MM-DD.";
                }
            }
            if (field === 'seatNumber') {
                const seatNumberValue = String(value).trim();
                if (!seatNumberValue || isNaN(seatNumberValue)) {
                    return "Seat number is required and must be a number.";
                }
                const seatNumber = parseInt(seatNumberValue, 10);
                if (seatNumber < 0) {
                    return "Seat number cannot be lower than 0.";
                }
            }
            if (field === 'passengerId') {
                if (!value || isNaN(value)) {
                    return "Passenger ID is required and must be a number.";
                }
                if (value === null || value === "") {
                    return "Passenger ID cannot be null or empty.";
                }
            }
            if (field === 'travelClassId') {
                if (!value || isNaN(value)) {
                    return "Travel class ID is required and must be a number.";
                }
                if (value === null || value === "") {
                    return "Travel class ID cannot be null or empty.";
                }
            }
            if (field === 'flightId') {
                if (!value || isNaN(value)) {
                    return "Flight ID is required and must be a number.";
                }
                if (value === null || value === "") {
                    return "Flight ID cannot be null or empty.";
                }
            }
            break;
        default:
            return null;
    }
    return null;
}