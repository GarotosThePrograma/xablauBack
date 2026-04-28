using xablauBack.Domain.Entities;

namespace xablauBack.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedProdutosAsync(AppDbContext context)
    {
        if (context.Produtos.Any())
        {
            return;
        }

        var produtos = new List<Produto>
        {
            new Produto
            {
                Nome = "Placa de Vídeo Challenger ASRock AMD Radeon RX 7600 8GB",
                Descricao = "Placa de Vídeo Challenger ASRock AMD Radeon RX 7600 8GB",
                Preco = 1996.65m,
                Estoque = 0,
                ImagemUrl = "https://images4.kabum.com.br/produtos/fotos/907814/placa-de-video-asrock-amd-radeon-rx-7600-challenger-pro-oc-8gb-gddr6-triple-fan-90-ga62zz-00uanf_1761582764_gg.jpg"
            },
            new Produto
            {
                Nome = "Placa De Vídeo Husky Alpha RX 580 AMD 8GB GDDR5",
                Descricao = "Placa De Vídeo Husky Alpha RX 580 AMD 8GB GDDR5",
                Preco = 899.99m,
                Estoque = 10,
                ImagemUrl = "https://images1.kabum.com.br/produtos/fotos/692461/placa-de-video-husky-alpha-rx-580-amd-8gb-gddr5-256-bit-hdmi-dvi-displayport-hvg580_1773838121_gg.jpg"
            },
            new Produto
            {
                Nome = "Placa de Vídeo ASRock RX 6600 Challenger White AMD Radeon 8GB",
                Descricao = "Placa de Vídeo ASRock RX 6600 Challenger White AMD Radeon 8GB",
                Preco = 1499.99m,
                Estoque = 5,
                ImagemUrl = "https://images7.kabum.com.br/produtos/fotos/695107/placa-de-video-asrock-rx-6600-challenger-white-amd-radeon-8gb-gddr6-directx-12-ultimate-rdna-2-90-ga4uzz-00uanf_1742841360_gg.jpg"
            },
            new Produto
            {
                Nome = "Monitor Gamer Curvo AOC Agon 34\", Ultrawide, WQHD, 180Hz, 0.5ms",
                Descricao = "Monitor Gamer Curvo AOC Agon 34\", Ultrawide, WQHD, 180Hz, 0.5ms",
                Preco = 1999.90m,
                Estoque = 11,
                ImagemUrl = "https://images7.kabum.com.br/produtos/fotos/909137/monitor-gamer-curvo-aoc-agon-34-ultrawide-wqhd-180hz-0-5ms-fast-va-altura-ajustavel-preto-u34g4c_1757617485_gg.jpg"
            },
            new Produto
            {
                Nome = "Monitor Gamer 27\" 1ms, 100hz, Ips, Amd Freesync, Full Hd, HDMI, Dp",
                Descricao = "Monitor Gamer 27\" 1ms, 100hz, Ips, Amd Freesync, Full Hd, HDMI, Dp",
                Preco = 719.10m,
                Estoque = 3,
                ImagemUrl = "https://images9.kabum.com.br/produtos/fotos/sync_mirakl/687719/xlarge/Monitor-Gamer-27-1ms-100hz-Ips-Amd-Freesync-Full-Hd-HDMI-Dp-Frameless-Hq-Premium-Hq27ip10_1768419216.jpg"
            },
            new Produto
            {
                Nome = "Monitor Gamer AOC 27\", Full HD, 200Hz, 0.3ms, IPS, G-Sync, HDR",
                Descricao = "Monitor Gamer AOC 27\", Full HD, 200Hz, 0.3ms, IPS, G-Sync, HDR",
                Preco = 899.90m,
                Estoque = 7,
                ImagemUrl = "https://images1.kabum.com.br/produtos/fotos/909131/monitor-gamer-aoc-27-fhd-200hz-0-3ms-ips-g-sync-hdr-preto-27g42he_1759351613_gg.jpg"
            },
            new Produto
            {
                Nome = "Console Nintendo Switch 2 + Jogo Digital Mario Kart World",
                Descricao = "Console Nintendo Switch 2 + Jogo Digital Mario Kart World",
                Preco = 4463.07m,
                Estoque = 1,
                ImagemUrl = "https://images8.kabum.com.br/produtos/fotos/779788/console-nintendo-switch-2-jogo-digital-mario-kart-world_1758717740_gg.jpg"
            },
            new Produto
            {
                Nome = "XBOX 360 Super Slim Standard Preto",
                Descricao = "XBOX 360 Super Slim Standard Preto",
                Preco = 1313.10m,
                Estoque = 4,
                ImagemUrl = "https://images6.kabum.com.br/produtos/fotos/sync_mirakl/614426/xlarge/XBOX-360-Super-Slim-Standard-Preto_1762175018.jpg"
            },
            new Produto
            {
                Nome = "Ps4 Playstation Slim 500gb",
                Descricao = "Ps4 Playstation Slim 500gb",
                Preco = 2497.00m,
                Estoque = 12,
                ImagemUrl = "https://images2.kabum.com.br/produtos/fotos/sync_mirakl/657652/xlarge/Ps4-Playstation-Slim-500gb_1765579432.jpg"
            },
            new Produto
            {
                Nome = "Teclado Gamer Razer Huntsman Mini, 60%, Chroma RGB, Swich Roxo",
                Descricao = "Teclado Gamer Razer Huntsman Mini, 60%, Chroma RGB, Swich Roxo",
                Preco = 1209.90m,
                Estoque = 2,
                ImagemUrl = "https://images1.kabum.com.br/produtos/fotos/735181/teclado-gamer-razer-huntsman-mini-60-chroma-rgb-swich-roxo-usb-c-preto-rz0303390500_1746555414_gg.jpg"
            },
            new Produto
            {
                Nome = "Teclado Mecânico Gamer Husky Anchorage Full Size, RGB, Switch Gateron",
                Descricao = "Teclado Mecânico Gamer Husky Anchorage Full Size, RGB, Switch Gateron",
                Preco = 179.99m,
                Estoque = 15,
                ImagemUrl = "https://images2.kabum.com.br/produtos/fotos/538692/teclado-mecanico-gamer-husky-anchorage-full-size-branco-abnt2-rgb-switch-gateron-ef-red-htg200brvr_1744641257_gg.jpg"
            },
            new Produto
            {
                Nome = "Teclado Gamer HyperX Alloy Core, RGB, ABNT2",
                Descricao = "Teclado Gamer HyperX Alloy Core, RGB, ABNT2",
                Preco = 349.99m,
                Estoque = 11,
                ImagemUrl = "https://images6.kabum.com.br/produtos/fotos/99696/teclado-gamer-hyperx-alloy-core-rgb-membrana-hx-kb5me2-br__1547726580_gg.jpg"
            }
        };

        context.Produtos.AddRange(produtos);

        await context.SaveChangesAsync();
    }
}
