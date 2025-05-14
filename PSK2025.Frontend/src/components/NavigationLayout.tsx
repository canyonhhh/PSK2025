import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import IconButton from "@mui/material/IconButton";
import MenuIcon from "@mui/icons-material/Menu";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import LogoutIcon from "@mui/icons-material/Logout";
import { Link, Outlet } from "react-router-dom";
import { useAuthContext } from "../context/AuthContext";
import { Role } from "../routing/roles";
import { AppRoutes } from "../routing/appRoutes";

export function NavigationLayout() {
    const { role, logout } = useAuthContext();

    if (!role) {
        return (
            <Box maxWidth="1200px" margin="1rem auto">
                <Outlet />
            </Box>
        );
    }

    const renderContent = () => {
        if (role === Role.MANAGER || role === Role.BARISTA) {
            return (
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
            );
        }
        if (role === Role.CUSTOMER) {
            return (
                <span>
                    <IconButton
                        size="large"
                        aria-label="account of current user"
                        aria-controls="menu-appbar"
                        aria-haspopup="true"
                        color="inherit"
                        onClick={logout}
                    >
                        <Link to={AppRoutes.CART}>
                            <ShoppingCartIcon />
                        </Link>
                        <Link to={AppRoutes.LOGIN}>
                            <LogoutIcon />
                        </Link>
                    </IconButton>
                </span>
            );
        }
    };

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
                    {renderContent()}
                </Toolbar>
            </AppBar>
            <Box maxWidth="1200px" margin="1rem auto">
                <Outlet />
            </Box>
        </Box>
    );
}
