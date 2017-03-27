using System.Windows;
using System.Data;
using System.Data.SQLite;
using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace ShootingCompetition
{
    public enum WindowOpenMode { Insert, Update }

    public partial class MainWindow : Window
    {
        DBManager dBManager;

        public MainWindow()
        {
            InitializeComponent();
            dBManager = new DBManager();
            initDataGrids();
        }

        void initDataGrids()
        {
            dataGridPlaces.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectPlaces"], "Places");
            dataGridClubs.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectClubs"], "Clubs");
            dataGridRanges.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectRanges"], "Ranges");
            dataGridPCompetitions.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectPCompetitions"], "PCompetitions");
            dataGridShooters.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectShooters"], "Shooters");
        }

        public void ReloadSource() {
            initDataGrids();
        }

        // Show precise shooting window
        private void MenuItemPrecise_Click(object sender, RoutedEventArgs e)
        {
            WindowPrecise window = new WindowPrecise(this, dBManager);
            window.Show();
        }

        // Exit application
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Show dialog and insert row in to DB 
        private void Button_ClickInsert(object sender, RoutedEventArgs e)
        {
            Window window = null;
            switch (tabControlRoot.SelectedIndex)
            {
                case 0:
                    window = new WindowShooters(WindowOpenMode.Insert, dBManager, dataGridShooters, dataGridClubs);
                    break;
                case 1:
                    window = new WindowCompetitions(WindowOpenMode.Insert, dBManager, dataGridPCompetitions, dataGridRanges);
                    break;
                case 2:
                    window = new WindowRanges(WindowOpenMode.Insert, dBManager, dataGridRanges, dataGridPlaces);
                    break;
                case 3:
                    window = new WindowClubs(WindowOpenMode.Insert, dBManager, dataGridClubs, dataGridPlaces);
                    break;
                case 4:
                    window = new WindowPlaces(WindowOpenMode.Insert, dBManager, dataGridPlaces);
                    break;
            }
            window.ShowDialog();
            window = null;
        }

        // Show dialog and update row in DB 
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            Window window = null;
            try
            {
                switch (tabControlRoot.SelectedIndex)
                {
                    case 0:
                        window = new WindowShooters(WindowOpenMode.Update, dBManager, dataGridShooters, dataGridClubs);
                        break;
                    case 1:
                        window = new WindowCompetitions(WindowOpenMode.Update, dBManager, dataGridPCompetitions, dataGridRanges);
                        break;
                    case 2:
                        window = new WindowRanges(WindowOpenMode.Update, dBManager, dataGridRanges, dataGridPlaces);
                        break;
                    case 3:
                        window = new WindowClubs(WindowOpenMode.Update, dBManager, dataGridClubs, dataGridPlaces);
                        break;
                    case 4:
                        window = new WindowPlaces(WindowOpenMode.Update, dBManager, dataGridPlaces);
                        break;
                }
                window.ShowDialog();
                window = null;
            }
            catch (System.NullReferenceException)
            {
                // System.NullReferenceException ex
                // \n\n + ex.ToString() omitted
                System.Windows.MessageBox.Show("PICK A ROW THAT YOU WANT TO UPDATE!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Delete button that selects the right delete sql statement
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView view;
            try
            {
                switch (tabControlRoot.SelectedIndex)
                {
                    case 0:
                        view = (DataRowView)dataGridShooters.SelectedItem;
                        DeleteRecord((long)view[0], dBManager.sqlLibrary["deleteShooter"]);
                        dataGridShooters.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectShooters"], "Shooters");
                        break;
                    case 1:
                        view = (DataRowView)dataGridPCompetitions.SelectedItem;
                        DeleteShots((long)view[0]); //Delete shots first
                        DeleteRecord((long)view[0], dBManager.sqlLibrary["deletePCompetition"]);
                        DeleteRecord((long)view[0], dBManager.sqlLibrary["deletePResults"]);                        
                        dataGridPCompetitions.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectPCompetitions"], "Competitions");
                        break;
                    case 2:
                        view = (DataRowView)dataGridRanges.SelectedItem;
                        DeleteRecord((long)view[0], dBManager.sqlLibrary["deleteRange"]);
                        dataGridRanges.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectRanges"], "Ranges");
                        break;
                    case 3:
                        view = (DataRowView)dataGridClubs.SelectedItem;
                        DeleteRecord((long)view[0], dBManager.sqlLibrary["deleteClub"]);
                        dataGridClubs.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectClubs"], "Clubs");
                        break;
                    case 4:
                        view = (DataRowView)dataGridPlaces.SelectedItem;
                        DeleteRecord((long)view[0], dBManager.sqlLibrary["deletePlace"]);
                        dataGridPlaces.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectPlaces"], "Places");
                        break;
                }
            }
            catch (System.NullReferenceException)
            {
                // (System.NullReferenceException ex)
                // \n\n" + ex.ToString() omitted
                System.Windows.MessageBox.Show("PICK A ROW THAT YOU WANT TO DELETE!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Delete record by id
        void DeleteRecord(long id, string deleteSQL)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = deleteSQL;
                command.Parameters.Add(new SQLiteParameter("id", id));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Reset serch
        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            dataGridShooters.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectShooters"], "Shooters");
            textBoxId.Text = "";
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            textBoxClub.Text = "";
        }

        // Serch by id, name, surname, club (other part of sql is in DBManager)
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT sh.id, sh.name, sh.surname, sh.clubid, cl.cname, sh.team, sh.note FROM Shooters sh LEFT OUTER JOIN Clubs cl ON sh.clubid=cl.id WHERE";

            long temp;
            if (textBoxId.Text != "" && long.TryParse(textBoxId.Text, out temp))
            {
                sql += " sh.id=" + textBoxId.Text;
            }
            else
            {
                sql += " 1=1";
            }

            if (textBoxName.Text != "")
            {
                sql += " AND name LIKE '" + textBoxName.Text + "'";
            }
            else
            {
                sql += " AND 1=1";
            }

            if (textBoxSurname.Text != "")
            {
                sql += " AND surname LIKE '" + textBoxSurname.Text + "'";
            }
            else
            {
                sql += " AND 1=1";
            }

            if (textBoxClub.Text != "")
            {
                sql += " AND cl.cname LIKE '" + textBoxClub.Text + "'";
            }
            else
            {
                sql += " AND 1=1";
            }

            if (textBoxTeam.Text != "")
            {
                sql += " AND sh.team LIKE '" + textBoxTeam.Text + "'";
            }
            else
            {
                sql += " AND 1=1";
            }

            // End of statement
            sql += ";";
            dataGridShooters.ItemsSource = dBManager.GetView(sql, "Shooters");
        }

        void DeleteShots(long sid)
        {
            List<long> ids = new List<long>();
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["selectPShots"];
                command.Parameters.Add(new SQLiteParameter("sid", sid));
                conn.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add(reader.GetInt64(0));
                    }
                }
            }
            foreach (long id in ids)
            {
                DeleteRecord(id, dBManager.sqlLibrary["deletePShots"]);
            }
        }
    }
}

