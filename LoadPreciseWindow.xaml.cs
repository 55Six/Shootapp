using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Collections.Generic;

namespace ShootingCompetition
{
    /// <summary>
    /// Interaction logic for LoadPreciseWindow.xaml
    /// </summary>
    public partial class LoadPreciseWindow : Window
    {
        DBManager dBManager;
        WindowPrecise windowP;
        DataView view;
        public bool Created { get; set; }
        private bool finished;

        public LoadPreciseWindow(DBManager dBManager, WindowPrecise windowP)
        {
            InitializeComponent();
            this.dBManager = dBManager;
            this.windowP = windowP;
            Created = false;
            view = dBManager.GetView(dBManager.sqlLibrary["selectComBox"], "ComBox");
            comboBoxCompetitions.ItemsSource = view;
            comboBoxCompetitions.SelectedValuePath = "id";
            comboBoxCompetitions.DisplayMemberPath = "title";
            comboBoxCompetitions.SelectedIndex = 0;
        }

        private void comboBoxCompetitions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTable data = view.ToTable();
            DataRow drow = data.Select("id=" + comboBoxCompetitions.SelectedValue.ToString())[0];
            labelId.Content = drow["id"];
            labelTitle.Content = drow["title"];
            labelRange.Content = drow["rname"].ToString() + " " + drow["placeid"].ToString() + " " + drow["pname"].ToString();
            labelDate.Content = drow["date"];
            labelType.Content = drow["tartype"];
            labelShots.Content = drow["shots"];
            labelRounds.Content = drow["rounds"];
            labelFinished.Content = drow["finished"];
            finished = (bool)drow["finished"];
        }

        // Prevent false input
        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            // Check input
            if (comboBoxCompetitions.Text == string.Empty)
            {
                MessageBox.Show("competition combo box can't be empty!", "Warning!");
                return;
            }

            //if (competition finished) create only printing text and set to finished
            if (finished)
            {
                LoadFromDB();
                Close();
            }
            else
            {
                windowP.Competition = new PreciseShooting(long.Parse(labelId.Content.ToString()), labelTitle.Content.ToString(), labelRange.Content.ToString(), DateTime.Parse(labelDate.Content.ToString()), int.Parse(labelType.Content.ToString()), int.Parse(labelShots.Content.ToString()), int.Parse(labelRounds.Content.ToString()));
                Created = true;
            }
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // IMPLEMENTACIJA LOADANJA IZ DB
        public void LoadFromDB()
        {
            PreciseShooting competition = new PreciseShooting(long.Parse(labelId.Content.ToString()), labelTitle.Content.ToString(), labelRange.Content.ToString(), DateTime.Parse(labelDate.Content.ToString()), int.Parse(labelType.Content.ToString()), int.Parse(labelShots.Content.ToString()), int.Parse(labelRounds.Content.ToString()));
            Shooter cshooter;

            if (competition == null)
            {
                return;
            }

            try
            {
                using (SQLiteConnection conn = dBManager.GetConnection())
                {
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = dBManager.sqlLibrary["selectForStatistics"];
                    command.Parameters.Add(new SQLiteParameter("cid", competition.Id));
                    conn.Open();


                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cshooter = new Shooter(long.Parse(reader["id"].ToString()), reader["name"].ToString(), reader["surname"].ToString(), reader["cname"].ToString(), reader["team"].ToString());

                            List<int?> nullhits = new List<int?>();

                            nullhits.Add(reader.GetInt32(6));
                            nullhits.Add(reader.GetInt32(7));
                            nullhits.Add(reader.GetInt32(8));
                            nullhits.Add(reader.GetInt32(9));
                            nullhits.Add(reader.GetInt32(10));
                            nullhits.Add(reader.GetInt32(11));
                            nullhits.Add(reader.GetInt32(12));
                            nullhits.Add(reader.GetInt32(13));
                            nullhits.Add(reader.GetInt32(14));
                            nullhits.Add(reader.GetInt32(15));
                            nullhits.Add(reader.GetInt32(16));
                            nullhits.Add(reader.GetInt32(17));

                            List<int> hits = new List<int>();
                            // Fill non null
                            foreach (int? hit in nullhits)
                            {
                                if (hit != null)
                                {
                                    hits.Add((int)hit);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // If shoots length discreptency break
                            if (hits.Count != 13 - competition.TarType)
                            {
                                MessageBox.Show("Data corupted!", "Error!");
                                return;
                            }
                            cshooter.ScoresPRound.Add(hits);

                            //Fill remaining hits with lists filled with zero
                            for (int i = 1; i < competition.Round; i++)
                            {
                                cshooter.ScoresPRound.Add(new List<int>(new int[13 - competition.TarType]));
                            }
                            // Add Constructed shooter
                            competition.Shooters.Add(cshooter);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception");
            }
            competition.EndCompetition();
            WindowStatistics statWin = new WindowStatistics(competition);
            statWin.Show();
        }
    }
}
