import React, { createContext } from "react";

const defaultApiUrl = "https://localhost:44326/api/v1";

export const DataContext = createContext({
    apiUrl: process.env.REACT_APP_API_URL || defaultApiUrl,
    pageSize: 10,
});

export function DataProvider({ children }) {
    const value = {
        apiUrl: process.env.REACT_APP_API_URL || defaultApiUrl,
        pageSize: 10,
    };

    return (
        <DataContext.Provider value={value}>
            {children}
        </DataContext.Provider>
    );
}