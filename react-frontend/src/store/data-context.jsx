import { createContext } from "react";

export const DataContext = createContext({
    apiUrl: "https://localhost:44362/api/v1",
    pageSize: 10,
});