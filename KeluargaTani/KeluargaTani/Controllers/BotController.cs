using System.Text;
using System.Text.Json;
using KeluargaTani.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeluargaTani.Controllers
{
    public class BotController : Controller
    {
        private readonly ILogger<BotController> _logger; 
        private readonly ApplicationDbContext _db;
        private readonly HttpClient _httpClient;
        public BotController(ILogger<BotController> logger,
                            ApplicationDbContext db,
                            HttpClient httpClient)
        {
            _logger = logger;
            _db = db;
            _httpClient = httpClient;
            
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://localhost:11434");
            }
        }

        public IActionResult Bot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAnalisis()
        {
            var dataKeuangan = new {
                TotalPendapatan = 15000000,
                TotalPengeluaran = 1500000,
                KeuntunganMarginSekarang = "90%",
                TargetMarginSekarang = "30%",
                ProdukTerlaris = "Tomat",
                Kendala = "akhir akhir ini pembeli lumayan jarang"
            };

            string dataJson = JsonSerializer.Serialize(dataKeuangan);

            string prompt = $@"
            Kamu adalah 'Kang Tani', asisten keuangan ahli untuk petani. 
            Data keuangan saat ini: {dataJson}

            Tugasmu:
            1. Buat 'ringkasan' analisis keuangan dalam 2-3 paragraf (gunakan \n\n untuk jarak paragraf).
            2. Buat 'insight' tentang apa yang positif dan saran atas kendala pembeli yang sepi.
            3. Proyeksi target periode depan: Naikkan omzet sekitar 10%-20%. Hitung profit dengan rumus (Target Omzet - Total Pengeluaran Saat Ini).
            4. Berikan skor kesehatan finansial (1-100) beserta alasan singkatnya.

            WAJIB balas HANYA dalam format JSON. Ganti nilai 0 dan string kosong di bawah ini dengan hasil analisismu:
            {{
                ""ringkasan"": """",
                ""insight"": """",
                ""skor_kesehatan"": """",
                ""target_omzet"": 0,
                ""target_profit"": 0,
                ""target_transaksi"": 0,
                ""target_margin"": 0
            }}";

            var requestBody = new
            {
                model = "qwen2.5:7b", 
                prompt = prompt,
                stream = false, 
                format = "json" 
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            using JsonDocument doc = JsonDocument.Parse(responseString);
            string aiResponseText = doc.RootElement.GetProperty("response").GetString();

            return Content(aiResponseText, "application/json");
        }
    
    }
}
