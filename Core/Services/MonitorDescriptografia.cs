using FileCryptoApp.Core.Configuration;
using FileCryptoApp.Infrastructure.FileManagement;

namespace FileCryptoApp.Core.Services
{
    public class MonitorDescriptografia
    {
        private readonly PastasConfig _config;
        private readonly CriptografiaService _criptografiaService;
        private readonly GerenciadorDeArquivos _gerenciadorArquivos;

        public MonitorDescriptografia(PastasConfig config,
                                    CriptografiaService criptografiaService,
                                    GerenciadorDeArquivos gerenciadorArquivos)
        {
            _config = config;
            _criptografiaService = criptografiaService;
            _gerenciadorArquivos = gerenciadorArquivos;
        }

        public void IniciarMonitoramento()
        {
            while (true)
            {
                try
                {
                    string[] arquivos = _gerenciadorArquivos.ObterArquivos(_config.CaminhoArquivosParaDescriptografar);
                    foreach (string arquivo in arquivos)
                    {
                        ProcessarArquivo(arquivo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no monitor de descriptografia: {ex.Message}");
                }
                Thread.Sleep(1000);
            }
        }

        private void ProcessarArquivo(string arquivo)
        {
            try
            {
                string nomeArquivo = _gerenciadorArquivos.ObterNomeArquivo(arquivo);
                string tempFile = _gerenciadorArquivos.CriarArquivoTemporario();
                string arquivoSaida = Path.Combine(_config.CaminhoArquivosDescriptografadosGerados, nomeArquivo.Replace(".encrypted", ""));
                string arquivoFinal = Path.Combine(_config.CaminhoArquivosDescriptografadosFinalizados, nomeArquivo);

                _criptografiaService.DescriptografarArquivo(arquivo, tempFile);
                _gerenciadorArquivos.MoverArquivo(tempFile, arquivoSaida);
                _gerenciadorArquivos.MoverArquivo(arquivo, arquivoFinal);

                Console.WriteLine($"Arquivo descriptografado: {nomeArquivo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {arquivo}: {ex.Message}");
            }
        }
    }
}
