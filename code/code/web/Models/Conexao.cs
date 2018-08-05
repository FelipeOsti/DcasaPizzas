using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using System.Data.Common;
using System.Threading;

namespace WebAppRoma.Models
{
    public class Conexao
    {
        private String sdsUsuar = "autopro";
        private String sdsSenha = "aut0pr0#";
        private OracleConnection conn = null;
        private bool bboRole = false;

        static Conexao _instance = null;

        private Conexao()
        {
            bboRole = false;
            conn = new OracleConnection();
        }

        public void fechaCon()
        {
            try
            {
                conn.Close();
                conn.Dispose();
                _instance = null;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public static Conexao Instance(string sdsEmail)
        {
            return new Conexao();
            /*
            try
            {
                if (sdsEmail == null) sdsEmail = "procauto@romagnole.com.br";

                Conexao _conn = null;
                if (_instance != null)
                {
                    _instance.TryGetValue(sdsEmail, out _conn);
                    if (_conn != null) return _conn;
                }
                else
                {
                    _instance = new Dictionary<string, Conexao>();
                }

                _conn = new Conexao();
                _instance.Add(sdsEmail, _conn);
                return _conn;
            }
            finally
            {
                sdsEmail = "";
            }*/
            
        }

        public DbDataReader execQuery(string sdsSQL)
        {
            DbDataReader reader = null;
            OracleCommand command = null;
            OracleConnection oraconn = null;
            try
            {
                oraconn = GetDBConnection(sdsUsuar, sdsSenha);

                command = new OracleCommand(sdsSQL);
                command.Connection = oraconn;

                reader = command.ExecuteReader();

                return reader;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public OracleDataReader execQueryOracle(string sdsSQL)
        {
            OracleDataReader reader = null;
            OracleCommand command = null;
            OracleConnection oraconn = null;
            try
            {
                oraconn = GetDBConnection(sdsUsuar, sdsSenha);

                command = new OracleCommand(sdsSQL);
                command.Connection = oraconn;

                reader = command.ExecuteReader();

                return reader;
            }
            catch
            {
                throw;
            }
        }

        private OracleConnection GetDBConnection(String user, String password)
        {
            /*string connString = "Data Source=(DESCRIPTION = " +
                                        "(ADDRESS_LIST = " +
                                          "(ADDRESS = (PROTOCOL = TCP)(HOST = guepardo1-vip)(PORT = 1521)) " +
                                          "(ADDRESS = (PROTOCOL = TCP)(HOST = guepardo2-vip)(PORT = 1521)) " +
                                          "(LOAD_BALANCE = yes) " +
                                          "(FAILOVER = true)) " +
                                        "(CONNECT_DATA = (SERVICE_NAME = ROMA)) " +
                                        "); Password =" + password + ";User ID=" + user;*/
            
            string connString = "Data Source=(DESCRIPTION = "
            + "(ADDRESS_LIST = "
                + "(ADDRESS = (PROTOCOL = TCP)(HOST = jaguar)(PORT = 1521))) "
            + " (CONNECT_DATA = "
            + " (SERVICE_NAME = PRODDEV))); Password = " + password + "; User ID = " + user;

            if ((conn.State == System.Data.ConnectionState.Broken) ||
                (conn.State == System.Data.ConnectionState.Closed))
            {
                try
                {
                    if (conn.State == System.Data.ConnectionState.Broken) conn.Close();

                    conn.ConnectionString = connString;
                    conn.Open();

                    OracleCommand command = new OracleCommand();
                    command.Connection = conn;
                    command.CommandText = "set role role_objetos identified by kx18747";
                    command.ExecuteNonQuery();
                    bboRole = true;
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                while (conn.State == System.Data.ConnectionState.Executing ||
                       conn.State == System.Data.ConnectionState.Fetching ||
                       conn.State == System.Data.ConnectionState.Connecting || 
                       !bboRole)
                {
                    Thread.Sleep(100);
                }
            }
            return conn;
        }      

        public float NextSequence(string sdsTabela)
        {
            DbDataReader reader = null;
            OracleCommand command = null;
            OracleConnection oraconn = null;
            try
            {
                oraconn = GetDBConnection(sdsUsuar, sdsSenha);

                command = new OracleCommand("select S" + sdsTabela + ".nextval as ID from dual");
                command.Connection = oraconn;

                reader = command.ExecuteReader();

                reader.Read();

                float nID = Convert.ToInt64(reader.GetValue(reader.GetOrdinal("ID")));

                return nID;
            }
            catch
            {
                throw;
            }
            finally
            {
                reader.Close();
            }

        }

        public Boolean ExecCommand(string sdsSQL)
        {
            OracleConnection oraconn = GetDBConnection(sdsUsuar, sdsSenha);
            OracleCommand command = null;
            try
            {
                command = new OracleCommand();
                command.Connection = oraconn;
                command.CommandText = sdsSQL;

                command.ExecuteNonQuery();

                return true;
            }
            catch
            {
                throw;
            }            
        }
    }
}
 