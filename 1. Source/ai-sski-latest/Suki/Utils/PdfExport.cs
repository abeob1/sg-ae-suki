using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
//iTextSharp
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Suki.Utils
{
    public class PdfExport
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tblReportData"></param>
        /// <param name="outPutStream"></param>
        /// <param name="relativeWidths"></param>
        public static void DataSourceToPdf(DataTable tblReportData, string outPutStream, float[] relativeWidths)
        {

            Document pdfDoc = new Document(new Rectangle(288f, 144f), 65, 75, 10, 10);
            pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(outPutStream, FileMode.OpenOrCreate));
            pdfDoc.Open();

            int colNumber = tblReportData.Columns.Count;
            PdfPTable table = new PdfPTable(relativeWidths);
            table.WidthPercentage = 115;
            for (int i = 0; i < colNumber; i++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(tblReportData.Columns[i].ColumnName, new iTextSharp.text.Font(Font.NORMAL, 9f, Font.BOLD, BaseColor.WHITE)));
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cell.BackgroundColor = new BaseColor(102, 0, 0);

                table.AddCell(cell);
            }
            foreach (DataRow r in tblReportData.Rows)
            {
                for (int i = 0; i < colNumber; i++)
                {
                    iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new Phrase(r[i].ToString(), new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 7.5f, Font.NORMAL, BaseColor.BLACK)));
                    cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    if (!Utils.AppConstants.isDouble(r[i].ToString()))
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    }
                    else
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    }
                    if (i == 4)
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    }
                    if (i == 0)
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    }
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);
            pdfDoc.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tblReportData"></param>
        /// <param name="outPutStream"></param>
        /// <param name="relativeWidths"></param>
        public static void DataSourceToPdf(DataSet dsData, string outPutStream, float[] relativeWidths)
        {

            Document pdfDoc = new Document(new Rectangle(288f, 144f), 65, 75, 10, 10);
            pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(outPutStream, FileMode.OpenOrCreate));
            pdfDoc.Open();

            //Header
         

            //Detail
            int colNumber = dsData.Tables[0].Columns.Count;
            PdfPTable table = new PdfPTable(relativeWidths);
            table.WidthPercentage = 115;
            for (int i = 0; i < colNumber; i++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(dsData.Tables[0].Columns[i].ColumnName, new iTextSharp.text.Font(Font.NORMAL, 9f, Font.BOLD, BaseColor.WHITE)));
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cell.BackgroundColor = new BaseColor(102, 0, 0);

                table.AddCell(cell);
            }
            foreach (DataRow r in dsData.Tables[0].Rows)
            {
                for (int i = 0; i < colNumber; i++)
                {
                    iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new Phrase(r[i].ToString(), new Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 7.5f, Font.NORMAL, BaseColor.BLACK)));
                    cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    if (!Utils.AppConstants.isDouble(r[i].ToString()))
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    }
                    else
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    }
                    if (i == 4)
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    }
                    if (i == 0)
                    {
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    }
                    table.AddCell(cell);
                }
            }
            //Footer
            pdfDoc.Add(table);
            pdfDoc.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="HTML"></param>
        /// <param name="outPutStream"></param>
        public static void HTMLToPdf(string HTML, string outPutStream)
        {
            Document document = new Document();
            PdfWriter.GetInstance(document, new FileStream(outPutStream, FileMode.Create));
            document.Open();

            iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
            iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
            hw.Parse(new StringReader(HTML));
            document.Close();
        }
    }
}
