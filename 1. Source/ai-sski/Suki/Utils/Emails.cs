using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace Suki.Utils
{
    public class Emails
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>

        public static string SendEmail(string from, string[] tos, string subject, string body, Attachment[] attachments, string embeddedImage, string poNo, string Id)
        {
            MasterData obj = new MasterData();
            try
            {
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpClient"].ToString();
                    smtp.Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpServerPort"].ToString()); ;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["FromEmail"].ToString(), System.Configuration.ConfigurationManager.AppSettings["EmailPassword"].ToString());
                    smtp.Timeout = 900000;
                }
                MailMessage mail = new MailMessage();

                if (embeddedImage != null)
                {
                    AlternateView View;
                    LinkedResource resource;

                    View = AlternateView.CreateAlternateViewFromString(body.ToString(), null, "text/html");

                    resource = new LinkedResource(embeddedImage);

                    resource.ContentId = "Logo";

                    View.LinkedResources.Add(resource);

                    mail.AlternateViews.Add(View);
                }

                mail.From = new MailAddress(from);
                foreach (string to in tos)
                {
                    mail.To.Add(to);
                }
                mail.Subject = subject;
                //mail.Body = body;
                if (attachments != null)
                {
                    foreach (Attachment attachment in attachments)
                    {
                        mail.Attachments.Add(attachment);
                    }
                }
                mail.Body = body;
                mail.IsBodyHtml = true;
                smtp.Send(mail);
                obj.UpdateStatusforEmailCheck(poNo, "Success", string.Empty ,Id);
            }
            catch (Exception ex)
            {
                //obj.UpdateStatusforEmailCheck(poNo, "Failure", ex.Message);
                AppConstants.WriteLog(ex.Message);
                return ex.Message;
            }
            return string.Empty;//"Email Sent successfuly";
        }
    }
}