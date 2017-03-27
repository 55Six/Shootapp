using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Data;

namespace ShootingCompetition
{
    public partial class WindowClubs : Window
    {
        WindowOpenMode mode;
        DBManager dBManager;
        DataGrid dataGridClubs;
        DataGrid dataGridPlaces;
        long id;

        public WindowClubs(WindowOpenMode mode, DBManager dBManager, DataGrid dataGridClubs, DataGrid dataGridPlaces)
        {
            InitializeComponent();
            this.mode = mode;
            this.dBManager = dBManager;
            this.dataGridClubs = dataGridClubs;
            this.dataGridPlaces = dataGridPlaces;
            initWindow();
        }

        void initWindow()
        {
            comboBoxPlace.ItemsSource = dataGridPlaces.ItemsSource;
            comboBoxPlace.DisplayMemberPath = "pname";
            comboBoxPlace.SelectedValuePath = "id";
            comboBoxPlace.SelectedIndex = 0;
            
            switch (mode)
            {
                case WindowOpenMode.Insert:
                    Title = "Insert Club Into Database";
                    buttonAction.Content = "Insert";
                    break;
                case WindowOpenMode.Update:
                    Title = "Update Club In Database";
                    buttonAction.Content = "Update";
                    DataRowView view = (DataRowView)dataGridClubs.SelectedItem;
                    id = (long)view[0];
                    textBoxClub.Text = view[1].ToString();
                    comboBoxPlace.SelectedValue = (long)view[2];
                    textBoxZip.Text = view[2].ToString();                  
                    break;
            }
        }

        // Insert or update club
        private void buttonAction_Click(object sender, RoutedEventArgs e)
        {
            // Input check
            if (textBoxClub.Text == string.Empty)
            {
                MessageBox.Show("Club box can't be empty!", "Warning!");
                return;
            }
            else if (comboBoxPlace.Text == string.Empty) {
                MessageBox.Show("Place combo box can't be empty!", "Warning!");
                return;
            }

            // Select mode: inserting or updating
            switch (mode)
            {
                case WindowOpenMode.Insert:
                    InsertClubRecord((long)comboBoxPlace.SelectedValue, textBoxClub.Text);
                    break;
                case WindowOpenMode.Update:
                    UpdateClubRecord((long)comboBoxPlace.SelectedValue, textBoxClub.Text);
                    break;
            }
            dataGridClubs.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectClubs"], "Clubs");
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Insert Club record
        public void InsertClubRecord(long placeid, string cname)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["insertClub"];
                command.Parameters.Add(new SQLiteParameter("cname", cname));
                command.Parameters.Add(new SQLiteParameter("placeid", placeid));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update Club record
        public void UpdateClubRecord(long placeid, string cname)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["updateClub"];
                command.Parameters.Add(new SQLiteParameter("cname", cname));
                command.Parameters.Add(new SQLiteParameter("placeid", placeid));
                command.Parameters.Add(new SQLiteParameter("id", id));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Change zip on combobox change
        private void comboBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBoxZip.Text = comboBoxPlace.SelectedValue.ToString();
        }
    }
}
