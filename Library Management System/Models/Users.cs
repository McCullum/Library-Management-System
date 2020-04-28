using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.Windows;

namespace Library_Management_System.Models
{
    public class Users : IUsers
    {
        private int userid;
        private string firstname;
        private string lastname;
        private string emailid;
        private string contactnumber;
        private string password;
        private char role;
        private bool isenabled;
        private bool isdeleted;
        private SqlConnection Con;
        private SqlCommand Cmd;


        public int UserId { 
            get => userid;
            set => userid = value; 
        }
        public string FirstName { 
            get => firstname ; 
            set => firstname = value; 
        }
        public string LastName { 
            get => lastname;
            set => lastname = value;
        }
        public string EmailId { 
            get => emailid; 
            set => emailid = value; 
        }
        public string ContactNumber {
            get => contactnumber;
            set => contactnumber = value;
        }
        public string Password { 
            get => password;
            set => password = value; 
        }
        public char Role { 
            get => role;
            set => role = value; 
        }
        public bool IsEnabled { 
            get => isenabled; 
            set => isenabled = value; 
        }
        public bool IsDeleted { 
            get => isdeleted; 
            set => isdeleted = value; 
        }

        public Users() {
            userid = 0;
            firstname = "";
            lastname = "";
            emailid = "";
            password = "";
            role = 'X';
            isenabled = false;
            isdeleted = true;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
        }

        public Users(int userid, string firstname, string lastname, string emailid, string password, char role, bool isenabled, bool isdeleted)
        {
            this.userid = userid;
            this.firstname = firstname;
            this.lastname = lastname;
            this.emailid = emailid;
            this.password = password;
            this.role = role;
            this.isenabled = isenabled;
            this.isdeleted = isdeleted;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
        }

        public bool AddNewUser() {
            string Query="";
            if (Role == 'A' || Role == 'L') {
                string Password = FirstName.Substring(0,1).ToLower()+ LastName.Substring(0, 1).ToLower() + ContactNumber.Substring(0, 3).ToLower();
                Password = GetEncryptionMd5.MD5Process(Password);
                Query = "Insert Into tblUsers(FirstName,LastName,EmailId,ContactNumber,Role,Password) Values(" +
               "'" + FirstName + "' , '" + LastName + "','" + EmailId + "' , '" + ContactNumber + "','" + Role + "','"+Password+"')";
            }
            else {
                Query = "Insert Into tblUsers(FirstName,LastName,EmailId,ContactNumber,Role) Values(" +
               "'" + FirstName + "' , '" + LastName + "','" + EmailId + "' , '" + ContactNumber + "','" + Role + "')";
            }
           
            Cmd.CommandText = Query;
            Con.Open();
            Cmd.Connection = Con;
            if (Cmd.ExecuteNonQuery() == 1)
            {
                Con.Close();
                return true;
            }
            else
            {
                Con.Close();
                return false;
            }
        }

        public string ValidateAddUserInput()
        {
            string ErrorMessage = "";
            Regex R = new Regex("^[2-9]\\d{2} \\d{3} \\d{ 4}$");
            if (FirstName.Trim() == "" || FirstName.Trim() == null || FirstName.Trim() == "First Name")
            {
                ErrorMessage += "\nPlease Enter First Name.";
            }
            if (LastName.Trim() == "" || LastName.Trim() == null || LastName.Trim() == "Last Name")
            {
                ErrorMessage += "\nPlease Enter Last Name.";
            }
            if (EmailId.Trim() == "" || EmailId.Trim() == null || EmailId.Trim() == "Email Id")
            {
                ErrorMessage += "\nPlease Enter Email Id.";
            }
            if (ContactNumber.Trim() == "" || ContactNumber.Trim() == null || ContactNumber.Trim() == "Contact Number")
            {
                ErrorMessage += "\nPlease Enter Contact Number.";
            }
            else if (ContactNumber.Trim().Length != 10) {
                ErrorMessage += "\nPlease Enter Valid 10 Digit Contact Number.";
            }
            //else if (!R.IsMatch(ContactNumber.Trim())) {
            //    ErrorMessage += "\nPlease Enter Valid Contact Number.";
            //}
            return ErrorMessage.Trim();

        }

        public DataTable DisplayActiveUserList ()
        {
            string Query = "Select * from tblUsers Where IsDeleted=1";
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
        }

        public DataTable DisplayActiveSearchUserList(string SearchContext)
        {
            string Query = "Select * from tblUsers Where IsDeleted=1 and (FirstName Like '%"+SearchContext+ "%' or " +
                                                                         "LastName  Like '%" + SearchContext + "%' or " +
                                                                         "EmailId Like '%" + SearchContext + "%' or" +
                                                                         " ContactNumber  Like '%" + SearchContext + "%')";
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
        }

