using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace VerificaEstadoAMI
{
    public partial class frmPrincipal : Form
    {
        private string userAPI = "TI_Corte_Reconexion";//ConfigurationManager.AppSettings["userAPI"].ToString();
        private string passAPI = "Eeh_2018"; // ConfigurationManager.AppSettings["passAPI"].ToString();

        public frmPrincipal()
        {
            InitializeComponent();
            //tGeneral.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["Timer"]);
        }

        public void AddText(string mensaje)
        {
            try
            {
                txtLog.Text += DateTime.Now.ToString("dd/MM hh:mm:ss") + ": " + mensaje + System.Environment.NewLine;
            }
            catch (Exception ex)
            {
            }
        }

        public void log(string mensaje)
        {
            try
            {
                string fecha = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                Stream strStreamW;
                StreamWriter strStreamWriter;
                string carpeta = Application.StartupPath + @"\Log\";
                string FilePath = carpeta + "Errores_" + DateTime.Today.ToString("dd_MM_yyyy") + ".txt";

                if (File.Exists(FilePath))
                {
                    strStreamWriter = File.AppendText(FilePath);
                }
                else
                {
                    strStreamW = File.OpenWrite(FilePath);
                    strStreamWriter = new StreamWriter(strStreamW, System.Text.Encoding.UTF8);
                }

                strStreamWriter.WriteLine(fecha + " - " + mensaje);
                strStreamWriter.Close();
                strStreamWriter.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {

        }

        private void tGeneral_Tick_1(object sender, EventArgs e)
        {
            DateTime fechaInicial = DateTime.Now;
            DateTime fechaFinal;

            try
            {
                clConexion clC = new clConexion();
                DataTable dt = clC.obtenerDataTable("exec sp_gm_medidores");

                fechaFinal = DateTime.Now;
                dt.PrimaryKey = new DataColumn[] { dt.Columns["OS"] };

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            string vEstado = ObtenerEstado(dr["METERNAME"].ToString());

                            if (dr["estado"].ToString().Equals(vEstado))
                            {
                                clC.InsertarBitacora(dr["OS"].ToString(), dr["CLAVE"].ToString(), dr["METERNAME"].ToString(), dr["Schedule"].ToString(), "1", "Solicitud Procesada Correctamente", "appsoeeh", fechaInicial.ToString("yyyyMMdd hh:mm:ss"), DateTime.Now.ToString("yyyyMMdd hh:mm:ss"));
                            }

                            AddText($"{dr["CLAVE"]} - {dr["OS"]} - {dr["METERNAME"]} - {vEstado}");
                        }
                        catch (Exception ex)
                        {
                            AddText(ex.Message);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                AddText(ex.Message);
            }
            
            tGeneral.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["Timer"]);
        }

        private string ObtenerEstado(string medidor)
        {
            string vEstado = "";
            try
            {
                CD_ServerSoap.CD_Server vCD = new CD_ServerSoap.CD_Server();
                vCD.MultiSpeakMsgHeaderValue = new CD_ServerSoap.MultiSpeakMsgHeader() { UserID = userAPI, Pwd = passAPI };
                CD_ServerSoap.loadActionCode vResponseState = vCD.GetCDMeterState(medidor);
                vEstado = vResponseState.ToString();
            }
            catch (Exception) { }
            return vEstado;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
        }
    }
}