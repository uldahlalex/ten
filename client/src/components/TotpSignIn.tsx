import React, {useState} from 'react';
import {totpClient} from "../apiControllerClients.ts";
import {
    TotpLoginRequestDto, TotpRegisterRequestDto,
    TotpRegisterResponseDto,
    TotpRotateRequestDto,
    TotpUnregisterRequestDto
} from "../generated-client.ts";
import {useAtom} from 'jotai';
import toast from "react-hot-toast";
import {jwtDecode} from "jwt-decode";
import {JwtAtom} from "../atoms.ts";
import {JwtClaims} from "../JwtClaims.ts";
import SignOut from "../signOut.tsx"; // assuming you're using jwt-decode

export default function TotpAuth() {
    const [jwt, setJwt] = useAtom(JwtAtom);
    const [totpCode, setTotpCode] = useState('');
    const [qrCode, setQrCode] = useState('');
    const [email, setEmail] = useState('');
    
    const handleRegister = async () => {
        try {
            const response = await totpClient.totpRegister(new TotpRegisterRequestDto({
                email: email 
            }));
            setQrCode(`data:image/png;base64,${response.qrCodeBase64}`);
            toast.success('Scan QR code with your authenticator app');
        } catch (error) {
            toast.error('Registration failed');
        }
    };

    const handleLogin = async () => {
        try {
            const response = await totpClient.totpLogin(new TotpLoginRequestDto({
                totpCode: totpCode,
                email: email
            }));

            setJwt(response);
            toast.success('Login successful!');
            setTotpCode('');
        } catch (error) {
            toast.error('Invalid code');
            setTotpCode('');
        }
    };

    const handleRotate = async () => {
        try {
            if (!jwt) {
                toast.error('Please login first');
                return;
            }

            const response = await totpClient.totpRotate(
                new TotpRotateRequestDto({currentTotpCode: totpCode}),
                jwt.jwt
            );

            setQrCode(`data:image/png;base64,${response.qrCodeBase64}`);
            toast.success('TOTP secret rotated. Scan new QR code');
            setTotpCode('');
        } catch (error) {
            toast.error('Failed to rotate TOTP secret');
            setTotpCode('');
        }
    };

    const handleUnregister = async () => {
        try {
            await totpClient.toptUnregister(new TotpUnregisterRequestDto({
                totpCode: totpCode, 
            }), jwt!.jwt);

            SignOut()

        } catch (error) {
            toast.error('Failed to unregister device');
            setTotpCode('');
        }
    };

    return (
        <div className="max-w-md mx-auto p-6 space-y-6">

            <div className="space-y-4">
                <h2 className="text-xl font-bold text-center">New Device Setup</h2>
                <button
                    onClick={handleRegister}
                    className="w-full p-3 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
                >
                    Register New Device
                </button>
            </div>

            <div className="space-y-4">
                <h2 className="text-xl font-bold text-center">Enter Authentication Code</h2>
                <input
                    type="text"
                    maxLength={6}
                    placeholder="Enter 6-digit code"
                    className="w-full p-3 border rounded-lg text-center text-2xl tracking-wider"
                    value={totpCode}
                    onChange={(e) => setTotpCode(e.target.value.replace(/\D/g, '').slice(0, 6))}
                />

                <div className="grid grid-cols-2 gap-3">
                    <button
                        onClick={handleLogin}
                        disabled={totpCode.length !== 6}
                        className="p-3 bg-green-500 text-white rounded-lg hover:bg-green-600 
                                     disabled:bg-gray-300 disabled:cursor-not-allowed"
                    >
                        Verify Code
                    </button>
                    {/*email field*/}
                    <input
                        type="email"
                        placeholder="Enter your email"
                        className="w-full p-3 border rounded-lg text-center"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        />

                    <button
                        onClick={handleRotate}
                        disabled={totpCode.length !== 6}
                        className="p-3 bg-yellow-500 text-white rounded-lg hover:bg-yellow-600 
                                     disabled:bg-gray-300 disabled:cursor-not-allowed"
                    >
                        Rotate Secret
                    </button>
                </div>

                <button
                    onClick={handleUnregister}
                    disabled={totpCode.length !== 6}
                    className="w-full p-3 bg-red-500 text-white rounded-lg hover:bg-red-600 
                                 disabled:bg-gray-300 disabled:cursor-not-allowed"
                >
                    Unregister Device
                </button>
            </div>


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
    );
}