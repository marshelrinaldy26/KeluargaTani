using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APAR.Helpers
{
    public class DataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string SearchValue { get; set; } = string.Empty;
        public string SortColumn { get; set; } = string.Empty;
        public string SortDirection { get; set; } = "asc";
        public List<string> Columns { get; set; } = new();
        public Dictionary<int, string> ColumnSearch { get; set; } = new();

        public string? GetColumnSearch(int index)
        {
            return ColumnSearch.TryGetValue(index, out var v) ? v : null;
        }

        public static DataTablesRequest Parse(HttpRequest request)
        {
            var dt = new DataTablesRequest();
            var form = request.Form;
            dt.Draw = TryParseInt(form["draw"], 0);
            dt.Start = TryParseInt(form["start"], 0);
            dt.Length = TryParseInt(form["length"], 10);
            dt.SearchValue = form["search[value]"].ToString();            

            var orderColumnIndex = form["order[0][column]"];            
            var sortDir = form["order[0][dir]"];            

            if (!string.IsNullOrEmpty(orderColumnIndex))
            {
                var colName = form[$"columns[{orderColumnIndex}][data]"];                
                dt.SortColumn = colName.ToString();
            }
            dt.SortDirection = !string.IsNullOrEmpty(sortDir) ? sortDir! : "asc";

            int i = 0;
            while (true)
            {
                var colData = form[$"columns[{i}][data]"];                
                if (string.IsNullOrEmpty(colData) && i > 20)
                    break;
                if (!string.IsNullOrEmpty(colData))
                {
                    dt.Columns.Add(colData!);
                    var colSearch = form[$"columns[{i}][search][value]"];                    
                    if (!string.IsNullOrEmpty(colSearch))
                        dt.ColumnSearch[i] = colSearch!;
                }
                i++;
            }

            return dt;
        }

        private static int TryParseInt(string? s, int defaultValue)
        {
            return int.TryParse(s, out var v) ? v : defaultValue;
        }
    }
}
