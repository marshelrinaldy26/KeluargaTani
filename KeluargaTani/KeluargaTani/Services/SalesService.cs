using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using KeluargaTani.Helper;
using KeluargaTani.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace KeluargaTani.Service
{
    public interface ISalesService
    {
        ServiceResponse<object> CreateNewSales(SalesModelDto data);
        IQueryable<SalesModelVM> ReadSalesData();
    }

    public class SalesService : ISalesService
    {
        private readonly ApplicationDbContext _db;

        public SalesService(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<SalesModelVM> ReadSalesData()
        {
            var query = _db.Penjualans
                .Select(x => new SalesModelVM
                {
                    id = x.Id,
                    tanggalTransaksi = x.TanggalTransaksi.ToString(),
                    pembeli = x.IdPembeliNavigation.NamaPembeli,
                    barangKategori = x.IdProdukNavigation.NamaProduk,
                    HargaJual = x.HargaJual,
                    Modal = x.Modal,
                    qty = x.Qty,
                    TipeDiskon = x.TipeDiskon,
                    NilaiDiskon = (decimal)x.NilaiDiskon
                })
                .AsEnumerable(); // Tarik ke RAM, biarkan class SalesModelVM yang melakukan perhitungan matematika

            return query;
        }

        public ServiceResponse<object> CreateNewSales(SalesModelDto data)
        {
            Penjualan penjualan = new Penjualan
            {
                TanggalTransaksi = data.tanggalTransaksi,
                IdPembeli = data.idPembeli ?? 0,
                IdProduk = data.idProduk ?? 0,
                CatatanBarang = data.catatanBarang,
                HargaJual = data.hargaJual,
                Modal = data.modal,
                Qty = data.qty,
                TipeDiskon = data.tipeDiskon,
                NilaiDiskon = data.nilaiDiskon,
                CatatanTambahan = data.catatan,
                CreatedAt = DateTime.Now
            };

            _db.Penjualans.Add(penjualan);
            _db.SaveChanges();

            return new ServiceResponse<object>
            {
                IsSuccess = true,
                Message = "Sukses disimpan"
            };
        }
    }
}
