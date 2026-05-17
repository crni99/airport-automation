import React, { lazy } from "react";
import { Route } from "react-router-dom";
import RequireAdminRole from "./RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const DestinationsList = lazy(() => import('../pages/destination/DestinationsList'));
const DestinationDetails = lazy(() => import('../pages/destination/DestinationDetails'));
const DestinationCreateForm = lazy(() => import('../pages/destination/DestinationCreateForm'));
const DestinationEditForm = lazy(() => import('../pages/destination/DestinationEditForm'));

const DestinationsRoutes = (
        <>
            <Route path={ENTITY_PATHS.DESTINATIONS} element={<DestinationsList />} />
            <Route path={`${ENTITY_PATHS.DESTINATIONS}/:id`} element={<DestinationDetails />} />
            <Route path={`${ENTITY_PATHS.DESTINATIONS}/create`} element={<RequireAdminRole element={<DestinationCreateForm />} />} />
            <Route path={`${ENTITY_PATHS.DESTINATIONS}/edit/:id`} element={<RequireAdminRole element={<DestinationEditForm />} />} />
        </>
);

export default DestinationsRoutes;