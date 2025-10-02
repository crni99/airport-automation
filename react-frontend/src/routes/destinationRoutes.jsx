import React from "react";
import { Route } from "react-router-dom";
import DestinationsList from "../components/destination/DestinationsList";
import DestinationDetails from "../components/destination/DestinationDetails";
import DestinationCreateForm from "../components/destination/DestinationCreateForm";
import DestinationEditForm from "../components/destination/DestinationEditForm";
import ProtectedRoute from "../routes/ProtectedRoute";
import { ENTITY_PATHS } from '../utils/const';

const DestinationsRoutes = (
    <>
        <Route
            path={ENTITY_PATHS.DESTINATIONS}
            element={<DestinationsList />}
        />
        <Route
            path={`${ENTITY_PATHS.DESTINATIONS}/:id`}
            element={<DestinationDetails />} />
        <Route
            path={`${ENTITY_PATHS.DESTINATIONS}/create`}
            element={<ProtectedRoute element={<DestinationCreateForm />} />}
        />
        <Route
            path={`${ENTITY_PATHS.DESTINATIONS}/edit/:id`}
            element={<ProtectedRoute element={<DestinationEditForm />} />}
        />
    </>
);

export default DestinationsRoutes;