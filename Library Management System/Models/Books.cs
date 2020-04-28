using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;

namespace Library_Management_System.Models
{
    public class Books : IBooks
    {
        private int bookid;
        private string booktitle;
        private string booktype;
        private string author;
        private int quantity;
        private int quantityonhand;
        public SqlConnection Con;
        private SqlCommand Cmd;
        

        public int BookId { 
            get => bookid;
            set => bookid = value;
        }
        public string BookTitle { 
            get => booktitle;
            set => booktitle = value; 
        }
        public string BookType { 
            get => booktype; 
            set => booktype = value; 
        }
        public string Author {
            get => author; 
            set => author = value; 
        }
        public int Quantity { 
            get => quantity; 
            set => quantity = value; }
        public int QuantityOnHand { 
            get => quantityonhand;
            set => quantityonhand = value;
        }

        public Books()
        {
            bookid = 0;
            booktitle = "";
            booktype = "";
            author = "";
            quantity = 0;
            quantityonhand = 0;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
        }

        public Books(int bookid, string booktitle, string booktype, string author, int quantity, int quantityonhand)
        {
            this.bookid = bookid;
            this.booktitle = booktitle;
            this.booktype = booktype;
            this.author = author;
            this.quantity = quantity;
            this.quantityonhand = quantityonhand;
            Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryManagementSystemConnectionString"].ConnectionString);
            Cmd = new SqlCommand();
        }

        //public int AddNewBook(string BookTitle, string BookType, string AuthorName,int Quantity) {
        //    string Query = "Insert Into tblBooks(BookTitle,BookType,Author,Quantity,QuantityOnHand) Values(" +
        //        "'"+ BookTitle + "' , '"+BookType+"','"+ AuthorName + "' , "+Quantity+","+Quantity+")";
        //    Cmd.CommandText = Query;
        //    Con.Open();
        //    Cmd.Connection = Con;
        //    if (Cmd.ExecuteNonQuery() == 1)
        //    {
        //        return 1;
        //    }
        //    else {
        //        return 0;
        //    }
        //}

