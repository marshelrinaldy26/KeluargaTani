using System;
using System.Collections.Generic;

namespace KeluargaTani.Models;

public partial class Pengeluaran
{
    public int Id { get; set; }

    public int IdProduk { get; set; }

    /// <summary>
    /// 1. Modal Tanam
    /// 2. Modal Perawatan
    /// 3. Modal Tanam Dan Pewatan
    /// 4. Pengeluaran Lain
    /// </summary>
    public string Kriteria { get; set; } = null!;

    public decimal JumlahPengeluaran { get; set; }

    public DateTime TanggalPengeluaran { get; set; }

    public DateOnly CreatedAt { get; set; }

    public virtual Produk IdProdukNavigation { get; set; } = null!;
}
