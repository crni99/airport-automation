import React, { lazy } from "react";
import { Route } from "react-router-dom";
import RequireAdminRole from "./RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const FlightsList = lazy(() => import('../pages/flight/FlightsList'));
const FlightDetails = lazy(() => import('../pages/flight/FlightDetails'));
const FlightCreateForm = lazy(() => import('../pages/flight/FlightCreateForm'));
const FlightEditForm = lazy(() => import('../pages/flight/FlightEditForm'));

const FlightsRoutes = (
        <>
            <Route path={ENTITY_PATHS.FLIGHTS} element={<FlightsList />} />
            <Route path={`${ENTITY_PATHS.FLIGHTS}/:id`} element={<FlightDetails />} />
            <Route path={`${ENTITY_PATHS.FLIGHTS}/create`} element={<RequireAdminRole element={<FlightCreateForm />} />} />
            <Route path={`${ENTITY_PATHS.FLIGHTS}/edit/:id`} element={<RequireAdminRole element={<FlightEditForm />} />} />
        </>
);

export default FlightsRoutes;