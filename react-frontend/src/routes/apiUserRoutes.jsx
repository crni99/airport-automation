import React from "react";
import { Route } from "react-router-dom";
import ApiUsersList from "../components/apiUser/ApiUsersList";
import ApiUserDetails from "../components/apiUser/ApiUserDetails";
import ApiUserEditForm from "../components/apiUser/ApiUserEditForm";
import ProtectedRouteV2 from "./ProtectedRouteV2";

const ApiUserRoutes = (
    <>
        <Route path="/apiUsers" 
            element={<ProtectedRouteV2 element={<ApiUsersList />} />} />
        <Route path="/apiUsers/:id" 
            element={<ProtectedRouteV2 element={<ApiUserDetails />} />} />
        <Route path="/apiUsers/edit/:id" 
            element={<ProtectedRouteV2 element={<ApiUserEditForm />} />} />
    </>
);

export default ApiUserRoutes;