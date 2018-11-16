using DCasaPizzasWeb.Models;
using DCasaPizzasWeb.Models.PagSeguro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Resources;
using Uol.PagSeguro.Service;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("api/retornopagamento")]
    public class RetornoPagamentoController : ApiController
    {
        bool isSandbox = true;

        public RetornoPagamentoController()
        {
            PagSeguroConfiguration.UrlXmlConfiguration = HttpRuntime.AppDomainAppPath + "/Configuration/PagSeguroConfig.xml";
        }

        [HttpPost]
        public string Retorno(RetornoPagamentoModel retorno)
        {
            try
            {
                AccountCredentials credentials = PagSeguroConfiguration.Credentials(isSandbox);
                Transaction transaction = NotificationService.CheckTransaction(credentials, retorno.notificationCode);

                var reference = long.Parse(new string(transaction.Reference.Where(char.IsDigit).ToArray()));

                if (transaction.TransactionStatus == Uol.PagSeguro.Enums.TransactionStatus.Paid)
                {                   
                    FinanceiroController finC = new FinanceiroController();
                    finC.RealizarPagamentoPagSeguro(reference);
                }

                SalvarRetorno(reference, transaction.Code, (int)transaction.TransactionStatus, transaction.GrossAmount);
                    
                return JsonConvert.SerializeObject(transaction);
            }
            catch
            {
                throw;
            }
        }

        internal void SalvarRetorno(long reference, string nnrCode, int nnrSituacao, decimal valor)
        {
            var con = new Conexao();
            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                var sdsSituacao = "";
                if (nnrSituacao == 1) sdsSituacao = "Aguardando Retorno";
                if (nnrSituacao == 2) sdsSituacao = "Em Ánalise";
                if (nnrSituacao == 3) sdsSituacao = "Paga";
                if (nnrSituacao == 4) sdsSituacao = "Disponível";
                if (nnrSituacao == 5) sdsSituacao = "Em disputa";
                if (nnrSituacao == 6) sdsSituacao = "Devolvida";
                if (nnrSituacao == 7) sdsSituacao = "Cancelada";
                con.ExecCommand("insert into solari.cr_histpagseg values (GETDATE(),'"+nnrCode+"','"+reference+"',"+nnrSituacao+",'"+sdsSituacao+"',"+valor.ToString(nfi) +")");
            }
            catch
            {
                throw;
            }
            finally
            {
                con.FechaConexao();
            }
        }

    }
}
