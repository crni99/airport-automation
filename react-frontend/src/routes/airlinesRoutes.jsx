import React, { lazy } from "react";
import { Route } from "react-router-dom";
import RequireAdminRole from "./RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const AirlinesList = lazy(() => import('../pages/airline/AirlinesList'));
const AirlineDetails = lazy(() => import('../pages/airline/AirlineDetails'));
const AirlineCreateForm = lazy(() => import('../pages/airline/AirlineCreateForm'));
const AirlineEditForm = lazy(() => import('../pages/airline/AirlineEditForm'));

const AirlinesRoutes = (
        <>
            <Route path={ENTITY_PATHS.AIRLINES} element={<AirlinesList />} />
            <Route path={`${ENTITY_PATHS.AIRLINES}/:id`} element={<AirlineDetails />} />
            <Route path={`${ENTITY_PATHS.AIRLINES}/create`} element={<RequireAdminRole element={<AirlineCreateForm />} />} />
            <Route path={`${ENTITY_PATHS.AIRLINES}/edit/:id`} element={<RequireAdminRole element={<AirlineEditForm />} />} />
        </>
);

export default AirlinesRoutes;