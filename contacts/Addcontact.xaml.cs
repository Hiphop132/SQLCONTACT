using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
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
using System.Windows.Shapes;
using Dapper;
namespace contacts {
    /// <summary>
    /// Interaction logic for Addcontact.xaml
    /// </summary>
    public partial class Addcontact : UserControl {
        private Contacts contactsUserControl; // Field to store the reference to the Contacts UserControl

        public Addcontact(Contacts contactsUserControl) {
            InitializeComponent();
            this.contactsUserControl = contactsUserControl;
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e) {
            // Retrieve the contact details from the input fields
            Contactinfo newContact = new Contactinfo {
                // Set the properties based on the input field values
                firstName = Firsttxt.Text,
                middleName = Midtxt.Text,
                lastName = Lasttxt.Text,
                nickName = Nicktxt.Text,
                title = Titletxt.Text,
                email = Emailtxt.Text,
                phone = Phonetxt.Text,
                street = Streettxt.Text,
                city = Citytxt.Text,
                state = Statetxt.Text,
                zip = Ziptxt.Text,
                country = Countrytxt.Text,
                website = Webtxt.Text,
                notes = Notetxt.Text,
                isActive = true
            };
            // Parse the birth date string and assign it to the contact object
            if (DateTime.TryParseExact(Birthtxt.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate)) {
                newContact.birthday = birthDate.Date; // Set the date portion only
            } else {
                // Handle invalid date format or empty input
                MessageBox.Show("Invalid birth date format. Please enter the date in MM/dd/yyyy format.");
                return;
            }
            // Call the method to save the contact to the database
            SaveContactToDatabase(newContact);

            // Update the ListBox in the Contacts UserControl
            contactsUserControl.DisplayContacts();

            //// Switch the content back to the Contacts UserControl
            //CC.Content = contactsUserControl;
        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e) {
            CC.Content = new Contacts();
        }

        private void SaveContactToDatabase(Contactinfo newContact) {
            // Define your code to save the new contact to the database using Dapper or any other approach
            // Here's a sample code snippet using Dapper:

            string connectionString = "Data Source=DESKTOP-42069;Initial Catalog=Contacts;Integrated Security=True";
            string query = "INSERT INTO ContactTable (firstName, middleName, lastName, nickName, title, birthday, email, phone, street, city, state, zip, country, website, notes, IsActive) VALUES (@FirstName,  @MiddleName, @LastName, @NickName, @Title, @Birthday, @Email, @Phone, @Street, @City, @State, @Zip, @Country, @Website, @Notes, @IsActive)";

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@FirstName", newContact.firstName);
                    command.Parameters.AddWithValue("@MiddleName", newContact.middleName);
                    command.Parameters.AddWithValue("@LastName", newContact.lastName);
                    command.Parameters.AddWithValue("@NickName", newContact.nickName);
                    command.Parameters.AddWithValue("@Title", newContact.title);
                    command.Parameters.AddWithValue("@Birthday", newContact.birthday);
                    command.Parameters.AddWithValue("@Email", newContact.email);
                    command.Parameters.AddWithValue("@Phone", newContact.phone);
                    command.Parameters.AddWithValue("@Street", newContact.street);
                    command.Parameters.AddWithValue("@City", newContact.city);
                    command.Parameters.AddWithValue("@State", newContact.state);
                    command.Parameters.AddWithValue("@Zip", newContact.zip);
                    command.Parameters.AddWithValue("@Country", newContact.country);
                    command.Parameters.AddWithValue("@Website", newContact.website);
                    command.Parameters.AddWithValue("@Notes", newContact.notes);
                    command.Parameters.AddWithValue("@IsActive", newContact.isActive);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
