import {AuthClient, TicktickTaskClient, TotpClient} from "./generated-client.ts";
import toast from "react-hot-toast";


const baseUrlProduction = "https://compulsory25.fly.dev";
const baseUrlDevelopment = "http://localhost:8080";
const prod = import.meta.env.PROD

function createHttpClientWithErrorHandling() {
 return {
  fetch: async (url: RequestInfo, init?: RequestInit) => {
   try {
    const response = await fetch(url, init);
    if (!response.ok) {
     const error = await response.json();
     toast.error(error.title);
     throw error;
    }
    return response;
   } catch (error) {
    toast.error(error.title ?? 'An error occurred');
    throw error;
   }
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