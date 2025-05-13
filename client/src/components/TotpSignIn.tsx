import React, { useState } from 'react';
import {totpClient} from "../apiControllerClients.ts";
import {TotpClient, TotpLoginRequestDto, TotpRegisterRequestDto} from "../generated-client.ts";
import toast from "react-hot-toast";

export default function TotpLogin() {
    const [email, setEmail] = useState('');
    const [totpCode, setTotpCode] = useState('');
    const [isNewUser, setIsNewUser] = useState(false);
    const [qrCode, setQrCode] = useState('');

    const handleRegister = async () => {
    
            const response = await totpClient.totpRegister(new TotpRegisterRequestDto({
                email: email
            }))
        
                setQrCode(`data:image/png;base64,${response.qrCodeBase64}`);
                toast('Scan QR code with your authenticator app');
         
      
    };

    const handleLogin = async () => {
   const response = await totpClient.totpLogin(new TotpLoginRequestDto({
       email: email,
       totpCode: totpCode
   }))


                localStorage.setItem('jwt', response);
                toast('Login successful!');
    
    };

    return (
        <div className="max-w-md mx-auto p-6 space-y-4">
            <div className="space-y-2">
                <input
                    type="text"
                    placeholder="Username"
                    className="w-full p-2 border rounded"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                {!isNewUser && (
                    <input
                        type="text"
                        placeholder="6-digit code"
                        maxLength={6}
                        className="w-full p-2 border rounded"
                        value={totpCode}
                        onChange={(e) => setTotpCode(e.target.value)}
                    />
                )}

                {isNewUser ? (
                    <button
                        onClick={handleRegister}
                        className="w-full p-2 bg-blue-500 text-white rounded"
                    >
                        Register
                    </button>
                ) : (
                    <button
                        onClick={handleLogin}
                        className="w-full p-2 bg-green-500 text-white rounded"
                    >
                        Login
                    </button>
                )}

                <button
                    onClick={() => setIsNewUser(!isNewUser)}
                    className="w-full p-2 bg-gray-200 rounded"
                >
                    {isNewUser ? 'Already have an account?' : 'Need to register?'}
                </button>
            </div>

            {qrCode && (
                <div className="text-center">
                    <img src={qrCode} alt="TOTP QR Code" className="mx-auto" />
                    <p className="text-sm text-gray-600 mt-2">
                        Scan this QR code with your authenticator app
                    </p>
                </div>
            )}
            
        </div>
    );
}