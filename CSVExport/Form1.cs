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

        private bool ExportToCsv()
        {
            var streamWriter = new StreamWriter(Settings.Default.ExportFileName);
            var query = Resources.SqlQuery;
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
                            streamWriter.WriteLine(dataReader[0] + "," + dataReader[1] + "," + dataReader[2] + "," + dataReader[3] + "," + dataReader[4] + ",");
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
