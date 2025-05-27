export const keys = {
    randomNumber: ["randomNumber"],
    allProducts: (pageCount: number) => [
        "allproducts",
        `allProducts_${pageCount}`,
    ],
    allProductsAll: ["allproducts"],
    orders: {
        all: ["all_orders"],
        page: (page: number) => ["all_orders", `all_orders_${page}`],
        my: ["my_orders"],
        barista: {
            all: ["all_orders_barista"],
            page: (page: number) => [
                "all_orders_barista",
                `all_orders_barista_${page}`,
            ],
        },
    },
    activeCart: ["activeCart"],
    ordersByUser: (userId: string) => [`orders_${userId}`],
    ordersOverTime: {
        all: ["orders_over_time"],
        byParams: (from: string, to: string, grouping: number) => [
            "orders_over_time",
            `from_${from}_to_${to}_grouping_${grouping}`,
        ],
    },
    topItems: {
        top: ["top_products"],
        bottom: ["shit_products"],
    },
    employees: ["employees"],
    appStatus: ["app_status"],
};
