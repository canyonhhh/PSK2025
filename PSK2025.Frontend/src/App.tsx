import { BrowserRouter, Routes, Route } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import AuthContextProvider from "./context/AuthContext";
import RequireAuth from "./routing/RequireAuth";
import MenuPage from "./pages/menuPage/MenuPage";
import { AppRoutes } from "./routing/appRoutes";
import NotFoundPage from "./pages/notFoundPage/NotFoundPage";
import LoginPage from "./pages/loginPage/LoginPage";
import RegisterPage from "./pages/registerPage/RegisterPage";

const queryClient = new QueryClient();

function App() {
  return (
    <AuthContextProvider>
      <QueryClientProvider client={queryClient}>
        <BrowserRouter>
          <Routes>
            <Route path={AppRoutes.LOGIN} element={<LoginPage />} />
            <Route path={AppRoutes.REGISTER} element={<RegisterPage />} />
            <Route element={<RequireAuth />}>
              <Route path={AppRoutes.MENU} index element={<MenuPage />} />
              <Route path={AppRoutes.NOT_FOUND} element={<NotFoundPage />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </QueryClientProvider>
    </AuthContextProvider>
  );
}

export default App;
