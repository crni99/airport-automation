import React from "react";
import { Route } from "react-router-dom";
import PlaneTicketsList from "../pages/planeTicket/PlaneTicketsList";
import PlaneTicketDetails from "../pages/planeTicket/PlaneTicketDetails";
import PlaneTicketCreateForm from "../pages/planeTicket/PlaneTicketCreateForm";
import PlaneTicketEditForm from "../pages/planeTicket/PlaneTicketEditForm";
import RequireAdminRole from "./RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const PlaneTicketsRoutes = (
    <>
        <Route 
            path={ENTITY_PATHS.PLANE_TICKETS} 
            element={<PlaneTicketsList />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PLANE_TICKETS}/:id`} 
            element={<PlaneTicketDetails />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PLANE_TICKETS}/create`}
            element={<RequireAdminRole element={<PlaneTicketCreateForm />} />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PLANE_TICKETS}/edit/:id`}
            element={<RequireAdminRole element={<PlaneTicketEditForm />} />} 
        />
    </>
);

export default PlaneTicketsRoutes;