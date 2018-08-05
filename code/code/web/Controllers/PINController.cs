using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Security.Cryptography;
using System.Text;
using System.Data.Common;
using WebAppRoma.Models;
using System.Web.Http.Description;

namespace WebAppRoma.Controllers
{
    [Authorize]
    [RoutePrefix("API/pin")]
    public class PINController : ApiController
    {
        [HttpGet]
        [Route("CriptografaSHA256")]
        public string CriptografaSHA256(string pin)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(pin), 0, Encoding.UTF8.GetByteCount(pin));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            var retorno = hash.ToString();
            return retorno.TrimStart('"').TrimEnd('"');
        }

        [HttpGet]
        [Route("VerificaPIN")]
        public bool VerificaPIN(string pin, string email)
        {
            var PinCorreto = GetPINEmail(email);
            if(pin == PinCorreto) return true;
            return false;
        }

        [HttpGet]
        [Route("GetPINEmail")]
        public string GetPINEmail(string email)
        {
            Conexao clsConn = Conexao.Instance(email);
            DbDataReader qryPin = null;
            try
            {                
                String sdsSQL = "select * from IN_PINEMAIL where DT_INATIVACAO is null and DS_EMAIL = '"+email+"'";
                qryPin = clsConn.execQuery(sdsSQL);

                if (qryPin.HasRows)
                {
                    return qryPin.GetString(qryPin.GetOrdinal("NR_PIN"));
                }
                else return null; //Nenhum PIN Cadastrado
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!qryPin.IsClosed) qryPin.Close();
                clsConn.fechaCon();
            }
        } 

        [HttpPost]
        [Route("SalvarPin")]
        [ResponseType(typeof(PIN))]
        public bool SalvarPin(PIN content)
        {
            Conexao clsConn = Conexao.Instance(content.DS_EMAIL);

            try
            {
                var pin = content.DS_PIN.TrimStart('"').TrimEnd('"'); ;
                var email = content.DS_EMAIL;

                if(pin == null || email == null)
                    return false;

                var _pin = GetPINEmail(email);
                if(_pin != null)
                {
                    String sdsSQL = "update IN_PINEMAIL set NR_PIN = '" + pin + "' where DT_INATIVACAO is null and DS_EMAIL = '" + email+"'";
                    return clsConn.ExecCommand(sdsSQL);
                }
                else
                {
                    String sdsSQL = "insert into IN_PINEMAIL (select SIN_PINEMAIL.nextval,'" + email + "','" + pin + "',null,null from dual)";
                    return clsConn.ExecCommand(sdsSQL);
                }
            }
            catch { throw; }
            finally
            {
                clsConn.fechaCon();
            }
        }
    }

    public class PIN
    {
        public string DS_PIN { get; set; }
        public string DS_EMAIL { get; set; }
    }
}
