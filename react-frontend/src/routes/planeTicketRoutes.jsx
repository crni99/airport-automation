import React from "react";
import { Route } from "react-router-dom";
import PlaneTicketsList from "../components/planeTicket/PlaneTicketsList";
import PlaneTicketDetails from "../components/planeTicket/PlaneTicketDetails";
import PlaneTicketCreateForm from "../components/planeTicket/PlaneTicketCreateForm";
import PlaneTicketEditForm from "../components/planeTicket/PlaneTicketEditForm";
import ProtectedRoute from "./ProtectedRoute";

const PlaneTicketsRoutes = (
    <>
        <Route path="/planeTickets" element={<PlaneTicketsList />} />
        <Route path="/planeTickets/:id" element={<PlaneTicketDetails />} />
        <Route path="/planeTickets/create" 
            element={<ProtectedRoute element={<PlaneTicketCreateForm />} />} />
        <Route path="/planeTickets/edit/:id" 
            element={<ProtectedRoute element={<PlaneTicketEditForm />} />} />
    </>
);

export default PlaneTicketsRoutes;