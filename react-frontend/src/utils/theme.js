import { createTheme, responsiveFontSizes } from '@mui/material/styles';

const baseTheme = createTheme({
    typography: {
        h5: {
            fontWeight: 500,
            fontSize: 26,
            letterSpacing: 0.5,
        },
    },
    shape: {
        borderRadius: 8,
    },
    mixins: {
        toolbar: {
            minHeight: 48,
        },
    },
});

let lightTheme = createTheme(baseTheme, {
    palette: {
        mode: 'light',
        primary: {
            light: '#63ccff',
            main: '#009be5',
            dark: '#006db3',
            contrastText: '#fff',
        },
        background: {
            default: '#eaeff1',
            paper: '#fff',
        },
        text: {
            primary: 'rgba(0, 0, 0, 0.87)',
            secondary: 'rgba(0, 0, 0, 0.54)',
        },
    },
    components: {
        MuiTableCell: {
            styleOverrides: {
                root: {
                    paddingTop: '12px',
                    paddingRight: '12px',
                    paddingBottom: '12px',
                    paddingLeft: '16px',
                },
            },
        },
        MuiTableRow: {
            styleOverrides: {
                root: {
                    "& th": {
                        // backgroundColor: "#0a1929",
                        fontWeight: 'bold',
                        fontSize: '0.9rem',
                    },
                    '&:nth-of-type(odd)': {
                        // backgroundColor: '#212529',
                    },
                    '&:nth-of-type(even)': {
                        // backgroundColor: '#2c3035',
                    },
                },
            },
        },
        MuiDrawer: {
            styleOverrides: {
                paper: {
                    backgroundColor: '#fff',
                    color: 'rgba(0, 0, 0, 0.87)',
                },
            },
        },
        MuiButton: {
            styleOverrides: {
                root: {
                    textTransform: 'none',
                },
                contained: {
                    boxShadow: 'none',
                    '&:active': {
                        boxShadow: 'none',
                    },
                },
            },
        },
        MuiTabs: {
            styleOverrides: {
                root: {
                    marginLeft: baseTheme.spacing(1),
                },
                indicator: {
                    height: 3,
                    borderTopLeftRadius: 3,
                    borderTopRightRadius: 3,
                    backgroundColor: baseTheme.palette.primary.main,
                },
            },
        },
        MuiTab: {
            defaultProps: {
                disableRipple: true,
            },
            styleOverrides: {
                root: {
                    textTransform: 'none',
                    margin: '0 16px',
                    minWidth: 0,
                    padding: 0,
                    [baseTheme.breakpoints.up('md')]: {
                        padding: 0,
                        minWidth: 0,
                    },
                },
            },
        },
        MuiIconButton: {
            styleOverrides: {
                root: {
                    padding: baseTheme.spacing(1),
                },
            },
        },
        MuiTooltip: {
            styleOverrides: {
                tooltip: {
                    borderRadius: 4,
                },
            },
        },
        MuiDivider: {
            styleOverrides: {
                root: {
                    backgroundColor: 'rgba(0, 0, 0, 0.12)',
                },
            },
        },
        MuiListItemButton: {
            styleOverrides: {
                root: {
                    '&.Mui-selected': {
                        color: baseTheme.palette.primary.main,
                    },
                },
            },
        },
        MuiListItemText: {
            styleOverrides: {
                primary: {
                    fontSize: '16px',
                    fontWeight: baseTheme.typography.fontWeightMedium,
                    '@media (max-height: 750px)': {
                        fontSize: '14px',
                    },
                    '@media (max-height: 705px)': {
                        fontSize: '12px',
                    },
                    '@media (max-height: 670px)': {
                        fontSize: '10px',
                    },
                },
            },
        },
        MuiListItemIcon: {
            styleOverrides: {
                root: {
                    color: 'inherit',
                    minWidth: 'auto',
                    marginRight: baseTheme.spacing(2),
                    '& svg': {
                        fontSize: 20,
                    },
                    paddingLeft: '8px',
                },
            },
        },
        MuiAvatar: {
            styleOverrides: {
                root: {
                    width: 32,
                    height: 32,
                },
            },
        },
    },
});

