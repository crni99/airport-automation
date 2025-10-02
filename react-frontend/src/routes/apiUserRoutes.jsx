import React from "react";
import { Route } from "react-router-dom";
import ApiUsersList from "../components/apiUser/ApiUsersList";
import ApiUserDetails from "../components/apiUser/ApiUserDetails";
import ApiUserEditForm from "../components/apiUser/ApiUserEditForm";
import ProtectedRouteV2 from "./ProtectedRouteV2";
import { ENTITY_PATHS } from '../utils/const';

const ApiUserRoutes = (
    <>
        <Route
            path={ENTITY_PATHS.API_USERS}
            element={<ProtectedRouteV2 element={<ApiUsersList />} />}
        />
        <Route
            path={`${ENTITY_PATHS.API_USERS}/:id`}
            element={<ProtectedRouteV2 element={<ApiUserDetails />} />}
        />
        <Route
            path={`${ENTITY_PATHS.API_USERS}/edit/:id`}
            element={<ProtectedRouteV2 element={<ApiUserEditForm />} />}
        />
    </>
);

export default ApiUserRoutes;