        public DataTable DisplayDeletedUserList()
        {
            string Query = "Select * from tblUsers Where IsDeleted=0";
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
        }

        public DataTable DisplayDeletedSearchUserList(string SearchContext)
        {
            string Query = "Select * from tblUsers Where IsDeleted=0 and (FirstName Like '%" + SearchContext + "%' or " +
                                                                         "LastName  Like '%" + SearchContext + "%' or " +
                                                                         "EmailId Like '%" + SearchContext + "%' or" +
                                                                         " ContactNumber  Like '%" + SearchContext + "%')";
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            Con.Close();
            return DT;
        }

        public bool DeleteUser() {
            string Query = "";
            Query = "Update tblUsers Set IsDeleted=0 Where UserId="+UserId;
            
            Cmd.CommandText = Query;
            if (Con.State == ConnectionState.Open) {
                Con.Close();
            }
            Con.Open();

            Cmd.Connection = Con;
            if (Cmd.ExecuteNonQuery() == 1)
            {
                Con.Close();
                return true;
            }
            else
            {
                Con.Close();
                return false;
            }
        }

        public bool CheckCurrentPassword(string OldPassword) {

            OldPassword = GetEncryptionMd5.MD5Process(OldPassword) ;
            string Query = "Select Password From tblUsers Where UserId=" + UserId ;
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                if (DR.GetValue(0).ToString().Trim() == OldPassword) {
                    return true;
                }
            }
            else
            {
                Con.Close();
                return false;
            }
            return false;
        }

        public bool ChangePassword(string NewPassword) {
            string Query = "";
            NewPassword = GetEncryptionMd5.MD5Process(NewPassword);
            Query = "Update tblUsers Set Password='"+NewPassword+"' Where UserId=" + UserId;

            Cmd.CommandText = Query;
            if (Con.State == ConnectionState.Open)
            {
                Con.Close();
            }
            Con.Open();

            Cmd.Connection = Con;
            if (Cmd.ExecuteNonQuery() == 1)
            {
                Con.Close();
                return true;
            }
            else
            {
                Con.Close();
                return false;
            }
        }

        public int GetUserIdBasedOUserEmail() {
            string Query = "Select UserId From tblUsers Where EmailId='" + EmailId + "'";
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                int Users;
                int.TryParse(DR.GetValue(0).ToString().Trim(), out Users);
                return Users;
            }
            else
            {
                Con.Close();
                return 0;
            }
        }

        public bool CheckUserEmailOrContactExistsOrNot() {
            string Query = "Select Count(UserId) From tblUsers Where EmailId='"+EmailId+"' Or ContactNumber='"+ContactNumber+"'";
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                int Users;
                int.TryParse(DR.GetValue(0).ToString().Trim(), out Users);
                if (Users > 0)
                {
                    Con.Close();
                    return true;
                }
                else
                {
                    Con.Close();
                    return false;
                }
            }
            else
            {
                Con.Close();
                return false;
            }
        }

        public bool CheckUserExistsOrNot() {
            string Query = "Select Count(UserId) From tblUsers Where UserId="+UserId;
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                if (DR.GetValue(0).ToString().Trim() == "1")
                {
                    Con.Close();
                    return true;
                }
                else
                {
                    Con.Close();
                    return false;
                }
            }
            else {
                Con.Close();
                return false;
            }
        }
        //public string CheckLogin(string Username, string Password) {
        //    Cmd = new SqlCommand("Select * From tblUsers Where EmailId='" + Username + "' " +
        //        "or ContactNumber='" + Username + "' and Password='" + Password + "' ", Con);
        //    Con.Open();
        //    SqlDataReader DR;
        //    DR = Cmd.ExecuteReader();

        //    if (DR.HasRows)
        //    {
        //        DR.Read();
        //        if ((Username == DR.GetValue(3).ToString() && Password == DR.GetValue(6).ToString()) ||
        //            (Username == DR.GetValue(4).ToString() && Password == DR.GetValue(6).ToString())
        //            )
        //        {
        //            string Role = "A=Admin, L= Librarian";
        //            Role = DR.GetValue(5).ToString().Trim();

        //            if (Role == "A" || Role == "a")
        //            {
        //                return "Admin";
        //            }
        //            else if (Role == "L" || Role == "l")
        //            {
        //                //User Is Librarian
        //                return "Librarian";
        //            }
        //            else
        //            {
        //                return "Error";
        //            }
                    
        //        }
        //        else
        //        {
        //            return "Invalid Password";
        //        }
        //    }
        //    else
        //    {
        //        //User Does Not Exists!
        //        return "Invalid User";
        //    }
        //}

        public int CompareTo([AllowNull] IUsers other)
        {
            throw new NotImplementedException();
        }
    }
}
