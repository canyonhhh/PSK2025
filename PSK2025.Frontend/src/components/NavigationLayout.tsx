import { useState } from "react";
import {
    AppBar,
    Box,
    Toolbar,
    Typography,
    IconButton,
    Drawer,
    List,
    ListItemText,
    ListItemButton,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import LogoutIcon from "@mui/icons-material/Logout";
import { Link, Outlet } from "react-router-dom";
import { useAuthContext } from "../context/AuthContext";
import { AppRoutes } from "../routing/appRoutes";
import { Role } from "../routing/roles";

export function NavigationLayout() {
    const { role, logout } = useAuthContext();
    const [drawerOpen, setDrawerOpen] = useState(false);

    const toggleDrawer = (open: boolean) => () => {
        setDrawerOpen(open);
    };

    if (!role) {
        return (
            <Box maxWidth="1200px" margin="1rem auto">
                <Outlet />
            </Box>
        );
    }

    const getRoleSpecificDrawerItems = () => {
        if (role === Role.CUSTOMER) {
            return (
                <List>
                    <ListItemButton component={Link} to={AppRoutes.CART}>
                        <ListItemText primary="Cart" />
                    </ListItemButton>
                    <ListItemButton component={Link} to={AppRoutes.MENU}>
                        <ListItemText primary="Menu" />
                    </ListItemButton>
                    <ListItemButton
                        component={Link}
                        to={AppRoutes.CUSTOMER_ORDERS}
                    >
                        <ListItemText primary="My Orders" />
                    </ListItemButton>
                </List>
            );
        }
        if (role === Role.BARISTA) {
            return (
                <List>
                    <ListItemButton component={Link} to={AppRoutes.MENU}>
                        <ListItemText primary="Menu" />
                    </ListItemButton>
                    <ListItemButton
                        component={Link}
                        to={AppRoutes.BARISTA_ORDERS}
                    >
                        <ListItemText primary="Orders" />
                    </ListItemButton>
                </List>
            );
        }
        if (role === Role.MANAGER) {
            return (
                <List>
                    <ListItemButton component={Link} to={AppRoutes.MENU}>
                        <ListItemText primary="Menu" />
                    </ListItemButton>
                    <ListItemButton component={Link} to={AppRoutes.ALL_ORDERS}>
                        <ListItemText primary="Orders" />
                    </ListItemButton>
                    <ListItemButton component={Link} to={AppRoutes.STATISTICS}>
                        <ListItemText primary="Statistics" />
                    </ListItemButton>
                </List>
            );
        }
    };

    const drawerItems = (
        <Box
            sx={{ width: 250 }}
            role="presentation"
            onClick={toggleDrawer(false)}
        >
            {getRoleSpecificDrawerItems()}
        </Box>
    );

    return (
        <Box sx={{ flexGrow: 1 }}>
            <AppBar position="static">
                <Toolbar>
                    <IconButton
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="menu"
                        sx={{ mr: 2 }}
                        onClick={toggleDrawer(true)}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography
                        variant="h6"
                        component="div"
                        sx={{ flexGrow: 1 }}
                    >
                        Coffee shop
                    </Typography>
                    <span>
                        <IconButton
                            size="large"
                            aria-label="account of current user"
                            aria-controls="menu-appbar"
                            aria-haspopup="true"
                            color="inherit"
                            onClick={logout}
                        >
                            <Link to={AppRoutes.LOGIN}>
                                <LogoutIcon />
                            </Link>
                        </IconButton>
                    </span>
                </Toolbar>
            </AppBar>
            <Drawer
                anchor="left"
                open={drawerOpen}
                onClose={toggleDrawer(false)}
            >
                {drawerItems}
            </Drawer>
            <Box maxWidth="1200px" margin="1rem auto" paddingX="1rem">
                <Outlet />
            </Box>
        </Box>
    );
}
