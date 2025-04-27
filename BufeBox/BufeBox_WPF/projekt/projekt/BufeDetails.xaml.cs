using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace projekt
{
    public partial class BufeDetails : Window
    {
        private const double AFA = 0.27;  // 27% ÁFA
        private int osszeg;
        private Bufe bufe;

        // Esemény, amely jelzi, hogy a rendelés elkészült
        public event EventHandler<BufeEventArgs> OrderCompleted;
        string kapcsolat = "server=localhost:3307,;uid=root;password=;database=bufe;ssl mode=none";
        public BufeDetails(Bufe bufe)
        {
            InitializeComponent();

            // A mezők kitöltése az átvett Bufe példánnyal
            this.bufe = bufe;

            RendeloneveTextBox.Text = bufe.rendeloneve;
            MitrendeltTextBox.Text = bufe.mitrendelt;
            MegjegyzesTextBox.Text = bufe.megjegyzes;
            IdopontTextBox.Text = bufe.idopont.ToString("yyyy-MM-dd HH:mm");

            double bruttoDupla = bufe.fizet * 1.27;
            int brutto = (int)Math.Round(bruttoDupla, 0, MidpointRounding.AwayFromZero);
            int kerekitett = Kerekit5Ft(brutto);
            FizetendoTextBox.Text = kerekitett.ToString("N0") + " Ft"; // ezres tagolással

            osszeg = bufe.fizet;
        }

        private void Bezárás_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Ablak bezárása
        }

        private void RendelesElkeszult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(kapcsolat))
                {
                    con.Open();
                    string sql = "UPDATE kosar SET elkeszult = 1 WHERE kkod = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", bufe.id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a rendelés frissítésekor: " + ex.Message);
            }

            OrderCompleted?.Invoke(this, new BufeEventArgs(this.bufe));
            this.Close();
        }

        private int Kerekit5Ft(int brutto)
        {
            int maradek = brutto % 5;
            if (maradek == 0) return brutto;
            return maradek < 3 ? brutto - maradek : brutto + (5 - maradek);
        }
    }
}
