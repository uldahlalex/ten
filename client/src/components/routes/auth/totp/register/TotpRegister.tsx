import React, {useState} from "react";
import {totpClient} from "../../../../../apiControllerClients.ts";
import toast from "react-hot-toast";

export default function TotpRegister() {

    const [email, setEmail] = useState<string>('');
    const [qrCode, setQrCode] = useState('');


    const handleRegister = async () => {
        try {
            const response = await totpClient.totpRegister(({
                email: email
            }));
            setQrCode(`data:image/png;base64,${response.qrCodeBase64}`);
            toast.success('Scan QR code with your authenticator app');
        } catch (error) {
            toast.error('Registration failed');
        }
    };
    return (<>

        <div className="space-y-4">
            <h2 className="text-xl font-bold text-center">New Device Setup</h2>
            <input
                type="text"
                className="input input-bordered w-full"
                placeholder="Enter your email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
            />
            <button
                onClick={handleRegister}
                className="w-full p-3 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
            >
                Register New Device
            </button>

            {
                (qrCode.length != 0) && (<div className="text-center space-y-3 p-4 border rounded-lg">
                    <img
                        src={qrCode}
                        alt="TOTP QR Code"
                        className="mx-auto max-w-[200px]"
                    />
                    <p className="text-sm text-gray-600">
                        Scan this QR code with your authenticator app
                    </p>
                </div>)

            }
        </div>

    </>)
}