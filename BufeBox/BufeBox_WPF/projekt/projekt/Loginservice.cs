using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace projekt
{
    public class LoginService
    {
        public string connectionString;

        public LoginService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string Belep(string felhasznaloNev, string jelszo)
        {
            string sql = $"select nev from dolgozo where felnev='{felhasznaloNev}' and jelszo=sha2('{jelszo}',256)";
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sql, con);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string nev = reader[0].ToString();
                        return (nev);
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }
    }
}

