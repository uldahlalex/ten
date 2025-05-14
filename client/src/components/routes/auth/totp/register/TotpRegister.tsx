import React from "react";
import {totpClient} from "../../../../../apiControllerClients.ts";
import {TotpRegisterRequestDto} from "../../../../../generated-client.ts";
import toast from "react-hot-toast";

export default function TotpRegister() {
    const handleRegister = async () => {
        // try {
        //     const response = await totpClient.totpRegister(new TotpRegisterRequestDto({
        //         email: email
        //     }));
        //     setQrCode(`data:image/png;base64,${response.qrCodeBase64}`);
        //     toast.success('Scan QR code with your authenticator app');
        // } catch (error) {
        //     toast.error('Registration failed');
        // }
    };
    return(<>

        <div className="space-y-4">
            <h2 className="text-xl font-bold text-center">New Device Setup</h2>
            <button
                onClick={handleRegister}
                className="w-full p-3 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
            >
                Register New Device
            </button>
        </div>

    </>)
}