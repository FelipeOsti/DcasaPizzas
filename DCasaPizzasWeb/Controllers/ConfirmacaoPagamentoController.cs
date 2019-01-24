using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Resources;
using Uol.PagSeguro.Service;

namespace DCasaPizzasWeb.Controllers
{
    public class ConfirmacaoPagamentoController : Controller
    {
        // GET: ConfirmacaoPagamento
        public ActionResult Index(string transaction_id)
        {
            PagSeguroConfiguration.UrlXmlConfiguration = HttpRuntime.AppDomainAppPath + "/Configuration/PagSeguroConfig.xml";

            bool isSandbox = false;
            string sdsSituacao = "";
            Transaction transaction = null;
            try
            {
                AccountCredentials credentials = PagSeguroConfiguration.Credentials(isSandbox);
                transaction = TransactionSearchService.SearchByCode(credentials, transaction_id);

                if (transaction.TransactionStatus == Uol.PagSeguro.Enums.TransactionStatus.Paid)
                {
                    var reference = long.Parse(transaction.Reference);
                    FinanceiroController finC = new FinanceiroController();
                    finC.RealizarPagamentoPagSeguro(reference);
                }
                var nnrSituacao = (int)transaction.TransactionStatus;
                if (nnrSituacao == 1) sdsSituacao = "Aguardando Pagamento";
                if (nnrSituacao == 2) sdsSituacao = "Em Ánalise";
                if (nnrSituacao == 3) sdsSituacao = "Paga";
                if (nnrSituacao == 4) sdsSituacao = "Disponível";
                if (nnrSituacao == 5) sdsSituacao = "Em disputa";
                if (nnrSituacao == 6) sdsSituacao = "Devolvida";
                if (nnrSituacao == 7) sdsSituacao = "Cancelada";

                ViewBag.transaction = transaction;
                ViewBag.sdsSituacao = sdsSituacao;

                return View();
            }
            catch
            {
                throw;
            }

        }
    }
}