using System;

namespace KeluargaTani.Models
{
    public class SalesModelDto
    {
        public DateTime tanggalTransaksi { get; set; }
        public int? idPembeli { get; set; }
        public int? idProduk { get; set; }
        public string catatanBarang { get; set; }
        public decimal hargaJual { get; set; }
        public decimal modal { get; set; }
        public int qty { get; set; }
        public string tipeDiskon { get; set; }
        public int nilaiDiskon { get; set; }
        public string catatan { get; set; }
    }

    public class SalesModelVM
    {
        public int id { get; set; }
        public string tanggalTransaksi { get; set; }
        public string pembeli { get; set; }
        public string barangKategori { get; set; }
        public decimal HargaJual { get; set; }
        public decimal Modal { get; set; }
        public int qty { get; set; }
        public string TipeDiskon { get; set; }
        public decimal NilaiDiskon { get; set; }
        public string hargaModal => $"Rp {HargaJual} / Rp {Modal}";
        public string diskon => TipeDiskon == "nom" ? $"Rp {NilaiDiskon}" : $"{NilaiDiskon}%";
        private decimal SubTotalJual => HargaJual * qty;
        private decimal TotalModal => Modal * qty;
        private decimal TotalDiskon => TipeDiskon == "nom" ? NilaiDiskon : SubTotalJual * (NilaiDiskon / 100m);
        private decimal NetTotalJual => SubTotalJual - TotalDiskon;
        private decimal NetProfit => NetTotalJual - TotalModal;
        public string totalJual => NetTotalJual.ToString();
        public string profit => NetProfit.ToString();
        public string margin => NetTotalJual == 0 
                                ? "0" 
                                : Math.Round((NetProfit / NetTotalJual) * 100, 2).ToString();
    }
}
