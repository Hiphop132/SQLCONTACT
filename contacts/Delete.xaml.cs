using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Dapper;

namespace contacts {
    public partial class Delete : UserControl {
        string thisConnection = "Data Source=DESKTOP-42069;Initial Catalog=Contacts;Integrated Security=True;";

        public Delete() {
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
                Name1.Text = selectedContact.firstName + " " + selectedContact.lastName;
                Address.Text = selectedContact.street + " " + selectedContact.city + " " + selectedContact.state;
                Number.Text = selectedContact.phone;
                State.Text = selectedContact.state;
                Zip.Text = selectedContact.zip;
                if (selectedContact.picture != null && selectedContact.picture.Length > 0) {
                    using (var stream = new MemoryStream(selectedContact.picture)) {
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


        public void DisplayContacts() {
            string query = "SELECT * FROM ContactTable WHERE IsActive = 0"; // Retrieve only inactive contacts
            List<Contactinfo> contacts = SqlQuery(query);

            contactListBox.ItemsSource = contacts;
        }

        private void Back_Click(object sender, RoutedEventArgs e) {
            CC.Content = new Contacts();
        }

        private void Restore_Click(object sender, RoutedEventArgs e) {
            if (contactListBox.SelectedItem != null) {
                var selectedContact = (Contactinfo)contactListBox.SelectedItem;

                string query = "UPDATE ContactTable SET IsActive = 1 WHERE Id = @Id";

                using (SqlConnection connection = new SqlConnection(thisConnection)) {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@Id", selectedContact.id);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0) {
                            // Clear the selected contact's information and display a message
                            ClearContactDetails();
                            MessageBox.Show("Contact restored successfully!");

                            // Refresh the contactListBox by re-displaying the contacts
                            DisplayContacts();
                        }
                    }
                }
            }
        }

        private void Delete1_Click(object sender, RoutedEventArgs e) {
            if (contactListBox.SelectedItem != null) {
                var selectedContact = (Contactinfo)contactListBox.SelectedItem;

                string query = "DELETE FROM ContactTable WHERE Id = @Id";

                using (SqlConnection connection = new SqlConnection(thisConnection)) {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@Id", selectedContact.id);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0) {
                            // Remove the selected contact from the contacts list
                            List<Contactinfo> contacts = ((List<Contactinfo>)contactListBox.ItemsSource);
                            contacts.Remove(selectedContact);

                            // Refresh the contactListBox by updating the ItemsSource
                            contactListBox.ItemsSource = null;
                            contactListBox.ItemsSource = contacts;

                            // Clear the selected contact's information and display a message
                            ClearContactDetails();
                            MessageBox.Show("Contact deleted successfully!");
                        }
                    }
                }
            }
        }

        private void ClearContactDetails() {
            Name1.Text = string.Empty;
            Address.Text = string.Empty;
            Number.Text = string.Empty;
            State.Text = string.Empty;
            Zip.Text = string.Empty;
            Image.Source = null;
        }



        private void Purge_Click(object sender, RoutedEventArgs e) {
            if (contactListBox.Items.Count > 0) {
                string query = "DELETE FROM ContactTable WHERE Id IN (";

                // Build the parameter placeholders for the contact ids
                for (int i = 0; i < contactListBox.Items.Count; i++) {
                    query += $"@Id{i}, ";
                }

                // Remove the trailing comma and space
                query = query.TrimEnd(',', ' ');

                query += ")";

                using (SqlConnection connection = new SqlConnection(thisConnection)) {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection)) {
                        // Add parameters for each contact id
                        for (int i = 0; i < contactListBox.Items.Count; i++) {
                            var contact = (Contactinfo)contactListBox.Items[i];
                            command.Parameters.AddWithValue($"@Id{i}", contact.id);
                        }

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0) {
                            MessageBox.Show("Contacts purged successfully!");

                            // Clear the contacts list and refresh the contactListBox
                            contactListBox.ItemsSource = null;
                            contactListBox.Items.Clear();

                            // Clear the selected contact's information
                            ClearContactDetails();
                        }
                    }
                }
            }
        }
    }
}

