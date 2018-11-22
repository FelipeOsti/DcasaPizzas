using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models
{
    public class Conexao
    {
        //string connString = "Database=solari;Server=solari.mysql.database.azure.com;Uid=solari@solari;Password=feta!25478";
        string SqlConnString = @"Server=tcp:solariweb.database.windows.net,1433;Initial Catalog=solari;Persist Security Info=False;User ID=solari;Password=fe!25478;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //List<MySqlConnection> connection = null;
        List<SqlConnection> SqlConn = null;

        public Conexao()
        {
            //connection = new List<MySqlConnection>();
            SqlConn = new List<SqlConnection>();
        }

        private void IniciarDBConnection()
        {
            //connection.Add(new MySqlConnection(connString));
            //connection[connection.Count-1].Open();

            SqlConn.Add(new SqlConnection(SqlConnString));
            SqlConn[SqlConn.Count - 1].Open();
        }

        public void FechaConexao()
        {
            //foreach (var conn in connection)
            //{
            //    conn.Close();
            //    conn.Dispose();
            //}
            //connection.Clear();

            foreach (var conn in SqlConn)
            {
                conn.Close();
                conn.Dispose();
            }
            SqlConn.Clear();
        }


        public SqlDataReader ExecQuery(string sdsSql)
        {
            try
            {
                IniciarDBConnection();
                //MySqlCommand cmd = new MySqlCommand();
                //cmd.Connection = connection[connection.Count - 1];
                //cmd.CommandText = sdsSql;
                //cmd.Prepare();

                //return cmd.ExecuteReader();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = SqlConn[SqlConn.Count - 1];
                cmd.CommandText = sdsSql;
                cmd.Prepare();

                return cmd.ExecuteReader();

            }
            catch (Exception ex)
            {
                Erro.GerarErro(ex.Source + " - " + ex.Message, "Executar Query SqlServer");
                throw;
            }
        }

        internal long UltimoID(string v1, string v2)
        {
            SqlDataReader q = null;
            try
            {
                long nid = 0;

                q = ExecQuery("select max(" + v2 + ") as max from solari." + v1);
                q.Read();
                nid = Convert.ToInt64(q["max"]) + 1;

                q.Close();
                FechaUltimaConexao();
                return nid;               
            }
            catch (Exception ex)
            {
                Erro.GerarErro(ex.Source + " - " + ex.Message, "Último ID SqlServer");
                throw;
            }
        }

        public bool ExecCommand(string sdsSql)
        {
            /* using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = sdsSql;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    Erro.GerarErro(ex.Source + " - " + ex.Message, "Executar Query MySQL");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }*/
            using (SqlConnection connection = new SqlConnection(SqlConnString))
            {
                connection.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = sdsSql;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    Erro.GerarErro(ex.Source + " - " + ex.Message, "Executar Query SqlServer");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        internal void FechaUltimaConexao()
        {
            //var conn = connection[connection.Count - 1];
            //conn.Close();
            //conn.Dispose();
            //connection.Remove(conn);

            var connSql = SqlConn[SqlConn.Count - 1];
            connSql.Close();
            connSql.Dispose();
            SqlConn.Remove(connSql);
        }
    }
}