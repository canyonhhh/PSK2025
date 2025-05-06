import { Box, Button, Modal, Typography } from "@mui/material";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: "fit-content",
  bgcolor: "background.paper",
  boxShadow: 24,
  p: 4,
};

interface ConfirmationModalProps {
  isOpen: boolean;
  handleClose: () => void;
  handleConfirm: () => void;
  handleDecline: () => void;
  confirmationText: string;
}

const ConfirmationModal = ({
  isOpen,
  handleConfirm,
  handleDecline,
  handleClose,
  confirmationText,
}: ConfirmationModalProps) => {
  return (
    <Modal open={isOpen} onClose={handleClose}>
      <Box sx={style}>
        <Typography
          marginBottom="3rem"
          id="modal-modal-title"
          variant="h6"
          component="h2"
        >
          {confirmationText}
        </Typography>

        <Box display="flex" gap="2rem" justifyContent="center">
          <Button variant="text" color="primary" onClick={handleConfirm}>
            Yes
          </Button>
          <Button variant="outlined" color="error" onClick={handleDecline}>
            No
          </Button>
        </Box>
      </Box>
    </Modal>
  );
};

export default ConfirmationModal;
