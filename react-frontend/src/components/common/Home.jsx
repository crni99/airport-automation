import React, { useState, useContext, useEffect } from 'react';
import { DataContext } from '../../store/DataContext';
import { getAuthToken, authenticateUser } from '../../utils/auth';

// Material-UI Components
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import MuiAlert from '@mui/material/Alert';
import Stack from '@mui/material/Stack';
import Footer from './Footer';

export default function Home() {
    const dataCtx = useContext(DataContext);
    const isLoggedIn = getAuthToken() !== null;
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);
    const [currentDateTime, setCurrentDateTime] = useState('');
    const [message, setMessage] = useState(null);

    useEffect(() => {
        const storedMessage = localStorage.getItem('authErrorMessage');
        if (storedMessage) {
            setMessage(storedMessage);
            localStorage.removeItem('authErrorMessage');
        }
    }, []);

    const handleFormSubmit = async (event) => {
        event.preventDefault();
        setLoading(true);
        setError(null);
        try {
            const authError = await authenticateUser(userName, password, dataCtx.apiUrl);
            if (authError) {
                setError(authError.message);
            }
        } catch (error) {
            console.error('Error:', error);
            setError('An unexpected error occurred. Please try again later.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const updateDateTime = () => {
            const now = new Date();
            const dateOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            const timeOptions = { hour12: false };
            const dateTime = `${now.toLocaleDateString([], dateOptions)} - ${now.toLocaleTimeString([], timeOptions)}`;
            setCurrentDateTime(dateTime);
        };

        updateDateTime();

        const intervalId = setInterval(updateDateTime, 1000);

        return () => {
            clearInterval(intervalId);
        };
    }, []);

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                minHeight: '100vh',
                overflow: 'hidden',
            }}
        >
            <Box
                component="main"
                role="main"
                sx={{
                    flexGrow: 1,
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    py: 4,
                }}
            >
                <Box
                    sx={{
                        width: '100%',
                        maxWidth: 450,
                        p: 4,
                        borderRadius: 2,
                        boxShadow: 3,
                        bgcolor: 'background.paper',
                    }}
                >
                    <Stack spacing={1} sx={{ mb: 4, textAlign: 'center' }}>
                        <Typography variant="h3" component="h1">
                            Airport Automation
                        </Typography>
                        <Typography variant="subtitle1" color="text.secondary">
                            {currentDateTime}
                        </Typography>
                    </Stack>
                    {!isLoggedIn && (
                        <Grid container justifyContent="center">
                            <Grid>
                                <Stack spacing={2}>
                                    {message && <MuiAlert severity="error">{message}</MuiAlert>}
                                    {error && <MuiAlert severity="error">{error}</MuiAlert>}
                                    {loading ? (
                                        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                                            <CircularProgress />
                                        </Box>
                                    ) : (
                                        <Box component="form" onSubmit={handleFormSubmit}>
                                            <Stack spacing={3}>
                                                <TextField
                                                    label="Username"
                                                    id="UserName"
                                                    name="UserName"
                                                    inputProps={{ maxLength: 50 }}
                                                    fullWidth
                                                    required
                                                    value={userName}
                                                    onChange={(e) => setUserName(e.target.value)}
                                                />
                                                <TextField
                                                    label="Password"
                                                    id="Password"
                                                    name="Password"
                                                    type="password"
                                                    inputProps={{ maxLength: 50 }}
                                                    fullWidth
                                                    required
                                                    value={password}
                                                    onChange={(e) => setPassword(e.target.value)}
                                                />
                                                <Button type="submit" variant="contained" fullWidth sx={{ mb: '8px !important' }} >
                                                    Sign In
                                                </Button>
                                            </Stack>
                                        </Box>
                                    )}
                                </Stack>
                            </Grid>
                        </Grid>
                    )}
                </Box>
            </Box>
            <Footer />
        </Box>
    );
}