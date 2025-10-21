using FileCryptoApp.Core.Configuration;
using FileCryptoApp.Core.Services;
using FileCryptoApp.Infrastructure.FileManagement;

namespace FileCryptoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var configPastas = new PastasConfig();
            var cryptoService = new CriptografiaService();
            var gerenciadorArquivos = new GerenciadorDeArquivos();

            configPastas.CriarPastas();
            EscreverLogsInicio(configPastas);

            var monitorCripto = new MonitorCriptografia(configPastas, cryptoService, gerenciadorArquivos);
            var monitorDescripto = new MonitorDescriptografia(configPastas, cryptoService, gerenciadorArquivos);

            Task taskCriptografar = Task.Run(() => monitorCripto.IniciarMonitoramento());
            Task taskDescriptografar = Task.Run(() => monitorDescripto.IniciarMonitoramento());

            Task.WaitAll(taskCriptografar, taskDescriptografar);
        }

        private static void EscreverLogsInicio(PastasConfig config)
        {
            Console.WriteLine("Iniciando monitoramento de pastas...");
            Console.WriteLine($"Pasta para criptografar: {config.CaminhoArquivosParaCriptografar}");
            Console.WriteLine($"Pasta para descriptografar: {config.CaminhoArquivosParaDescriptografar}");
        }
    }
}
