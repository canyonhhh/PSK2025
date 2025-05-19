import { Box, Typography } from "@mui/material";
import {
    DatePicker,
    TimePicker,
    LocalizationProvider,
} from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs, { Dayjs } from "dayjs";

interface PickupTimeSelectorProps {
    date: Dayjs | null;
    setDate: (day: Dayjs | null) => void;
}

const PickupTimeSelector = ({ date, setDate }: PickupTimeSelectorProps) => {
    const now = dayjs();
    const isToday = date?.isSame(now, "day");

    console.log(now);
    return (
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <Box
                sx={{
                    borderRadius: 3,
                    boxShadow: 3,
                    p: 3,
                    backgroundColor: "background.paper",
                    margin: "auto",
                    marginBottom: "2rem",
                }}
            >
                <Typography variant="h6" gutterBottom>
                    Pickup Time
                </Typography>

                <Box mb={2}>
                    <Typography variant="body2" gutterBottom>
                        Date
                    </Typography>
                    <DatePicker
                        value={date}
                        disablePast
                        onChange={(newDate) => setDate(newDate)}
                        format="dddd, MMMM D, YYYY"
                        slotProps={{
                            textField: { fullWidth: true, size: "small" },
                        }}
                    />
                </Box>

                <Box mb={2}>
                    <Typography variant="body2" gutterBottom>
                        Time
                    </Typography>
                    <TimePicker
                        minTime={isToday ? now : undefined}
                        value={date}
                        onChange={(newTime) => setDate(newTime)}
                        slotProps={{
                            textField: { fullWidth: true, size: "small" },
                        }}
                    />
                </Box>

                <Box
                    mt={3}
                    p={2}
                    borderRadius={2}
                    border="1px solid #e0e0e0"
                    bgcolor="#fafafa"
                >
                    <Typography variant="subtitle1" fontWeight="bold">
                        Pickup At:
                    </Typography>
                    <Typography variant="body2">
                        {date?.format("ddd, MMM D [at] HH:mm")}
                    </Typography>
                </Box>
            </Box>
        </LocalizationProvider>
    );
};

export default PickupTimeSelector;
