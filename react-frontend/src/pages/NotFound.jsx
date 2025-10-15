import * as React from 'react';
import Box from '@mui/material/Box';
import MuiAlert from '@mui/material/Alert';
import Typography from '@mui/material/Typography';
import AlertTitle from '@mui/material/AlertTitle';
import WarningAmberIcon from '@mui/icons-material/WarningAmber';

export default function NotFound() {
    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                height: '50vh',
            }}
        >
            <Box sx={{ width: { xs: '90%', sm: '75%', md: '50%' } }}>
                <MuiAlert
                    severity="warning"
                    icon={<WarningAmberIcon sx={{ fontSize: '32px' }} />}
                    sx={{
                        textAlign: 'center',
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                    }}
                >
                    <AlertTitle>
                        <Typography variant="h5" component="div">
                            404 - Page Not Found
                        </Typography>
                    </AlertTitle>
                    <Typography variant="body1">
                        We couldn't find the page you were looking for.
                    </Typography>
                    <Typography variant="body1" sx={{ pb: 2 }}>
                        Please check the URL and try again.
                    </Typography>
                </MuiAlert>
            </Box>
        </Box>
    );
}