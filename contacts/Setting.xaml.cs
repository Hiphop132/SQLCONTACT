using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows;
using Dapper;
using System.Linq;
namespace contacts {
    public partial class Setting : UserControl {
        private Contacts contactsUserControl;
        private string sortOrder = "isFavorite DESC"; // Default sorting order
        public bool IsFirstNameChecked { get; set; }
        public bool IsLastNameChecked { get; set; }
        string thisConnection = "Data Source=DESKTOP-42069;Initial Catalog=Contacts;Integrated Security=True;";

        public Setting(Contacts contactsUserControl) {
            InitializeComponent();
            this.contactsUserControl = contactsUserControl;
            IsFirstNameChecked = First.IsChecked ?? false;
            IsLastNameChecked = Last.IsChecked ?? false;
        }
        private List<Contactinfo> SqlQuery(string query) {
            using (var connection = new SqlConnection(thisConnection)) {
                return connection.Query<Contactinfo>(query).ToList();
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e) {
            CC.Content = new Contacts();
        }
        private void Delete_Click(object sender, RoutedEventArgs e) {
            CC.Content = new Delete();
        }

        private void First_Checked(object sender, RoutedEventArgs e) {
            sortOrder = "firstName ASC";
            UpdateContactList();
        }

        private void First_Unchecked(object sender, RoutedEventArgs e) {
            sortOrder = "isFavorite DESC";
            UpdateContactList();
        }

        private void Last_Checked(object sender, RoutedEventArgs e) {
            sortOrder = "lastName ASC";
            UpdateContactList();
        }

        private void Last_Unchecked(object sender, RoutedEventArgs e) {
            sortOrder = "isFavorite DESC";
            UpdateContactList();
        }

        private void UpdateContactList() {
            string query = $"SELECT * FROM ContactTable WHERE IsActive = 1 ORDER BY {sortOrder}";
            List<Contactinfo> contacts = SqlQuery(query);

            contactsUserControl.contactListBox.ItemsSource = contacts;

            // Update the checkbox properties
            IsFirstNameChecked = First.IsChecked ?? false;
            IsLastNameChecked = Last.IsChecked ?? false;
        }
    }
}
