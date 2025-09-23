import React from 'react';
import { Route, Routes } from 'react-router-dom';
import AirlineRoutes from './routes/airlineRoutes';
import DestinationsRoutes from './routes/destinationRoutes';
import TravelClassesRoutes from './routes/travelClassesRoutes';
import PassengersRoutes from './routes/passengersRoutes';
import PilotsRoutes from './routes/pilotsRoutes';
import FlightsRoutes from './routes/flightRoutes';
import PlaneTicketsRoutes from './routes/planeTicketRoutes';
import Home from './components/common/Home';
import HealthCheck from './components/common/HealthCheck';
import Unauthorized from './components/common/Unauthorized';
import ApiUsersRoutes from './routes/apiUserRoutes';
import ProtectedRouteV3 from './routes/ProtectedRouteV3';
import { Container, Box } from '@mui/material';

import Navbar from './components/common/header/Navbar';
import { getAuthToken } from "./utils/auth";

function App() {
  const isLoggedIn = getAuthToken() !== null;

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ display: 'flex', flexGrow: 1 }}>
        {isLoggedIn && (
          <Box
            sx={{
              width: 240,
              flexShrink: 0,
              overflowY: 'auto'
            }}
          >
            <Navbar />
          </Box>
        )}
        <Box component="main" sx={{ flexGrow: 1, overflowY: 'auto' }}>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/health-check" element={<HealthCheck />} />
            <Route path="/unauthorized" element={<Unauthorized />} />
            <Route element={<ProtectedRouteV3 />}>
              <Route path="/airlines/*" element={<Container sx={{ mt: 4 }}>{AirlineRoutes}</Container>} />
              <Route path="/destinations/*" element={<Container sx={{ mt: 4 }}>{DestinationsRoutes}</Container>} />
              <Route path="/travel-classes/*" element={<Container sx={{ mt: 4 }}>{TravelClassesRoutes}</Container>} />
              <Route path="/passengers/*" element={<Container sx={{ mt: 4 }}>{PassengersRoutes}</Container>} />
              <Route path="/pilots/*" element={<Container sx={{ mt: 4 }}>{PilotsRoutes}</Container>} />
              <Route path="/api-users/*" element={<Container sx={{ mt: 4 }}>{ApiUsersRoutes}</Container>} />
              <Route path="/flights/*" element={<Container sx={{ mt: 4 }}>{FlightsRoutes}</Container>} />
              <Route path="/plane-tickets/*" element={<Container sx={{ mt: 4 }}>{PlaneTicketsRoutes}</Container>} />
            </Route>
          </Routes>
        </Box>
      </Box>
    </Box >
  );
}

export default App;