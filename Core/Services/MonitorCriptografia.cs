using FileCryptoApp.Core.Configuration;
using FileCryptoApp.Infrastructure.FileManagement;

namespace FileCryptoApp.Core.Services
{
    public class MonitorCriptografia
    {
        private readonly PastasConfig _config;
        private readonly CriptografiaService _criptografiaService;
        private readonly GerenciadorDeArquivos _gerenciadorArquivos;

        public MonitorCriptografia(PastasConfig config, 
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
                    string[] arquivos = _gerenciadorArquivos.ObterArquivos(_config.CaminhoArquivosParaCriptografar);
                    foreach (string arquivo in arquivos)
                    {
                        ProcessarArquivo(arquivo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no monitor de criptografia: {ex.Message}");
                }
                Thread.Sleep(1000);
            }
        }

        private void ProcessarArquivo(string arquivo)
        {
            try
            {
                string nomeArquivo = _gerenciadorArquivos.ObterNomeArquivo(arquivo);
                string arquivoTemporario = _gerenciadorArquivos.CriarArquivoTemporario();
                string caminhoFinalDoArquivoCriptografado = Path.Combine(_config.CaminhoArquivosCriptografadosGerados, nomeArquivo + ".encrypted");
                string caminhoFinalDoArquivoOriginal = Path.Combine(_config.CaminhoArquivosCriptografadosFinalizados, nomeArquivo);

                _criptografiaService.CriptografarArquivo(arquivo, arquivoTemporario);
                _gerenciadorArquivos.MoverArquivo(arquivoTemporario, caminhoFinalDoArquivoCriptografado);
                _gerenciadorArquivos.MoverArquivo(arquivo, caminhoFinalDoArquivoOriginal);

                Console.WriteLine($"Arquivo criptografado: {nomeArquivo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {arquivo}: {ex.Message}");
            }
        }
    }
}
