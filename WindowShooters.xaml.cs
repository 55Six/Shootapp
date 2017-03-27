using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;

namespace ShootingCompetition
{
    public partial class WindowShooters : Window
    {
        WindowOpenMode mode;
        DBManager dBManager;
        DataGrid dataGridShooters;
        DataGrid dataGridClubs;
        long id;

        public WindowShooters(WindowOpenMode mode, DBManager dBManager, DataGrid dataGridShooters, DataGrid dataGridClubs)
        {
            InitializeComponent();
            this.mode = mode;
            this.dBManager = dBManager;
            this.dataGridShooters = dataGridShooters;
            this.dataGridClubs = dataGridClubs;
            initWindow();
        }

        // Sets window
        void initWindow()
        {
            comboBoxClub.ItemsSource = dataGridClubs.ItemsSource;
            comboBoxClub.DisplayMemberPath = "cname";
            comboBoxClub.SelectedValuePath = "id";
            comboBoxClub.SelectedIndex = 0;
            switch (mode)
            {
                case WindowOpenMode.Insert:
                    Title = "Insert Shooter Into Shooters database";
                    buttonAction.Content = "Insert";
                    break;
                case WindowOpenMode.Update:
                    Title = "Update Shooter In Shooters database";
                    buttonAction.Content = "Update";
                    DataRowView view = (DataRowView)dataGridShooters.SelectedItem;
                    id = (long)view[0];
                    textBoxName.Text = (string)view[1];
                    textBoxSurname.Text = (string)view[2];
                    comboBoxClub.SelectedValue = (long)view[3];
                    textBoxTeam.Text = view[5].ToString();
                    textBoxNote.Text = (string)view[6];                    
                    break;
            }
        }

        // Insert shooter into Db
        private void InsertShooterRecord(long clubid, string name, string surname, string note, string team)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["insertShooter"];
                command.Parameters.Add(new SQLiteParameter("name", name));
                command.Parameters.Add(new SQLiteParameter("surname", surname));
                command.Parameters.Add(new SQLiteParameter("note", note));
                command.Parameters.Add(new SQLiteParameter("clubid", clubid));
                command.Parameters.Add(new SQLiteParameter("team", team));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update shooter in DB
        private void UpdateShooterRecord(long clubid, string name, string surname, string note, string team)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["updateShooter"];
                command.Parameters.Add(new SQLiteParameter("name", name));
                command.Parameters.Add(new SQLiteParameter("surname", surname));
                command.Parameters.Add(new SQLiteParameter("note", note));
                command.Parameters.Add(new SQLiteParameter("clubid", clubid));
                command.Parameters.Add(new SQLiteParameter("id", id));
                command.Parameters.Add(new SQLiteParameter("team", team));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonAction_Click(object sender, RoutedEventArgs e)
        {
            // Check input
            if (textBoxName.Text == string.Empty) {
                MessageBox.Show("Name box can't be empty!", "Warning!");                
                return;
            }
            else if (textBoxSurname.Text == string.Empty)
            {
                MessageBox.Show("Surname box can't be empty!", "Warning!");
                return;
            }
            if (comboBoxClub.Text == string.Empty)
            {
                MessageBox.Show("Club combo box can't be empty!", "Warning!");
                return;
            }

            // Select mode: inserting or updating
            switch (mode)
            {
                case WindowOpenMode.Insert:
                    InsertShooterRecord((long)comboBoxClub.SelectedValue, textBoxName.Text, textBoxSurname.Text, textBoxNote.Text, textBoxTeam.Text.ToUpper());
                    break;
                case WindowOpenMode.Update:
                    UpdateShooterRecord((long)comboBoxClub.SelectedValue, textBoxName.Text, textBoxSurname.Text, textBoxNote.Text, textBoxTeam.Text.ToUpper());
                    break;
            }
            dataGridShooters.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectShooters"], "Shooters");
            Close();
        }
    }
}
