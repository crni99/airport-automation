import React from "react";
import { Route } from "react-router-dom";
import ApiUsersList from "../pages/apiUser/ApiUsersList";
import ApiUserDetails from "../pages/apiUser/ApiUserDetails";
import ApiUserEditForm from "../pages/apiUser/ApiUserEditForm";
import RequireSuperAdminRole from "./RequireSuperAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const ApiUserRoutes = (
    <>
        <Route
            path={ENTITY_PATHS.API_USERS}
            element={<RequireSuperAdminRole element={<ApiUsersList />} />}
        />
        <Route
            path={`${ENTITY_PATHS.API_USERS}/:id`}
            element={<RequireSuperAdminRole element={<ApiUserDetails />} />}
        />
        <Route
            path={`${ENTITY_PATHS.API_USERS}/edit/:id`}
            element={<RequireSuperAdminRole element={<ApiUserEditForm />} />}
        />
    </>
);

export default ApiUserRoutes;