let darkTheme = createTheme(baseTheme, {
    palette: {
        mode: 'dark',
        primary: {
            light: '#63ccff',
            main: '#009be5',
            dark: '#006db3',
            contrastText: '#fff',
        },
        background: {
            default: '#121212',
            paper: '#1d1d1d',
        },
        text: {
            primary: '#fff',
            secondary: 'rgba(255, 255, 255, 0.7)',
        },
    },
    components: {
        MuiTableHead: {
            styleOverrides: {
                root: {
                    backgroundColor: '#121212',
                },
            },
        },
        MuiTableCell: {
            styleOverrides: {
                root: {
                    paddingTop: '12px',
                    paddingRight: '12px',
                    paddingBottom: '12px',
                    paddingLeft: '16px',
                },
            },
        },
        MuiTableRow: {
            styleOverrides: {
                root: {
                    "& th": {
                        backgroundColor: "#0a1929",
                        fontWeight: 'bold',
                        fontSize: '0.9rem',
                    },
                    '&:nth-of-type(odd)': {
                        backgroundColor: '#212529',
                    },
                    '&:nth-of-type(even)': {
                        backgroundColor: '#2c3035',
                    },
                },
            },
        },
        MuiTablePagination: {
            styleOverrides: {
                root: ({ ownerState, theme }) => ({
                    '& .MuiIconButton-root': {
                        color: 'white',
                    },
                    '& .MuiIconButton-root.Mui-disabled': {
                        color: 'rgba(255, 255, 255, 0.7)',
                    },
                }),
            },
        },
        MuiDrawer: {
            styleOverrides: {
                paper: {
                    backgroundColor: '#0a1929',
                    color: 'rgba(255, 255, 255, 0.7)',
                },
            },
        },
        MuiButton: {
            styleOverrides: {
                root: {
                    textTransform: 'none',
                },
                contained: {
                    boxShadow: 'none',
                    '&:active': {
                        boxShadow: 'none',
                    },
                },
            },
        },
        MuiTabs: {
            styleOverrides: {
                root: {
                    marginLeft: baseTheme.spacing(1),
                },
                indicator: {
                    height: 3,
                    borderTopLeftRadius: 3,
                    borderTopRightRadius: 3,
                    backgroundColor: baseTheme.palette.primary.main,
                },
            },
        },
        MuiTab: {
            defaultProps: {
                disableRipple: true,
            },
            styleOverrides: {
                root: {
                    textTransform: 'none',
                    margin: '0 16px',
                    minWidth: 0,
                    padding: 0,
                    [baseTheme.breakpoints.up('md')]: {
                        padding: 0,
                        minWidth: 0,
                    },
                },
            },
        },
        MuiIconButton: {
            styleOverrides: {
                root: {
                    padding: baseTheme.spacing(1),
                },
            },
        },
        MuiTooltip: {
            styleOverrides: {
                tooltip: {
                    borderRadius: 4,
                },
            },
        },
        MuiDivider: {
            styleOverrides: {
                root: {
                    backgroundColor: 'rgba(255, 255, 255, 0.12)',
                },
            },
        },
        MuiListItemButton: {
            styleOverrides: {
                root: {
                    '&.Mui-selected': {
                        color: '#4fc3f7',
                    },
                },
            },
        },
        MuiListItemText: {
            styleOverrides: {
                primary: {
                    fontSize: '16px',
                    fontWeight: baseTheme.typography.fontWeightMedium,
                    '@media (max-height: 750px)': {
                        fontSize: '14px',
                    },
                    '@media (max-height: 705px)': {
                        fontSize: '12px',
                    },
                    '@media (max-height: 670px)': {
                        fontSize: '10px',
                    },
                },
            },
        },
        MuiListItemIcon: {
            styleOverrides: {
                root: {
                    color: 'inherit',
                    minWidth: 'auto',
                    marginRight: baseTheme.spacing(2),
                    '& svg': {
                        fontSize: 20,
                    },
                    paddingLeft: '8px',
                },
            },
        },
        MuiAvatar: {
            styleOverrides: {
                root: {
                    width: 32,
                    height: 32,
                },
            },
        },
        MuiSelect: {
            styleOverrides: {
                icon: {
                    color: 'white',
                },
            },
        },
        MuiNativeSelect: {
            styleOverrides: {
                icon: {
                    color: 'white',
                },
            },
        },
    },
});

lightTheme = responsiveFontSizes(lightTheme);
darkTheme = responsiveFontSizes(darkTheme);

export { lightTheme, darkTheme };