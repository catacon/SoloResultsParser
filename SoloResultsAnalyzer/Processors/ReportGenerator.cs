using System.Data.Common;
using iText;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace SoloResultsAnalyzer.Processors
{
    public class ReportGenerator
    {
        private DbConnection _dbConnection;

        public ReportGenerator(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void GenerateEventReport(int eventId)
        {
            PdfDocument pdf = new PdfDocument(new PdfWriter("test.pdf"));
            Document doc = new Document(pdf);

            Table table = new Table(5, true);

            for (int i = 0; i < 5; i++)
            {
                table.AddHeaderCell(new Cell().SetKeepTogether(true).Add(new Paragraph("Header " + i)).SetBold());
            }

            doc.Add(table);
            for (int i = 0; i < 500; i++)
            {
                if (i % 5 == 0)
                {
                    table.Flush();
                }
                table.AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("Test " + i).SetMargins(0, 0, 0, 0)));
            }

            table.Complete();

            doc.Close();
        }
    }
}
