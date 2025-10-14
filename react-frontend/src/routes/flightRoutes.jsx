import React from "react";
import { Route } from "react-router-dom";
import FlightsList from "../pages/flight/FlightsList";
import FlightDetails from "../pages/flight/FlightDetails";
import FlightCreateForm from "../pages/flight/FlightCreateForm";
import FlightEditForm from "../pages/flight/FlightEditForm";
import ProtectedRoute from "./ProtectedRoute";
import { ENTITY_PATHS } from '../utils/const';

const FlightsRoutes = (
    <>
        <Route
            path={ENTITY_PATHS.FLIGHTS}
            element={<FlightsList />}
        />
        <Route
            path={`${ENTITY_PATHS.FLIGHTS}/:id`}
            element={<FlightDetails />}
        />
        <Route
            path={`${ENTITY_PATHS.FLIGHTS}/create`}
            element={<ProtectedRoute element={<FlightCreateForm />} />}
        />
        <Route
            path={`${ENTITY_PATHS.FLIGHTS}/edit/:id`}
            element={<ProtectedRoute element={<FlightEditForm />} />}
        />
    </>
);

export default FlightsRoutes;