using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Util;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;


namespace StationeryStore.TemplatesAndGenerators
{
    public class PurchaseOrderGenerator
    {
        PurchaseService purchaseService = new PurchaseService();

        public WordprocessingDocument PurchaseOrder(PurchaseOrderEF po, List<PurchaseOrderDetailsEF> poDetailList)
        {
            string supplierCode = po.SupplierCode;
            string templatePath = "/TemplatesAndGenerators/PurchaseOrderTemplate.dotx";
            string resultPath = "/out/purchaseOrder.docx";
            using (WordprocessingDocument document = WordprocessingDocument.CreateFromTemplate(templatePath))
            {
                var body = document.MainDocumentPart.Document.Body;
                var paragraphs = body.Elements<Paragraph>();

                var texts = paragraphs.SelectMany(p => p.Elements<Run>()).SelectMany(r => r.Elements<Text>());

                foreach (Text text in texts)
                {
                    switch (text.Text)
                    {
                        case "<PurchaseOrderId>":
                            text.Text = $"{po.OrderId}";
                            break;
                        case "<SupplierName>":
                            text.Text = $"{po.Supplier.SupplierName}";
                            break;
                        case "<DeliveryAddress>":
                            text.Text = po.DeliveryAddress;
                            break;
                        case "<ContactPerson>":
                            text.Text = po.Supplier.ContactName;
                            break;
                        case "<DeliverByDate>":
                            text.Text = Timestamp.dateFromTimestamp(po.DeliverByDate);
                            break;
                        case "<DetailsTable>":
                            text.ReplaceChild(PurchaseOrderDetailsTable(poDetailList, supplierCode), text);
                            break;
                        case "<CreatedByName>":
                            text.Text = po.CreatedBy.Name;
                            break;
                        case "<DateIssued>":
                            text.Text = DateTime.UtcNow.ToShortDateString();
                            break;
                        default:
                            Console.WriteLine(text.Text);
                            Console.ReadKey();
                            break;
                    }
                }

                // Save result document, not modifying the template
                document.SaveAs(resultPath);
                return document;
            }
        }

        private Table PurchaseOrderDetailsTable(List<PurchaseOrderDetailsEF> poDetails, string supplierCode)
        {
            List<SupplierDetailsEF> supplierItems = purchaseService.FindSupplierItems(supplierCode);
            // Create an empty table.
            Table table = new Table();

            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = new TableProperties(
                new TableBorders(
                    new TopBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 24
                    },
                    new BottomBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 24
                    },
                    new LeftBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 24
                    },
                    new RightBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 24
                    },
                    new InsideHorizontalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 24
                    },
                    new InsideVerticalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 24
                    }
                )
            );

            // Append the TableProperties object to the empty table.
            table.AppendChild<TableProperties>(tblProp);

            // Create a row.
            TableRow tr = new TableRow();
            tr.TableRowProperties.AppendChild(new TableHeader());

            // Create a cell.
            TableCell tc1 = new TableCell();

            // Specify the width property of the table cell.
            tc1.Append(new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "50" }));

            // Specify the table cell content.
            tc1.Append(new Paragraph(new Run(new Text("Item No"))));
            tr.Append(tc1);

            TableCell tc2 = new TableCell();
            tc2.Append(new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "200" }));
            tc2.Append(new Paragraph(new Run(new Text("Description"))));
            tr.Append(tc2);

            TableCell tc3 = new TableCell();
            tc3.Append(new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "100" }));
            tc3.Append(new Paragraph(new Run(new Text("Quantity"))));
            tr.Append(tc3);

            TableCell tc4 = new TableCell();
            tc4.Append(new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "100" }));
            tc4.Append(new Paragraph(new Run(new Text("Price/Unit"))));
            tr.Append(tc4);

            TableCell tc5 = new TableCell();
            tc5.Append(new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "100" }));
            tc5.Append(new Paragraph(new Run(new Text("Subtotal"))));
            tr.Append(tc5);

            //add headers to table.
            table.Append(tr);

            int count = 0;
            foreach (var d in poDetails)
            {
                count++;
                SupplierDetailsEF sdef = supplierItems.Where(x => x.ItemCode == d.ItemCode).SingleOrDefault();
                double subtotal = d.QuantityOrdered * sdef.UnitPrice;

                TableRow newRow = new TableRow();

                TableCell cell1 = new TableCell();
                cell1.Append(new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "50" }));
                cell1.Append(new Paragraph(new Run(new Text(count.ToString()))));
                newRow.Append(cell1);

                TableCell cell2 = new TableCell();
                cell2.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "200" }));
                cell2.Append(new Paragraph(new Run(new Text(d.Stock.Description))));
                newRow.Append(cell2);

                TableCell cell3 = new TableCell();
                cell3.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "100" }));
                cell3.Append(new Paragraph(new Run(new Text(d.QuantityOrdered.ToString()))));
                newRow.Append(cell3);

                TableCell cell4 = new TableCell();
                cell4.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "100" }));
                cell4.Append(new Paragraph(new Run(new Text(sdef.UnitPrice.ToString()))));
                newRow.Append(cell4);

                TableCell cell5 = new TableCell();
                cell5.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "100" }));
                cell5.Append(new Paragraph(new Run(new Text(subtotal.ToString()))));
                newRow.Append(cell5);

                table.Append(newRow);
            }

            return table;
        }
    }
}