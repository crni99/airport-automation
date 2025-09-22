import React from "react";
import { Route } from "react-router-dom";
import PlaneTicketsList from "../components/planeTicket/PlaneTicketsList";
import PlaneTicketDetails from "../components/planeTicket/PlaneTicketDetails";
import PlaneTicketCreateForm from "../components/planeTicket/PlaneTicketCreateForm";
import PlaneTicketEditForm from "../components/planeTicket/PlaneTicketEditForm";
import ProtectedRoute from "./ProtectedRoute";

const PlaneTicketsRoutes = (
    <>
        <Route path="/plane-tickets" element={<PlaneTicketsList />} />
        <Route path="/plane-tickets/:id" element={<PlaneTicketDetails />} />
        <Route path="/plane-tickets/create" 
            element={<ProtectedRoute element={<PlaneTicketCreateForm />} />} />
        <Route path="/plane-tickets/edit/:id" 
            element={<ProtectedRoute element={<PlaneTicketEditForm />} />} />
    </>
);

export default PlaneTicketsRoutes;