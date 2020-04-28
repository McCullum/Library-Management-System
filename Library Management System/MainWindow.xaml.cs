using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Configuration;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection Con;
        SqlCommand Cmd;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
            //Remove Before Submission
            //AdminHomePage adminHomePage = new AdminHomePage(1, "BBM@gmail.com");
            //adminHomePage.Show();
            //this.Close();
        }

        private void txtUName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "Email Or Contact No.") {
                txtUsername.Text = "";
                txtUsername.Foreground = Brushes.Black;
            }
        }

        private void txtUName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "" || txtUsername.Text == null) {
                txtUsername.Text = "Email Or Contact No.";
                txtUsername.Foreground = Brushes.DarkGray;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Con.Close();
            String Username = txtUsername.Text;
            String Password = GetEncryptionMd5.MD5Process(txtPassword.Password.Trim());

            Cmd = new SqlCommand("Select * From tblUsers Where EmailId='" + Username + "' " +
                "or ContactNumber='"+ Username +"' and Password='" + Password + "' ", Con);
            Con.Open();
            SqlDataReader DR;
            DR = Cmd.ExecuteReader();

            if (DR.HasRows)
            {
                DR.Read();
                if ((Username == DR.GetValue(3).ToString() && Password == DR.GetValue(6).ToString()) ||
                    (Username == DR.GetValue(4).ToString() && Password == DR.GetValue(6).ToString())
                    )
                {

                    txtUsername.Text = "Email Or Contact No.";
                    txtUsername.Foreground = Brushes.DarkGray;
                    txtPassword.Password = "";
                    string Role = "A=Admin, L= Librarian";
                    Role = DR.GetValue(5).ToString().Trim();

                    if (Role =="A" || Role == "a" || Role == "L" || Role == "l")
                    {
                        //User Is Admin
                        AdminHomePage adminHomePage = new AdminHomePage(int.Parse(DR.GetValue(0).ToString()), DR.GetValue(3).ToString());
                        adminHomePage.Show();
                        this.Close();
                    }
                    else {
                        return;
                    }
                    lblInvalid.Visibility = Visibility.Hidden;
                }
                else
                {
                    lblInvalid.Visibility = Visibility.Visible;
                    lblInvalid.Content = "Invalid Password!";
                    txtPassword.Password = "";
                }
            }
            else
            {
                //User Does Not Exists!
                lblInvalid.Content = "User Does Not Exists!";
                lblInvalid.Visibility = Visibility.Visible;
                txtUsername.Text = "Email Or Contact No.";
                txtUsername.Foreground = Brushes.DarkGray;
                txtPassword.Password = "";
            }
        }
    }
}
