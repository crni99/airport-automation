import React from 'react';
import Drawer from '@mui/material/Drawer';
import List from '@mui/material/List';
import Divider from '@mui/material/Divider';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import ListItemButton from '@mui/material/ListItemButton';
import IconButton from '@mui/material/IconButton';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import { MAIN_NAVBAR_ITEMS, ROLES } from '../../../utils/const';
import { useNavigate, useLocation } from "react-router-dom";
import ConnectingAirportsIcon from '@mui/icons-material/ConnectingAirports';
import MonitorHeartIcon from '@mui/icons-material/MonitorHeart';
import Brightness4Icon from '@mui/icons-material/Brightness4';
import Brightness7Icon from '@mui/icons-material/Brightness7';
import LogoutIcon from '@mui/icons-material/Logout';
import { handleSignOut, getAuthToken, getRole } from "../../../utils/auth";
import { useTheme } from '../../../store/ThemeContext';
import { useSidebar } from '../../../store/SidebarContext';
import Box from '@mui/material/Box';

const drawerWidthExpanded = 240;
const drawerWidthShrunk = 60;

const Navbar = () => {
    const { isExpanded, toggleSidebar } = useSidebar();
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
                    width: isExpanded ? drawerWidthExpanded : drawerWidthShrunk,
                    boxSizing: 'border-box',
                    display: 'flex',
                    flexDirection: 'column',
                    transition: (theme) =>
                        theme.transitions.create('width', {
                            easing: theme.transitions.easing.sharp,
                            duration: theme.transitions.duration.enteringScreen,
                        }),
                    overflowX: 'hidden',
                },
            }}
        >
            <Box
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: isExpanded ? 'flex-end' : 'center',
                    minHeight: '64px',
                }}
            >
                {isExpanded && (
                    <Box
                        onClick={() => navigate('/')}
                        sx={{
                            ml: 1,
                            mr: 'auto',
                            whiteSpace: 'nowrap',
                            cursor: 'pointer',
                            flexGrow: 1,
                            display: 'flex',
                            alignItems: 'center'
                        }}
                    >
                        <ListItemIcon sx={{ minWidth: 0, mr: 1, justifyContent: 'center' }}>
                            <ConnectingAirportsIcon />
                        </ListItemIcon>
                        <ListItemText sx={{ my: 0 }}>
                            Airport Automation
                        </ListItemText>
                    </Box>
                )}
                <IconButton
                    onClick={toggleSidebar}
                    sx={{
                        color: theme === 'dark' ? 'rgba(255, 255, 255, 0.7)' : 'inherit', p: 2
                    }}
                >
                    {isExpanded ? <ChevronLeftIcon /> : <ChevronRightIcon />}
                </IconButton>
            </Box>
            <Divider />
            <Box sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                <Divider />
                <List>
                    {filteredMainNavbarItems.map((item) => (
                        <ListItem key={item.id} disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                onClick={() => navigate(item.route)}
                                selected={pathname === `/${item.route}`}
                                sx={{
                                    minHeight: 48,
                                    justifyContent: isExpanded ? 'initial' : 'center',
                                    px: 2.5,
                                }}
                            >
                                <ListItemIcon
                                    sx={{
                                        minWidth: 0,
                                        mr: isExpanded ? 3 : 'auto',
                                        justifyContent: 'center',
                                    }}
                                >
                                    {item.icon}
                                </ListItemIcon>
                                {isExpanded && <ListItemText primary={item.label} sx={{ opacity: isExpanded ? 1 : 0 }} />}
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
                        <ListItem key='healthcheck' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                onClick={() => navigate('health-check')}
                                selected={pathname === '/health-check'}
                                sx={{
                                    minHeight: 48,
                                    justifyContent: isExpanded ? 'initial' : 'center',
                                    px: 2.5,
                                }}
                            >
                                <ListItemIcon
                                    sx={{
                                        minWidth: 0,
                                        mr: isExpanded ? 3 : 'auto',
                                        justifyContent: 'center',
                                    }}
                                >
                                    <MonitorHeartIcon />
                                </ListItemIcon>
                                {isExpanded && <ListItemText primary='Health Check' sx={{ opacity: isExpanded ? 1 : 0 }} />}
                            </ListItemButton>
                        </ListItem>
                        <ListItem key='themeSwitch' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                onClick={toggleTheme}
                                sx={{
                                    minHeight: 48,
                                    justifyContent: isExpanded ? 'initial' : 'center',
                                    px: 2.5,
                                }}
                            >
                                <ListItemIcon
                                    sx={{
                                        minWidth: 0,
                                        mr: isExpanded ? 3 : 'auto',
                                        justifyContent: 'center',
                                    }}
                                >
                                    {theme === 'light' ? <Brightness4Icon /> : <Brightness7Icon />}
                                </ListItemIcon>
                                {isExpanded && <ListItemText primary={theme === 'light' ? 'Dark Mode' : 'Light Mode'} sx={{ opacity: isExpanded ? 1 : 0 }} />}
                            </ListItemButton>
                        </ListItem>
                    </List>
                    <Divider />
                    <List>
                        <ListItem key='sign-out' disablePadding sx={{ display: 'block' }}>
                            <ListItemButton
                                onClick={handleSignOutClick}
                                sx={{
                                    minHeight: 48,
                                    justifyContent: isExpanded ? 'initial' : 'center',
                                    px: 2.5,
                                }}
                            >
                                <ListItemIcon
                                    sx={{
                                        minWidth: 0,
                                        mr: isExpanded ? 3 : 'auto',
                                        justifyContent: 'center',
                                    }}
                                >
                                    <LogoutIcon />
                                </ListItemIcon>
                                {isExpanded && <ListItemText primary='Sign out' sx={{ opacity: isExpanded ? 1 : 0 }} />}
                            </ListItemButton>
                        </ListItem>
                    </List>
                </>
            )}
        </Drawer>
    );
};

export default Navbar;