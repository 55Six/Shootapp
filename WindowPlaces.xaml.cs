using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Data;

namespace ShootingCompetition
{
    public partial class WindowPlaces : Window
    {
        WindowOpenMode mode;
        DBManager dBManager;
        DataGrid dataGridPlaces;

        public WindowPlaces(WindowOpenMode mode, DBManager dBManager, DataGrid dataGridPlaces)
        {
            InitializeComponent();
            this.mode = mode;
            this.dBManager = dBManager;
            this.dataGridPlaces = dataGridPlaces;
            initWindow();
        }

        // Initialize window
        void initWindow()
        {
            switch (mode)
            {
                case WindowOpenMode.Insert:
                    buttonAction.Content = "Insert";
                    Title = "Insert Place Into Database";
                    break;
                case WindowOpenMode.Update:
                    DataRowView view = (DataRowView)dataGridPlaces.SelectedItem;
                    buttonAction.Content = "Update";
                    Title = "Update Place In Database";
                    textBoxZip.Text = view[0].ToString();
                    textBoxZip.IsReadOnly = true;
                    textBoxPlace.Text = view[1].ToString();
                    break;
            }
        }

        // Insert or update place
        private void buttonAction_Click(object sender, RoutedEventArgs e)
        {
            long id;

            // Input check 
            if (textBoxPlace.Text == string.Empty) {
                MessageBox.Show("Place box can't be empty!", "Warning!");
                return;
            } else if (textBoxZip.Text == string.Empty) {
                MessageBox.Show("Zip box can't be empty!", "Warning!");
                return;
            }
            else if (!long.TryParse(textBoxZip.Text, out id))
            {
                MessageBox.Show("Zip can only be numeric!", "Warning!");
                return;
            }

            // Select mode: inserting or updating
            switch (mode)
            {
                case WindowOpenMode.Insert:
                    InsertPlaceRecord(textBoxPlace.Text, id);
                    break;
                case WindowOpenMode.Update:
                    UpdatePlaceRecord(textBoxPlace.Text, id);
                    break;
            }
            dataGridPlaces.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectPlaces"], "Places");
            Close();
        }

        // Close window
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Insert place record
        private void InsertPlaceRecord(string pname, long id)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["insertPlace"];
                command.Parameters.Add(new SQLiteParameter("id", id));
                command.Parameters.Add(new SQLiteParameter("pname", pname));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update place record
        private void UpdatePlaceRecord(string pname, long id)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["updatePlace"];
                command.Parameters.Add(new SQLiteParameter("id", id));
                command.Parameters.Add(new SQLiteParameter("pname", pname));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
