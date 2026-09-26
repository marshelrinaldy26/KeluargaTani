using System;
using System.Collections.Generic;

namespace KeluargaTani.Models;

public partial class Penjualan
{
    public int Id { get; set; }

    public int IdPembeli { get; set; }

    public int IdProduk { get; set; }

    public DateTime TanggalTransaksi { get; set; }

    public string? CatatanBarang { get; set; }

    public decimal HargaJual { get; set; }

    public decimal Modal { get; set; }

    public int Qty { get; set; }

    public string? TipeDiskon { get; set; }

    public int NilaiDiskon { get; set; }

    public string? CatatanTambahan { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Pembeli IdPembeliNavigation { get; set; } = null!;

    public virtual Produk IdProdukNavigation { get; set; } = null!;
}
