import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.js'; 
import reportWebVitals from './reportWebVitals.js'; 
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider } from './store/ThemeContext.jsx'; 
import { SidebarProvider } from './store/SidebarContext.jsx'; 
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import CssBaseline from '@mui/material/CssBaseline';

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(
  <React.StrictMode>
    <BrowserRouter>
      <ThemeProvider>
        <SidebarProvider>
          <CssBaseline />
          <App />
        </SidebarProvider>
      </ThemeProvider>
    </BrowserRouter>
  </React.StrictMode>
);

reportWebVitals();