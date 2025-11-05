import React from 'react';
import { Route, Routes } from 'react-router-dom';
import AirlineRoutes from './routes/airlineRoutes.jsx';
import DestinationsRoutes from './routes/destinationRoutes.jsx';
import TravelClassesRoutes from './routes/travelClassesRoutes.jsx';
import PassengersRoutes from './routes/passengersRoutes.jsx';
import PilotsRoutes from './routes/pilotsRoutes.jsx';
import FlightsRoutes from './routes/flightRoutes.jsx';
import PlaneTicketsRoutes from './routes/planeTicketRoutes.jsx';
import Home from './pages/Home.jsx';
import HealthCheck from './pages/HealthCheck.jsx';
import NotFound from './pages/NotFound.jsx';
import Unauthorized from './pages/Unauthorized.jsx';
import ApiUsersRoutes from './routes/apiUserRoutes.jsx';
import ProtectedRouteV3 from './routes/ProtectedRouteV3.jsx';
import { Container, Box } from '@mui/material';
import Navbar from './components/common/header/Navbar.jsx';
import Footer from './components/common/Footer.jsx';
import { getAuthToken } from "./utils/auth.js";
import { ENTITY_PATHS } from './utils/const.js';
import { useSidebar } from './store/SidebarContext.jsx';

function App() {
  
  const isLoggedIn = getAuthToken() !== null;
  const { sidebarWidth } = useSidebar();

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <Box sx={{ display: 'flex', flexGrow: 1 }}>
        {isLoggedIn && (
          <Box
            sx={{
              width: sidebarWidth,
              flexShrink: 0,
              overflowY: 'auto',
              transition: (theme) =>
                theme.transitions.create('width', {
                  easing: theme.transitions.easing.sharp,
                  duration: theme.transitions.duration.enteringScreen,
                }),
            }}
          >
            <Navbar />
          </Box>
        )}
        <Box component="main" sx={{ flexGrow: 1, overflowY: 'auto', pl: 3, pr: 3 }}>
          <Container maxWidth={false}>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path={ENTITY_PATHS.HEALTH_CHECKS} element={<HealthCheck />} />
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
              <Route path="*" element={<NotFound />} />
            </Routes>
          </Container>
          {!isLoggedIn && (
            <Footer />
          )}
        </Box>
      </Box>
    </Box >
  );
}

export default App;
