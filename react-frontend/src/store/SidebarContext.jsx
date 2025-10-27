import React, { createContext, useContext, useState } from 'react';

const SidebarContext = createContext();

export const useSidebar = () => useContext(SidebarContext);

const drawerWidthExpanded = 240;
const drawerWidthShrunk = 60;

export const SidebarProvider = ({ children }) => {
    const [isExpanded, setIsExpanded] = useState(true);

    const toggleSidebar = () => {
        setIsExpanded(prev => !prev);
    };

    const sidebarWidth = isExpanded ? drawerWidthExpanded : drawerWidthShrunk;

    return (
        <SidebarContext.Provider value={{ isExpanded, toggleSidebar, sidebarWidth }}>
            {children}
        </SidebarContext.Provider>
    );
};