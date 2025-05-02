import {MyControllerClassClient, AuthClient} from "./generated-client.ts";

const baseUrl = import.meta.env.VITE_API_BASE_URL
const prod = import.meta.env.PROD

 export const authClient = new AuthClient(prod ? "https://" + baseUrl : "http://" + baseUrl);