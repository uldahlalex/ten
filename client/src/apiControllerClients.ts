import {AuthClient, ProblemDetails, TicktickTaskClient, TotpClient} from "@/models";
import toast from "react-hot-toast";


const baseUrlProduction = "https://compulsory25.fly.dev";
const baseUrlDevelopment = "http://localhost:8080";
const prod = import.meta.env.PROD

function createHttpClientWithErrorHandling() {
    return {
        fetch: async (url: RequestInfo, init?: RequestInit) => {
            const response = await fetch(url, init);
            if (response.status == 400) {
                const error = await response.json() as ProblemDetails;
                toast.error(error.title!);
                throw error;
            }
            return response;
        }
    };
}

// Usage:
export const authClient = new AuthClient(
    prod ? baseUrlProduction : baseUrlDevelopment,
    createHttpClientWithErrorHandling()
);
export const totpClient = new TotpClient(
    prod ? baseUrlProduction : baseUrlDevelopment,
    createHttpClientWithErrorHandling()
);
export const taskClient = new TicktickTaskClient(
    prod ? baseUrlProduction : baseUrlDevelopment,
    createHttpClientWithErrorHandling()
);