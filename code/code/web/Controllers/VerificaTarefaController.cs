using Oracle.DataAccess.Client;
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
    [RoutePrefix("API/Tarefa")]
    public class VerificaTarefaController : ApiController
    {
        TarefaUNIFACE tarefa = new TarefaUNIFACE();

        [HttpGet]
        [Route("RetornaTarefa")]
        public TarefaUNIFACE RetornaTarefa(TarefaUNIFACE _tarefa)
        {
            OracleDataReader qryTarefa = null;
            OracleDataReader qryRetorno = null;
            Conexao con = Conexao.Instance(_tarefa.DS_EMAIL);
            try
            {                
                qryTarefa = con.execQueryOracle("select * from IN_TAREFAAPP where ID_TAREFAAPP = "+ _tarefa.ID_TAREFAAPP);

                if (qryTarefa.HasRows)
                {
                    qryTarefa.Read();

                    tarefa.ID_TAREFAAPP = tarefa.ID_TAREFAAPP;
                    tarefa.DS_EMAIL = qryTarefa.GetString(qryTarefa.GetOrdinal("DS_EMAIL"));
                    tarefa.CD_SISTEMA = qryTarefa.GetString(qryTarefa.GetOrdinal("CD_SISTEMA"));
                    tarefa.CD_TAREFA = qryTarefa.GetString(qryTarefa.GetOrdinal("CD_TAREFA"));
                    tarefa.CD_CHAVE = qryTarefa.GetString(qryTarefa.GetOrdinal("CD_CHAVE"));
                    tarefa.FL_STATUS = Convert.ToInt16(qryTarefa.GetValue(qryTarefa.GetOrdinal("FL_STATUS")));

                    if(!qryTarefa.IsDBNull(qryTarefa.GetOrdinal("DT_RETORNO")))
                        tarefa.DT_RETORNO = qryTarefa.GetDateTime(qryTarefa.GetOrdinal("DT_RETORNO"));

                    qryRetorno = con.execQueryOracle("select * from IN_TAREFAAPPCLOB where ID_TAREFAAPP = " + _tarefa.ID_TAREFAAPP);
                    if (qryRetorno.HasRows)
                    {
                        qryRetorno.Read();
                        Oracle.DataAccess.Types.OracleClob retorno = null;

                        if (!qryRetorno.IsDBNull(qryRetorno.GetOrdinal("DS_RETORNO")))
                        {
                            retorno = qryRetorno.GetOracleClob(qryRetorno.GetOrdinal("DS_RETORNO"));
                            tarefa.DS_RETORNO = retorno.Value.ToString();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }finally
            {
                if (!qryTarefa.IsClosed) qryTarefa.Close();
                if (!qryRetorno.IsClosed) qryRetorno.Close();
                con.fechaCon();
            }
            
            return tarefa;
        }

        internal void ReiniciaTarefa(TarefaUNIFACE _tarefa)
        {
            Conexao con = Conexao.Instance(_tarefa.DS_EMAIL);
            try
            {                
                con.ExecCommand("update IN_TAREFAAPP set FL_STATUS = 1 where ID_TAREFAAPP = " + _tarefa.ID_TAREFAAPP);
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
    }
}
