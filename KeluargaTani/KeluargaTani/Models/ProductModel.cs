public class ProductModelDto
{
    public string namaProduk {get;set;}
    public string kategori {get;set;}
    public int? stokAwal {get;set;}
    public sbyte status {get;set;}
}

public class ProductModelVM
{
    public string namaProduk {get;set;}
    public string kategori {get;set;}
    public int? stokAwal {get;set;}
    public sbyte status {get;set;}
    public int estimasiMargin {get;set;}
}



