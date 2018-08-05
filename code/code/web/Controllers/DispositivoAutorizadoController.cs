using AppRomagnole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    [Authorize]
    [RoutePrefix("api/dispositivo")]
    public class DispositivoAutorizadoController : ApiController
    {
        [HttpGet]
        [Route("GetDispositivoIMEI")]
        public List<ModelDispositivo> GetDispositivoIMEI(string IMEI)
        {
            List<ModelDispositivo> lstDispositivo = new List<ModelDispositivo>();

            DbDataReader qryDisp = null;
            Conexao conn = Conexao.Instance(null);
            try
            {                
                qryDisp = conn.execQuery("select * from IN_DISPOSITIVO where DS_IMEI = '"+IMEI+"' and DT_INATIVO is null");
                if (qryDisp.HasRows)
                {
                    while (qryDisp.Read())
                    {
                        lstDispositivo.Add(new ModelDispositivo()
                        {
                            email = qryDisp.GetString(qryDisp.GetOrdinal("DS_EMAIL")),
                            IMEI = qryDisp.GetString(qryDisp.GetOrdinal("DS_IMEI")),
                            sdsDispositivo = qryDisp.GetString(qryDisp.GetOrdinal("DS_DISPOSITIVO")),
                            sdsVersao = qryDisp.GetString(qryDisp.GetOrdinal("DS_VERSAO")),
                        });
                    }
                }

                return lstDispositivo;    
            }
            catch { throw; }
            finally {
                if (!qryDisp.IsClosed) qryDisp.Close();
                conn.fechaCon();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("DispositivoAutorizado")]
        public string DispositivoAutorizado(ModelDispositivo DispInfo, string emailRequis)
        {
            DbDataReader qryDisp = null;
            Conexao con = Conexao.Instance(emailRequis);
            try
            {                
                qryDisp = con.execQuery("select * from IN_DISPOSITIVO where DS_EMAIL = '" + DispInfo.email + "' and DS_IMEI = '" + DispInfo.IMEI + "' order by DT_ENVIO desc");

                if (qryDisp.HasRows)
                {
                    bool bboCancela = false;

                    while (qryDisp.Read())
                    {
                        if (qryDisp.IsDBNull(qryDisp.GetOrdinal("DT_INATIVO")))
                        {
                            if (qryDisp.IsDBNull(qryDisp.GetOrdinal("DT_LIBERACAO")))
                                return "A"; //Está ativo mas precisa de liberação

                            return "T"; //Possui liberação e está ativo
                        }

                        if (!qryDisp.IsDBNull(qryDisp.GetOrdinal("DT_CANCELACODIGO"))) //Caso o campo estja preenchido
                            bboCancela = true; //Ja houve um pedido de cancelamento de código
                    }

                    //Caso não possua nenhum ativo
                    if (bboCancela) //Ja houve solicitação de cancelamento
                        return "F";

                    //Caso não tenha nada ativo e nunca houve solicitação de cancelamento, ou seja, inativação por algum outro motivo.
                    SolicitaAprovacao(DispInfo,emailRequis);
                    return "A";
                }
                else
                {
                    SolicitaAprovacao(DispInfo,emailRequis);
                    return "A";
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!qryDisp.IsClosed) qryDisp.Close();
                con.fechaCon();
            }
        }

        [HttpPost]
        [Route("AprovaCodigoAPP")]
        public string AprovaCodigoAPP(ModelDispositivo DispInfo, string codigo, string emailRequis)
        {
            Conexao con = Conexao.Instance(emailRequis);
            try
            {
                PINController pinC = new PINController();
                string codigoCrip = pinC.CriptografaSHA256(codigo.ToUpper());
                
                con.ExecCommand("update IN_DISPOSITIVO set DT_LIBERACAO = sysdate where DS_EMAIL = '" + DispInfo.email + "' and DS_IMEI = '" + DispInfo.IMEI + "' and DS_CODIGO = '" + codigoCrip + "'");
                return "T";
            }
            catch
            {
                throw;
            }
            finally
            {
                con.fechaCon();
            }
        }

        [HttpPost]
        [Route("ValidaCodigoAprovacao")]
        public string ValidaCodigoAprovacao(ModelDispositivo DispInfo, string codigo, string emailRequis)
        {
            DbDataReader qryDisp = null;
            Conexao con = Conexao.Instance(emailRequis);
            try
            {
                PINController pinC = new PINController();
                string codigoCrip = pinC.CriptografaSHA256(codigo);
                
                qryDisp = con.execQuery("select * from IN_DISPOSITIVO where DS_EMAIL = '" + DispInfo.email + "' and DS_IMEI = '" + DispInfo.IMEI + "' and DS_CODIGO = '"+codigoCrip+"'");

                if (qryDisp.HasRows) return "T";
                return "F";
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!qryDisp.IsClosed) qryDisp.Close();
                con.fechaCon();
            }
        }

        private void SolicitaAprovacao(ModelDispositivo dispInfo,string emailRequis)
        {
            Conexao con = Conexao.Instance(emailRequis);
            try
            {
                string codigoLiberacao = criaCodLiberacaoAPP();
                PINController pinCont = new PINController();
                string codHexLib = pinCont.CriptografaSHA256(codigoLiberacao);
                
                float nidDispositivo = con.NextSequence("IN_DISPOSITIVO");

                string sdsConteudoEmail = montarConteudoEmailLiberacao(dispInfo, codigoLiberacao, nidDispositivo);
                ConteudoEmail contEmail = new ConteudoEmail
                {
                    titulo = "Solicitação de Liberação do Dispositivo",
                    destino = dispInfo.email,
                    isHTML = true,
                    conteudo = sdsConteudoEmail
                };

                EnviaEmailController email = new EnviaEmailController();
                if (email.EnviaEmail(contEmail))
                {                    
                    string sdsInsert = "insert into IN_DISPOSITIVO values("+nidDispositivo+",'" + dispInfo.email + "','" + dispInfo.IMEI + "','" + dispInfo.sdsDispositivo + "','" + dispInfo.sdsVersao + "','" + codHexLib + "',sysdate,null,null,null)";
                    con.ExecCommand(sdsInsert);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                con.fechaCon();
            }
        }

        private string montarConteudoEmailLiberacao(ModelDispositivo dispInfo, string codigoLiberacao, float nidDispositivo)
        {
            string sdsLinkCancela = System.Configuration.ConfigurationManager.AppSettings["ida:Audience"];
            sdsLinkCancela = sdsLinkCancela + "site/CancelaSolicAPP?ID=" + nidDispositivo;

            string idCodigo = "IMEI";
            if (dispInfo.IMEI.Length > 20) idCodigo = "ID Apple";

            string conteudo = @"<html>
	<head>
		<style> 
			.divSombra {
				margin : 20 ;
				box-shadow: -2px 2px 15px #888888;
				padding:10px 10px 10px 10px;				
			}			
			.divBox {
				width:100px;
				margin : 30 ;
				padding:10px 10px 10px 10px;	
				background:gray;
				color:white;
				font-size:18pt;
				text-align:center;
			}			
			.Cancela{
				margin : 50 0 0 0 ;
				color:red;
				font-size:10pt;
			}			
			.titulo {font-family: Arial; font-size:13pt;}
			.text { font-family: Arial; font-size:11pt;}
			.topico {font-family: Arial; font-size:11pt;}
		</style>
	</head>	
	<div class='divSombra'>
		<img src='http://romagnole.com.br/img/logo.png'>
	</div>	
	<div class='divSombra'>	
		<b class='titulo'>Olá,</b>
		<p class='text'>
			Foi realizada uma nova solicitação de liberação para o dispositivo:
		</p>		
		<ul>
			<li class='text'><b class='topico'>"+idCodigo+@":</b> "+dispInfo.IMEI+ @" </li>
			<li class='text'><b class='topico'>Dispositivo:</b> " + dispInfo.sdsDispositivo + @" </li>
			<li class='text'><b class='topico'>Versão S.O.:</b> " + dispInfo.sdsVersao + @" </li>
		</ul>			
		<p class='text'>
			Informe o número abaixo no APP para realizar a liberação:
		</p>		
		<div class='divBox'>
			<b>"+ codigoLiberacao + @"</b>
		</div>		
		<p class='Cancela'>
			Não foi você que soliciou a liberação? <a class='Cancela' href='" + sdsLinkCancela + @"'> <b>Cancele aqui!</b></a>
		</p>
	</div>
</html>";

            return conteudo;
        }

        private string criaCodLiberacaoAPP()
        {
            int tamanho = 5;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
