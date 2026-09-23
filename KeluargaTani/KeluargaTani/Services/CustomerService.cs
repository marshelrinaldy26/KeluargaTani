using System.Data;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using KeluargaTani.Helper;
using KeluargaTani.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeluargaTani.Service
{
    public interface ICustomerService
    {
        ServiceResponse<object> TambahPembeliBaru(PembeliModel data);
        IQueryable<CustomerDataDto> ReadCustomerData();
    }

    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilService _utilServices;
        private DataTableHelper _dthelper;
        public CustomerService(ApplicationDbContext db,
                            UserManager<ApplicationUser> userManager,
                            IHttpContextAccessor httpContextAccessor,
                            IUtilService utilServices,
                            DataTableHelper dthelper)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _utilServices = utilServices;
            _dthelper = dthelper;
        }

        public ServiceResponse<object> TambahPembeliBaru(PembeliModel data)
        {   
            Pembeli pembeli = new Pembeli
            {
                NamaPembeli = data.namaPembeli,
                NomorKontak = data.nomorPembeli,
                AlamatLahan = data.alamatPembeli,
                Catatan = data.catatanPembeli
            };

            _db.Pembelis.Add(pembeli);
            _db.SaveChanges();

            return new ServiceResponse<object> { 
                IsSuccess = true,
                Message = "Sukses disimpan"
            };
        }

        public IQueryable<CustomerDataDto> ReadCustomerData()
        {
            var query = _db.Pembelis.Select(x => new CustomerDataDto
            {
                namaPembeli = x.NamaPembeli,
                kontakPembeli = x.NomorKontak,
                alamat = x.AlamatLahan,
                frekuensi = "-",
                totalBelanja = 0,
                kontribusiLaba = "-",
            });

            return query;
        }

    }
}