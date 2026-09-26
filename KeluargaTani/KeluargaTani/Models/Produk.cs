using System;
using System.Collections.Generic;

namespace KeluargaTani.Models;

public partial class Produk
{
    public int Id { get; set; }

    public string NamaProduk { get; set; } = null!;

    public string Kategori { get; set; } = null!;

    public int? Stok { get; set; }

    /// <summary>
    /// 1. Tersedia 
    /// 2. Stock Menipis
    /// 3. Habis
    /// 4. Non Aktif
    /// </summary>
    public sbyte Status { get; set; }

    public virtual ICollection<Penjualan> Penjualans { get; set; } = new List<Penjualan>();
}
