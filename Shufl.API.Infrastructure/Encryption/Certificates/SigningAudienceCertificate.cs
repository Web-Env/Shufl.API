using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Shufl.API.Infrastructure.Encryption.Certificates
{
    public class SigningAudienceCertificate : IDisposable
    {
        private readonly RSA rsa;

        public SigningAudienceCertificate()
        {
            rsa = RSA.Create();
        }

        public SigningCredentials GetAudienceSigningKey()
        {
            string privateXmlKey = File.ReadAllText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Encryption/Keys/shufl_private_key.xml")
            );
            rsa.FromXmlString(privateXmlKey);

            return new SigningCredentials(
                key: new RsaSecurityKey(rsa),
                algorithm: SecurityAlgorithms.RsaSha256);
        }

        public void Dispose()
        {
            rsa?.Dispose();
        }
    }
}
