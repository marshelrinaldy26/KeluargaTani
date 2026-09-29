using System;
using System.Collections.Generic;

namespace KeluargaTani.Models;

public partial class Pengaturan
{
    public int Id { get; set; }

    public string NamaPengaturan { get; set; } = null!;

    public string NoTelp { get; set; } = null!;

    public int TargetMargin { get; set; }

    public string? AlamatUsaha { get; set; }
}
