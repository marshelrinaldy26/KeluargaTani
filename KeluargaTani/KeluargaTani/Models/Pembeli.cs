using System;
using System.Collections.Generic;

namespace KeluargaTani.Models;

public partial class Pembeli
{
    public int Id { get; set; }

    public string NamaPembeli { get; set; } = null!;

    public string NomorKontak { get; set; } = null!;

    public string? AlamatLahan { get; set; }

    public string? Catatan { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Penjualan> Penjualans { get; set; } = new List<Penjualan>();
}
