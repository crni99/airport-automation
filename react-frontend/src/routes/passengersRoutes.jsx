import React, { lazy } from "react";
import { Route } from "react-router-dom";
import RequireAdminRole from "../routes/RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const PassengersList = lazy(() => import('../pages/passenger/PassengersList'));
const PassengerDetails = lazy(() => import('../pages/passenger/PassengerDetails'));
const PassengerCreateForm = lazy(() => import('../pages/passenger/PassengerCreateForm'));
const PassengerEditForm = lazy(() => import('../pages/passenger/PassengerEditForm'));

const PassengersRoutes = (
        <>
            <Route path={ENTITY_PATHS.PASSENGERS} element={<PassengersList />} />
            <Route path={`${ENTITY_PATHS.PASSENGERS}/:id`} element={<PassengerDetails />} />
            <Route path={`${ENTITY_PATHS.PASSENGERS}/create`} element={<RequireAdminRole element={<PassengerCreateForm />} />} />
            <Route path={`${ENTITY_PATHS.PASSENGERS}/edit/:id`} element={<RequireAdminRole element={<PassengerEditForm />} />} />
        </>
);

export default PassengersRoutes;