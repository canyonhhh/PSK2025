import * as React from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import FormLabel from "@mui/material/FormLabel";
import FormControl from "@mui/material/FormControl";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import MuiCard from "@mui/material/Card";
import { styled } from "@mui/material/styles";
import { Link, useNavigate } from "react-router-dom";
import { AppRoutes } from "../../routing/appRoutes";
import { useMutation } from "@tanstack/react-query";
import { useState } from "react";
import ChangePassword from "../loginPage/components/ChangePasword";
import { registerRequest } from "../../api/requests/user/userRequests";
import { useAuthContext } from "../../context/AuthContext";
import { LoginResponseDto } from "../../api/requests/auth/types/LoginResponseDto";
import Stack from "@mui/material/Stack";

const Card = styled(MuiCard)(({ theme }) => ({
    display: "flex",
    flexDirection: "column",
    alignSelf: "center",
    width: "100%",
    padding: theme.spacing(4),
    gap: theme.spacing(2),
    margin: "auto",
    [theme.breakpoints.up("sm")]: {
        maxWidth: "450px",
    },
    boxShadow:
        "hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px",
    ...theme.applyStyles("dark", {
        boxShadow:
            "hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px",
    }),
}));

const SignInContainer = styled(Stack)(({ theme }) => ({
    height: "calc((1 - var(--template-frame-height, 0)) * 100dvh)",
    minHeight: "100%",
    padding: theme.spacing(2),
    [theme.breakpoints.up("sm")]: {
        padding: theme.spacing(4),
    },
    "&::before": {
        content: '""',
        display: "block",
        position: "absolute",
        zIndex: -1,
        inset: 0,
        backgroundImage:
            "radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))",
        backgroundRepeat: "no-repeat",
        ...theme.applyStyles("dark", {
            backgroundImage:
                "radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))",
        }),
    },
}));

