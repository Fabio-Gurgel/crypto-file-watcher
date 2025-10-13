using System.Security.Cryptography;

namespace FileCryptoApp
{
    class Program
    {
        static readonly string caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
        static readonly string caminhoArquivosParaCriptografar = Path.Combine(caminhoBase, "arquivos_para_criptografar");
        static readonly string caminhoArquivosParaDescriptografar = Path.Combine(caminhoBase, "arquivos_para_descriptografar");
        static readonly string caminhoArquivosFinalizados = Path.Combine(caminhoBase, "arquivos_finalizados");
        static readonly string caminhoArquivosGerados = Path.Combine(caminhoBase, "arquivos_gerados");

        // Pastas específicas para organização
        static readonly string caminhoArquivosCriptografadosFinalizados = Path.Combine(caminhoArquivosFinalizados, "criptografados");
        static readonly string caminhoArquivosDescriptografadosFinalizados = Path.Combine(caminhoArquivosFinalizados, "descriptografados");
        static readonly string caminhoArquivosCriptografadosGerados = Path.Combine(caminhoArquivosGerados, "criptografados");
        static readonly string caminhoArquivosDescriptografadosGerados = Path.Combine(caminhoArquivosGerados, "descriptografados");

        // Chave AES (256 bits) - Em um cenário real isso deve ser armazenado de forma segura
        static readonly byte[] chaveAES = {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16,
            0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24,
            0x25, 0x26, 0x27, 0x28, 0x29, 0x30, 0x31, 0x32
        };

        static void Main(string[] args)
        {
            CriarPastas();

            EscreverLogsDeInicioDaExecucaoDoProjeto();

            Task taskCriptografar = Task.Run(() => MonitorDeCriptografia());
            Task taskDescriptografar = Task.Run(() => MonitorDeDescriptografia());

            Task.WaitAll(taskCriptografar, taskDescriptografar);
        }

        private static void EscreverLogsDeInicioDaExecucaoDoProjeto()
        {
            Console.WriteLine("Iniciando monitoramento de pastas...");
            Console.WriteLine("Pasta para criptografar: " + caminhoArquivosParaCriptografar);
            Console.WriteLine("Pasta para descriptografar: " + caminhoArquivosParaDescriptografar);
        }

        static void CriarPastas()
        {
            Directory.CreateDirectory(caminhoArquivosParaCriptografar);
            Directory.CreateDirectory(caminhoArquivosParaDescriptografar);
            Directory.CreateDirectory(caminhoArquivosCriptografadosFinalizados);
            Directory.CreateDirectory(caminhoArquivosDescriptografadosFinalizados);
            Directory.CreateDirectory(caminhoArquivosCriptografadosGerados);
            Directory.CreateDirectory(caminhoArquivosDescriptografadosGerados);
        }

        static void MonitorDeCriptografia()
        {
            while (true)
            {
                try
                {
                    string[] files = Directory.GetFiles(caminhoArquivosParaCriptografar);
                    foreach (string file in files)
                    {
                        CriptografarArquivo(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no monitor de criptografia: {ex.Message}");
                }

                Thread.Sleep(1000);
            }
        }

        static void MonitorDeDescriptografia()
        {
            while (true)
            {
                try
                {
                    string[] arquivos = Directory.GetFiles(caminhoArquivosParaDescriptografar);
                    foreach (string arquivo in arquivos)
                    {
                        DescriptografarArquivo(arquivo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no monitor de descriptografia: {ex.Message}");
                }

                Thread.Sleep(1000);
            }
        }

        static void CriptografarArquivo(string arquivoDeEntrada)
        {
            try
            {
                string nomeDoArquivo = Path.GetFileName(arquivoDeEntrada);
                string tempDoArquivo = Path.GetTempFileName();
                string caminhoArquivoDeSaida = Path.Combine(caminhoArquivosCriptografadosGerados, nomeDoArquivo + ".encrypted");
                string caminhoArquivoFinalizado = Path.Combine(caminhoArquivosCriptografadosFinalizados, nomeDoArquivo);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = chaveAES;
                    aesAlg.GenerateIV();

                    using (FileStream fsInput = new FileStream(arquivoDeEntrada, FileMode.Open))
                    using (FileStream fsEncrypted = new FileStream(tempDoArquivo, FileMode.Create))
                    {
                        fsEncrypted.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                        using (CryptoStream cs = new CryptoStream(fsEncrypted,
                            aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            fsInput.CopyTo(cs);
                        }
                    }

                    File.Move(tempDoArquivo, caminhoArquivoDeSaida);
                    File.Move(arquivoDeEntrada, caminhoArquivoFinalizado);

                    Console.WriteLine($"Arquivo criptografado: {nomeDoArquivo}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criptografar {arquivoDeEntrada}: {ex.Message}");
            }
        }

        static void DescriptografarArquivo(string arquivoDeEntrada)
        {
            try
            {
                string nomeDoArquivo = Path.GetFileName(arquivoDeEntrada);
                string tempDoArquivo = Path.GetTempFileName();
                string caminhoArquivoDeSaida = Path.Combine(caminhoArquivosDescriptografadosGerados, nomeDoArquivo.Replace(".encrypted", ""));
                string caminhoArquivoFinalizado = Path.Combine(caminhoArquivosDescriptografadosFinalizados, nomeDoArquivo);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = chaveAES;

                    using (FileStream fsInput = new FileStream(arquivoDeEntrada, FileMode.Open))
                    {
                        byte[] iv = new byte[16];
                        fsInput.Read(iv, 0, 16);
                        aesAlg.IV = iv;

                        using (CryptoStream cs = new CryptoStream(fsInput,
                            aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                        using (FileStream fsDecrypted = new FileStream(tempDoArquivo, FileMode.Create))
                        {
                            cs.CopyTo(fsDecrypted);
                        }
                    }

                    File.Move(tempDoArquivo, caminhoArquivoDeSaida);
                    File.Move(arquivoDeEntrada, caminhoArquivoFinalizado);

                    Console.WriteLine($"Arquivo descriptografado: {nomeDoArquivo}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao descriptografar {arquivoDeEntrada}: {ex.Message}");
            }
        }
    }
}
