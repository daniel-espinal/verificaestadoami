using System;
using System.Data;
using System.Data.SqlClient;

namespace VerificaEstadoAMI
{
    internal class clConexion
    {
        private SqlConnection vConexion;
        private String sConexion;
        // CEMC 03.10.2022
        public clConexion()
        {
            sConexion = "Server=192.168.100.28;Database=eehApps;User Id=appsoe;Password=:S032017*;"; //ConfigurationManager.AppSettings["bd"].ToString();
            //sConexion = "Server=192.168.100.17;Database=eehApps;User Id=wendel.padilla;Password=123456"; //ConfigurationManager.AppSettings["bd"].ToString();
            vConexion = new SqlConnection(sConexion);
        }

        public DataTable obtenerDataTable(String vQuery)
        {
            DataTable vDatos = new DataTable();
            try
            {
                SqlDataAdapter vDataAdapter = new SqlDataAdapter(vQuery, vConexion);
                vDataAdapter.Fill(vDatos);
            }
            catch
            {
                throw;
            }

            vConexion.Close();

            return vDatos;
        }

        public void InsertarBitacora(string os, string clave, string medidor, string tarea, string cod_resultado, String msj_resultado, string cod_usuario, string fechaInicial, string fechaFinal)
        {
            SqlConnection con2 = new SqlConnection();
            con2.ConnectionString = sConexion;
            con2.Open();

            try
            {
                SqlCommand comSP = new SqlCommand("SP_RESUELVEOSAMI", con2);
                comSP.CommandType = CommandType.StoredProcedure;
                comSP.Parameters.Add("@OS", SqlDbType.Int).Value = Convert.ToInt64(os);
                comSP.Parameters.Add("@CLAVE", SqlDbType.Int).Value = Convert.ToInt64(clave);
                comSP.Parameters.Add("@MEDIDOR", SqlDbType.VarChar).Value = medidor;
                comSP.Parameters.Add("@RESULTADO", SqlDbType.Int).Value = Convert.ToInt32(cod_resultado);
                comSP.Parameters.Add("@MENSAJE", SqlDbType.VarChar).Value = msj_resultado;
                comSP.Parameters.Add("@SCHEDULE_ID", SqlDbType.VarChar).Value = tarea;
                comSP.Parameters.Add("@USUARIO", SqlDbType.VarChar).Value = cod_usuario;
                comSP.Parameters.Add("@fechaInicio", SqlDbType.VarChar, 17).Value = fechaInicial;
                comSP.Parameters.Add("@fechaFin", SqlDbType.VarChar, 17).Value = fechaFinal;
                comSP.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                con2.Close();
            }
        }
    }
}