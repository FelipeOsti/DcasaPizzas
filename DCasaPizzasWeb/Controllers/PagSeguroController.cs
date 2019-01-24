using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Uol.PagSeguro;
using Uol.PagSeguro.Constants;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Exception;
using Uol.PagSeguro.Resources;
using DCasaPizzasWeb.Models;
using DCasaPizzasWeb.Models.PagSeguro;
using DCasaPizzasWeb.Models.PagSeguro;
using Uol.PagSeguro.Service;
using Newtonsoft.Json;
using System.Text;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("api/pagseguro")]
    public class PagSeguroController : ApiController
    {
        bool isSandbox = false;

        public PagSeguroController()
        {
            PagSeguroConfiguration.UrlXmlConfiguration = HttpRuntime.AppDomainAppPath + "/Configuration/PagSeguroConfig.xml";
        }

        internal string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return String.Empty;

            byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(texto);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        [Route("CriarPagamento")]
        public string CriarPagamento(PagSeguroModel pagSeguro)
        {            
            EnvironmentConfiguration.ChangeEnvironment(isSandbox);

            // Instantiate a new payment request
            PaymentRequest payment = new PaymentRequest();

            var itemParam = new ParameterItem("encoding", "UTF-8");
            payment.Parameter.Items.Add(itemParam);

            // Sets the currency
            payment.Currency = Currency.Brl;

            foreach (var item in pagSeguro.produtos)
            {
                // Add an item for this payment request
                payment.Items.Add(new Item(item.id, RemoverAcentos(item.descricao) ,item.qtde,item.unitario));
            }
            // Sets a reference code for this payment request, it is useful to identify this payment in future notifications.
            payment.Reference = pagSeguro.paymontReference;

            // Sets shipping information for this payment request
            payment.Shipping = new Shipping();
            payment.Shipping.ShippingType = pagSeguro.shippingType;// ShippingType.Sedex;

            //Passando valor para ShippingCost
            payment.Shipping.Cost = pagSeguro.shippingCost;

            payment.Shipping.Address = new Address(pagSeguro.enderecoEntrega.pais, pagSeguro.enderecoEntrega.estado,
                pagSeguro.enderecoEntrega.cidade, pagSeguro.enderecoEntrega.bairro, pagSeguro.enderecoEntrega.cep,
                pagSeguro.enderecoEntrega.endereco, pagSeguro.enderecoEntrega.numero, pagSeguro.enderecoEntrega.complemento);

            // Sets your customer information.
            payment.Sender = new Sender(pagSeguro.cliente.nome, pagSeguro.cliente.email, new Phone(pagSeguro.cliente.ddd,pagSeguro.cliente.telefone));

            SenderDocument document = new SenderDocument(Documents.GetDocumentByType("CPF"), pagSeguro.cliente.documento);
            payment.Sender.Documents.Add(document);

            // Sets the url used by PagSeguro for redirect user after ends checkout process
            payment.RedirectUri = new Uri("http://google.com");

            // Add checkout metadata information
            //payment.AddMetaData(MetaDataItemKeys.GetItemKeyByDescription("CPF do passageiro"), "086.111.629-19", 1);
            //payment.AddMetaData("PASSENGER_PASSPORT", "23456", 1);

            // Another way to set checkout parameters
            //payment.AddParameter("senderBirthday", "07/05/1980");
            //payment.AddIndexedParameter("itemColor", "verde", 1);
            //payment.AddIndexedParameter("itemId", "0003", 3);
            //payment.AddIndexedParameter("itemDescription", "Mouse", 3);
            //payment.AddIndexedParameter("itemQuantity", "1", 3);
            //payment.AddIndexedParameter("itemAmount", "200.00", 3);

            // Add discount per payment method
            //payment.AddPaymentMethodConfig(PaymentMethodConfigKeys.DiscountPercent, 5.00, PaymentMethodGroup.CreditCard);

            // Add installment without addition per payment method
            //payment.AddPaymentMethodConfig(PaymentMethodConfigKeys.MaxInstallmentsNoInterest, 6, PaymentMethodGroup.CreditCard);

            // Add installment limit per payment method
            //payment.AddPaymentMethodConfig(PaymentMethodConfigKeys.MaxInstallmentsLimit, 8, PaymentMethodGroup.CreditCard);

            // Add and remove groups and payment methods
            List<string> accept = new List<string>();
            accept.Add(ListPaymentMethodNames.DebitoBradesco);
            accept.Add(ListPaymentMethodNames.DebitoBancoDoBrasil);
            accept.Add(ListPaymentMethodNames.Boleto);
            payment.AcceptPaymentMethodConfig(ListPaymentMethodGroups.CreditCard, accept);

            //List<string> exclude = new List<string>();
            //exclude.Add(ListPaymentMethodNames.Boleto);
            //payment.ExcludePaymentMethodConfig(ListPaymentMethodGroups.Boleto, exclude);

            try
            {
                /// Create new account credentials
                /// This configuration let you set your credentials from your ".cs" file.
                //AccountCredentials credentials = new AccountCredentials("solarinegocios@gmail.com", "B8F8E56F7F1F4D658427A25683472131");

                /// @todo with you want to get credentials from xml config file uncommend the line below and comment the line above.
                AccountCredentials credentials = PagSeguroConfiguration.Credentials(isSandbox);

                Uri paymentRedirectUri = payment.Register(credentials);

                return paymentRedirectUri.AbsoluteUri;
            }
            catch
            {
                throw;
            }
        }
    }    
}
