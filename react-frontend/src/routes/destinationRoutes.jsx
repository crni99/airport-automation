import React from "react";
import { Route } from "react-router-dom";
import DestinationsList from "../pages/destination/DestinationsList";
import DestinationDetails from "../pages/destination/DestinationDetails";
import DestinationCreateForm from "../pages/destination/DestinationCreateForm";
import DestinationEditForm from "../pages/destination/DestinationEditForm";
import RequireAdminRole from "../routes/RequireAdminRole";
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
            element={<RequireAdminRole element={<DestinationCreateForm />} />}
        />
        <Route
            path={`${ENTITY_PATHS.DESTINATIONS}/edit/:id`}
            element={<RequireAdminRole element={<DestinationEditForm />} />}
        />
    </>
);

export default DestinationsRoutes;