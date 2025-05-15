import {AuthClient, TicktickTaskClient, TotpClient} from "./generated-client.ts";
import toast from "react-hot-toast";
import {ProblemDetails} from "./generated-client.ts";


const baseUrlProduction = "https://compulsory25.fly.dev";
const baseUrlDevelopment = "http://localhost:8080";
const prod = import.meta.env.PROD

function createHttpClientWithErrorHandling() {
 return {
  fetch: async (url: RequestInfo, init?: RequestInit) => {
   try {
    const response = await fetch(url, init);
    if (response.status == 400) {
     const error = await response.json() as ProblemDetails;
     toast.error(error.title!);
     throw error;
    }
    return response;
   } catch (error) {
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