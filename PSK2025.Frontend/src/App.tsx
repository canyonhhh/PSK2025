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
import ManagerOrdersPage from "./pages/ordersPage/ManagerOrdersPage";
import CustomerOrdersPage from "./pages/customerOrders/CustomerOrdersPage";
import BaristaOrdersPage from "./pages/baristaOrdersPage/BaristaOrdersPage";
import StatisticsPage from "./pages/statisticsPage/StatisticsPage";

const queryClient = new QueryClient();

function App() {
    return (
        <AuthContextProvider>
            <QueryClientProvider client={queryClient}>
                <BrowserRouter>
                    <Routes>
                        <Route element={<NavigationLayout />}>
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
                                >
                                    <Route
                                        path={AppRoutes.ALL_ORDERS}
                                        element={<ManagerOrdersPage />}
                                    />
                                    <Route
                                        path={AppRoutes.STATISTICS}
                                        element={<StatisticsPage />}
                                    />
                                </Route>
                                <Route
                                    element={
                                        <RequireRole
                                            authorizeFor={Role.BARISTA}
                                        />
                                    }
                                >
                                    <Route
                                        path={AppRoutes.BARISTA_ORDERS}
                                        element={<BaristaOrdersPage />}
                                    />
                                </Route>
                                <Route
                                    element={
                                        <RequireRole
                                            authorizeFor={Role.CUSTOMER}
                                        />
                                    }
                                >
                                    <Route
                                        path={AppRoutes.CART}
                                        element={<CartPage />}
                                    />
                                    <Route
                                        path={AppRoutes.CUSTOMER_ORDERS}
                                        element={<CustomerOrdersPage />}
                                    />
                                </Route>
                                <Route
                                    path={AppRoutes.MENU}
                                    element={<ProductMenu />}
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
