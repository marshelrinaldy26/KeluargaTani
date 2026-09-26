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
    public interface IProductService
    {
        ServiceResponse<object> CreateNewProduct(ProductModelDto data);
        IQueryable<ProductModelVM> ReadProductData();
        List<ListDropdownVM> GetProductList();
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilService _utilServices;
        private DataTableHelper _dthelper;
        public ProductService(ApplicationDbContext db,
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

        public IQueryable<ProductModelVM> ReadProductData()
        {
            var query = _db.Produks.Select(x => new ProductModelVM
            {
                namaProduk = x.NamaProduk,
                kategori = x.Kategori,
                stokAwal = x.Stok,
                status = x.Status,
                estimasiMargin = 30, //masih dummy
            });

            return query;
        }

        public ServiceResponse<object> CreateNewProduct(ProductModelDto data)
        {   
            Produk produk = new Produk
            {
                NamaProduk = data.namaProduk,
                Kategori = data.kategori,
                Stok = data.stokAwal,
                Status = data.status,
            };

            _db.Produks.Add(produk);
            _db.SaveChanges();

            return new ServiceResponse<object> { 
                IsSuccess = true,
                Message = "Sukses disimpan"
            };
        }

        public List<ListDropdownVM> GetProductList()
        {
            return _db.Produks
                .Select(x => new ListDropdownVM
                {
                    value = x.Id.ToString(),
                    text = x.NamaProduk
                })
                .ToList();
        }
    }
}