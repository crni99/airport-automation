import React from "react";
import { Route } from "react-router-dom";
import AirlinesList from "../pages/airline/AirlinesList";
import AirlineDetails from "../pages/airline/AirlineDetails";
import AirlineCreateForm from "../pages/airline/AirlineCreateForm";
import AirlineEditForm from "../pages/airline/AirlineEditForm";
import ProtectedRoute from "../routes/ProtectedRoute";
import { ENTITY_PATHS } from '../utils/const';

const AirlinesRoutes = (
    <>
        <Route
            path={ENTITY_PATHS.AIRLINES}
            element={<AirlinesList />}
        />
        <Route
            path={`${ENTITY_PATHS.AIRLINES}/:id`}
            element={<AirlineDetails />}
        />
        <Route
            path={`${ENTITY_PATHS.AIRLINES}/create`}
            element={<ProtectedRoute element={<AirlineCreateForm />} />}
        />
        <Route
            path={`${ENTITY_PATHS.AIRLINES}/edit/:id`}
            element={<ProtectedRoute element={<AirlineEditForm />} />}
        />
    </>
);

export default AirlinesRoutes;