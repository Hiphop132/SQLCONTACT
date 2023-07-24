using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Dapper;
using Microsoft.Win32;

namespace contacts {
    public partial class Contacts : UserControl {
        string thisConnection = "Data Source=DESKTOP-42069;Initial Catalog=Contacts;Integrated Security=True;";

        public Contacts() {
            InitializeComponent();
            DisplayContacts();
        }

        private List<Contactinfo> SqlQuery(string query) {
            using (var connection = new SqlConnection(thisConnection)) {
                return connection.Query<Contactinfo>(query).ToList();
            }
        }

        private void ContactListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (contactListBox.SelectedIndex != -1) {
                var selectedContact = (Contactinfo)contactListBox.SelectedItem;
                Name.Text = selectedContact.firstName;
                Address.Text = selectedContact.street + " " + selectedContact.city + " " + selectedContact.state + " " + selectedContact.zip;
                Email.Text = selectedContact.email;
                Number.Text = selectedContact.phone;
                Website.Text = selectedContact.website;
                Notes.Text = selectedContact.notes;

                // Load the contact's image if available
                if (selectedContact.picture != null && selectedContact.picture.Length > 0) {
                    using (var stream = new System.IO.MemoryStream(selectedContact.picture)) {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = stream;
                        bitmapImage.EndInit();
                        Image.Source = bitmapImage;
                    }
                } else {
                    // Set the Image control's Source property to null to leave the image empty
                    Image.Source = null;
                }
            }
        }


        private void deleteBtn_Click(object sender, RoutedEventArgs e) {
            selectedContact = (Contactinfo)contactListBox.SelectedItem;
            selectedContact.isActive = false;

            string connectionString = "Data Source=DESKTOP-42069;Initial Catalog=Contacts;Integrated Security=True";
            string query = "UPDATE ContactTable SET IsActive = 0 WHERE id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@id", selectedContact.id);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0) {
                        // Clear the selected contact's information and display a message
                        ClearContactDetails();
                        MessageBox.Show("Contact deleted successfully!");
                    }
                }
            }
        }

        private void ClearContactDetails() {
            Name.Text = string.Empty;
            Address.Text = string.Empty;
            Email.Text = string.Empty;
            Number.Text = string.Empty;
            Website.Text = string.Empty;
            Notes.Text = string.Empty;
            Image.Source = null;
        }


        public void DisplayContacts() {
            string query = "SELECT * FROM ContactTable WHERE IsActive = 1 ORDER BY isFavorite DESC";
            List<Contactinfo> contacts = SqlQuery(query);

            contactListBox.ItemsSource = contacts;
        }

        private Contactinfo selectedContact;

        private void Edit_Click(object sender, RoutedEventArgs e) {
            if (contactListBox.SelectedItem != null) {
                selectedContact = (Contactinfo)contactListBox.SelectedItem;

                Editcontacts editContacts = new Editcontacts();
                editContacts.SelectedContact = selectedContact;
                CC.Content = editContacts;
            }
        }

        private void Settingsbtn_Click(object sender, RoutedEventArgs e) {
            Setting settingUserControl = new Setting(this);
            CC.Content = settingUserControl;
        }

        private void Add_Click(object sender, RoutedEventArgs e) {
            Addcontact addContactUserControl = new Addcontact(this);
            CC.Content = addContactUserControl;
        }
    }

    public class Contactinfo {
        public int id { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string nickName { get; set; }
        public string title { get; set; }
        public DateTime birthday { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string website { get; set; }
        public string notes { get; set; }
        public byte[] picture { get; set; }
        public bool isFavorite { get; set; }
        public bool isActive { get; set; }
    }
}
