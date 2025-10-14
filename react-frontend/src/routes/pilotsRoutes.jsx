import React from "react";
import { Route } from "react-router-dom";
import PilotsList from "../pages/pilot/PilotsList";
import PilotDetails from "../pages/pilot/PilotDetails";
import PilotCreateForm from "../pages/pilot/PilotCreateForm";
import PilotEditForm from "../pages/pilot/PilotEditForm";
import ProtectedRoute from "../routes/ProtectedRoute";
import { ENTITY_PATHS } from '../utils/const';

const PilotsRoutes = (
    <>
        <Route 
            path={ENTITY_PATHS.PILOTS} 
            element={<PilotsList />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PILOTS}/:id`} 
            element={<PilotDetails />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PILOTS}/create`}
            element={<ProtectedRoute element={<PilotCreateForm />} />} 
        />
        <Route 
            path={`${ENTITY_PATHS.PILOTS}/edit/:id`}
            element={<ProtectedRoute element={<PilotEditForm />} />} 
        />
    </>
);

export default PilotsRoutes;