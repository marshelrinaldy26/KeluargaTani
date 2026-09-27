using System;
using System.Collections.Generic;
using System.Linq;
using KeluargaTani.Models;
using Microsoft.EntityFrameworkCore;

namespace KeluargaTani.Service
{
    public class DashboardSummaryVM
    {
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal AvgMargin { get; set; }
        public decimal TotalDiscount { get; set; }
        public int TotalTransactions { get; set; }
    }

    public class ChartDataVM
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> SalesSeries { get; set; } = new List<decimal>();
        public List<decimal> ProfitSeries { get; set; } = new List<decimal>();
        public List<decimal> MarginSeries { get; set; } = new List<decimal>();
        public List<decimal> CostSeries { get; set; } = new List<decimal>();
        
        public List<string> CategoryLabels { get; set; } = new List<string>();
        public List<decimal> CategorySeries { get; set; } = new List<decimal>();
    }

    public interface IDashboardService
    {
        DashboardSummaryVM GetDashboardSummary();
        ChartDataVM GetChartData(string period);
    }

    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _db;
        public DashboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public DashboardSummaryVM GetDashboardSummary()
        {
            // Load sales data
            var salesList = _db.Penjualans.ToList();
            
            decimal totalSales = 0;
            decimal totalCost = 0;
            decimal totalDiscount = 0;
            
            foreach (var sale in salesList)
            {
                decimal subTotalJual = sale.HargaJual * sale.Qty;
                decimal discount = sale.TipeDiskon == "nom" ? sale.NilaiDiskon : subTotalJual * (sale.NilaiDiskon / 100m);
                decimal netTotalJual = subTotalJual - discount;
                
                totalSales += netTotalJual;
                totalCost += sale.Modal * sale.Qty;
                totalDiscount += discount;
            }
            
            decimal totalProfit = totalSales - totalCost;
            decimal avgMargin = totalSales == 0 ? 0 : (totalProfit / totalSales) * 100m;
            
            return new DashboardSummaryVM
            {
                TotalSales = totalSales,
                TotalCost = totalCost,
                TotalProfit = totalProfit,
                AvgMargin = Math.Round(avgMargin, 2),
                TotalDiscount = totalDiscount,
                TotalTransactions = salesList.Count
            };
        }

        public ChartDataVM GetChartData(string period)
        {
            var query = _db.Penjualans.Include(p => p.IdProdukNavigation).ToList();
            
            // Determine start date based on period
            DateTime startDate = DateTime.Now;
            if (period == "daily") startDate = DateTime.Now.AddDays(-14);
            else if (period == "weekly") startDate = DateTime.Now.AddDays(-60);
            else if (period == "monthly") startDate = DateTime.Now.AddMonths(-12);
            else if (period == "semester") startDate = DateTime.Now.AddMonths(-36);
            else startDate = DateTime.Now.AddDays(-30);

            var filtered = query.Where(x => x.TanggalTransaksi >= startDate).ToList();

            var vm = new ChartDataVM();
            
            IEnumerable<IGrouping<string, Penjualan>> grouped;
            if (period == "daily")
            {
                grouped = filtered.GroupBy(x => x.TanggalTransaksi.ToString("dd MMM yyyy"));
            }
            else if (period == "weekly")
            {
                grouped = filtered.GroupBy(x => {
                    var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                    int week = cal.GetWeekOfYear(x.TanggalTransaksi, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    return $"{x.TanggalTransaksi.Year}-W{week:D2}";
                });
            }
            else if (period == "monthly")
            {
                grouped = filtered.GroupBy(x => x.TanggalTransaksi.ToString("MMM yyyy"));
            }
            else 
            {
                grouped = filtered.GroupBy(x => {
                    int sem = x.TanggalTransaksi.Month <= 6 ? 1 : 2;
                    return $"{x.TanggalTransaksi.Year}-S{sem}";
                });
            }

            var orderedGroups = grouped.OrderBy(g => g.Min(x => x.TanggalTransaksi)).ToList();

            foreach(var g in orderedGroups)
            {
                vm.Labels.Add(g.Key);
                decimal sumSales = 0;
                decimal sumCost = 0;
                
                foreach(var sale in g)
                {
                    decimal subTotalJual = sale.HargaJual * sale.Qty;
                    decimal discount = sale.TipeDiskon == "nom" ? sale.NilaiDiskon : subTotalJual * (sale.NilaiDiskon / 100m);
                    decimal netTotalJual = subTotalJual - discount;
                    
                    sumSales += netTotalJual;
                    sumCost += sale.Modal * sale.Qty;
                }
                
                decimal profit = sumSales - sumCost;
                decimal margin = sumSales == 0 ? 0 : (profit / sumSales) * 100m;
                
                vm.SalesSeries.Add(sumSales);
                vm.ProfitSeries.Add(profit);
                vm.CostSeries.Add(sumCost);
                vm.MarginSeries.Add(Math.Round(margin, 2));
            }
            
            var categoryGroup = filtered.GroupBy(x => x.IdProdukNavigation?.NamaProduk ?? "Lainnya");
            foreach(var cg in categoryGroup)
            {
                vm.CategoryLabels.Add(cg.Key);
                
                decimal sumSales = 0;
                foreach(var sale in cg)
                {
                    decimal subTotalJual = sale.HargaJual * sale.Qty;
                    decimal discount = sale.TipeDiskon == "nom" ? sale.NilaiDiskon : subTotalJual * (sale.NilaiDiskon / 100m);
                    sumSales += (subTotalJual - discount);
                }
                vm.CategorySeries.Add(sumSales);
            }
            
            return vm;
        }
    }
}
