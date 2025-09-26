import { createTheme, responsiveFontSizes } from '@mui/material/styles';

const darkBlueColor = '#0a1929';
const whiteColor = '#ffffff';
const greyColor = 'rgba(255, 255, 255, 0.7)';

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
            contrastText: whiteColor,
        },
        background: {
            default: '#eaeff1',
            paper: whiteColor,
        },
        text: {
            primary: 'rgba(0, 0, 0, 0.87)',
            secondary: 'rgba(0, 0, 0, 0.54)',
        },
    },
    components: {
        MuiTableContainer: {
            styleOverrides: {
                root: {
                    '@media (min-height: 0px) and (max-height: 655px)': {
                        maxHeight: '373px',
                    },
                    '@media (min-height: 656px) and (max-height: 725px)': {
                        maxHeight: '438px',
                    },
                    '@media (min-height: 726px) and (max-height: 795px)': {
                        maxHeight: '503px',
                    },
                    '@media (min-height: 796px) and (max-height: 865px)': {
                        maxHeight: '568px',
                    },
                    '@media (min-height: 866px) and (max-height: 934px)': {
                        maxHeight: '698px',
                    },
                    '@media (min-height: 935px) and (max-height: 1004px)': {
                        maxHeight: '698px',
                    },
                    '@media (min-height: 1005px) and (max-height: 1074px)': {
                        maxHeight: '764px',
                    },
                    '@media (min-height: 1075px) and (max-height: 1144px)': {
                        maxHeight: '828px',
                    },
                    '@media (min-height: 1145px) and (max-height: 1214px)': {
                        maxHeight: '894px',
                    },
                    '@media (min-height: 1215px) and (max-height: 1284px)': {
                        maxHeight: '958px',
                    },
                    '@media (min-height: 1285px) and (max-height: 1354px)': {
                        maxHeight: '1024px',
                    },
                    '@media (min-height: 1355px) and (max-height: 1424px)': {
                        maxHeight: '1090px',
                    },
                    '@media (min-height: 1425px) and (max-height: 1494px)': {
                        maxHeight: '1154px',
                    },
                    '@media (min-height: 1495px) and (max-height: 1564px)': {
                        maxHeight: '1220px',
                    },
                    '@media (min-height: 1565px) and (max-height: 1634px)': {
                        maxHeight: '1286px',
                    },
                    '@media (min-height: 1634px)': {
                        maxHeight: '1350px',
                    },
                    '&::-webkit-scrollbar': {
                        width: '8px',
                        height: '8px',
                    },
                    '&::-webkit-scrollbar-track': {
                        backgroundColor: darkBlueColor,
                    },
                    '&::-webkit-scrollbar-thumb': {
                        backgroundColor: greyColor,
                        borderRadius: '10px',
                        '&:hover': {
                            backgroundColor: whiteColor,
                        },
                    },
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
                        // backgroundColor: ${darkBlueColor},
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
                    backgroundColor: whiteColor,
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
            contrastText: whiteColor,
        },
        background: {
            default: '#121212',
            paper: '#1d1d1d',
        },
        text: {
            primary: whiteColor,
            secondary: 'rgba(255, 255, 255, 0.7)',
        },
    },
    components: {
        MuiTableContainer: {
            styleOverrides: {
                root: {
                    '@media (min-height: 0px) and (max-height: 655px)': {
                        maxHeight: '373px',
                    },
                    '@media (min-height: 656px) and (max-height: 725px)': {
                        maxHeight: '438px',
                    },
                    '@media (min-height: 726px) and (max-height: 795px)': {
                        maxHeight: '503px',
                    },
                    '@media (min-height: 796px) and (max-height: 865px)': {
                        maxHeight: '568px',
                    },
                    '@media (min-height: 866px) and (max-height: 934px)': {
                        maxHeight: '698px',
                    },
                    '@media (min-height: 935px) and (max-height: 1004px)': {
                        maxHeight: '698px',
                    },
                    '@media (min-height: 1005px) and (max-height: 1074px)': {
                        maxHeight: '764px',
                    },
                    '@media (min-height: 1075px) and (max-height: 1144px)': {
                        maxHeight: '828px',
                    },
                    '@media (min-height: 1145px) and (max-height: 1214px)': {
                        maxHeight: '894px',
                    },
                    '@media (min-height: 1215px) and (max-height: 1284px)': {
                        maxHeight: '958px',
                    },
                    '@media (min-height: 1285px) and (max-height: 1354px)': {
                        maxHeight: '1024px',
                    },
                    '@media (min-height: 1355px) and (max-height: 1424px)': {
                        maxHeight: '1090px',
                    },
                    '@media (min-height: 1425px) and (max-height: 1494px)': {
                        maxHeight: '1154px',
                    },
                    '@media (min-height: 1495px) and (max-height: 1564px)': {
                        maxHeight: '1220px',
                    },
                    '@media (min-height: 1565px) and (max-height: 1634px)': {
                        maxHeight: '1286px',
                    },
                    '@media (min-height: 1634px)': {
                        maxHeight: '1350px',
                    },
                    '&::-webkit-scrollbar': {
                        width: '8px',
                        height: '8px',
                    },
                    '&::-webkit-scrollbar-track': {
                        backgroundColor: darkBlueColor,
                    },
                    '&::-webkit-scrollbar-thumb': {
                        backgroundColor: greyColor,
                        borderRadius: '10px',
                        '&:hover': {
                            backgroundColor: whiteColor,
                        },
                    },
                },
            },
        },
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
                        backgroundColor: darkBlueColor,
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
        MuiTextField: {
            styleOverrides: {
                root: ({ ownerState }) => ({
                    '& input[type="time"]': {
                        colorScheme: 'light',
                        '&::-webkit-calendar-picker-indicator': {
                            filter: 'invert(100%)',
                        },
                    },
                    '& input[type="date"]': {
                        colorScheme: 'light',
                        '&::-webkit-calendar-picker-indicator': {
                            filter: 'invert(100%)',
                        },
                    },
                }),
            },
        },
        MuiTablePagination: {
            styleOverrides: {
                root: ({ ownerState, theme }) => ({
                    '& .MuiIconButton-root': {
                        color: whiteColor,
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
                    backgroundColor: darkBlueColor,
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
        MuiInputBase: {
            styleOverrides: {
                input: {
                    '&:-webkit-autofill, &:-webkit-autofill:hover, &:-webkit-autofill:focus, &:-webkit-autofill:active': {
                        WebkitBoxShadow: `0 0 0 1000px ${darkBlueColor} inset !important`,
                        WebkitTextFillColor: `${whiteColor} !important`,
                        caretColor: `${whiteColor} !important`,
                    },
                    '&:-internal-autofill-selected': {
                        WebkitBoxShadow: `0 0 0 1000px ${darkBlueColor} inset !important`,
                        WebkitTextFillColor: `${whiteColor} !important`,
                        caretColor: `${whiteColor} !important`,
                    },
                },
            },
        },
    },
});

lightTheme = responsiveFontSizes(lightTheme);
darkTheme = responsiveFontSizes(darkTheme);

export { lightTheme, darkTheme };