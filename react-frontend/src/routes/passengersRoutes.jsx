import React from "react";
import { Route } from "react-router-dom";
import PassengersList from "../components/passenger/PassengersList";
import PassengerDetails from "../components/passenger/PassengerDetails";
import PassengerCreateForm from "../components/passenger/PassengerCreateForm";
import PassengerEditForm from "../components/passenger/PassengerEditForm";
import ProtectedRoute from "../routes/ProtectedRoute";

const PassengersRoutes = (
    <>
        <Route path="/passengers" element={<PassengersList />} />
        <Route path="/passengers/:id" element={<PassengerDetails />} />
        <Route path="/passengers/create"
            element={<ProtectedRoute element={<PassengerCreateForm />} />} />
        <Route path="/passengers/edit/:id"
            element={<ProtectedRoute element={<PassengerEditForm />} />} />
    </>
);

export default PassengersRoutes;