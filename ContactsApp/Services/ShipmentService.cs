using ContactsApp.Models;
using ContactsApp.Services.Interfaces;
using EasyPost.Models.API;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Collections.ObjectModel;

namespace ContactsApp.Services
{
    public class ShipmentService : GenericService<ShipmentModel, int>, IShipmentService
    {
        public ShipmentService(ContactsDbContext dbContext) : base(dbContext)
        {
            
        }

        public void ExportToPdf(ObservableCollection<Shipment> shipments, string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                PdfPTable table = new PdfPTable(9);
                table.AddCell("Shipment ID");
                table.AddCell("Carrier");
                table.AddCell("Service");
                table.AddCell("Price");
                table.AddCell("Currency");
                table.AddCell("From Address");
                table.AddCell("To Address");
                table.AddCell("Created Date");
                table.AddCell("Updated Date");

                foreach (var shipment in shipments)
                {
                    table.AddCell(shipment.Id);
                    table.AddCell(shipment.SelectedRate.Carrier);
                    table.AddCell(shipment.SelectedRate.Service);
                    table.AddCell(shipment.SelectedRate.Price.ToString());
                    table.AddCell(shipment.SelectedRate.ListCurrency);
                    table.AddCell(shipment.FromAddress.Id);
                    table.AddCell(shipment.ToAddress.Id);
                    table.AddCell(shipment.CreatedAt.ToString());
                    table.AddCell(shipment.UpdatedAt.ToString());
                }

                pdfDoc.Add(table);
                pdfDoc.Close();
            }
        }

        public void ExportToCsv(ObservableCollection<Shipment> shipments, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var records = shipments.Select(s => new
                {
                    s.Id,
                    Carrier = s.SelectedRate.Carrier,
                    Service = s.SelectedRate.Service,
                    Price = s.SelectedRate.Price.ToString(),
                    Currency = s.SelectedRate.ListCurrency,
                    FromAddressId = s.FromAddress.Id,
                    ToAddressId = s.ToAddress.Id,
                    CreatedAt = s.CreatedAt.ToString(),
                    UpdatedAt = s.UpdatedAt.ToString()
                });

                csv.WriteRecords(records);
            }
        }
    }
}
