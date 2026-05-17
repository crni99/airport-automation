import React, { useState, useEffect, Suspense } from 'react';
import { Route, Routes } from 'react-router-dom';
import RequireAuth from './routes/RequireAuth.jsx';
import { Container, Box, CircularProgress } from '@mui/material';
import Navbar from './components/common/header/Navbar.jsx';
import Footer from './components/common/Footer.jsx';
import { getAuthToken } from "./utils/auth.js";
import { ENTITY_PATHS } from './utils/const.js';
import { useSidebar } from './store/SidebarContext.jsx';

import Home from './pages/Home.jsx';
import NotFound from './pages/NotFound.jsx';
import Unauthorized from './pages/Unauthorized.jsx';
import HealthCheck from './pages/HealthCheck.jsx';

import AirlinesRoutes from './routes/airlinesRoutes.jsx';
import ApiUsersRoutes from './routes/apiUsersRoutes.jsx';
import DestinationsRoutes from './routes/destinationsRoutes.jsx';
import FlightsRoutes from './routes/flightsRoutes.jsx';
import PassengersRoutes from './routes/passengersRoutes.jsx';
import PilotsRoutes from './routes/pilotsRoutes.jsx';
import PlaneTicketsRoutes from './routes/planeTicketsRoutes.jsx';
import TravelClassesRoutes from './routes/travelClassesRoutes.jsx';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(() => getAuthToken() !== null);
  const { sidebarWidth } = useSidebar();

  useEffect(() => {
    const handleStorageChange = () => setIsLoggedIn(getAuthToken() !== null);
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

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
            <Suspense fallback={<Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}><CircularProgress /></Box>}>
              <Routes>
                <Route path="/" element={<Home />} />
                <Route path={ENTITY_PATHS.HEALTH_CHECKS} element={<HealthCheck />} />
                <Route path="/unauthorized" element={<Unauthorized />} />
                <Route element={<RequireAuth />}>
                  {AirlinesRoutes}
                  {ApiUsersRoutes}
                  {DestinationsRoutes}
                  {FlightsRoutes}
                  {PassengersRoutes}
                  {PilotsRoutes}
                  {PlaneTicketsRoutes}
                  {TravelClassesRoutes}
                </Route>
                <Route path="*" element={<NotFound />} />
              </Routes>
            </Suspense>
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