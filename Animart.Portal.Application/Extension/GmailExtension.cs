using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace Animart.Portal.Extension
{
    public class GmailExtension
    {
        private SmtpClient _client;
        private readonly string _emailAddress;
        private readonly string _password;

        public static string ANIMART_EMAILADDRESS = "marketing@animart.co.id";
        public static string ANIMART_PASSWORD = "GOSALES2017gogo";

        public GmailExtension() : this(ANIMART_EMAILADDRESS, ANIMART_PASSWORD)
        {

        }

        public GmailExtension(string emailAddress, string password)
        {
            _emailAddress = emailAddress;
            _password = password;
            InitializeSmtpClient();
        }

        private void InitializeSmtpClient()
        {
            _client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailAddress, _password)
            };
        }

        public bool SendMessage(string subject, string body, string receiverMail,
            bool withInvoice, Invoice.Invoice inv,Shipment.ShipmentCost ship)
        {
            try
            {
                if (!withInvoice)
                {
                    MailMessage message = new MailMessage();

                    message.From = new MailAddress(_emailAddress);
                    message.To.Add(receiverMail);
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = body;

                    _client.Send(message);
                }
                else
                {
                    SendInvoice(subject, body, receiverMail,inv,ship);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SendInvoice(string subject, string body, string receiverMail,
            Invoice.Invoice inv, Shipment.ShipmentCost ship)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[5] {
                            new DataColumn("Code"),
                            new DataColumn("Name"),
                            new DataColumn("Quantity"),
                            new DataColumn("Price"),
                            new DataColumn("Total")});
                for (int e = 0; e < inv.OrderItems.Count; e++)
                {
                    var item = inv.OrderItems.ElementAt(e);
                    dt.Rows.Add(item.Item.Code,item.Name,
                        item.QuantityAdjustment, "Rp." + item.PriceAdjustment.ToString("N0") + ",00",
                        "Rp." + (item.QuantityAdjustment*item.PriceAdjustment).ToString("N0")+",00");
                }

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                        sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Invoice Details</b></td></tr>");
                        sb.Append("<tr><td colspan = '2'><img style='text-align:right;' src='http://www.animartportal.co.id/Content/images/animart_logo.png'/></td></tr>");
                        sb.Append("<tr><td><b>Invoice No:</b>");
                        sb.Append(inv.InvoiceNumber);
                        sb.Append("</td><td><b>Date: </b>");
                        sb.Append(DateTime.Now.ToString("dd-MMMM-yyyy, hh:mm tt"));
                        sb.Append(" </td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>Order No: </b> ");
                        var orderItem = inv.OrderItems.FirstOrDefault();
                        sb.Append(orderItem != null ? orderItem.PurchaseOrder.Code : "-");
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>Customer Name: </b>");
                        sb.Append(orderItem != null ? orderItem.CreatorUser.Name : "-");
                        sb.Append("</td></tr>");
                        sb.Append("</table>");
                        sb.Append("<br><br>");
                        sb.Append("<b>Order(s) Information:</b><br><br>");
                        sb.Append("<table border = '1'>");
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<th style = 'background-color: rgb(63,81,181);color:#ffffff'>");
                            sb.Append(column.ColumnName);
                            sb.Append("</th>");
                        }
                        sb.Append("</tr>");
                        foreach (DataRow row in dt.Rows)
                        {
                            sb.Append("<tr>");
                            foreach (DataColumn column in dt.Columns)
                            {
                                sb.Append("<td>");
                                sb.Append(row[column]);
                                sb.Append("</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append("<tr><td align='right' colspan = '4'><b>Subtotal: </b></td><td>");
                        sb.Append("Rp. "+inv.OrderItems.Sum(e => e.QuantityAdjustment*e.PriceAdjustment).ToString("N0") + ",00");
                        sb.Append("</td></tr>");
                        int weight = ((int)inv.TotalWeight + 999)/1000;
                        decimal shipmentCost = ship.FirstKilo;
                        shipmentCost += Math.Max(weight - ship.KiloQuantity,0)*ship.NextKilo;
                        sb.Append("<tr><td align='right' colspan = '4'><b>Shipping Cost (" + weight + " kg): </b></td><td>");
                        sb.Append("Rp. " + (shipmentCost).ToString("N0") + ",00");
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td align='right' colspan = '4'><b>Total: </b></td><td>");
                        sb.Append("Rp. " + (shipmentCost + inv.GrandTotal).ToString("N0") + ",00");
                        sb.Append("</td></tr>");
                        sb.Append("</table>");

                        sb.Append("<br><b>Shipping Information:</b><br><br>");
                        sb.Append("<table border = '1'>");
                        sb.Append("<tr><th><b>Phone Number</b></th><td colspan='5'>" + (orderItem != null ? orderItem.PurchaseOrder != null ? orderItem.PurchaseOrder.Address : "-" : "-") + "</td></tr>");
                        sb.Append("<tr><th><b>Address</b></th><td colspan='5'>" + (orderItem != null ? orderItem.PurchaseOrder != null ? orderItem.PurchaseOrder.Address : "-" : "-") + "</td></tr>");
                        sb.Append("<tr><th><b>Province</b></th><td>" + (orderItem != null ? orderItem.PurchaseOrder != null ? orderItem.PurchaseOrder.Province : "-" : "-") + "</td>");
                        sb.Append("<th><b>City</b></th><td>" + (orderItem != null ? orderItem.PurchaseOrder != null ? orderItem.PurchaseOrder.City : "-" : "-") + "</td>");
                        sb.Append("<th><b>Postal Code</b></th><td>" + (orderItem != null ? orderItem.PurchaseOrder != null ? orderItem.PurchaseOrder.PostalCode : "-" : "-") + "</td></tr>");
                        sb.Append("<tr><th><b>Expedition</b></th><td colspan='5'>" + inv.Expedition + "</td></tr>");
                        sb.Append("</table>");
                        sb.Append("<br>Pembayaran harap dilakukan melalui no.rekening:");
                        sb.Append("<br><b>Bank Central Asia (BCA)</b>");
                        sb.Append("<br><b>Acc : 453-221-7788</b>");
                        sb.Append("<br><b>A/n : Frans Setiadi / Indra Lukito</b>");
                        sb.Append("<br><br><b>* Mohon agar segera mengabari Animart setelah melakukan transfer via telp (022)-6126-824 atau email.</b>");
                        StringReader sr = new StringReader(sb.ToString());

                        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            pdfDoc.Open();
                            htmlparser.Parse(sr);
                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();

                            MailMessage message = new MailMessage();

                            message.From = new MailAddress(_emailAddress);
                            message.To.Add(receiverMail);
                            message.Subject = subject;
                            message.IsBodyHtml = true;
                            message.Body = body;
                            message.Attachments.Add(new Attachment(new MemoryStream(bytes), "invoice-"+inv.InvoiceNumber+".pdf"));
                            _client.Send(message);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
