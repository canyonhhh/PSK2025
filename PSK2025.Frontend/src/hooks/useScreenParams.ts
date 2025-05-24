import { useEffect, useState } from "react";

export enum AppSize {
    SMALL,
    LARGE,
}

export const useScreenParams = (): AppSize => {
    const [appSize, setAppSize] = useState<AppSize>(AppSize.SMALL);

    useEffect(() => {
        const handleResize = () => {
            if (window.innerWidth > 1000) {
                setAppSize(AppSize.LARGE);
            } else {
                setAppSize(AppSize.SMALL);
            }
        };

        window.addEventListener("resize", handleResize);

        return window.removeEventListener("resize", handleResize);
    }, []);

    return appSize;
};
