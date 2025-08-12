import React, { useContext } from 'react';
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
import { DataContext } from './store/data-context';
import Unauthorized from './components/common/Unauthorized';
import ApiUsersRoutes from './routes/apiUserRoutes';

/*
Optimization: 
Look for opportunities to optimize performance, 
such as memoizing context values or using React's useMemo hook to prevent unnecessary re-renders.
*/

function App() {
  const dataContext = useContext(DataContext);

  return (
    <DataContext.Provider value={dataContext}>
      <Header />
      <div className="container">
        <div className="row">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/HealthCheck" element={<HealthCheck />} />
            <Route path='/unauthorized' element={<Unauthorized />} />

            {AirlineRoutes}
            {DestinationsRoutes}
            {TravelClassesRoutes}
            {PassengersRoutes}
            {PilotsRoutes}
            {ApiUsersRoutes}
            {FlightsRoutes}
            {PlaneTicketsRoutes}

          </Routes>
        </div>
      </div>
      <Footer />
    </DataContext.Provider>
  );
}

export default App;