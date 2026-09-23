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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeluargaTani.Service
{
    public interface IUtilService
    {
        JsonResult ProcessDataTable<T>(IQueryable<T> query);
    }

    public class UtilService : IUtilService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataTableHelper _dthelper;
        public UtilService(ApplicationDbContext db,
                            UserManager<ApplicationUser> userManager,
                            IHttpContextAccessor httpContextAccessor,
                            DataTableHelper dthelper)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _dthelper = dthelper;
        }

         public JsonResult ProcessDataTable<T>(IQueryable<T> query)
        {
            DataTableRequestDTO dt = _dthelper.RequestParameter();
            var form = _dthelper.Request.Form;

            // 1. Global Search
            var globalSearchValue = form["search[value]"].FirstOrDefault();
            if (!string.IsNullOrEmpty(globalSearchValue))
            {
                query = SearchHelper.GlobalSearch(query, form, _dthelper.ColumnCount(), globalSearchValue);
            }

            // 2. Column Search
            for (int i = 0; i < _dthelper.ColumnCount(); i++)
            {
                var search_col_val = _dthelper.GetValueSearching(i);
                if (!string.IsNullOrEmpty(search_col_val.columnSearch))
                {
                    query = SearchHelper.Search(query, search_col_val.columnName, search_col_val.columnSearch);
                }
            }

            var count = query.Count();

            if (!string.IsNullOrEmpty(dt.OrderColumnName))
            {
                query = query.OrderBy($"{dt.OrderColumnName} {dt.OrderDirection}");
            }

            var data = query.Skip(dt.Start).Take(dt.Length).ToList();

            return new JsonResult(new { data = data, recordsTotal = count, recordsFiltered = count, draw = dt.Draw });
        }


    }
}