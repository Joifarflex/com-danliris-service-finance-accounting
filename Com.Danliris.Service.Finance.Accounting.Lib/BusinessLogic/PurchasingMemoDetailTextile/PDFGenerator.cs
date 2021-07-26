﻿using Com.Danliris.Service.Finance.Accounting.Lib.Enums;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public static class PDFGenerator
    {
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _biggerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

        public static MemoryStream Generate(PurchasingMemoDetailTextileDto purchasingMemoDetailTextile, string username, int timeoffset)
        {
            var document = new Document(PageSize.A4, 25, 25, 25, 25);
            var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            SetHeader(document, purchasingMemoDetailTextile);

            SetTable(document, purchasingMemoDetailTextile);

            SetFooter(document, purchasingMemoDetailTextile, timeoffset);

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private static void SetHeader(Document document, PurchasingMemoDetailTextileDto purchasingMemoDetailTextile)
        {
            var table = new PdfPTable(1)
            {
                WidthPercentage = 100
            };

            var cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var rightCell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var centeredCell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE

            };

            cell.Phrase = new Phrase("PT. DANLIRIS", _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Kel. Banaran (Sel. Laweyan) Telp. 714400", _smallFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PO. Box. 166 Solo-57100 Indonesia", _smallFont);
            table.AddCell(cell);

            var importLocal = purchasingMemoDetailTextile.SupplierIsImport ? "IMPOR" : "LOKAL";

            centeredCell.Phrase = new Phrase($"PEMBAYARAN HUTANG DAG {importLocal} TEKSTIL RK UANG MUKA PEMBELIAN {importLocal}", _biggerFont);
            centeredCell.PaddingTop = 5;
            table.AddCell(centeredCell);

            rightCell.Phrase = new Phrase($"TEX(151100)", _smallFont);
            rightCell.PaddingBottom = 10;
            table.AddCell(rightCell);

            document.Add(table);
        }

        private static void SetTableHeader(PdfPTable table, PurchasingMemoType type)
        {
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
            };

            var source = type == PurchasingMemoType.Disposition ? "NO DISPOSISI" : "NO SPB";

            cell.PaddingTop = 10;
            cell.Rowspan = 2;
            cell.Phrase = new Phrase("No", _smallBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("KAS BON", _smallBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("KETERANGAN", _smallBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("VALAS ", _smallBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("JUMLAH (Rp) BAYAR", _smallBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("NO DISPOSISI", _smallBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("VALAS ", _smallBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("JUMLAH (Rp) BELI", _smallBoldFont);
            table.AddCell(cell);

        }

        private static void SetTable(Document document, PurchasingMemoDetailTextileDto purchasingMemoDetailTextile)
        {
            var table = new PdfPTable(8)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 5f, 12f, 23f, 15f, 15f, 12f, 15f, 15f });

            SetTableHeader(table, purchasingMemoDetailTextile.Type);

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellColspan3 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 3
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellNoBorderAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = Rectangle.NO_BORDER
            };

            double totalDebit = 0;
            double totalCredit = 0;
            int no = 1;

            if (purchasingMemoDetailTextile.Type == PurchasingMemoType.Disposition)
            {
                var sumPurchaseAmount = purchasingMemoDetailTextile.Items.SelectMany(element => element.Disposition.Details).Sum(element => element.PurchaseAmount);
                var sumPaymentAmount = purchasingMemoDetailTextile.Items.SelectMany(element => element.Disposition.Details).Sum(element => element.PaymentAmount);
                var sumPurchaseAmountCurrency = purchasingMemoDetailTextile.Items.SelectMany(element => element.Disposition.Details).Sum(element => element.PurchaseAmountCurrency);
                var sumPaymentAmountCurrency = purchasingMemoDetailTextile.Items.SelectMany(element => element.Disposition.Details).Sum(element => element.PaymentAmountCurrency);
                var difference = sumPaymentAmount - sumPurchaseAmount;
                var differenceCurrency = sumPaymentAmountCurrency - sumPurchaseAmountCurrency;
                foreach (var item in purchasingMemoDetailTextile.Items)
                {
                    foreach (var detail in item.Disposition.Details)
                    {
                        cell.Phrase = new Phrase(no.ToString(), _smallerFont);
                        table.AddCell(cell);

                        cell.Phrase = new Phrase(detail.Expenditure.DocumentNo, _smallFont);
                        table.AddCell(cell);

                        cellAlignLeft.Phrase = new Phrase(detail.Remark, _smallFont);
                        table.AddCell(cellAlignLeft);

                        cellAlignRight.Phrase = new Phrase(detail.PaymentAmountCurrency.ToString("#,##0.#0"), _smallFont);
                        table.AddCell(cellAlignRight);

                        cellAlignRight.Phrase = new Phrase(detail.PaymentAmount.ToString("#,##0.#0"), _smallFont);
                        table.AddCell(cellAlignRight);

                        cell.Phrase = new Phrase(detail.UnitPaymentOrder.UnitPaymentOrderNo, _smallFont);
                        table.AddCell(cell);

                        cellAlignRight.Phrase = new Phrase(detail.PurchaseAmountCurrency.ToString("#,##0.#0"), _smallFont);
                        table.AddCell(cellAlignRight);

                        cellAlignRight.Phrase = new Phrase(detail.PurchaseAmount.ToString("#,##0.#0"), _smallFont);
                        table.AddCell(cellAlignRight);

                        //totalDebit += detail.DebitAmount;
                        //totalCredit += detail.CreditAmount;
                        no++;
                    }
                }

                cellNoBorderAlignRight.Colspan = 3;
                cellNoBorderAlignRight.Phrase = new Phrase("Total Bayar", _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Colspan = 1;
                cellNoBorderAlignRight.Phrase = new Phrase(sumPaymentAmountCurrency.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(sumPaymentAmount.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase("Total Beli", _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(sumPurchaseAmountCurrency.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(sumPurchaseAmount.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Colspan = 6;
                cellNoBorderAlignRight.Phrase = new Phrase("Selisih", _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(differenceCurrency.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(difference.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);
            }
            else
            {
                var sumPurchaseAmount = purchasingMemoDetailTextile.Details.Sum(element => element.PurchaseAmount);
                var sumPaymentAmount = purchasingMemoDetailTextile.Details.Sum(element => element.PaymentAmount);
                var sumPurchaseAmountCurrency = purchasingMemoDetailTextile.Details.Sum(element => element.PurchaseAmountCurrency);
                var sumPaymentAmountCurrency = purchasingMemoDetailTextile.Details.Sum(element => element.PaymentAmountCurrency);
                var difference = sumPaymentAmount - sumPurchaseAmount;
                var differenceCurrency = sumPaymentAmountCurrency - sumPurchaseAmountCurrency;

                foreach (var detail in purchasingMemoDetailTextile.Details)
                {
                    cell.Phrase = new Phrase(no.ToString(), _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(detail.Expenditure.DocumentNo, _smallFont);
                    table.AddCell(cell);

                    cellAlignLeft.Phrase = new Phrase(detail.Remark, _smallFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignRight.Phrase = new Phrase(detail.PaymentAmountCurrency.ToString("#,##0.#0"), _smallFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(detail.PaymentAmount.ToString("#,##0.#0"), _smallFont);
                    table.AddCell(cellAlignRight);

                    cell.Phrase = new Phrase(detail.UnitPaymentOrder.UnitPaymentOrderNo, _smallFont);
                    table.AddCell(cell);

                    cellAlignRight.Phrase = new Phrase(detail.PurchaseAmountCurrency.ToString("#,##0.#0"), _smallFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(detail.PurchaseAmount.ToString("#,##0.#0"), _smallFont);
                    table.AddCell(cellAlignRight);

                    //totalDebit += detail.DebitAmount;
                    //totalCredit += detail.CreditAmount;
                    no++;
                }

                cellNoBorderAlignRight.Colspan = 3;
                cellNoBorderAlignRight.Phrase = new Phrase("Total Bayar", _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Colspan = 1;
                cellNoBorderAlignRight.Phrase = new Phrase(sumPaymentAmountCurrency.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(sumPaymentAmount.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase("Total Beli", _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(sumPurchaseAmountCurrency.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(sumPurchaseAmount.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Colspan = 6;
                cellNoBorderAlignRight.Phrase = new Phrase("Selisih", _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(differenceCurrency.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);

                cellNoBorderAlignRight.Phrase = new Phrase(difference.ToString("#,##0.#0"), _smallFont);
                table.AddCell(cellNoBorderAlignRight);
            }

            //cellColspan3.Phrase = new Phrase("Jumlah Total", _smallBoldFont);
            //table.AddCell(cellColspan3);

            //cellAlignRight.Phrase = new Phrase(totalDebit.ToString("#,##0.#0"), _smallBoldFont);
            //table.AddCell(cellAlignRight);

            //cellAlignRight.Phrase = new Phrase(totalCredit.ToString("#,##0.#0"), _smallBoldFont);
            //table.AddCell(cellAlignRight);

            document.Add(table);
        }

        private static void SetFooter(Document document, PurchasingMemoDetailTextileDto purchasingMemoDetailTextile, int offSet)
        {
            PdfPTable table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };

            table.SetWidths(new float[] { 1f, 1f, 1f });

            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
            };

            PdfPCell cellColspan2 = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 2
            };

            cellColspan2.Phrase = new Phrase($"Keterangan : {purchasingMemoDetailTextile.Remark}", _smallFont);
            cellColspan2.PaddingBottom = 10;
            table.AddCell(cellColspan2);
            cell.Phrase = new Phrase();
            table.AddCell(cell);

            //cellColspan2.Phrase = new Phrase("Nomor Memo Pusat :", _smallFont);
            //table.AddCell(cellColspan2);
            //cell.Phrase = new Phrase();
            //table.AddCell(cell);

            cell.Phrase = new Phrase();
            table.AddCell(cell);
            cell.Phrase = new Phrase();
            table.AddCell(cell);
            cell.Phrase = new Phrase();
            table.AddCell(cell);

            cell.Phrase = new Phrase("Mengetahui", _smallFont);
            table.AddCell(cell);
            cell.Phrase = new Phrase();
            table.AddCell(cell);
            cell.Phrase = new Phrase($"Solo, {DateTime.UtcNow.AddHours(offSet).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", _smallFont);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Kepala Pembukuan", _smallFont);
            table.AddCell(cell);
            cell.Phrase = new Phrase();
            table.AddCell(cell);
            cell.Phrase = new Phrase("Yang Membuat", _smallFont);
            table.AddCell(cell);

            for (var i = 0; i < 4; i++)
            {
                cell.Phrase = new Phrase();
                table.AddCell(cell);
                cell.Phrase = new Phrase();
                table.AddCell(cell);
                cell.Phrase = new Phrase();
                table.AddCell(cell);
            }

            cell.Phrase = new Phrase("(..................)", _smallFont);
            table.AddCell(cell);
            cell.Phrase = new Phrase();
            table.AddCell(cell);
            cell.Phrase = new Phrase($"(..................)", _smallFont);
            table.AddCell(cell);

            document.Add(table);
        }
    }
}
