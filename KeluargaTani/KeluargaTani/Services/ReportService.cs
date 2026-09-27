using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KeluargaTani.Models;

namespace KeluargaTani.Service
{
    public class ReportProductItem
    {
        public string ProductName { get; set; }
        public int TotalQty { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Profit => TotalSales - TotalCost - TotalExpenses;
        public decimal Margin => TotalSales == 0 ? 0 : (Profit / TotalSales) * 100m;
    }

    public class ReportSummaryVM
    {
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal AvgMargin { get; set; }
        public decimal TotalDiscount { get; set; }
        public int TotalTransactions { get; set; }

        public string TopVolume { get; set; }
        public string BottomVolume { get; set; }
        public string TopRevenue { get; set; }
        public string BottomRevenue { get; set; }
        public string TopProfit { get; set; }
        public string BottomProfit { get; set; }
        public string TopMargin { get; set; }
        public string BottomMargin { get; set; }

        public string TopFrequent { get; set; }
        public string TopSpender { get; set; }

        public List<ReportProductItem> ProductReports { get; set; } = new List<ReportProductItem>();
    }

    public interface IReportService
    {
        ReportSummaryVM GetReportSummary(string filter);
    }

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        public ReportService(ApplicationDbContext db)
        {
            _db = db;
        }

        public ReportSummaryVM GetReportSummary(string filter)
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.Now;

            var today = DateTime.Now.Date;

            if (filter == "today")
                startDate = today;
            else if (filter == "week")
                startDate = today.AddDays(-((int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1)); // start of week (monday)
            else if (filter == "month")
                startDate = new DateTime(today.Year, today.Month, 1);
            else if (filter == "semester")
                startDate = today.Month <= 6 ? new DateTime(today.Year, 1, 1) : new DateTime(today.Year, 7, 1);
            else // all
                startDate = DateTime.MinValue;

            var sales = _db.Penjualans
                .Include(x => x.IdProdukNavigation)
                .Include(x => x.IdPembeliNavigation)
                .Where(x => x.TanggalTransaksi >= startDate && x.TanggalTransaksi <= endDate.AddDays(1))
                .ToList();

            var expenses = _db.Pengeluarans
                .Where(x => x.TanggalPengeluaran >= startDate && x.TanggalPengeluaran <= endDate.AddDays(1))
                .ToList();

            var vm = new ReportSummaryVM();
            vm.TotalTransactions = sales.Count;

            // Group sales by product
            var productSalesGroup = sales.GroupBy(x => x.IdProduk);
            var expensesGroup = expenses.GroupBy(x => x.IdProduk);

            var productItems = new List<ReportProductItem>();

            var allProductIds = productSalesGroup.Select(x => x.Key).Union(expensesGroup.Select(x => x.Key)).ToList();
            var allProducts = _db.Produks.Where(p => allProductIds.Contains(p.Id)).ToList();

            foreach(var pid in allProductIds)
            {
                var p = allProducts.FirstOrDefault(x => x.Id == pid);
                var pSales = productSalesGroup.FirstOrDefault(x => x.Key == pid);
                var pExp = expensesGroup.FirstOrDefault(x => x.Key == pid);

                var item = new ReportProductItem
                {
                    ProductName = p?.NamaProduk ?? "Unknown",
                    TotalQty = pSales?.Sum(x => x.Qty) ?? 0,
                    TotalCost = pSales?.Sum(x => x.Modal * x.Qty) ?? 0,
                    TotalExpenses = pExp?.Sum(x => x.JumlahPengeluaran) ?? 0
                };

                decimal itemSales = 0;
                decimal itemDiscount = 0;

                if (pSales != null)
                {
                    foreach (var s in pSales)
                    {
                        decimal subTotal = s.HargaJual * s.Qty;
                        decimal disc = s.TipeDiskon == "nom" ? s.NilaiDiskon : subTotal * (s.NilaiDiskon / 100m);
                        itemSales += (subTotal - disc);
                        itemDiscount += disc;
                    }
                }
                
                item.TotalSales = itemSales;
                
                vm.TotalSales += itemSales;
                vm.TotalCost += item.TotalCost + item.TotalExpenses;
                vm.TotalDiscount += itemDiscount;

                productItems.Add(item);
            }

            vm.ProductReports = productItems.OrderByDescending(x => x.TotalSales).ToList();
            vm.TotalProfit = vm.TotalSales - vm.TotalCost;
            vm.AvgMargin = vm.TotalSales == 0 ? 0 : Math.Round((vm.TotalProfit / vm.TotalSales) * 100m, 2);

            var topVol = productItems.OrderByDescending(x => x.TotalQty).FirstOrDefault();
            var botVol = productItems.OrderBy(x => x.TotalQty).FirstOrDefault();
            vm.TopVolume = topVol != null ? $"{topVol.ProductName} ({topVol.TotalQty} pcs)" : "-";
            vm.BottomVolume = botVol != null ? $"{botVol.ProductName} ({botVol.TotalQty} pcs)" : "-";

            var topRev = productItems.OrderByDescending(x => x.TotalSales).FirstOrDefault();
            var botRev = productItems.OrderBy(x => x.TotalSales).FirstOrDefault();
            vm.TopRevenue = topRev != null ? $"{topRev.ProductName} (Rp {topRev.TotalSales:N0})" : "-";
            vm.BottomRevenue = botRev != null ? $"{botRev.ProductName} (Rp {botRev.TotalSales:N0})" : "-";

            var topProf = productItems.OrderByDescending(x => x.Profit).FirstOrDefault();
            var botProf = productItems.OrderBy(x => x.Profit).FirstOrDefault();
            vm.TopProfit = topProf != null ? $"{topProf.ProductName} (Rp {topProf.Profit:N0})" : "-";
            vm.BottomProfit = botProf != null ? $"{botProf.ProductName} (Rp {botProf.Profit:N0})" : "-";

            var topMarg = productItems.OrderByDescending(x => x.Margin).FirstOrDefault();
            var botMarg = productItems.OrderBy(x => x.Margin).FirstOrDefault();
            vm.TopMargin = topMarg != null ? $"{topMarg.ProductName} ({topMarg.Margin:N2}%)" : "-";
            vm.BottomMargin = botMarg != null ? $"{botMarg.ProductName} ({botMarg.Margin:N2}%)" : "-";

            var custGroup = sales.Where(x => x.IdPembeliNavigation != null).GroupBy(x => x.IdPembeliNavigation.NamaPembeli);
            var topFreq = custGroup.OrderByDescending(x => x.Count()).FirstOrDefault();
            vm.TopFrequent = topFreq != null ? $"{topFreq.Key} ({topFreq.Count()} trx)" : "-";

            var topSpend = custGroup.OrderByDescending(g => g.Sum(x => {
                decimal sub = x.HargaJual * x.Qty;
                return sub - (x.TipeDiskon == "nom" ? x.NilaiDiskon : sub * (x.NilaiDiskon/100m));
            })).FirstOrDefault();

            if (topSpend != null)
            {
                decimal totalSp = topSpend.Sum(x => {
                    decimal sub = x.HargaJual * x.Qty;
                    return sub - (x.TipeDiskon == "nom" ? x.NilaiDiskon : sub * (x.NilaiDiskon/100m));
                });
                vm.TopSpender = $"{topSpend.Key} (Rp {totalSp:N0})";
            }
            else
            {
                vm.TopSpender = "-";
            }

            return vm;
        }
    }
}
