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
          <Container>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/health-check" element={<HealthCheck />} />
              <Route path="/unauthorized" element={<Unauthorized />} />
              <Route element={<ProtectedRouteV3 />}>
                {AirlineRoutes}
                {DestinationsRoutes}
                {TravelClassesRoutes}
                {PassengersRoutes}
                {PilotsRoutes}
                {ApiUsersRoutes}
                {FlightsRoutes}
                {PlaneTicketsRoutes}
              </Route>
            </Routes>
          </Container>
        </Box>
      </Box>
    </Box >
  );
}

export default App;