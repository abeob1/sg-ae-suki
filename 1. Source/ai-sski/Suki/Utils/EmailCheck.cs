using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace Suki.Utils
{
    public class EmailCheck
    {
        public void InsertPODetailsforEmailCheck(string poNumber, string EmailId)
        {
            string connectionString = "Persist Security Info=False;User ID=sa;Password=Pass@123;Initial Catalog=UAT_HSukiGroup;Server=192.168.1.2";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [tempEmailLog] (PONumber, EmailId) VALUES (@poNumber,@EmailId)");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@PONumber", poNumber);
                cmd.Parameters.AddWithValue("@EmailId", EmailId);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateStatusforEmailCheck(string poNumber, string status)
        {
            string connectionString = "Persist Security Info=False;User ID=sa;Password=Pass@123;Initial Catalog=UAT_HSukiGroup;Server=192.168.1.2";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [tempEmailLog] SET Status = @status where PONumber = @poNumber");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@PONumber", poNumber);
                cmd.Parameters.AddWithValue("@Status", status);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}