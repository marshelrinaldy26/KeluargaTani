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
        ServiceResponse<object> TambahPembeliBaru(PembeliModelDto data);
        Task<ServiceResponse<object>> TambahRoleBaru(RoleModelDto data);
        IQueryable<CustomerDataVM> ReadCustomerData();
        List<ListDropdownVM> GetCustomerList();
    }

    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilService _utilServices;
        private DataTableHelper _dthelper;
        public CustomerService(ApplicationDbContext db,
                            UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            IHttpContextAccessor httpContextAccessor,
                            IUtilService utilServices,
                            DataTableHelper dthelper)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _utilServices = utilServices;
            _dthelper = dthelper;
        }

        public async Task<ServiceResponse<object>> TambahRoleBaru(RoleModelDto data)
        {
            if (string.IsNullOrWhiteSpace(data.namaRole))
            {
                return new ServiceResponse<object> { IsSuccess = false, Message = "Nama role tidak boleh kosong" };
            }

            var roleExists = await _roleManager.RoleExistsAsync(data.namaRole);
            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(data.namaRole));
                if (result.Succeeded)
                {
                    return new ServiceResponse<object> { IsSuccess = true, Message = "Role berhasil ditambahkan" };
                }
                return new ServiceResponse<object> { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
            }
            return new ServiceResponse<object> { IsSuccess = false, Message = "Role sudah ada" };
        }

        public ServiceResponse<object> TambahPembeliBaru(PembeliModelDto data)
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

        public IQueryable<CustomerDataVM> ReadCustomerData()
        {
            var query = _db.Pembelis.Select(x => new CustomerDataVM
            {
                namaPembeli = x.NamaPembeli,
                kontakPembeli = x.NomorKontak,
                alamat = x.AlamatLahan,
                frekuensi = x.Penjualans.Count(),
                totalBelanja = x.Penjualans.Sum(p => (decimal?)((p.HargaJual * p.Qty) - p.NilaiDiskon)) ?? 0,
                kontribusiLaba = x.Penjualans.Sum(p => (decimal?)(((p.HargaJual * p.Qty) - p.NilaiDiskon) - (p.Modal * p.Qty))) ?? 0
            });

            return query;
        }

        public List<ListDropdownVM> GetCustomerList()
        {
            var list = _db.Pembelis
                .Select(x => new ListDropdownVM
                {
                    value = x.Id.ToString(),
                    text = x.NamaPembeli
                })
                .ToList();

            return list;
        }

    }
}