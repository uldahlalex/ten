import {AuthClient, JwtResponse, ProblemDetails, TicktickTaskClient, TotpClient} from "@/models";
import toast from "react-hot-toast";


const baseUrlProduction = "https://compulsory25.fly.dev";
const baseUrlDevelopment = "http://localhost:8080";
const prod = import.meta.env.PROD

function createHttpClientWithErrorHandling() {
    return {
        fetch: async (url: RequestInfo, init?: RequestInit) => {
            // Get JWT token from localStorage
            const token = localStorage.getItem('jwt');
            const tokenAsObject = token ? JSON.parse(token) as JwtResponse : undefined;
            
            // Add Authorization header if token exists and not already present
            const headers = new Headers(init?.headers);
            if (token && !headers.has('Authorization')) {
                headers.set('Authorization', `Bearer ${tokenAsObject?.jwt}`);
            }
            
            // Create new init object with updated headers
            const updatedInit: RequestInit = {
                ...init,
                headers: headers
            };
            
            const response = await fetch(url, updatedInit);
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