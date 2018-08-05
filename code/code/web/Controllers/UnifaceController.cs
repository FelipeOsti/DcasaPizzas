using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppRoma.Models;
using System.Threading;

namespace WebAppRoma.Controllers
{
    [Authorize]
    [RoutePrefix("API/Uniface")]
    public class UnifaceController : ApiController
    {
        [HttpPost]
        [Route("startSige")]
        public TarefaUNIFACE startSIGE(TarefaUNIFACE tarefa)
        {
            try
            {
                float nidTarefa = SalvaTarefa(tarefa);
                tarefa.ID_TAREFAAPP = nidTarefa;
                TarefaUNIFACE tarefaAtt = new TarefaUNIFACE();
                VerificaTarefaController verifica = new VerificaTarefaController();

                int contErro = 0;
                bool bboSegundaVez = false;
                do
                {
                    Thread.Sleep(500); //Meio Segundo para cada verificação                   
                    tarefaAtt = verifica.RetornaTarefa(tarefa);
                    
                    contErro++;
                    if (contErro > 60 && !bboSegundaVez) //30s
                    {
                        verifica.ReiniciaTarefa(tarefaAtt); //30s
                        bboSegundaVez = true;
                        contErro = 0;
                    }else if(bboSegundaVez && contErro > 60)
                    {
                        tarefaAtt.DS_RETORNO = "Tempo de resposta excedido!";
                        break;
                    }

                } while (tarefaAtt.FL_STATUS <= 2);


                return tarefaAtt;               
            }
            catch
            {
                throw;
            }
        }

        private float SalvaTarefa(TarefaUNIFACE tarefa)
        {
            Conexao con = Conexao.Instance(tarefa.DS_EMAIL);
            try
            {                
                float nidTarefa = con.NextSequence("IN_TAREFAAPP");
                string sdsCommand = "insert into IN_TAREFAAPP values(" + nidTarefa.ToString() + ",'" + tarefa.DS_EMAIL + "','" + tarefa.CD_SISTEMA + "','" + tarefa.CD_TAREFA + "','" + tarefa.CD_CHAVE + "',1,null)";
                con.ExecCommand(sdsCommand);
                return nidTarefa;
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
