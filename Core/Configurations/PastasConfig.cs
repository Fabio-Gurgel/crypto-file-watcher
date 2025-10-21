namespace FileCryptoApp.Core.Configuration
{
    public class PastasConfig
    {
        public string CaminhoBase { get; } = AppDomain.CurrentDomain.BaseDirectory;
        
        public string CaminhoArquivosParaCriptografar => 
            Path.Combine(CaminhoBase, "arquivos_para_criptografar");
        public string CaminhoArquivosParaDescriptografar => 
            Path.Combine(CaminhoBase, "arquivos_para_descriptografar");
        
        // Pastas finais
        public string CaminhoArquivosCriptografadosFinalizados => 
            Path.Combine(CaminhoBase, "arquivos_finalizados", "criptografados");
        public string CaminhoArquivosDescriptografadosFinalizados => 
            Path.Combine(CaminhoBase, "arquivos_finalizados", "descriptografados");
        public string CaminhoArquivosCriptografadosGerados => 
            Path.Combine(CaminhoBase, "arquivos_gerados", "criptografados");
        public string CaminhoArquivosDescriptografadosGerados => 
            Path.Combine(CaminhoBase, "arquivos_gerados", "descriptografados");

        public void CriarPastas()
        {
            Directory.CreateDirectory(CaminhoArquivosParaCriptografar);
            Directory.CreateDirectory(CaminhoArquivosParaDescriptografar);
            Directory.CreateDirectory(CaminhoArquivosCriptografadosFinalizados);
            Directory.CreateDirectory(CaminhoArquivosDescriptografadosFinalizados);
            Directory.CreateDirectory(CaminhoArquivosCriptografadosGerados);
            Directory.CreateDirectory(CaminhoArquivosDescriptografadosGerados);
        }
    }
}
