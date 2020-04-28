using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;

namespace Library_Management_System.Models
{
    public class Fines : IFines
    {
        private int fineid;
        private int issueid;
        private float fineamount;
        private float remainingfineamount;
        private SqlConnection Con;
        private SqlCommand Cmd;

        public int FineId { 
            get => fineid;
            set => fineid = value; 
        }
        public int IssueId { 
            get => issueid; 
            set => issueid = value; 
        }
        public float FineAmount { 
            get => fineamount;
            set => fineamount = value; 
        }
        public float RemainingFineAmount { 
            get => remainingfineamount;
            set => remainingfineamount = value; 
        }

        public Fines()
        {
            fineid = 0;
            issueid = 0;
            fineamount = 0;
            remainingfineamount = 0;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
        }

        public Fines(int fineid, int issueid, float fineamount, float remainingfineamount)
        {
            this.fineid = fineid;
            this.issueid = issueid;
            this.fineamount = fineamount;
            this.remainingfineamount = remainingfineamount;
        }

        public DataTable GetRemainingFineAmountSearch(string SearchContext) {
            string Query = "Select tblIssueBook.IssueId, tblUsers.FirstName + tblUsers.LastName As Name, tblUsers.EmailId, tblBooks.BookTitle, tblIssueBook.IssueDate, " +
                "tblFines.RemainingFineAmount , tblFines.FineAmount " +
                "From tblUsers Inner Join tblIssueBook On tblUsers.UserId = tblIssueBook.UserId " +
                "Inner Join tblBooks On tblBooks.BookId = tblIssueBook.BookId " +
                "Inner Join tblFines On tblFines.IssueId = tblIssueBook.IssueId " +
                "Where RemainingFineAmount!=0 and ( tblIssueBook.IssueId Like '" + SearchContext+"' Or tblUsers.FirstName LIKE '%" + SearchContext+ "%' Or tblUsers.LastName LIKE '%" + SearchContext + "%' " +
                "Or tblUsers.EmailId LIKE '%" + SearchContext + "%' Or tblBooks.BookTitle LIKE '%" + SearchContext + "%')";
            Con.Open();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            
            DA.Fill(DT);
            Con.Close();
            return DT;
        }

        public DataTable GetAllFineAmountList()
        {
            string Query = "Select tblIssueBook.IssueId, tblFines.FineAmount, tblUsers.FirstName + tblUsers.LastName As Name, tblUsers.EmailId, tblBooks.BookTitle, tblIssueBook.IssueDate, " +
                "tblFines.RemainingFineAmount " +
                "From tblUsers " +
                "Inner Join tblIssueBook On tblUsers.UserId = tblIssueBook.UserId " +
                "Inner Join tblBooks On tblBooks.BookId = tblIssueBook.BookId " +
                "Inner Join tblFines On tblFines.IssueId = tblIssueBook.IssueId";
            Con.Open();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            
            DA.Fill(DT);
            Con.Close();
            return DT;
        }

        public DataTable GetAllFineAmountListSearch(string SearchContext)
        {
            string Query = "Select tblIssueBook.IssueId, tblUsers.FirstName + tblUsers.LastName As Name, tblUsers.EmailId, tblBooks.BookTitle, tblIssueBook.IssueDate, " +
               "tblFines.RemainingFineAmount , tblFines.FineAmount " +
               "From tblUsers Inner Join tblIssueBook On tblUsers.UserId = tblIssueBook.UserId " +
               "Inner Join tblBooks On tblBooks.BookId = tblIssueBook.BookId " +
               "Inner Join tblFines On tblFines.IssueId = tblIssueBook.IssueId " +
               "Where RemainingFineAmount!=0 and ( tblIssueBook.IssueId Like '" + SearchContext + "'Or tblUsers.FirstName LIKE '%" + SearchContext + "%' Or tblUsers.LastName LIKE '%" + SearchContext + "%' " +
               "Or tblUsers.EmailId LIKE '%" + SearchContext + "%' Or tblBooks.BookTitle LIKE '%" + SearchContext + "%')";
            Con.Open();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();

            DA.Fill(DT);
            Con.Close();
            return DT;
        }

        public DataTable GetRemainingFineAmountList()
        {
            string Query = "Select tblIssueBook.IssueId, tblFines.FineAmount, tblUsers.FirstName + tblUsers.LastName As Name, tblUsers.EmailId, tblBooks.BookTitle, tblIssueBook.IssueDate, " +
                "tblFines.RemainingFineAmount " +
                "From tblUsers " +
                "Inner Join tblIssueBook On tblUsers.UserId = tblIssueBook.UserId " +
                "Inner Join tblBooks On tblBooks.BookId = tblIssueBook.BookId " +
                "Inner Join tblFines On tblFines.IssueId = tblIssueBook.IssueId Where tblFines.RemainingFineAmount!=0";
            Con.Open();
            SqlDataAdapter DA = new SqlDataAdapter();
            DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            
            DA.Fill(DT);
            Con.Close();
            return DT;
        }

        public float GetRemainingFineAmountBasedOnFineId() {
            string Query = "Select RemainingAmount From tblFines Where RemainingFineAmount!=0 and FineId=" + FineId;
            Con.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                float RAmount;
                float.TryParse(DR.GetValue(0).ToString(),out RAmount);
                Con.Close();
                return RAmount;
            }
            else
            {
                Con.Close();
                return -1;
            }
        }

        public bool AcceptFine(double EnteredFineAmount) {
            string Query = "Update tblFines Set RemainingFineAmount = RemainingFineAmount - " + EnteredFineAmount + " Where IssueId=" + IssueId;
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

        public bool AddNewFineEntry() {
            string Query = "Insert Into tblFines(IssueId,FineAmount,RemainingFineAmount) Values("+IssueId+","+FineAmount+","+RemainingFineAmount+")";
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

        public double GetRamainingFineAmountOfSpecificUser() {
            string Query = "Select RemainingFineAmount From tblFines Where IssueId="+IssueId;
            Cmd.CommandText = Query;
            Con.Open();
            Cmd.Connection = Con;
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                double RAmount = 0.0;
                double.TryParse(DR.GetValue(0).ToString().Trim(), out RAmount);
                return RAmount;
            }
            else {
                return 0;
            }
        }

        public int CompareTo([AllowNull] IFines other)
        {
            throw new NotImplementedException();
        }
    }
}
