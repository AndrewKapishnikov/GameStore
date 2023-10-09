using GameStore.Web.App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace GameStore.Web.App
{
    //PDT it is Payment data transfer
    public class PDTHolder
    {
        public double GrossTotal { get; set; }
        public int InvoiceNumber { get; set; }
        public string PaymentStatus { get; set; }
        public string PayerFirstName { get; set; }
        public double PaymentFee { get; set;}
        public string BusinessEmail { get; set; }
        public string PayerEmail { get; set; }
        public string TxToken { get; set; }
        public string PayerLastName { get; set; }
        public string ReceiverEmail { get; set; }
        public  string ItemName { get; set; }
        public string Currency { get; set; }
        public string TransactionId { get; set; }
        public string SubscriberId { get; set; }
        public string Custom { get; set; }

        private static string authToken, txToken, query, strResponse;

        public static PDTHolder Success(string tx, PayPalConfig payPalConfig)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            authToken = payPalConfig.AuthToken;
            txToken = tx;
            query = string.Format("cmd=_notify-synch&tx={0}&at={1}",txToken,authToken);
            string url = payPalConfig.PostUrl;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Headers.Add("User-Agent", "GamesStore"); //
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = query.Length;
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            stOut.Write(query);
            stOut.Close();
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
            if (strResponse.StartsWith("SUCCESS"))
                return PDTHolder.Parse(strResponse);
            return null;
        }

        private static PDTHolder Parse(string postData)
        {
            string sKey, sValue;
            PDTHolder pDt = new PDTHolder();
            try
            {
                string[] StringArray = postData.Split('\n');
                int i;
                for (i = 1; i < StringArray.Length - 1; i++)
                {
                    string[] array = StringArray[i].Split('=');

                    sKey = array[0];
                    sValue = HttpUtility.UrlDecode(array[1]);
                    switch (sKey)
                    {
                        case "mc_gross":
                            sValue = sValue.Replace(".", ",");
                            pDt.GrossTotal = Convert.ToDouble(sValue);
                            break;
                        case "invoice":
                            pDt.InvoiceNumber = Convert.ToInt32(sValue);
                            break;
                        case "payment_status":
                            pDt.PaymentStatus = Convert.ToString(sValue);
                            break;
                        case "first_name":
                            pDt.PayerFirstName = Convert.ToString(sValue);
                            break;
                        case "mc_fee":
                            sValue = sValue.Replace(".", ",");
                            pDt.PaymentFee = Convert.ToDouble(sValue);
                            break;
                        case "business":
                            pDt.BusinessEmail = Convert.ToString(sValue);
                            break;
                        case "payer_email":
                            pDt.PayerEmail = Convert.ToString(sValue);
                            break;
                        case "Tx Token":
                            pDt.TxToken = Convert.ToString(sValue);
                            break;
                        case "last_name":
                            pDt.PayerLastName = Convert.ToString(sValue);
                            break;
                        case "reciever_email":
                            pDt.ReceiverEmail = Convert.ToString(sValue);
                            break;
                        case "item_name":
                            pDt.ItemName = Convert.ToString(sValue);
                            break;
                        case "mc_currency":
                            pDt.Currency = Convert.ToString(sValue);
                            break;
                        case "txn_id":
                            pDt.TransactionId = Convert.ToString(sValue);
                            break;
                        case "custom":
                            pDt.Custom = Convert.ToString(sValue);
                            break;
                        case "subscr_id":
                            pDt.SubscriberId = Convert.ToString(sValue);
                            break;
                    }
                }

                return pDt;
            }
            catch(Exception ex)
            {
                 throw ex;
            }

        }

        public IReadOnlyDictionary<string, string> GetPayPalPaymentParameters(int orderId)
        {
            var paymentsParameters = new Dictionary<string, string>()
            {
                { nameof(orderId), orderId.ToString() },
                { nameof(GrossTotal), GrossTotal.ToString() },
                { nameof(InvoiceNumber), InvoiceNumber.ToString() },
                { nameof(PaymentStatus), PaymentStatus },
                { nameof(PayerFirstName), PayerFirstName },
                { nameof(PaymentFee), PaymentFee.ToString() },
                { nameof(BusinessEmail), BusinessEmail },
                { nameof(PayerEmail), PayerEmail },
                { nameof(TxToken), TxToken },
                { nameof(PayerLastName), PayerLastName },
                { nameof(ReceiverEmail), ReceiverEmail },
                { nameof(ItemName), ItemName },
                { nameof(Currency), Currency },
                { nameof(TransactionId), TransactionId },
                { nameof(SubscriberId), SubscriberId },
                { nameof(Custom), Custom }
            };
            return paymentsParameters;
        }
    }
}
