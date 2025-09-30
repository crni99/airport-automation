import { createContext } from "react";

const defaultApiUrl = "https://localhost:44362/api/v1";

export const DataContext = createContext({
    apiUrl: process.env.REACT_APP_API_URL || defaultApiUrl,
    pageSize: 10,
});
