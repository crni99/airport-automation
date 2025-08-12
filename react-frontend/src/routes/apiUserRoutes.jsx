import React from "react";
import { Route } from "react-router-dom";
import ApiUsersList from "../components/apiUser/ApiUsersList";
import ApiUserDetails from "../components/apiUser/ApiUserDetails";
import ApiUserEditForm from "../components/apiUser/ApiUserEditForm";
import ProtectedRoutev2 from "./ProtectedRoutev2";

const ApiUserRoutes = (
    <>
        <Route path="/apiUsers" 
            element={<ProtectedRoutev2 element={<ApiUsersList />} />} />
        <Route path="/apiUsers/:id" 
            element={<ProtectedRoutev2 element={<ApiUserDetails />} />} />
        <Route path="/apiUsers/edit/:id" 
            element={<ProtectedRoutev2 element={<ApiUserEditForm />} />} />
    </>
);

export default ApiUserRoutes;