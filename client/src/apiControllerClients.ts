import {AuthClient, TicktickTaskClient, TotpClient} from "./generated-client.ts";

const baseUrlProduction = "https://compulsory25.fly.dev";
const baseUrlDevelopment = "http://localhost:8080";
const prod = import.meta.env.PROD

 export const authClient = new AuthClient(prod ? baseUrlProduction :  baseUrlDevelopment);
 export const totpClient = new TotpClient(prod ? baseUrlProduction :  baseUrlDevelopment);

export const taskClient = new TicktickTaskClient(prod ? baseUrlProduction :  baseUrlDevelopment);