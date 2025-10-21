namespace FileCryptoApp.Infrastructure.FileManagement
{
    public class GerenciadorDeArquivos
    {
        public void MoverArquivo(string origem, string destino)
        {
            File.Move(origem, destino);
        }

        public string[] ObterArquivos(string pasta)
        {
            return Directory.GetFiles(pasta);
        }

        public string ObterNomeArquivo(string caminho)
        {
            return Path.GetFileName(caminho);
        }

        public string CriarArquivoTemporario()
        {
            return Path.GetTempFileName();
        }
    }
}
