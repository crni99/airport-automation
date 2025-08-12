import React from "react";
import { Route } from "react-router-dom";
import TravelClassesList from "../components/travelClass/TravelClassesList";

const TravelClassesRoutes = (
    <>
        <Route path="/travelClasses" element={<TravelClassesList />} />
    </>
);

export default TravelClassesRoutes;