        public int UpdateBook() {
            string Query;
            Query = "Update tblBooks Set BookTitle = '"+BookTitle+"' , BookType = '"+BookType+"' , " +
                "Author = '"+Author+"', Quantity = "+Quantity+" Where BookId = "+BookId+"";
            Cmd.CommandText = Query;
            Con.Open();
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

        public int AddNewBook()
        {
            string Query = "Insert Into tblBooks(BookTitle,BookType,Author,Quantity,QuantityOnHand) Values(" +
                "'" + BookTitle + "' , '" + BookType + "','" + Author + "' , " + Quantity + "," + QuantityOnHand + ")";
            Cmd.CommandText = Query;
            Con.Open();
            Cmd.Connection = Con;
            if (Cmd.ExecuteNonQuery() == 1)
            {
                Con.Close();
                return 1;
            }
            else
            {
                Con.Close();
                return 0;
            }
        }

        public int DeleteBook() {
            string Query;

            Query = "Select Quantity, QuantityOnHand From tblBooks Where BookId="+BookId+"";
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            Con.Open();
            SqlDataReader DR = Cmd.ExecuteReader();
            DR.Read();
            if (DR.GetValue(0).ToString().Trim() == DR.GetValue(1).ToString().Trim())
            {
                Query = "Delete From tblBooks Where BookId = " + BookId + "";
                Cmd.CommandText = Query;
                Con.Open();
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
            else {
                return 2;
            }
        }
        public string ValidateUserInput(string BookTitle, string BookType, string AuthorName, string Quantity) {
            string ErrorMessage = "";
            int Temp;
            if (BookTitle.Trim() == "" || BookTitle.Trim() == null || BookTitle.Trim() == "Book Title") {
                ErrorMessage += "\nPlease Enter Book Title";
            }
            if (AuthorName.Trim() == "" || AuthorName.Trim() == null || AuthorName.Trim() == "Author Name")
            {
                ErrorMessage += "\nPlease Enter Author Name";
            }
            if (BookType.Trim() == "" || BookType.Trim() == null) {
                ErrorMessage += "\nPlease Enter Book Type";
            }
            if (Quantity.Trim() == "" || Quantity.Trim() == null || Quantity.Trim() == "0")
            {
                ErrorMessage += "\nPlease Enter Quantity Of Book";
            }

            else if(!int.TryParse(Quantity,out Temp)) {
                ErrorMessage += "\nPlease Enter Valid Quantity Of Books.";
            }

            return ErrorMessage;
            
        }

        public List<int> getAllBookId() {
            List<int> BooksId = new List<int>();
            string Query = "Select BookId From tblBooks";
            SqlDataAdapter DA = new SqlDataAdapter(Query, Con);
            DataTable DT = new DataTable();
            Con.Open();
            DA.Fill(DT);
            foreach (DataRow DR in DT.Rows)
            {
                BooksId.Add(int.Parse(DR[0].ToString()));
            }
            Con.Close();
            return BooksId;

        }

        public ObservableCollection<Books> DisplayBooks() {
            ObservableCollection<Books> AllBooks = new ObservableCollection<Books>();
            AllBooks.Clear();
            
            SqlDataAdapter DA = new SqlDataAdapter();
            DataTable DT = new DataTable();
            DA = new SqlDataAdapter("Select * From tblBooks", Con);
            Con.Open();
            DA.Fill(DT);
            foreach (DataRow RowOfDataTable in DT.Rows) {
                var BooksObject = new Books()
                {
                    BookId = (int)RowOfDataTable["BookId"],
                    BookTitle = (string)RowOfDataTable["BookTitle"],
                    BookType = (string)RowOfDataTable["BookType"],
                    Author = (string)RowOfDataTable["Author"],
                    Quantity = (int)RowOfDataTable["Quantity"],
                    QuantityOnHand = (int)RowOfDataTable["QuantityOnHand"]
                };
                AllBooks.Add(BooksObject);
            }
            return AllBooks;
        }


        public ObservableCollection<Books> DisplaySearchedBooks(string SearchContext)
        {
            ObservableCollection<Books> AllBooks = new ObservableCollection<Books>();
            AllBooks.Clear();

            SqlDataAdapter DA = new SqlDataAdapter();
            DataTable DT = new DataTable();
            string Query = "Select * From tblBooks Where BookId like '%"+SearchContext+"%' or BookTitle like '%"+SearchContext+"%' or" +
                " Author like '%"+SearchContext+"%' or BookType like '%"+SearchContext+"%'";
            DA = new SqlDataAdapter(Query, Con);
            Con.Open();
            DA.Fill(DT);
            foreach (DataRow RowOfDataTable in DT.Rows)
            {
                var BooksObject = new Books()
                {
                    BookId = (int)RowOfDataTable["BookId"],
                    BookTitle = (string)RowOfDataTable["BookTitle"],
                    BookType = (string)RowOfDataTable["BookType"],
                    Author = (string)RowOfDataTable["Author"],
                    Quantity = (int)RowOfDataTable["Quantity"],
                    QuantityOnHand = (int)RowOfDataTable["QuantityOnHand"]
                };
                AllBooks.Add(BooksObject);
            }
            return AllBooks;
        }

        public ObservableCollection<Books> DisplaySearchedBooksOnlyIdOrTitle(string SearchContext)
        {
            ObservableCollection<Books> AllBooks = new ObservableCollection<Books>();
            AllBooks.Clear();

            SqlDataAdapter DA = new SqlDataAdapter();
            DataTable DT = new DataTable();
            string Query = "Select * From tblBooks Where BookId like '%" + SearchContext + "%' or BookTitle like '%" + SearchContext + "%'";
            DA = new SqlDataAdapter(Query, Con);
            Con.Open();
            DA.Fill(DT);
            foreach (DataRow RowOfDataTable in DT.Rows)
            {
                var BooksObject = new Books()
                {
                    BookId = (int)RowOfDataTable["BookId"],
                    BookTitle = (string)RowOfDataTable["BookTitle"],
                    BookType = (string)RowOfDataTable["BookType"],
                    Author = (string)RowOfDataTable["Author"],
                    Quantity = (int)RowOfDataTable["Quantity"],
                    QuantityOnHand = (int)RowOfDataTable["QuantityOnHand"]
                };
                AllBooks.Add(BooksObject);
            }
            return AllBooks;
        }

        public void getBookDetailsBasedOnBookId() {
            string Query = "Select * From tblBooks Where BookId = "+BookId+"";
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            Con.Open();
            SqlDataReader DR = Cmd.ExecuteReader();
            DR.Read();
            BookTitle = DR.GetValue(1).ToString().Trim();
            BookType = DR.GetValue(2).ToString().Trim();
            Author = DR.GetValue(3).ToString().Trim();
            Quantity = int.Parse(DR.GetValue(4).ToString().Trim());
            Con.Close();
        }

        public int GetBookIdBasedOnBookTitle() {
            string Query = "Select BookId From tblBooks Where BookTitle='" + BookTitle + "'";
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            Con.Open();
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                int BId;
                int.TryParse(DR.GetValue(0).ToString(), out BId);
                Con.Close();
                return BId;
            }
            else
            {
                Con.Close();
                return 0;
            }
            
        }

        public void DescreaseQuantityOnHands() {
            string Query = "Update tblBooks Set QuantityOnHand = QuantityOnHand-1 Where BookId=" + BookId;
            Cmd.CommandText = Query;
            Con.Open();
            Cmd.Connection = Con;
            Cmd.ExecuteNonQuery();
            Con.Close();
        }
        public void IncreaseQuantityOnHands()
        {
            string Query = "Update tblBooks Set QuantityOnHand = QuantityOnHand + 1 Where BookId=" + BookId;
            Cmd.CommandText = Query;
            Con.Open();
            Cmd.Connection = Con;
            Cmd.ExecuteNonQuery();
            Con.Close();
        }

        public bool CheckBookAvailability() {

            BookId = GetBookIdBasedOnBookTitle();
            string Query = "Select QuantityOnHand From tblBooks Where BookId="+BookId;
            Cmd.CommandText = Query;
            Cmd.Connection = Con;
            Con.Open();
            SqlDataReader DR = Cmd.ExecuteReader();
            if (DR.HasRows)
            {
                DR.Read();
                int QuOnHand;
                int.TryParse(DR.GetValue(0).ToString().Trim(), out QuOnHand);
                Con.Close();
                if (QuOnHand > 0)
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
            else {
                return false;
            }
        }

        public int CompareTo([AllowNull] IBooks other)
        {
            throw new NotImplementedException();
        }
    }
}
