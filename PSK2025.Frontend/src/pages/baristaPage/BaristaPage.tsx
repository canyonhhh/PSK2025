import {
    Box,
    Button,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Grid,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TextField,
    Typography,
    Paper,
    TablePagination,
} from "@mui/material";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { keys } from "../../api/queryKeyFactory";
import {
    changeRoleReuqest,
    deleteUserRequest,
    fetchAllUsers,
    registerRequest,
} from "../../api/requests/user/userRequests";
import { RegisterDto } from "../../api/requests/user/types/RegisterDto";
import { UserRole } from "../../api/requests/user/types/UserRoles";
import { LoginResponseDto } from "../../api/requests/auth/types/LoginResponseDto";
import { jwtDecode } from "jwt-decode";
import { TokenPayload } from "../../context/AuthContext";

const BaristaPage = () => {
    const queryClient = useQueryClient();
    const [open, setOpen] = useState(false);
    const [page, setPage] = useState(0); // 0-based for TablePagination
    const [rowsPerPage, setRowsPerPage] = useState(10);

    const [formValues, setFormValues] = useState<RegisterDto>({
        firstName: "",
        lastName: "",
        email: "",
        password: "",
        phoneNumber: "",
    });

    const {
        data: paginatedEmployees,
        isLoading,
        isError,
    } = useQuery({
        queryFn: () => fetchAllUsers(page + 1, rowsPerPage),
        queryKey: keys.employees,
    });

    const employees = paginatedEmployees?.items || [];

    const { mutate: deleteUser } = useMutation({
        mutationFn: deleteUserRequest,
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.employees }),
    });

    const { mutate: updateUserRoleToBarista } = useMutation({
        mutationFn: (userId: string) =>
            changeRoleReuqest(userId, UserRole.BARISTA),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: keys.employees }),
    });

    const { mutate: createEmployee, isPending: isCreating } = useMutation({
        mutationFn: (registerDto: RegisterDto) => registerRequest(registerDto),
        onSuccess: (data: LoginResponseDto) => {
            const decoded = jwtDecode<TokenPayload>(data.token);
            const id =
                ((decoded as any)[
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                ] as string) ?? "";
            updateUserRoleToBarista(id);
            setOpen(false);
            setFormValues({
                firstName: "",
                lastName: "",
                email: "",
                password: "",
                phoneNumber: "",
            });
        },
    });

    const handleInputChange = (
        e: React.ChangeEvent<HTMLInputElement>,
    ): void => {
        const { name, value } = e.target;
        setFormValues((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleChangePage = (_: any, newPage: number) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
    ) => {
        setRowsPerPage(parseInt(e.target.value, 10));
        setPage(0);
    };

    return (
        <Box mt={4} mx="auto" maxWidth="900px">
            <Typography variant="h5" gutterBottom>
                Baristas
            </Typography>

            <Button
                variant="contained"
                color="primary"
                onClick={() => setOpen(true)}
                sx={{ mb: 2 }}
            >
                Create New Barista
            </Button>

            {isLoading ? (
                <CircularProgress />
            ) : isError ? (
                <Typography color="error">Failed to fetch users.</Typography>
            ) : (
                <>
                    <TableContainer component={Paper}>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>First Name</TableCell>
                                    <TableCell>Last Name</TableCell>
                                    <TableCell>Email</TableCell>
                                    <TableCell>Actions</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {employees.map((user, index) => (
                                    <TableRow
                                        key={user.id}
                                        sx={{
                                            backgroundColor:
                                                index % 2 === 0
                                                    ? "rgba(0, 0, 0, 0.02)"
                                                    : "transparent",
                                        }}
                                    >
                                        <TableCell>{user.firstName}</TableCell>
                                        <TableCell>{user.lastName}</TableCell>
                                        <TableCell>{user.email}</TableCell>
                                        <TableCell>
                                            <Button
                                                variant="outlined"
                                                color="error"
                                                onClick={() =>
                                                    deleteUser(user.id)
                                                }
                                            >
                                                Delete
                                            </Button>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                        <TablePagination
                            component="div"
                            count={paginatedEmployees?.totalCount || 0}
                            page={page}
                            onPageChange={handleChangePage}
                            rowsPerPage={rowsPerPage}
                            onRowsPerPageChange={handleChangeRowsPerPage}
                            rowsPerPageOptions={[5, 10, 25, 50]}
                        />
                    </TableContainer>
                </>
            )}

            {/* Modal for New Barista */}
            <Dialog
                open={open}
                onClose={() => setOpen(false)}
                fullWidth
                maxWidth="sm"
            >
                <DialogTitle>Create New Barista</DialogTitle>
                <DialogContent>
                    <Grid container spacing={2} mt={1}>
                        <Grid size={6}>
                            <TextField
                                fullWidth
                                label="First Name"
                                name="firstName"
                                value={formValues.firstName}
                                onChange={handleInputChange}
                            />
                        </Grid>
                        <Grid size={6}>
                            <TextField
                                fullWidth
                                label="Last Name"
                                name="lastName"
                                value={formValues.lastName}
                                onChange={handleInputChange}
                            />
                        </Grid>
                        <Grid size={6}>
                            <TextField
                                fullWidth
                                label="Email"
                                name="email"
                                value={formValues.email}
                                onChange={handleInputChange}
                            />
                        </Grid>
                        <Grid size={12}>
                            <TextField
                                fullWidth
                                label="Phone Number"
                                name="phoneNumber"
                                value={formValues.phoneNumber}
                                onChange={handleInputChange}
                            />
                        </Grid>
                        <Grid size={12}>
                            <TextField
                                fullWidth
                                label="Password"
                                name="password"
                                type="password"
                                value={formValues.password}
                                onChange={handleInputChange}
                            />
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setOpen(false)}>Cancel</Button>
                    <Button
                        onClick={() => createEmployee(formValues)}
                        variant="contained"
                        disabled={isCreating}
                    >
                        {isCreating ? "Creating..." : "Create"}
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
};

export default BaristaPage;
