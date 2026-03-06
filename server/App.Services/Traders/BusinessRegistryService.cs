using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace App.Services.Traders
{
    /// <summary>
    /// Βασικά στοιχεία μητρώου για νομικά πρόσωπα, νομικές οντότητες,
    /// και φυσικά πρόσωπα με εισόδημα από επιχειρηματική δραστηριότητα
    /// </summary>
    public partial interface IBusinessRegistryService
    {
        Dictionary<string, string> GetDocument(string username, string password, string afmCalledFor);
    }
    public partial class BusinessRegistryService : IBusinessRegistryService
    {
        private readonly string _wsdl = @"https://www1.gsis.gr/wsaade/RgWsPublic2/RgWsPublic2?WSDL";

        private HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = WebRequest.Create(_wsdl) as HttpWebRequest;
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "application/soap+xml; charset=utf-8";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            return webRequest;
        }

        private string GetXml(string username, string password, string afmCalledFor)
        {
            HttpWebRequest request = CreateWebRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            string xml = @"<env:Envelope xmlns:env='http://www.w3.org/2003/05/soap-envelope' xmlns:ns1='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd' xmlns:ns2='http://rgwspublic2/RgWsPublic2Service' xmlns:ns3='http://rgwspublic2/RgWsPublic2'>
                           <env:Header>
                              <ns1:Security>
                                 <ns1:UsernameToken>
                                    <ns1:Username> _username </ns1:Username>
                                    <ns1:Password> _password </ns1:Password>
                                 </ns1:UsernameToken>
                              </ns1:Security>
                           </env:Header>
                           <env:Body>
                              <ns2:rgWsPublic2AfmMethod>
                                 <ns2:INPUT_REC>
                                    <ns3:afm_called_by/>
                                    <ns3:afm_called_for> _afmCalledFor </ns3:afm_called_for>
                                 </ns2:INPUT_REC>
                              </ns2:rgWsPublic2AfmMethod>
                           </env:Body>
                        </env:Envelope>";

            xml = xml.Replace("_username", username).Replace("_password", password).Replace("_afmCalledFor", afmCalledFor);

            soapEnvelopeXml.LoadXml(xml);

            using Stream stream = request.GetRequestStream();
            soapEnvelopeXml.Save(stream);

            using WebResponse response = request.GetResponse();
            using StreamReader rd = new StreamReader(response.GetResponseStream());
            string soapResult = rd.ReadToEnd();
            return soapResult;
                
        }
        public virtual Dictionary<string, string> GetDocument(string username, string password, string afmCalledFor)
        {
            var xmlValue = GetXml(username, password, afmCalledFor);
            var values = new Dictionary<string, string>();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlValue);

                foreach (XmlNode afm_called_by_rec in xmlDoc.GetElementsByTagName("afm_called_by_rec"))
                {
                    foreach (XmlNode node in afm_called_by_rec.ChildNodes)
                    {
                        values.Add(node.Name, node.InnerText.Trim());
                    }
                }

                foreach (XmlNode basic_rec in xmlDoc.GetElementsByTagName("basic_rec"))
                {
                    foreach (XmlNode node in basic_rec.ChildNodes)
                    {
                        values.Add(node.Name, node.InnerText.Trim());
                    }
                }

                foreach (XmlNode firm_act_tab in xmlDoc.GetElementsByTagName("firm_act_tab"))
                {
                    foreach (XmlNode item in firm_act_tab.ChildNodes)
                    {
                        foreach (XmlNode node in item.ChildNodes)
                        {
                            values.Add(node.Name, node.InnerText.Trim());
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return values;
        }
    }
}
