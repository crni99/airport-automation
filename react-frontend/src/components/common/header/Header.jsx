import React, { useEffect } from 'react';
import { handleSignOut, getAuthToken, getRole } from "../../../utils/auth";
import { setupThemeToggle } from "../../../utils/themeToggle";
import NavItem from './NavItem';
import NavDropdown from './NavDropdown';

export default function Header() {

    const isLoggedIn = getAuthToken() !== null;
    const role = getRole();

    const handleSignOutClick = () => {
        handleSignOut();
    };

    useEffect(() => {
        const cleanupToggle = setupThemeToggle('darkModeToggle');
        return () => {
            if (cleanupToggle) {
                cleanupToggle();
            }
        };
    }, []);

    return (
        <header>
            <nav className="navbar navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3">
                <div className="container-fluid">
                    <a className="navbar-brand" href="/">AirportAutomationReact</a>
                    <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                        {isLoggedIn && (
                            <ul className="navbar-nav flex-grow-1">
                                <NavDropdown type="Passengers" text="Passenger" items={[
                                    { label: "All Passengers", action: "" },
                                    ...(role !== 'User' ? [{ label: "Add Passenger", action: "Create" }] : [])
                                ]} />
                                <NavItem type="TravelClasses" text="Travel Class" />
                                <NavDropdown type="Destinations" text="Destination" items={[
                                    { label: "All Destinations", action: "" },
                                    ...(role !== 'User' ? [{ label: "Add Destination", action: "Create" }] : [])
                                ]} />
                                <NavDropdown type="Pilots" text="Pilot" items={[
                                    { label: "All Pilots", action: "" },
                                    ...(role !== 'User' ? [{ label: "Add Pilot", action: "Create" }] : [])
                                ]} />
                                <NavDropdown type="Airlines" text="Airline" items={[
                                    { label: "All Airlines", action: "" },
                                    ...(role !== 'User' ? [{ label: "Add Airline", action: "Create" }] : [])
                                ]} />
                                <NavDropdown type="Flights" text="Flight" items={[
                                    { label: "All Flights", action: "" },
                                    ...(role !== 'User' ? [{ label: "Add Flight", action: "Create" }] : [])
                                ]} />
                                <NavDropdown type="PlaneTickets" text="Plane Ticket" items={[
                                    { label: "All Plane Tickets", action: "" },
                                    ...(role !== 'User' ? [{ label: "Add Plane Ticket", action: "Create" }] : [])
                                ]} />
                                {role === 'SuperAdmin' && (
                                    <NavItem type="ApiUsers" text="Api User" />
                                )}
                            </ul>
                        )}
                        <ul className="navbar-nav ms-auto">
                            <li className="nav-item">
                                <button id="darkModeToggle" className="btn btn-secondary">Dark Mode</button>
                            </li>
                            {isLoggedIn && (
                                <NavItem type="HealthCheck" text="Status Check" />
                            )}
                            {isLoggedIn && (
                                <li className="nav-item">
                                    <a id="signOut" className="nav-link" onClick={handleSignOutClick} href="/">Sign Out</a>
                                </li>
                            )}
                        </ul>
                    </div>
                </div>
            </nav>
        </header>
    );
}
