using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels.Commands;
using CsvHelper;
using EasyPost.Models.API;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Document = iTextSharp.text.Document;

namespace ContactsApp.ViewModels
{
    public class AllShipmentsViewModel : INotifyPropertyChanged
    {
        private readonly IEasyPostService _easyPostService;

        public ObservableCollection<Shipment> Shipments { get; set; }
        public ICommand ExportToPdfCommand { get; }
        public ICommand ExportToCsvCommand { get; }

        public AllShipmentsViewModel(IEasyPostService easyPostService)
        {
            _easyPostService = easyPostService;
            Shipments = new ObservableCollection<Shipment>();
            ExportToPdfCommand = new RelayCommand(ExportToPdf);
            ExportToCsvCommand = new RelayCommand(ExportToCsv);
            LoadAllShipments();
        }

        private async void LoadAllShipments()
        {
            try
            {
                var shipments = await _easyPostService.GetAllShipmentsAsync();
                Shipments.Clear();
                foreach (var shipment in shipments)
                {
                    Shipments.Add(shipment);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load shipments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExportToPdf()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "Shipments.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
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

                    foreach (var shipment in Shipments)
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
        }

        private void ExportToCsv()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv",
                FileName = "Shipments.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<Shipment>();
                    csv.NextRecord();

                    foreach (var shipment in Shipments)
                    {
                        csv.WriteRecord(shipment);
                        csv.NextRecord();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
