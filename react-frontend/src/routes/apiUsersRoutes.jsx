import React, { lazy } from "react";
import { Route } from "react-router-dom";
import RequireSuperAdminRole from "./RequireSuperAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const ApiUsersList = lazy(() => import('../pages/apiUser/ApiUsersList'));
const ApiUserDetails = lazy(() => import('../pages/apiUser/ApiUserDetails'));
const ApiUserEditForm = lazy(() => import('../pages/apiUser/ApiUserEditForm'));

const ApiUserRoutes = (
        <>
            <Route path={ENTITY_PATHS.API_USERS} element={<RequireSuperAdminRole element={<ApiUsersList />} />} />
            <Route path={`${ENTITY_PATHS.API_USERS}/:id`} element={<RequireSuperAdminRole element={<ApiUserDetails />} />} />
            <Route path={`${ENTITY_PATHS.API_USERS}/edit/:id`} element={<RequireSuperAdminRole element={<ApiUserEditForm />} />} />
        </>
);

export default ApiUserRoutes;