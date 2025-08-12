import React from "react";
import { Route } from "react-router-dom";
import AirlinesList from "../components/airline/AirlinesList";
import AirlineDetails from "../components/airline/AirlineDetails";
import AirlineCreateForm from "../components/airline/AirlineCreateForm";
import AirlineEditForm from "../components/airline/AirlineEditForm";
import ProtectedRoute from "../routes/ProtectedRoute";

const AirlinesRoutes = (
    <>
        <Route path="/airlines" element={<AirlinesList />} />
        <Route path="/airlines/:id" element={<AirlineDetails />} />
        <Route path="/airlines/create" 
            element={<ProtectedRoute element={<AirlineCreateForm />} />} />
        <Route path="/airlines/edit/:id" 
            element={<ProtectedRoute element={<AirlineEditForm />} />} />
    </>
);

export default AirlinesRoutes;