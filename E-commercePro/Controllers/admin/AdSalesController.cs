using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_commercePro.Models;
using E_commercePro.Data;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using DinkToPdf;
using System.Text;
using ClosedXML.Excel;
namespace E_commercePro.Controllers.admin
{
    public class AdSalesController : Controller
    {
        private readonly ApplicationDbContext _db;
       

        public AdSalesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult SalesReports()
        {
            // Retrieve sales data including user names
            var salesData = _db.Orders
                .Include(o => o.UserSignUp)
                .ToList();
                
            

            return View(salesData);
        }

        [Route("admin/salesreports")]
        [HttpGet]
        public IActionResult SalesReports(string filterType, DateTime? fromDate, DateTime? toDate, int? pageNumber)
        {
            {
                int pageSize = 10;
                IQueryable<OrderE> salesData;

                switch (filterType)
                {
                    case "yearly":
                        var year = fromDate?.Year ?? DateTime.Now.Year;
                        salesData = _db.Orders
                            .Where(o => o.OrderDate.Year == year)
                            .Include(o => o.UserSignUp);
                        break;
                    case "monthly":
                        var month = fromDate?.Month ?? DateTime.Now.Month;
                        var monthYear = fromDate?.Year ?? DateTime.Now.Year;
                        salesData = _db.Orders
                            .Where(o => o.OrderDate.Year == monthYear && o.OrderDate.Month == month)
                            .Include(o => o.UserSignUp);
                        break;
                    case "daily":
                        if (fromDate.HasValue)
                        {
                            salesData = _db.Orders
                                .Where(o => o.OrderDate.Date == fromDate.Value.Date)
                                .Include(o => o.UserSignUp);
                        }
                        else
                        {
                            salesData = _db.Orders.Include(o => o.UserSignUp);
                        }
                        break;
                    default:
                        salesData = _db.Orders.Include(o => o.UserSignUp);
                        break;
                }

                var totalItems = salesData.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                int currentPageNumber = pageNumber.GetValueOrDefault(1); // Use pageNumber parameter or default to 1

                var orders = salesData
                    .Skip((currentPageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = currentPageNumber;

                return View(orders);
            }


        }

      


        [HttpGet]
        public async Task<IActionResult> ExportAllSalesReports()
        {
            var allOrders = await _db.Orders.Include(o => o.UserSignUp).ToListAsync();

            // Calculate totals
            decimal totalAmount = allOrders.Sum(o => o.OrderAmount);
            decimal totalDiscount = allOrders.Sum(o => o.Amount);
            int totalSalesCount = allOrders.Count;

            // Generate PDF with all orders data
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter pdfWriter = new PdfWriter(memoryStream);
            PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            Document document = new Document(pdfDocument);

            // Create a table with headers
            Table table = new Table(new float[] { 2, 2, 2, 2, 2 });
            table.AddHeaderCell("Order ID");
            table.AddHeaderCell("Customer");
            table.AddHeaderCell("Amount");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Status");

            // Add orders data to the table
            foreach (var order in allOrders)
            {
                table.AddCell(order.Id.ToString());
                table.AddCell(order.UserSignUp.Name);
                table.AddCell(order.OrderAmount.ToString());
                table.AddCell(order.OrderDate.ToString());
                table.AddCell(order.Status.ToString());
            }

            // Add the table to the document
            document.Add(table);

            // Add totals to the document
            document.Add(new Paragraph($"Total discount: {totalDiscount}"));
            document.Add(new Paragraph($"Total: {totalAmount}"));
            document.Add(new Paragraph($"Total Sales: {totalSalesCount}"));

            document.Close();

            byte[] pdfBytes = memoryStream.ToArray();

            // Download PDF
            memoryStream.Close();

            return File(pdfBytes, "application/pdf", "AllSalesReports.pdf");
        }



        public IActionResult ExportToExcel()
        {
            var salesReports = _db.Orders
                                 .Include(o => o.UserSignUp) // Include related UserSignUp data
                                 .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sales Report");

                // Add header row
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order ID";
                worksheet.Cell(currentRow, 2).Value = "Customer Name";
                worksheet.Cell(currentRow, 3).Value = "Price";
                worksheet.Cell(currentRow, 4).Value = "Coupon";
                worksheet.Cell(currentRow, 5).Value = "Discount";
                worksheet.Cell(currentRow, 6).Value = "Date";
                worksheet.Column(6).Width = 15; // Adjust the column width for the "Date" column
                worksheet.Cell(currentRow, 7).Value = "Status";
                worksheet.Cell(currentRow, 8).Value = "Payment Mode";

                // Add data rows
                currentRow++;
                double totalAmount = 0;
                double totalDiscount = 0;
                foreach (var report in salesReports)
                {
                    worksheet.Cell(currentRow, 1).Value = report.Id;
                    worksheet.Cell(currentRow, 2).Value = report.UserSignUp.Name;
                    worksheet.Cell(currentRow, 3).Value = report.OrderAmount;
                    worksheet.Cell(currentRow, 4).Value = report.CouponCod;
                    worksheet.Cell(currentRow, 5).Value = report.Amount;
                    worksheet.Cell(currentRow, 6).Value = report.OrderDate;
                    worksheet.Cell(currentRow, 6).Style.DateFormat.Format = "dd-MM-yyyy"; // Set the date format for each cell
                    worksheet.Cell(currentRow, 7).Value = report.Status.ToString();
                    worksheet.Cell(currentRow, 8).Value = report.PaymentMethod.ToString();

                    totalAmount += report.OrderAmount;
                    totalDiscount += report.Amount;

                    currentRow++;
                }

                // Add total rows
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = "Total";
                worksheet.Cell(currentRow, 3).Value = totalAmount;
                worksheet.Cell(currentRow, 5).Value = totalDiscount;
                worksheet.Cell(currentRow, 8).Value = salesReports.Count();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
                }
            }
        }




    }
}

