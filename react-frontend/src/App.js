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
import Header from './components/common/header/Header';
import HealthCheck from './components/common/HealthCheck';
import Footer from './components/common/Footer';
import Unauthorized from './components/common/Unauthorized';
import ApiUsersRoutes from './routes/apiUserRoutes';
import ProtectedRouteV3 from './routes/ProtectedRouteV3';

function App() {
  
  return (
    <>
      <Header />
      <div className="container">
        <div className="row">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/HealthCheck" element={<HealthCheck />} />
            <Route path='/unauthorized' element={<Unauthorized />} />

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
        </div>
      </div>
      <Footer />
    </>
  );
}

export default App;
