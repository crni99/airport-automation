import React from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import IconButton from '@mui/material/IconButton';
import GitHubIcon from '@mui/icons-material/GitHub';
import LinkedInIcon from '@mui/icons-material/LinkedIn';
import MailIcon from '@mui/icons-material/Mail';
import Stack from '@mui/material/Stack';

export default function Footer() {
    const currentYear = new Date().getFullYear();

    return (
        <Box
            component="footer"
            sx={{
                borderTop: 1,
                borderColor: 'divider',
                py: 2,
                mt: 'auto',
                bgcolor: 'background.paper',
            }}
        >
            <Box
                sx={{
                    maxWidth: 'lg',
                    mx: 'auto',
                    px: 2,
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                }}
            >
                <Typography variant="body2" color="text.secondary">
                    {currentYear} &copy; Airport Automation React
                </Typography>
                <Stack direction="row" spacing={1}>
                    <IconButton
                        aria-label="GitHub"
                        href="https://github.com/crni99"
                        target="_blank"
                        rel="noopener noreferrer"
                        color="inherit"
                    >
                        <GitHubIcon />
                    </IconButton>
                    <IconButton
                        aria-label="LinkedIn"
                        href="https://www.linkedin.com/in/ognj3n"
                        target="_blank"
                        rel="noopener noreferrer"
                        color="inherit"
                    >
                        <LinkedInIcon />
                    </IconButton>
                    <IconButton
                        aria-label="Email"
                        href="mailto:andjelicb.ognjen@gmail.com"
                        target="_blank"
                        rel="noopener noreferrer"
                        color="inherit"
                    >
                        <MailIcon />
                    </IconButton>
                </Stack>
            </Box>
        </Box>
    );
}