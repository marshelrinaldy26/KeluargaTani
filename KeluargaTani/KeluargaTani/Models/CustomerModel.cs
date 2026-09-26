public class CustomerDataDto
{
    public string namaPembeli {get;set;}
    public string kontakPembeli {get;set;}
    public string alamat {get;set;}
}

public class CustomerDataVM
{
    public string namaPembeli {get;set;}
    public string kontakPembeli {get;set;}
    public string alamat {get;set;}
    public string? frekuensi {get;set;}
    public decimal? totalBelanja {get;set;}
    public string? kontribusiLaba {get;set;}
}