import AirlinesIcon from '@mui/icons-material/Airlines';
import TravelExploreIcon from '@mui/icons-material/TravelExplore';
import FlightClassIcon from '@mui/icons-material/FlightClass';
import Person4Icon from '@mui/icons-material/Person4';
import FlightIcon from '@mui/icons-material/Flight';
import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import AirplaneTicketIcon from '@mui/icons-material/AirplaneTicket';
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts';

export const ENTITIES = {
    AIRLINES: 'Airlines',
    API_USERS: 'ApiUsers',
    DESTINATIONS: 'Destinations',
    FLIGHTS: 'Flights',
    PASSENGERS: 'Passengers',
    PILOTS: 'Pilots',
    PLANE_TICKETS: 'PlaneTickets',
    TRAVEL_CLASSES: 'TravelClasses',
    HEALTH_CHECKS: 'HealthCheck',
};

export const ENTITY_PATHS = {
    AIRLINES: 'airlines',
    API_USERS: 'api-users',
    DESTINATIONS: 'destinations',
    FLIGHTS: 'flights',
    PASSENGERS: 'passengers',
    PILOTS: 'pilots',
    PLANE_TICKETS: 'plane-tickets',
    TRAVEL_CLASSES: 'travel-classes',
    HEALTH_CHECKS: 'health-checks',
}

export const ROLES = {
    SUPER_ADMIN: 'SuperAdmin',
    ADMIN: 'Admin',
    USER: 'User'
}

export const MAIN_NAVBAR_ITEMS = [
    {
        id: 0,
        icon: <AirlinesIcon />,
        label: ENTITIES.AIRLINES,
        route: 'airlines',
    },
    {
        id: 1,
        icon: <TravelExploreIcon />,
        label: ENTITIES.DESTINATIONS,
        route: 'destinations',
    },
    {
        id: 2,
        icon: <FlightClassIcon />,
        label: 'Travel Classes',
        route: 'travel-classes',
    },
    {
        id: 3,
        icon: <Person4Icon />,
        label: ENTITIES.PILOTS,
        route: 'pilots',
    },
    {
        id: 4,
        icon: <FlightIcon />,
        label: ENTITIES.FLIGHTS,
        route: 'flights',
    },
    {
        id: 5,
        icon: <PeopleAltIcon />,
        label: ENTITIES.PASSENGERS,
        route: 'passengers',
    },
    {
        id: 6,
        icon: <AirplaneTicketIcon />,
        label: 'Tickets',
        route: 'plane-tickets',
    },
    {
        id: 7,
        icon: <ManageAccountsIcon />,
        label: 'Api Users',
        route: 'api-users',
    },
]