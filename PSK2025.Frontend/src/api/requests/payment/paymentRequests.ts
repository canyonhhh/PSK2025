import api from "../../api";

const CONTROLLER = "/Payment";

export interface CreatePaymentIntentDto {
    currency: string;
    expectedCompletionTime: string;
}

export interface CreatePaymentIntentResponse {
    paymentIntentId: string;
    clientSecret: string;
    amount: number;
    currency: string;
}

export interface ConfirmPaymentDto {
    paymentIntentId: string;
}

export interface ConfirmPaymentResponse {
    success: boolean;
    orderId?: string;
    message: string;
}

export const createPaymentIntent = async (
    data: CreatePaymentIntentDto,
): Promise<CreatePaymentIntentResponse> => {
    const response = await api.post<CreatePaymentIntentResponse>(
        `${CONTROLLER}/create-intent`,
        data,
    );
    return response.data;
};

export const confirmPayment = async (
    data: ConfirmPaymentDto,
): Promise<ConfirmPaymentResponse> => {
    const response = await api.post<ConfirmPaymentResponse>(
        `${CONTROLLER}/confirm`,
        data,
    );
    return response.data;
};