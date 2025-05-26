import api from "../../api";
import { ChartPoint } from "./types/ChartPoint";
import { ItemOrderCountDto } from "./types/ItemOrderCountDto";

const CONTROLLER = "/Statistics";

export const getOrdersOverTime = async (
    from?: string,
    to?: string,
    grouping?: number,
): Promise<ChartPoint[]> => {
    const params = new URLSearchParams();

    if (from !== undefined) params.append("from", from);
    if (to !== undefined) params.append("to", to);
    if (grouping !== undefined) params.append("grouping", `${grouping}`);

    const data = await api.get<ChartPoint[]>(
        `${CONTROLLER}/orders-over-time?${params.toString()}`,
    );

    return data.data;
};

export const getOrderedItems = async (
    count: number,
    direction: "descending" | "ascending",
): Promise<ItemOrderCountDto[]> => {
    const params = new URLSearchParams();

    if (count !== undefined) params.append("count", `${count}`);
    if (direction !== undefined) params.append("direction", direction);

    const data = await api.get<ItemOrderCountDto[]>(
        `${CONTROLLER}/ordered-items?${params.toString()}`,
    );

    return data.data;
};

export const getItemOverTime = async (
    productId: string,
    from: string,
    to: string,
    grouping: number,
): Promise<ChartPoint[]> => {
    const params = new URLSearchParams();

    if (from !== undefined) params.append("from", from);
    if (to !== undefined) params.append("to", to);
    if (grouping !== undefined) params.append("grouping", `${grouping}`);

    const data = await api.get<ChartPoint[]>(
        `${CONTROLLER}/item/${productId}/over-time?${params.toString()}`,
    );

    return data.data;
};
