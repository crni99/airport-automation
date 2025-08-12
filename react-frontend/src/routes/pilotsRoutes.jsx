import React from "react";
import { Route } from "react-router-dom";
import PilotsListsList from "../components/pilot/PilotsList";
import PilotDetailsDetails from "../components/pilot/PilotDetails";
import PilotCreateFormCreateForm from "../components/pilot/PilotCreateForm";
import PilotEditFormEditForm from "../components/pilot/PilotEditForm";
import ProtectedRoute from "../routes/ProtectedRoute";

const PilotsRoutes = (
    <>
        <Route path="/pilots" element={<PilotsListsList />} />
        <Route path="/pilots/:id" element={<PilotDetailsDetails />} />
        <Route path="/pilots/create"
        element={<ProtectedRoute element={<PilotCreateFormCreateForm />} />} />
        <Route path="/pilots/edit/:id"
            element={<ProtectedRoute element={<PilotEditFormEditForm />} />} />
    </>
);

export default PilotsRoutes;