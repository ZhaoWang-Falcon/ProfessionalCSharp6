﻿using System;
using System.Security.Cryptography;
using System.Text;
using static System.Console;

namespace SigningDemo
{
    public class Program
    {

        private CngKey _aliceKeySignature;
        private byte[] _alicePubKeyBlob;

        public static void Main()
        {
            var p = new Program();
            p.Run();
        }

        public void Run()
        {
            InitAliceKeys();

            byte[] aliceData = Encoding.UTF8.GetBytes("Alice");
            byte[] aliceSignature = CreateSignature(aliceData, _aliceKeySignature);
            WriteLine($"Alice created signature: {Convert.ToBase64String(aliceSignature)}");

            if (VerifySignature(aliceData, aliceSignature, _alicePubKeyBlob))
            {
                WriteLine("Alice signature verified successfully");
            }
        }

        public void InitAliceKeys()
        {
            _aliceKeySignature = CngKey.Create(CngAlgorithm.ECDsaP521);
            _alicePubKeyBlob = _aliceKeySignature.Export(CngKeyBlobFormat.GenericPublicBlob);
        }

        private byte[] CreateSignature(byte[] data, CngKey key)
        {

            byte[] signature;
            using (var signingAlg = new ECDsaCng(key))
            {
#if DNXCORE50
                signature = signingAlg.SignData(data, HashAlgorithmName.SHA512);
#else
                signature = signingAlg.SignData(data);
                signingAlg.Clear();
#endif


            }
            return signature;
        }

        private bool VerifySignature(byte[] data, byte[] signature, byte[] pubKey)
        {
            bool retValue = false;
            using (CngKey key = CngKey.Import(pubKey, CngKeyBlobFormat.GenericPublicBlob))
            using (var signingAlg = new ECDsaCng(key))
            {
#if DNXCORE50
                retValue = signingAlg.VerifyData(data, signature, HashAlgorithmName.SHA512);
#else
                retValue = signingAlg.VerifyData(data, signature);
                signingAlg.Clear();
#endif
            }
            return retValue;
        }
    }
}