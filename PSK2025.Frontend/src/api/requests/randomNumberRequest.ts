import api from "../api";
import { RandomNumber } from "../types/RandomNumber";

// TODO: Add error handling
export const fetchRandomNumber = async (): Promise<RandomNumber> => {
    const response = await api.get<RandomNumber>("/api/RandomNumber");
    return response.data;
};
