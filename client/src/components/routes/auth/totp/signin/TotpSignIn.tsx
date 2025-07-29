import React, {useState} from 'react';
import {totpClient} from "../../../../../apiControllerClients";
import {useAtom} from 'jotai';
import toast from "react-hot-toast";
import {JwtAtom} from "@/atoms";
import SignOut from "../../../../../functions/signOut";
import {useNavigate} from "react-router-dom";
import {mainLayoutPath} from "../../../../routes.ts";

export default function TotpSignIn() {
    const [jwt, setJwt] = useAtom(JwtAtom);
    const [totpCode, setTotpCode] = useState('');
    const [email, setEmail] = useState('');
    const navigate = useNavigate();


    const handleLogin = async () => {
        try {
            const response = await totpClient.totpLogin(({
                totpCode: totpCode,
                email: email
            }));

            setJwt(response);
            toast.success('Login successful!');
            setTotpCode('');
            navigate(mainLayoutPath)
        } catch {
            toast.error('Invalid code');
            setTotpCode('');
        }
    };

    // const handleRotate = async () => {
    //     try {
    //         if (!jwt) {
    //             toast.error('Please login first');
    //             return;
    //         }
    //
    //         const response = await totpClient.totpRotate(
    //             new TotpRotateRequestDto({currentTotpCode: totpCode}),
    //             jwt.jwt
    //         );
    //
    //         setQrCode(`data:image/png;base64,${response.qrCodeBase64}`);
    //         toast.success('TOTP secret rotated. Scan new QR code');
    //         setTotpCode('');
    //     } catch (error) {
    //         toast.error('Failed to rotate TOTP secret');
    //         setTotpCode('');
    //     }
    // };

    const handleUnregister = async () => {
        try {
            await totpClient.toptUnregister({
                totpCode: totpCode,
            });

            SignOut()

        } catch {
            toast.error('Failed to unregister device');
            setTotpCode('');
        }
    };

    return (
        <div className="flex flex-col items-center justify-center h-screen">


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


        </div>
    );
}