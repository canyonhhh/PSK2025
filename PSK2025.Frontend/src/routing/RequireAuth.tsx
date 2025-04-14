import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthContext } from "../context/AuthContext";
import { AppRoutes } from "./appRoutes";

const RequireAuth = () => {
  const { token } = useAuthContext();
  const location = useLocation();

  if (!token) {
    return <Navigate to={AppRoutes.LOGIN} state={{ from: location }} replace />;
  }

  return <Outlet />;
};

export default RequireAuth;
