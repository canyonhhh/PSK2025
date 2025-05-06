import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthContext } from "../context/AuthContext";
import { Role } from "./roles";
import { AppRoutes } from "./appRoutes";

type Props = {
  authorizeFor: Role;
};

const RequireRole = ({ authorizeFor }: Props) => {
  const { role } = useAuthContext();
  const location = useLocation();

  if (!authorizeFor || authorizeFor !== role) {
    return (
      <Navigate to={AppRoutes.NOT_FOUND} state={{ from: location }} replace />
    );
  }

  return <Outlet />;
};

export default RequireRole;
