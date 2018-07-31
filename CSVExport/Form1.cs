using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CSVExport
{
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using FluentFTP;
    using Properties;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var exportResult = ExportToCsv();

            if (!exportResult) return;
            var client = new FtpClient(Settings.Default.FTPServer)
            {
                Credentials = new NetworkCredential(Settings.Default.FTPUserName,
                    Settings.Default.FTPPassword)
            };

            client.Connect();
            if (client.FileExists(Settings.Default.RemotFTPFilePath + Settings.Default.ExportFileName))
            {
                client.DeleteFile(Settings.Default.RemotFTPFilePath + Settings.Default.ExportFileName); // Remove existing file if exists
            }

            client.UploadFile(Settings.Default.ExportFileName,
                Settings.Default.RemotFTPFilePath);
        }
        //need to pass along from and to date
        private bool ExportToCsv()
        {
            var streamWriter = new StreamWriter(Settings.Default.ExportFileName);
            var query = Resources.SqlQuery;
            if (dtFromDate.Checked && dtToDate.Checked)
            {
                query = query + " AND (iHeader.TxnDate between @FromDate AND @ToDate) ";
            }

            try
            {
                var sqlCommand = new SqlCommand();
                var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
                sqlCommand.Connection = connection;
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = query;
                connection.Open();
                using (connection)
                {
                    using (var dataReader = sqlCommand.ExecuteReader())
                    using (streamWriter)
                    {
                        var dataTable = new DataTable();

                        for (var i = 0; i < dataReader.FieldCount; i++)
                        {
                            dataTable.Columns.Add(dataReader.GetName(i));
                        }

                        streamWriter.WriteLine(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                        
                        while (dataReader.Read())
                        {
                            streamWriter.WriteLine(dataReader[0] + "," + dataReader[1] + "," + dataReader[2] + "," + dataReader[3] + "," + dataReader[4] + "," +
                                                   dataReader[5] + "," + dataReader[6] + "," + dataReader[7] + "," + dataReader[8] + "," + dataReader[9] + "," +
                                                   dataReader[10] + "," + dataReader[11] + "," + dataReader[12] + "," + dataReader[13] + "," + dataReader[14] + "," +
                                                   dataReader[15] + "," + dataReader[16] + "," + dataReader[17] + "," + dataReader[18] + "," + dataReader[19] + "," +
                                                   dataReader[20] + "," + dataReader[21] + "," + dataReader[22] + "," + dataReader[23] + "," + dataReader[24] + "," +
                                                   dataReader[25] + "," + dataReader[26] + "," + dataReader[27] + "," + dataReader[28] + "," + dataReader[29] + "," +
                                                   dataReader[30] + "," + dataReader[31] + "," + dataReader[32] + "," + dataReader[33] + "," + dataReader[34] + "," +
                                                   dataReader[35] + "," + dataReader[36] + "," + dataReader[37] + "," + dataReader[38] + "," + dataReader[39] + "," +
                                                   dataReader[40] + "," + dataReader[41] + "," + dataReader[42] + "," + dataReader[43] + "," + dataReader[44] + "," +
                                                   dataReader[45] + "," + dataReader[46] + "," + dataReader[47] + "," + dataReader[48] + "," + dataReader[49] + "," +
                                                   dataReader[50] + "," + dataReader[51] + "," + dataReader[52] + "," + dataReader[53] + "," + dataReader[54] + "," +
                                                   dataReader[55] + "," + dataReader[56] + "," + dataReader[57] + "," + dataReader[58] + "," + dataReader[59] + "," +
                                                   dataReader[60] + "," + dataReader[61] + "," + dataReader[62] + "," + dataReader[63] + "," + dataReader[64] + "," +
                                                   dataReader[65] + "," + dataReader[66] + "," + dataReader[67] + "," + dataReader[68] + "," + dataReader[69] + "," +
                                                   dataReader[70] + "," + dataReader[71] + "," + dataReader[72] + "," + dataReader[73] + "," + dataReader[74] + "," +
                                                   dataReader[75] + "," + dataReader[76] + "," + dataReader[77] + "," + dataReader[78] + "," + dataReader[79] + "," +
                                                   dataReader[80] + "," + dataReader[81] + "," + dataReader[82] + "," + dataReader[83] + "," + dataReader[84] + "," +
                                                   dataReader[85] + "," + dataReader[86] + "," + dataReader[87] + "," + dataReader[88] + "," + dataReader[89] + "," +
                                                   dataReader[90] + "," + dataReader[91] + "," + dataReader[92] + "," + dataReader[93] + "," + dataReader[94] + "," +
                                                   dataReader[95] + "," + dataReader[96] + "," + dataReader[97] + "," + dataReader[98] + "," + dataReader[99] + "," +
                                                   dataReader[100] + "," + dataReader[101] + "," + dataReader[102] + "," + dataReader[103] + "," + dataReader[104] + "," +
                                                   dataReader[105] + "," + dataReader[106] + "," + dataReader[107] + "," );
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
       
    }
}
