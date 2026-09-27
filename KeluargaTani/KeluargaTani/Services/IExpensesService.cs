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
    public class ExpensesModelDto
    {
        public int IdProduk { get; set; }
        public string Kriteria { get; set; }
        public decimal JumlahPengeluaran { get; set; }
        public DateTime TanggalPengeluaran { get; set; }
    }

    public class ExpensesModelVM
    {
        public int Id { get; set; }
        public string NamaProduk { get; set; }
        public string Kriteria { get; set; }
        public decimal JumlahPengeluaran { get; set; }
        public string TanggalPengeluaran { get; set; }
    }

    public interface IExpensesService
    {
        ServiceResponse<object> CreateNewExpense(ExpensesModelDto data);
        IQueryable<ExpensesModelVM> ReadExpensesData();
    }

    public class ExpensesService : IExpensesService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilService _utilServices;
        private DataTableHelper _dthelper;
        public ExpensesService(ApplicationDbContext db,
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

        public IQueryable<ExpensesModelVM> ReadExpensesData()
        {
            var query = _db.Pengeluarans
                .Include(p => p.IdProdukNavigation)
                .Select(x => new ExpensesModelVM
                {
                    Id = x.Id,
                    NamaProduk = x.IdProdukNavigation.NamaProduk,
                    Kriteria = x.Kriteria,
                    JumlahPengeluaran = x.JumlahPengeluaran,
                    TanggalPengeluaran = x.TanggalPengeluaran.ToString("yyyy-MM-dd")
                });

            return query;
        }

        public ServiceResponse<object> CreateNewExpense(ExpensesModelDto data)
        {   
            Pengeluaran expense = new Pengeluaran
            {
                IdProduk = data.IdProduk,
                Kriteria = data.Kriteria,
                JumlahPengeluaran = data.JumlahPengeluaran,
                TanggalPengeluaran = data.TanggalPengeluaran,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };

            _db.Pengeluarans.Add(expense);
            _db.SaveChanges();

            return new ServiceResponse<object> { 
                IsSuccess = true,
                Message = "Sukses disimpan"
            };
        }
    }
}