export default function RegisterPage() {
    const navigate = useNavigate();
    const { login } = useAuthContext();
    const onRegister = (data: LoginResponseDto) => {
        login(data.token);
        navigate(AppRoutes.MENU);
    };

    const [registerError, setRegisterError] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [repeatPassword, setRepeatPassword] = useState("");
    const [firstName, setFirstname] = useState("");
    const [lastName, setLastName] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");

    const [emailError, setEmailError] = useState(false);
    const [emailErrorMessage, setEmailErrorMessage] = useState("");
    const [repeatPasswordErrorMessage, setRepeatPasswordErrorMessage] =
        useState("");
    const [passwordError, setPasswordError] = useState(false);
    const [passwordErrorMessage, setPasswordErrorMessage] = useState("");
    const [phoneNumberErrorMessage, setPhoneNumberErrorMessage] = useState("");
    const [open, setOpen] = useState(false);
    const [firstNameErrorMessage, setFirstNameErrorMessage] = useState("");
    const [lastNameErrorMessage, setLastNameErrorMessage] = useState("");
    const request = useMutation({
        mutationFn: registerRequest,
        onError: () => setRegisterError(true),
        onSuccess: onRegister,
    });

    const handleClose = () => {
        setOpen(false);
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (emailError || passwordError) {
            event.preventDefault();
            return;
        }
        request.mutate({
            email: email,
            password: password,
            firstName: firstName,
            lastName: lastName,
            phoneNumber: phoneNumber,
        });
    };

    const validateInputs = () => {
        let isValid = true;

        if (!email || !/\S+@\S+\.\S+/.test(email)) {
            setEmailError(true);
            setEmailErrorMessage("Please enter a valid email address.");
            isValid = false;
        } else {
            setEmailError(false);
            setEmailErrorMessage("");
        }

        if (!password || password.length < 6) {
            setPasswordError(true);
            setPasswordErrorMessage(
                "Password must be at least 6 characters long.",
            );
            isValid = false;
        } else {
            setPasswordError(false);
            setPasswordErrorMessage("");
        }

        if (!firstName) {
            setFirstNameErrorMessage("Please enter a first name");
        } else {
            setFirstNameErrorMessage("");
        }

        if (!lastName) {
            setLastNameErrorMessage("Please enter a last name");
        } else {
            setLastNameErrorMessage("");
        }

        if (password != repeatPassword) {
            setRepeatPasswordErrorMessage("Entered password do not match");
        } else {
            setRepeatPasswordErrorMessage("");
        }

        if (!phoneNumber) {
            setPhoneNumberErrorMessage("Please enter a valid phone number");
        } else {
            setPhoneNumberErrorMessage("");
        }

        return isValid;
    };

    return (
        <SignInContainer direction="column" justifyContent="space-between">
            <CssBaseline />
            <Card variant="outlined">
                <Typography
                    component="h1"
                    variant="h4"
                    sx={{
                        width: "100%",
                        fontSize: "clamp(2rem, 10vw, 2.15rem)",
                    }}
                >
                    Sign up
                </Typography>
                <Box
                    component="form"
                    onSubmit={handleSubmit}
                    noValidate
                    sx={{
                        display: "flex",
                        flexDirection: "column",
                        width: "100%",
                        gap: 2,
                    }}
                >
                    <FormControl>
                        <FormLabel htmlFor="email">Email</FormLabel>
                        <TextField
                            error={emailError}
                            helperText={emailErrorMessage}
                            id="email"
                            type="email"
                            name="email"
                            placeholder="your@email.com"
                            autoComplete="email"
                            autoFocus
                            required
                            fullWidth
                            variant="outlined"
                            color={emailError ? "error" : "primary"}
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel htmlFor="firstName">First name</FormLabel>
                        <TextField
                            error={!!firstNameErrorMessage}
                            helperText={firstNameErrorMessage}
                            id="firstName"
                            type="text"
                            name="firstName"
                            placeholder="Name"
                            autoFocus
                            required
                            fullWidth
                            variant="outlined"
                            color={firstNameErrorMessage ? "error" : "primary"}
                            value={firstName}
                            onChange={(e) => setFirstname(e.target.value)}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel htmlFor="firstName">Last name</FormLabel>
                        <TextField
                            error={!!lastNameErrorMessage}
                            helperText={lastNameErrorMessage}
                            id="lastName"
                            type="text"
                            name="lastName"
                            placeholder="Surname"
                            autoFocus
                            required
                            fullWidth
                            variant="outlined"
                            color={lastNameErrorMessage ? "error" : "primary"}
                            value={lastName}
                            onChange={(e) => setLastName(e.target.value)}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel htmlFor="phoneNumber">
                            Phone Number
                        </FormLabel>
                        <TextField
                            error={!!phoneNumberErrorMessage}
                            helperText={phoneNumberErrorMessage}
                            id="phoneNumber"
                            type="tel"
                            name="phoneNumber"
                            placeholder="370********"
                            autoFocus
                            required
                            fullWidth
                            variant="outlined"
                            color={
                                phoneNumberErrorMessage ? "error" : "primary"
                            }
                            value={phoneNumber}
                            onChange={(e) => setPhoneNumber(e.target.value)}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel htmlFor="password">Password</FormLabel>
                        <TextField
                            error={passwordError}
                            helperText={passwordErrorMessage}
                            name="password"
                            placeholder="••••••"
                            type="password"
                            id="password"
                            autoComplete="current-password"
                            autoFocus
                            required
                            fullWidth
                            variant="outlined"
                            color={passwordError ? "error" : "primary"}
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel htmlFor="password">
                            Repeat password
                        </FormLabel>
                        <TextField
                            error={!!repeatPasswordErrorMessage}
                            helperText={repeatPasswordErrorMessage}
                            name="repeat-password"
                            placeholder="••••••"
                            type="password"
                            id="repeat-password"
                            autoComplete="current-password"
                            autoFocus
                            required
                            fullWidth
                            variant="outlined"
                            color={
                                repeatPasswordErrorMessage ? "error" : "primary"
                            }
                            value={repeatPassword}
                            onChange={(e) => setRepeatPassword(e.target.value)}
                        />
                    </FormControl>
                    <ChangePassword open={open} handleClose={handleClose} />
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        onClick={validateInputs}
                    >
                        Sign up
                    </Button>
                </Box>
                <Typography sx={{ textAlign: "center" }}>
                    Already have an account?{" "}
                    <Link to={AppRoutes.LOGIN}>Sign in</Link>
                </Typography>
                <Box minHeight="24px" textAlign="center" mt={1}>
                    <Typography
                        variant="body2"
                        sx={{
                            color: registerError ? "error.main" : "transparent",
                            fontSize: "0.95rem",
                            transition: "color 0.3s ease",
                        }}
                    >
                        {registerError ? "Failed to sign up." : ""}
                    </Typography>
                </Box>
            </Card>
        </SignInContainer>
    );
}
