using System.Security.Cryptography;

namespace FileCryptoApp.Core.Services
{
    public class CriptografiaService
    {
        private readonly byte[] _chaveAES = {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16,
            0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24,
            0x25, 0x26, 0x27, 0x28, 0x29, 0x30, 0x31, 0x32
        };

        public void CriptografarArquivo(string arquivoOriginal, string arquivoTemporarioASerSalvo)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _chaveAES;
                aesAlg.GenerateIV();

                using (FileStream fsInput = new FileStream(arquivoOriginal, FileMode.Open))
                using (FileStream fsEncrypted = new FileStream(arquivoTemporarioASerSalvo, FileMode.Create))
                {
                    fsEncrypted.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream cs = new CryptoStream(fsEncrypted,
                        aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        fsInput.CopyTo(cs);
                    }
                }
            }
        }

        public void DescriptografarArquivo(string arquivoOriginal, string arquivoTemporarioASerSalvo)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _chaveAES;

                using (FileStream fsInput = new FileStream(arquivoOriginal, FileMode.Open))
                {
                    byte[] iv = new byte[16];
                    fsInput.Read(iv, 0, 16);
                    aesAlg.IV = iv;

                    using (CryptoStream cs = new CryptoStream(fsInput,
                        aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    using (FileStream fsDecrypted = new FileStream(arquivoTemporarioASerSalvo, FileMode.Create))
                    {
                        cs.CopyTo(fsDecrypted);
                    }
                }
            }
        }
    }
}
