import React from 'react';
import Drawer from '@mui/material/Drawer';
import List from '@mui/material/List';
import Divider from '@mui/material/Divider';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import ListItemButton from '@mui/material/ListItemButton';
import { MAIN_NAVBAR_ITEMS, ROLES } from '../../../utils/const';
import { useNavigate, useLocation } from "react-router-dom";
import ConnectingAirportsIcon from '@mui/icons-material/ConnectingAirports';
import MonitorHeartIcon from '@mui/icons-material/MonitorHeart';
import Brightness4Icon from '@mui/icons-material/Brightness4';
import Brightness7Icon from '@mui/icons-material/Brightness7';
import LogoutIcon from '@mui/icons-material/Logout';
import { handleSignOut, getAuthToken, getRole } from "../../../utils/auth";
import { useTheme } from '../../../store/ThemeContext';
import Box from '@mui/material/Box';

const Navbar = () => {

    const isLoggedIn = getAuthToken() !== null;
    const role = getRole();
    const { theme, toggleTheme } = useTheme();
    const navigate = useNavigate();
    const { pathname } = useLocation();

    const handleSignOutClick = () => {
        handleSignOut();
    };

    const filteredMainNavbarItems = role === ROLES.SUPER_ADMIN
        ? MAIN_NAVBAR_ITEMS
        : MAIN_NAVBAR_ITEMS.filter(item => item.label !== 'Api Users');

    return (
        <Drawer
            variant="permanent"
            anchor="left"
            sx={{
                '& .MuiDrawer-paper': {
                    width: '240px',
                    boxSizing: 'border-box',
                    display: 'flex',
                    flexDirection: 'column',
                },
            }}
        >
            <List>
                <ListItem disablePadding>
                    <ListItemButton onClick={() => navigate('/')}>
                        <ListItemIcon>
                            <ConnectingAirportsIcon />
                        </ListItemIcon>
                        <ListItemText>
                            Airport Automation
                        </ListItemText>
                    </ListItemButton>
                </ListItem>
            </List>
            <Divider />

            <Box sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                <Divider />
                <List>
                    {filteredMainNavbarItems.map((item) => (
                        <ListItem key={item.id} disablePadding>
                            <ListItemButton
                                onClick={() => navigate(item.route)}
                                selected={pathname === `/${item.route}`}
                            >
                                <ListItemIcon>
                                    {item.icon}
                                </ListItemIcon>
                                <ListItemText primary={item.label} />
                            </ListItemButton>
                        </ListItem>
                    ))}
                </List>
                <Divider />
            </Box>

            {isLoggedIn && (
                <>
                    <Divider sx={{ mt: 12 }} />
                    <List>
                        <ListItem key='healthcheck' disablePadding>
                            <ListItemButton
                                onClick={() => navigate('health-check')}
                                selected={pathname === '/health-check'}
                            >
                                <ListItemIcon>
                                    <MonitorHeartIcon />
                                </ListItemIcon>
                                <ListItemText primary='Health Check' />
                            </ListItemButton>
                        </ListItem>
                        <ListItem key='themeSwitch' disablePadding>
                            <ListItemButton onClick={toggleTheme}>
                                <ListItemIcon>
                                    {theme === 'light' ? <Brightness4Icon /> : <Brightness7Icon />}
                                </ListItemIcon>
                                <ListItemText primary={theme === 'light' ? 'Dark Mode' : 'Light Mode'} />
                            </ListItemButton>
                        </ListItem>
                    </List>
                    <Divider />
                    <List>
                        <ListItem key='sign-out' disablePadding>
                            <ListItemButton onClick={handleSignOutClick}>
                                <ListItemIcon>
                                    <LogoutIcon />
                                </ListItemIcon>
                                <ListItemText primary='Sign out' />
                            </ListItemButton>
                        </ListItem>
                    </List>
                </>
            )}
        </Drawer>
    );
};

export default Navbar;