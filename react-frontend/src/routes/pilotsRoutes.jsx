import React, { lazy } from "react";
import { Route } from "react-router-dom";
import RequireAdminRole from "../routes/RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const PilotsList = lazy(() => import('../pages/pilot/PilotsList'));
const PilotDetails = lazy(() => import('../pages/pilot/PilotDetails'));
const PilotCreateForm = lazy(() => import('../pages/pilot/PilotCreateForm'));
const PilotEditForm = lazy(() => import('../pages/pilot/PilotEditForm'));

const PilotsRoutes = (
        <>
            <Route path={ENTITY_PATHS.PILOTS} element={<PilotsList />} />
            <Route path={`${ENTITY_PATHS.PILOTS}/:id`} element={<PilotDetails />} />
            <Route path={`${ENTITY_PATHS.PILOTS}/create`} element={<RequireAdminRole element={<PilotCreateForm />} />} />
            <Route path={`${ENTITY_PATHS.PILOTS}/edit/:id`} element={<RequireAdminRole element={<PilotEditForm />} />} />
        </>
);

export default PilotsRoutes;