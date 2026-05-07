import React from "react";
import { Route } from "react-router-dom";
import PassengersList from "../pages/passenger/PassengersList";
import PassengerDetails from "../pages/passenger/PassengerDetails";
import PassengerCreateForm from "../pages/passenger/PassengerCreateForm";
import PassengerEditForm from "../pages/passenger/PassengerEditForm";
import RequireAdminRole from "../routes/RequireAdminRole";
import { ENTITY_PATHS } from '../utils/const';

const PassengersRoutes = (
    <>
        <Route 
            path={ENTITY_PATHS.PASSENGERS} 
            element={<PassengersList />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PASSENGERS}/:id`} 
            element={<PassengerDetails />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PASSENGERS}/create`}
            element={<RequireAdminRole element={<PassengerCreateForm />} />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PASSENGERS}/edit/:id`}
            element={<RequireAdminRole element={<PassengerEditForm />} />} 
        />
    </>
);

export default PassengersRoutes;