import React, { lazy } from "react";
import { Route } from "react-router-dom";
import RequireAdminRole from "./RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const PlaneTicketsList = lazy(() => import('../pages/planeTicket/PlaneTicketsList'));
const PlaneTicketDetails = lazy(() => import('../pages/planeTicket/PlaneTicketDetails'));
const PlaneTicketCreateForm = lazy(() => import('../pages/planeTicket/PlaneTicketCreateForm'));
const PlaneTicketEditForm = lazy(() => import('../pages/planeTicket/PlaneTicketEditForm'));

const PlaneTicketsRoutes = (
        <>
            <Route path={ENTITY_PATHS.PLANE_TICKETS} element={<PlaneTicketsList />} />
            <Route path={`${ENTITY_PATHS.PLANE_TICKETS}/:id`} element={<PlaneTicketDetails />} />
            <Route path={`${ENTITY_PATHS.PLANE_TICKETS}/create`} element={<RequireAdminRole element={<PlaneTicketCreateForm />} />} />
            <Route path={`${ENTITY_PATHS.PLANE_TICKETS}/edit/:id`} element={<RequireAdminRole element={<PlaneTicketEditForm />} />} />
        </>
);

export default PlaneTicketsRoutes;