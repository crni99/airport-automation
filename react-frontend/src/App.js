import React from 'react';
import { Route, Routes } from 'react-router-dom';
import AirlineRoutes from './routes/airlineRoutes';
import DestinationsRoutes from './routes/destinationRoutes';
import TravelClassesRoutes from './routes/travelClassesRoutes';
import PassengersRoutes from './routes/passengersRoutes';
import PilotsRoutes from './routes/pilotsRoutes';
import FlightsRoutes from './routes/flightRoutes';
import PlaneTicketsRoutes from './routes/planeTicketRoutes';
import Home from './pages/Home';
import HealthCheck from './pages/HealthCheck';
import NotFound from './pages/NotFound';
import Unauthorized from './pages/Unauthorized';
import ApiUsersRoutes from './routes/apiUserRoutes';
import ProtectedRouteV3 from './routes/ProtectedRouteV3';
import { Container, Box } from '@mui/material';
import Navbar from './components/common/header/Navbar';
import Footer from './components/common/Footer';
import { getAuthToken } from "./utils/auth";
import { ENTITY_PATHS } from './utils/const';
import { useSidebar } from './store/SidebarContext';

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
