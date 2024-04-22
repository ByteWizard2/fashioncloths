using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commercePro.Controllers.admin
{
    public class AdDashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdDashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Dashboard()
        {
            var allOrders = await _db.Orders.Include(o => o.UserSignUp).ToListAsync();
            var allProducts = await _db.Products.ToListAsync();
            var allCategories = await _db.Categories.ToListAsync();

            // Calculate monthly revenue based on local time
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyRevenue = allOrders
                .Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear)
                .Sum(o => o.OrderAmount - o.Amount);

            // Calculate totals
            decimal totalAmount = allOrders.Sum(o => o.OrderAmount);
            decimal totalDiscount = allOrders.Sum(o => o.Amount);
            decimal total = totalAmount - totalDiscount;
            int totalSalesCount = allOrders.Count;

            // Products
            decimal totalProducts = allProducts.Count;
            decimal totalCategories = allCategories.Count;

            // Prepare data for the line chart
            var yearlyChartData = await GetYearlyChartData(currentYear, 2020);
            var monthlyChartData = await GetMonthlyChartData(currentYear);

            int currentyear = DateTime.Now.Year;


            var topBestSellingProducts = await GetTopBestSellingProducts();
            var topBestSellingCategories = await GetTopBestSellingCategories();

            ViewBag.TopBestSellingProducts = topBestSellingProducts;
            ViewBag.TopBestSellingCategories = topBestSellingCategories;

            ViewBag.TotalAmount = total;
            ViewBag.TotalSales = totalSalesCount;
            ViewBag.TotalProduct = totalProducts;
            ViewBag.TotalCategory = totalCategories;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.YearlyChartData = yearlyChartData;
            ViewBag.MonthlyChartData = monthlyChartData;
            ViewBag.Currentyear = currentyear;

            return View();
        }

        [HttpGet]
        public async Task<ActionResult<SalesChartData>> GetSalesChartData(string filter, string time)
        {
            int currentYear = int.Parse(time);
            int startYear = 2020;

            switch (filter)
            {
                case "yearly":
                    return await GetYearlyChartData(currentYear, startYear);
                case "monthly":
                    return await GetMonthlyChartData(currentYear);
                default:
                    return await GetDefaultChartData();
            }
        }

        private async Task<SalesChartData> GetYearlyChartData(int currentYear, int startYear)
        {
            // Fetch all the products
            var allProducts = await _db.Products.ToListAsync();

            var yearlySales = await _db.Orders
                .Where(o => o.OrderDate.Year >= startYear && o.OrderDate.Year <= currentYear)
                .GroupBy(o => o.OrderDate.Year)
                .Select(g => new
                {
                    Label = g.Key.ToString(),
                    SalesData = g.Count(),
                    RevenueData = g.Sum(o => o.OrderAmount - o.Amount),
                    ProductsData = allProducts.Count
                })
                .ToListAsync();

            return new SalesChartData
            {
                Labels = Enumerable.Range(startYear, currentYear - startYear + 1).Select(y => y.ToString()).ToArray(),
                SalesData = Enumerable.Range(startYear, currentYear - startYear + 1).Select(y => yearlySales.FirstOrDefault(ys => ys.Label == y.ToString())?.SalesData ?? 0).ToArray(),
                RevenueData = Enumerable.Range(startYear, currentYear - startYear + 1).Select(y => yearlySales.FirstOrDefault(ys => ys.Label == y.ToString())?.RevenueData ?? 0m).Select(r => Math.Round(r / 1000, 3)).ToArray(),
                ProductsData = Enumerable.Range(startYear, currentYear - startYear + 1).Select(y => yearlySales.FirstOrDefault(ys => ys.Label == y.ToString())?.ProductsData ?? 0).ToArray()
            };
        }

        private async Task<SalesChartData> GetMonthlyChartData(int currentYear)
        {
            // Fetch all the products
            var allProducts = await _db.Products.ToListAsync();

            var monthlySales = await _db.Orders
                .Where(o => o.OrderDate.Year == currentYear)
                .GroupBy(o => new { o.OrderDate.Month })
                .Select(g => new
                {
                    Label = new DateTime(currentYear, g.Key.Month, 1).ToString("MMM"),
                    SalesData = g.Count(),
                    RevenueData = g.Sum(o => o.OrderAmount - o.Amount),
                    ProductsData = allProducts.Count
                })
                .ToListAsync();

            return new SalesChartData
            {
                Labels = Enumerable.Range(1, 12).Select(m => new DateTime(currentYear, m, 1).ToString("MMM")).ToArray(),
                SalesData = Enumerable.Range(1, 12).Select(m => monthlySales.FirstOrDefault(ms => ms.Label == new DateTime(currentYear, m, 1).ToString("MMM"))?.SalesData ?? 0).ToArray(),
                RevenueData = Enumerable.Range(1, 12).Select(m => monthlySales.FirstOrDefault(ms => ms.Label == new DateTime(currentYear, m, 1).ToString("MMM"))?.RevenueData ?? 0m).Select(r => Math.Round(r / 1000, 3)).ToArray(),
                ProductsData = Enumerable.Range(1, 12).Select(m => monthlySales.FirstOrDefault(ms => ms.Label == new DateTime(currentYear, m, 1).ToString("MMM"))?.ProductsData ?? 0).ToArray()
            };
        }

        private async Task<SalesChartData> GetDefaultChartData()
        {
            return new SalesChartData
            {
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                SalesData = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                RevenueData = new decimal[] { 0.000m, 0.000m, 0.000m, 0.000m, 0.000m, 0.000m, 0.000m, 0.000m, 0.000m, 0.000m, 0.000m, 0.000m },
                ProductsData = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
        }

        private async Task<List<BestSellingProduct>> GetTopBestSellingProducts(int limit = 10)
        {
            return await _db.OrderedItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new BestSellingProduct
                {
                    ProductId = g.Key,
                    ProductName = _db.Products.FirstOrDefault(p => p.Id == g.Key).Name,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(limit)
                .ToListAsync();
        }

        private async Task<List<BestSellingCategory>> GetTopBestSellingCategories(int limit = 10)
        {
            return await _db.OrderedItems
                .GroupBy(oi => _db.Products.FirstOrDefault(p => p.Id == oi.ProductId).CategoryId)
                .Select(g => new BestSellingCategory
                {
                    CategoryId = g.Key,
                    CategoryName = _db.Categories.FirstOrDefault(c => c.Id == g.Key).Name,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(c => c.TotalSold)
                .Take(limit)
                .ToListAsync();
        }
    }
}