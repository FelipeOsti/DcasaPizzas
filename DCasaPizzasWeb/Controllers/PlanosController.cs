using DCasaPizzasWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("api/planos")]
    public class PlanosController : ApiController
    {
        [Route("getPlanos")]
        public List<CM_PLANOModel> GetPlanos()
        {
            SqlDataReader qPlanos = null;
            var con = new Conexao();
            try
            {
                var retorno = new List<CM_PLANOModel>();

                qPlanos = con.ExecQuery("select * from solari.cm_plano");

                if (qPlanos.HasRows)
                {
                    while (qPlanos.Read())
                    {
                        retorno.Add(new CM_PLANOModel()
                        {
                            ID_PLANO = Convert.ToInt64(qPlanos["ID_PLANO"]),
                            DS_PLANO = (string)qPlanos["DS_PLANO"],
                            VL_PLANO = Convert.ToDouble(qPlanos["VL_PLANO"]),
                            FL_PLANO = (string)qPlanos["FL_PLANO"],
                            OB_PLANO = (string)qPlanos["OB_PLANO"]                           
                        });
                    }
                }

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (qPlanos != null)
                    if (!qPlanos.IsClosed) qPlanos.Close();
                con.FechaConexao();
            }
        }

        [Route("getPlano/{nidPlano}")]
        public CM_PLANOModel GetPlano(long nidPlano)
        {
            SqlDataReader qPlanos = null;
            var con = new Conexao();
            try
            {
                var retorno = new CM_PLANOModel();

                qPlanos = con.ExecQuery("select * from solari.cm_plano where ID_PLANO = "+nidPlano);

                if (qPlanos.HasRows)
                {
                    qPlanos.Read();
                    retorno = new CM_PLANOModel()
                    {
                        ID_PLANO = Convert.ToInt64(qPlanos["ID_PLANO"]),
                        DS_PLANO = (string)qPlanos["DS_PLANO"],
                        VL_PLANO = Convert.ToDouble(qPlanos["VL_PLANO"]),
                        FL_PLANO = (string)qPlanos["FL_PLANO"],
                        OB_PLANO = (string)qPlanos["OB_PLANO"]
                    };                    
                }

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (qPlanos != null)
                    if (!qPlanos.IsClosed) qPlanos.Close();
                con.FechaConexao();
            }
        }
    }
}
