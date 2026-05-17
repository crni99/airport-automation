import React, { lazy } from "react";
import { Route } from "react-router-dom";

const TravelClassesList = lazy(() => import('../pages/travelClass/TravelClassesList'));

const TravelClassesRoutes = (
        <>
            <Route path="/travel-classes" element={<TravelClassesList />} />
        </>
);

export default TravelClassesRoutes;