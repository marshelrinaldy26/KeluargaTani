using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using KeluargaTani.Helper;
using System.Linq.Dynamic.Core;
using KeluargaTani.Models;


public class DataTableHelper
{
    private readonly IHttpContextAccessor _accessor;

    public DataTableHelper(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public HttpRequest Request => _accessor.HttpContext.Request;

    //Metode ini berfungsi untuk mengambil parameter dari permintaan DataTables
    public DataTableRequestDTO RequestParameter()
    {
        int draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");
        int start = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
        int length = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
        string orderDirection = Request.Form["order[0][dir]"].FirstOrDefault() ?? "";
        string orderColumnName = string.IsNullOrWhiteSpace(Request.Form["order[0][column]"].FirstOrDefault()) ? "" : Request.Form["columns[" + Request.Form["order[0][column]"].First() + "][data]"].FirstOrDefault() ?? "";

        orderColumnName = RenamedRequestedColumnHeader(orderColumnName);
        return new DataTableRequestDTO
        {
            Draw = draw,
            Start = start,
            Length = length,
            OrderDirection = orderDirection,
            OrderColumnName = orderColumnName
        };
    }
    private string RenamedRequestedColumnHeader(string columnName)
    {
        // if (string.IsNullOrEmpty(columnName))
        //     return "";
        // else
        //     return char.ToUpper(columnName[0]) + columnName[1..];
        if (string.IsNullOrEmpty(columnName))
            return "";

        // Handle nested property: "franchisee.nama" → "Franchisee.Nama"
        if (columnName.Contains("."))
        {
            return string.Join(".", columnName.Split(".")
                .Select(part => char.ToUpper(part[0]) + part[1..]));
        }

        return char.ToUpper(columnName[0]) + columnName[1..];
    }

    public int ColumnCount()
    {
        var req = Request.Form.Keys.Where(x => x.StartsWith("columns[")).Select(y =>
        {
            var regex = Regex.Match(y, @"columns\[(\d+)\]");
            return regex.Success ? int.Parse(regex.Groups[1].Value) : -1;
        }).DefaultIfEmpty(-1).Max();

        return req + 1;
    }

    public DataTableSearchingDTO GetValueSearching(int index)
    {
        var searchcol = Request.Form[$"columns[{index}][data]"].FirstOrDefault();
        var searchval = Request.Form[$"columns[{index}][search][value]"].FirstOrDefault();

        return new DataTableSearchingDTO
        {
            columnName = searchcol,
            columnSearch = searchval
        };
    }

    public JsonResult ProcessDataTable<T>(IQueryable<T> query)
    {
        DataTableRequestDTO dt = RequestParameter();

        var globalSearchValue = Request.Form["search[value]"].FirstOrDefault();
        if (!string.IsNullOrEmpty(globalSearchValue))
        {
            query = SearchHelper.GlobalSearch(query, Request.Form, ColumnCount(), globalSearchValue);
        }

        for (int i = 0; i < ColumnCount(); i++)
        {
            var search_col_val = GetValueSearching(i);
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