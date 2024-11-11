using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfExcelSplitApp
{
    public class PdfSplitter
    {
        public static void SplitPdf(string inputPdfPath, string outputDirectory, List<string> applicationNumbers)
        {
            // Ensure the directory exists
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (PdfDocument reader = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import))
            {
                int totalPages = reader.PageCount;

                // Check if the number of application numbers matches the number of pages in the PDF
                if (applicationNumbers.Count != totalPages)
                {
                    throw new InvalidOperationException("The number of application numbers does not match the number of pages in the PDF.");
                }

                // Iterate over each page in the PDF and save as a separate file
                for (int i = 0; i < totalPages; i++)
                {
                    string applicationNumber = applicationNumbers[i];

                    using (PdfDocument newPdf = new PdfDocument())
                    {
                        // Copy the page to the new PDF
                        newPdf.AddPage(reader.Pages[i]);

                        // Save the new PDF with the name based on the application number
                        string outputPdfPath = Path.Combine(outputDirectory, $"{applicationNumber}.pdf");
                        newPdf.Save(outputPdfPath);
                    }
                }
            }
        }
    }
}
