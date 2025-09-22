import { createTheme } from '@mui/material/styles';

const baseTheme = createTheme({
    breakpoints: {
        values: {
            xs: 0,
            sm: 600,
            md: 900,
            lg: 1200,
            xl: 2560, // Your 4K breakpoint
        },
    },
});

const lightTheme = createTheme(baseTheme, {
    typography: {
        listItemTextPrimary: {
            fontWeight: 'bold',
        },
    },
    components: {
        MuiTableContainer: {
            styleOverrides: {
                root: ({ theme }) => ({
                    overflow: 'auto',
                    [theme.breakpoints.down('sm')]: {
                        maxHeight: '300px',
                    },
                    [theme.breakpoints.up('sm')]: {
                        maxHeight: '455px',
                    },
                    // This 'xl' breakpoint now works
                    [theme.breakpoints.up('xl')]: {
                        maxHeight: 'calc(455px + 80px)',
                    },
                }),
            },
        },
        MuiListItemText: {
            styleOverrides: {
                primary: {
                    fontWeight: 'bold',
                },
            },
        },
    },
    palette: {
        mode: 'light',
        primary: {
            main: '#1976d2',
        },
    },
});

const darkTheme = createTheme(baseTheme, {
    components: {
        MuiTableCell: {
            styleOverrides: {
                head: {
                    fontWeight: 'bold',
                },
            },
        },
    },
    palette: {
        mode: 'dark',
        primary: {
            main: '#90caf4',
        },
    },
});

export { lightTheme, darkTheme };