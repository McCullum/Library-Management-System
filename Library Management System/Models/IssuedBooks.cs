using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Library_Management_System.Models
{
    public class IssuedBooks : IIssuedBooks
    {
        private int issuedid;
        private int userid;
        private int bookid;
        private DateTime issueddate;
        private DateTime returneddate;
        private bool isfined;
        private SqlConnection Con;
        private SqlCommand Cmd;

        public int IssuedId { 
            get => issuedid; 
            set => issuedid = value; 
        }
        public int UserId { 
            get => userid; 
            set => userid = value; 
        }
        public int BookId { 
            get => bookid; 
            set => bookid = value; 
        }
        public DateTime IssuedDate { 
            get => issueddate;
            set => issueddate = value;
        }
        public DateTime ReturnedDate { 
            get => returneddate; 
            set => returneddate = value; 
        }
        public bool IsFined { 
            get => isfined;
            set => isfined = value;
        }

        public IssuedBooks()
        {
            issuedid = 0;
            userid = 0;
            bookid = 0;
            issueddate = DateTime.MinValue;
            returneddate = DateTime.MinValue;
            isfined = false;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
        }

        public IssuedBooks(int issuedid, int userid, int bookid, DateTime issueddate, DateTime returneddate, bool isfined)
        {
            this.issuedid = issuedid;
            this.userid = userid;
            this.bookid = bookid;
            this.issueddate = issueddate;
            this.returneddate = returneddate;
            this.isfined = isfined;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
        }

        public int IssueBook() {
            string Query = "Insert Into tblIssueBook(UserId,BookId,IssueDate) Values("+UserId+","+BookId+",'"+ DateTime.Now.ToString("yyyy-MM-dd")+"')";
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            if (Cmd.ExecuteNonQuery() == 1)
            {
                return 1;
            }
            else {
                return 0;
            }
        }

        public double CountDays() {
            string CD = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime CurrentDate = DateTime.Parse(CD);
            string Query;
            Query = "Select IssueDate From tblIssueBook Where IssueId = " + IssuedId;
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                IssuedDate = DateTime.Parse(DR.GetString(0));
            }
            DR.Close();
            Con.Close();
            double TotalDays = (CurrentDate - IssuedDate).TotalDays;
            return TotalDays;
        }

        public DataTable DisplaySearchedIssueBooksOnlyIdOrTitle() {
            string Query = "Select tblIssueBook.IssueId, tblBooks.BookTitle, tblUsers.FirstName +' '+ tblUsers.LastName As Name, tblIssueBook.IssueDate From tblBooks INNER JOIN tblIssueBook ON  tblBooks.BookId = tblIssueBook.BookId INNER JOIN tblUsers ON tblUsers.UserId=tblIssueBook.UserId Where tblIssueBook.ReturnDate IS NULL and tblIssueBook.IssueId="+IssuedId;
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
        }

        public DataTable ViewAllCurrentIssueBooks() {

            string Query = "Select tblIssueBook.IssueId, tblBooks.BookTitle, tblUsers.FirstName +' '+ tblUsers.LastName As Name, tblIssueBook.IssueDate From tblBooks INNER JOIN tblIssueBook ON  tblBooks.BookId = tblIssueBook.BookId INNER JOIN tblUsers ON tblUsers.UserId=tblIssueBook.UserId Where tblIssueBook.ReturnDate IS NULL";
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
        }

        public DataTable DisplayAllIssuedBooks()
        {

            string Query = "Select tblIssueBook.IssueId, tblBooks.BookTitle, tblUsers.FirstName +' '+ tblUsers.LastName As Name, tblIssueBook.IssueDate, tblIssueBook.ReturnDate From tblBooks INNER JOIN tblIssueBook ON  tblBooks.BookId = tblIssueBook.BookId INNER JOIN tblUsers ON tblUsers.UserId=tblIssueBook.UserId";
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
            //Con.Open();
            //Cmd.CommandText = Query;
            //Cmd.Connection = Con;
            //SqlDataReader DR = Cmd.ExecuteReader();
            //Con.Close();
            //return DR;

        }

        public DataTable ViewSearchedAllIssueBooks(string SearchContext)
        {

            string Query = "Select tblIssueBook.IssueId, tblBooks.BookTitle, tblUsers.FirstName +' '+ tblUsers.LastName As Name, tblIssueBook.IssueDate From tblBooks INNER JOIN tblIssueBook ON  tblBooks.BookId = tblIssueBook.BookId INNER JOIN tblUsers ON tblUsers.UserId=tblIssueBook.UserId Where " +
                "tblIssueBook.IssueId Like '%" + SearchContext + "%' or tblUsers.FirstName Like '%" + SearchContext + "%' or tblUsers.LastName Like '%" + SearchContext + "%' or tblBooks.BookTitle Like '%" + SearchContext + "%'";
            Con.Close();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
            //Con.Open();
            //Cmd.CommandText = Query;
            //Cmd.Connection = Con;
            //SqlDataReader DR = Cmd.ExecuteReader();
            //Con.Close();
            //return DR;

        }

        public DataTable ViewSearchedAllCurrentIssueBooks(string SearchContext)
        {

            string Query = "Select tblIssueBook.IssueId, tblBooks.BookTitle, tblUsers.FirstName +' '+ tblUsers.LastName As Name, tblIssueBook.IssueDate From tblBooks INNER JOIN tblIssueBook " +
                "ON  tblBooks.BookId = tblIssueBook.IssueId INNER JOIN tblUsers ON tblUsers.UserId=tblIssueBook.UserId Where tblIssueBook.ReturnDate IS NULL And (" +
                "tblIssueBook.IssueId Like '%" + SearchContext + "%' or tblUsers.FirstName Like '%" + SearchContext + "%' or tblUsers.LastName Like '%" + SearchContext + "%' or " +
                "tblBooks.BookTitle Like '%" + SearchContext + "%')";
            Con.Close();
            
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            return DT;
            //Con.Open();
            //Cmd.CommandText = Query;
            //Cmd.Connection = Con;
            //SqlDataReader DR = Cmd.ExecuteReader();
            //Con.Close();
            //return DR;

        }

        public int NumberOfIssuedBookByUser()
        {
            string Query;
            Query = "Select Count(IssueId) From tblIssueBook Where UserId="+UserId+" and ReturnDate Is NULL";
            Con.Open();
            int TotalBook=0;
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                TotalBook = int.Parse(DR.GetValue(0).ToString());
            }
            else {
                TotalBook = 0;
            }
            DR.Close();
            Con.Close();
            return TotalBook;
        }

        public bool IsBookAlreadyIssuedOrNot() {
            string Query;
            Query = "Select Count(IssueId) From tblIssueBook Where UserId=" + UserId + " and BookId="+BookId+" and ReturnDate Is NULL";
            Con.Open();
            int TotalBook = 0;
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                TotalBook = int.Parse(DR.GetValue(0).ToString());
            }
            else
            {
                TotalBook = 0;
            }
            DR.Close();
            Con.Close();
            if (TotalBook >= 1)
            {
                return true;
            }
            else {
                return false;
            }
        }

        public int ReturnBook() {
            string CD = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime CurrentDate = DateTime.Parse(CD);
            string Query = "";
            if (IsFined == true) {
                Query = "Update tblIssueBook Set ReturnDate = '" + CurrentDate + "', IsFined=1 Where IssueId=" + IssuedId;
            }
            else
            {
                Query = "Update tblIssueBook Set ReturnDate = '" + CurrentDate + "' Where IssueId=" + IssuedId;
            }
            
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            if (Cmd.ExecuteNonQuery() == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int CompareTo([AllowNull] IIssuedBooks other)
        {
            throw new NotImplementedException();
        }
    }
}
