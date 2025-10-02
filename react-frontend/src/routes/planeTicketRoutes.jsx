import React from "react";
import { Route } from "react-router-dom";
import PlaneTicketsList from "../components/planeTicket/PlaneTicketsList";
import PlaneTicketDetails from "../components/planeTicket/PlaneTicketDetails";
import PlaneTicketCreateForm from "../components/planeTicket/PlaneTicketCreateForm";
import PlaneTicketEditForm from "../components/planeTicket/PlaneTicketEditForm";
import ProtectedRoute from "./ProtectedRoute";
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
            element={<ProtectedRoute element={<PlaneTicketCreateForm />} />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PLANE_TICKETS}/edit/:id`}
            element={<ProtectedRoute element={<PlaneTicketEditForm />} />} 
        />
    </>
);

export default PlaneTicketsRoutes;