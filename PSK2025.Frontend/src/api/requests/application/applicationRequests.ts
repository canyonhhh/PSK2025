import api from "../../api";
import { StatusResponse } from "./types/StatusResponse";

const CONTROLLER = "/AppSettings";

export const getAppStatus = async () => {
    const result = await api.get<StatusResponse>(`${CONTROLLER}/status`);

    return result.data;
};

export const pauseOrders = async () => {
    await api.post<void>(`${CONTROLLER}/pause`);
};

export const resumeOrders = async () => {
    await api.post<void>(`${CONTROLLER}/resume`);
};
