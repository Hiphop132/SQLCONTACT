using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Dapper;

namespace contacts {
    public partial class Editcontacts : UserControl {
        public Contactinfo SelectedContact { get; set; }

        public Editcontacts() {
            InitializeComponent();
            DataContext = this; // Set the DataContext to the UserControl itself
        }

        // When the UserControl is loaded, populate the TextBoxes with the selected contact's data
        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (SelectedContact != null) {
                // Assign TextBoxes with corresponding properties of the SelectedContact
                Firsttxt.Text = SelectedContact.firstName;
                Midtxt.Text = SelectedContact.middleName;
                Nicktxt.Text = SelectedContact.nickName;
                Lasttxt.Text = SelectedContact.lastName;
                Titletxt.Text = SelectedContact.title;
                Birthtxt.Text = SelectedContact.birthday.ToString("MM/dd/yyyy");
                Emailtxt.Text = SelectedContact.email;
                Phonetxt.Text = SelectedContact.phone;
                Streettxt.Text = SelectedContact.street;
                Citytxt.Text = SelectedContact.city;
                Statetxt.Text = SelectedContact.state;
                Ziptxt.Text = SelectedContact.zip;
                Countrytxt.Text = SelectedContact.country;
                Webtxt.Text = SelectedContact.website;
                Notetxt.Text = SelectedContact.notes;
                IsFavoriteCheckBox.IsChecked = SelectedContact.isFavorite; // Set the checked state of the IsFavorite checkbox

                // Load the contact's image if available
                if (SelectedContact.picture != null && SelectedContact.picture.Length > 0) {
                    Image.Source = LoadImageFromBytes(SelectedContact.picture);
                }
            }
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e) {
            // Retrieve the updated contact information from the TextBoxes
            string firstName = Firsttxt.Text;
            string middleName = Midtxt.Text;
            string lastName = Lasttxt.Text;
            string nickName = Nicktxt.Text;
            string title = Titletxt.Text;
            string birthday = Birthtxt.Text;
            string email = Emailtxt.Text;
            string phone = Phonetxt.Text;
            string street = Streettxt.Text;
            string city = Citytxt.Text;
            string state = Statetxt.Text;
            string zip = Ziptxt.Text;
            string country = Countrytxt.Text;
            string website = Webtxt.Text;
            string notes = Notetxt.Text;

            // Retrieve the IsFavorite state
            bool isFavorite = IsFavoriteCheckBox.IsChecked ?? false;

            // Update the contact information in the database using the appropriate SQL query
            string connectionString = "Data Source=DESKTOP-42069;Initial Catalog=Contacts;Integrated Security=True";
            string query = "UPDATE ContactTable SET FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, NickName = @NickName, Title = @Title, Birthday = @Birthday, Email = @Email, Phone = @Phone, Street = @Street, City = @City, State = @State, Zip = @Zip, Country = @Country, Website = @Website, Notes = @Notes, IsFavorite = @IsFavorite, Picture = @Picture WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@MiddleName", middleName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@NickName", nickName);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Birthday", birthday);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Street", street);
                    command.Parameters.AddWithValue("@City", city);
                    command.Parameters.AddWithValue("@State", state);
                    command.Parameters.AddWithValue("@Zip", zip);
                    command.Parameters.AddWithValue("@Country", country);
                    command.Parameters.AddWithValue("@Website", website);
                    command.Parameters.AddWithValue("@Notes", notes);
                    command.Parameters.AddWithValue("@IsFavorite", isFavorite);
                    command.Parameters.AddWithValue("@Picture", SelectedContact.picture);
                    command.Parameters.AddWithValue("@Id", SelectedContact.id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0) {
                        MessageBox.Show("Contact updated successfully!");
                    }
                }
            }
        }

        private void Photo_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpeg;*.jpg;*.gif;*.bmp)|*.png;*.jpeg;*.jpg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true) {
                // Get the selected image file path
                string imagePath = openFileDialog.FileName;

                // Read the image file as binary data
                byte[] imageData = File.ReadAllBytes(imagePath);

                // Update the selected contact's picture property
                if (SelectedContact != null) {
                    SelectedContact.picture = imageData;

                    // Set the ImageBox's Source property to display the selected image
                    Image.Source = LoadImageFromBytes(imageData);
                }
            }
        }

        private BitmapImage LoadImageFromBytes(byte[] imageData) {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(imageData)) {
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            bitmapImage.Freeze(); // Freeze the bitmap to prevent memory leaks
            return bitmapImage;
        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e) {
            CC.Content = new Contacts();
        }

        private void IsFavoriteCheckBox_Checked(object sender, RoutedEventArgs e) {
            SelectedContact.isFavorite = IsFavoriteCheckBox.IsChecked ?? false;
        }

        private void IsFavoriteCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            SelectedContact.isFavorite = false;
            UpdateIsFavoriteInDatabase(false);
        }

        private void UpdateIsFavoriteInDatabase(bool isFavorite) {
            string connectionString = "Data Source=DESKTOP-42069;Initial Catalog=Contacts;Integrated Security=True";
            string query = "UPDATE ContactTable SET IsFavorite = @IsFavorite WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@IsFavorite", isFavorite);
                    command.Parameters.AddWithValue("@Id", SelectedContact.id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0) {
                        MessageBox.Show("Contact's IsFavorite status updated in the database!");
                    }
                }
            }
        }
    }
}
