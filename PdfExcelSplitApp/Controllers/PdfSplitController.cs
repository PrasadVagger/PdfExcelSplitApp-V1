using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfExcelSplitApp; // Ensure this points to your PdfSplitter class

namespace PdfExcelSplitApp.Controllers
{
    public class PdfSplitController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PdfSplitController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public IActionResult SplitPdf(IFormFile pdfFile, string applicationNumbers, string outputFolder)
        {
            if (pdfFile == null)
            {
                return Json(new { success = false, message = "Please upload a PDF file." });
            }

            // Check that application numbers are entered
            if (string.IsNullOrEmpty(applicationNumbers))
            {
                return Json(new { success = false, message = "Please enter application numbers." });
            }

            if (string.IsNullOrEmpty(outputFolder))
            {
                return Json(new { success = false, message = "Please specify the output folder." });
            }

            // Split the application numbers entered by the user into a list of strings
            var applicationNumbersList = applicationNumbers.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                                           .Select(app => app.Trim())
                                                           .ToList();

            // Save the uploaded PDF
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string outputDir = Path.Combine(_webHostEnvironment.WebRootPath, "split_pdfs", outputFolder);  // Include output folder

            // Ensure directories exist
            Directory.CreateDirectory(uploadsDir);
            Directory.CreateDirectory(outputDir);

            // Save the uploaded PDF
            string inputPdfPath = Path.Combine(uploadsDir, pdfFile.FileName);
            using (var fileStream = new FileStream(inputPdfPath, FileMode.Create))
            {
                pdfFile.CopyTo(fileStream);
            }

            try
            {
                // Split the PDF and save to the specified folder with application numbers
                PdfSplitter.SplitPdf(inputPdfPath, outputDir, applicationNumbersList);

                return Json(new { success = true, message = "PDF has been successfully split and saved with application numbers." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // Display the Index view on GET request (for showing the form)
        public IActionResult Index()
        {
            return View();
        }
    }
}
