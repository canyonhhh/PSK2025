import { BrowserRouter, Routes, Route } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import AuthContextProvider from "./context/AuthContext";
import RequireAuth from "./routing/RequireAuth";
import { AppRoutes } from "./routing/appRoutes";
import NotFoundPage from "./pages/notFoundPage/NotFoundPage";
import LoginPage from "./pages/loginPage/LoginPage";
import RegisterPage from "./pages/registerPage/RegisterPage";
import { Role } from "./routing/roles";
import RequireRole from "./routing/RequireRole";
import { NavigationLayout } from "./components/NavigationLayout";
import { ProductMenu } from "./components/productMenu/ProductMenu";
import CartPage from "./pages/cartPage/CartPage";

const queryClient = new QueryClient();

function App() {
    return (
        <AuthContextProvider>
            <QueryClientProvider client={queryClient}>
                <BrowserRouter>
                    <Routes>
                        <Route element={<NavigationLayout />}>
                            <Route path={"/cart"} element={<CartPage />} />
                            <Route
                                path={AppRoutes.LOGIN}
                                element={<LoginPage />}
                            />
                            <Route
                                path={AppRoutes.REGISTER}
                                element={<RegisterPage />}
                            />
                            <Route element={<RequireAuth />}>
                                <Route
                                    element={
                                        <RequireRole
                                            authorizeFor={Role.MANAGER}
                                        />
                                    }
                                ></Route>
                                <Route
                                    element={
                                        <RequireRole
                                            authorizeFor={Role.BARISTA}
                                        />
                                    }
                                ></Route>
                                <Route
                                    element={
                                        <RequireRole
                                            authorizeFor={Role.CUSTOMER}
                                        />
                                    }
                                ></Route>
                                <Route
                                    path={AppRoutes.MENU}
                                    element={<ProductMenu columnCount={1} />}
                                    index
                                />
                                <Route
                                    path={AppRoutes.NOT_FOUND}
                                    element={<NotFoundPage />}
                                />
                            </Route>
                        </Route>
                    </Routes>
                </BrowserRouter>
            </QueryClientProvider>
        </AuthContextProvider>
    );
}

export default App;
