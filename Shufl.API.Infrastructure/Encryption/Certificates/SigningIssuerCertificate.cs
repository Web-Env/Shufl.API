using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Shufl.API.Infrastructure.Encryption.Certificates
{
    public class SigningIssuerCertificate : IDisposable
    {
        private readonly RSA rsa;

        public SigningIssuerCertificate()
        {
            rsa = RSA.Create();
        }

        public RsaSecurityKey GetIssuerSigningKey()
        {
            string publicXmlKey = File.ReadAllText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Encryption/Keys/shufl_public_key.xml")
            );
            rsa.FromXmlString(publicXmlKey);

            return new RsaSecurityKey(rsa);
        }

        public void Dispose()
        {
            rsa?.Dispose();
        }
    }
}
