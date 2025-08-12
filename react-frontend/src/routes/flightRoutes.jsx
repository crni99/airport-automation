import React from "react";
import { Route } from "react-router-dom";
import FlightsList from "../components/flight/FlightsList";
import FlightDetails from "../components/flight/FlightDetails";
import FlightCreateForm from "../components/flight/FlightCreateForm";
import FlightEditForm from "../components/flight/FlightEditForm";
import ProtectedRoute from "./ProtectedRoute";

const FlightsRoutes = (
    <>
        <Route path="/flights" element={<FlightsList />} />
        <Route path="/flights/:id" element={<FlightDetails />} />
        <Route path="/flights/create" 
            element={<ProtectedRoute element={<FlightCreateForm />} />} />
        <Route path="/flights/edit/:id" 
            element={<ProtectedRoute element={<FlightEditForm />} />} />
    </>
);

export default FlightsRoutes;