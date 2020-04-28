using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for AdminHomePage.xaml
    /// </summary>
    public partial class AdminHomePage : Window
    {
        string CurrentUserEmailId;
        private int currentUserId;
        private int issuedIdForBinding=1;
        public int IssuedIdForBinding{
            get => issuedIdForBinding;
            set => issuedIdForBinding = value;
        }
        public int CurrentUserId { get=>currentUserId; set=>currentUserId=value; }
        public AdminHomePage(int UId,string UserEmailId)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Title = UserEmailId + " Library Management System";
            issuedIdForBinding = 3;
            IssuedIdForBinding = 3;
            ManageSizeOfControls();
            CurrentUserEmailId = UserEmailId;
            currentUserId = UId;
            CurrentUserEmail.Content = CurrentUserEmailId;
            txtUserIdForIssueBook.Text = "User Id";
            txtFineAmountToPay.Text = "0";
            //txtOldPassword.Password = "Old Password";
            txtUpdateBookIdList();
            DisableAllUpdateControls();
            DisableAllDeleteControls();
            DisplayAllBooks();
            DisplayAllBooksInIssueBook();
            ViewIssuedBookData();
            ActiveManageIssueBooks();
            DisplayPendingAllBooks();
            DisplayAllIssuedBooks();
            DisplayAllActiveUsers();
            DisplayAllActiveUserDataInDeleteGridView();
            DisplayAllFines();
            DisplayRemainingFines();
            DisplayAllRemaingFines();
        }

        private void ManageSizeOfControls() {
            double Width = this.Width;
            double Height = this.Height;

            //Manage Books
            Width = Width * 65 / 100;
            Height = Height * 70 / 100;
            ManageBooksTabControl.Height = Height;
            ManageBooksTabControl.Width = Width;
            dgvViewBooks.Height = ManageBooksTabControl.Height * 70 / 100;
            dgvViewBooks.Width = ManageBooksTabControl.Width * 65 / 100;

            //ManageIssueBooks
            ManageIssueBooksTabControl.Height = Height;
            ManageIssueBooksTabControl.Width = Width;
            dgvIssueBookData.Width = ManageIssueBooksTabControl.Width * 42 / 100;
            dgvIssueBookData.Height = ManageIssueBooksTabControl.Height * 65 / 100;

            dgvReturnBookData.Width = ManageIssueBooksTabControl.Width * 42 / 100;
            dgvReturnBookData.Height = ManageIssueBooksTabControl.Height * 65 / 100;

            dgvViewPendingBooks.Height = ManageBooksTabControl.Height * 70 / 100;
            dgvViewPendingBooks.Width = ManageBooksTabControl.Width * 65 / 100;

            dgvViewAllIssuedBooks.Height = ManageBooksTabControl.Height * 70 / 100;
            dgvViewAllIssuedBooks.Width = ManageBooksTabControl.Width * 65 / 100;


            //Manage User
            ManageUsersTabControl.Height = Height;
            ManageUsersTabControl.Width = Width;

            dgvViewAllActiveUser.Height = ManageUsersTabControl.Height * 70 / 100;
            dgvViewAllActiveUser.Width = ManageUsersTabControl.Width * 65 / 100;

            dgvViewAllDeletedUser.Height = ManageUsersTabControl.Height * 70 / 100;
            dgvViewAllDeletedUser.Width = ManageUsersTabControl.Width * 65 / 100;

            dgvActiveUsersData.Width = ManageUsersTabControl.Width * 42 / 100;
            dgvActiveUsersData.Height = ManageUsersTabControl.Height * 65 / 100;

            dgvActiveUsersData.Width = ManageUsersTabControl.Width * 42 / 100;
            dgvActiveUsersData.Height = ManageUsersTabControl.Height * 65 / 100;

            //Setting
            ManageSettingTabControl.Height = Height;
            ManageSettingTabControl.Width = Width;

            //Manage Fines
            ManageFineTabControl.Height = Height;
            ManageFineTabControl.Width = Width;

            dgvAllFines.Height = ManageFineTabControl.Height * 70 / 100;
            dgvAllFines.Width = ManageFineTabControl.Width * 65 / 100;

            dgvRemainingFine.Height = ManageFineTabControl.Height * 70 / 100;
            dgvRemainingFine.Width = ManageFineTabControl.Width * 65 / 100;

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ManageSizeOfControls();   
        }

        //Issue Book Module
        //Issue Book
        private void txtUserIdForIssueBook_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUserIdForIssueBook.Text.Trim() == "User Id")
            {
                txtUserIdForIssueBook.Text = "";
                txtUserIdForIssueBook.Foreground = Brushes.Black;
            }
        }

        private void txtUserIdForIssueBook_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUserIdForIssueBook.Text.Trim() == "")
            {
                txtUserIdForIssueBook.Text = "User Id";
                txtUserIdForIssueBook.Foreground = Brushes.Black;
            }
        }

        private void btnIssueBook_Click(object sender, RoutedEventArgs e)
        {
            int UserId;
            if (int.TryParse(txtUserIdForIssueBook.Text.Trim().ToString(), out UserId))
            {
                Users U = new Users();
                U.UserId = UserId;
                if (U.CheckUserExistsOrNot())
                {
                    Books B = new Books();
                    B.BookTitle = txtIssueBookTitle.Text.ToString().Trim();
                    B.BookId = B.GetBookIdBasedOnBookTitle();
                    if (B.CheckBookAvailability())
                    {
                        IssuedBooks I = new IssuedBooks();
                        I.BookId = B.BookId;
                        I.UserId = U.UserId;
                        if (I.NumberOfIssuedBookByUser() >= 5)
                        {
                            MessageBox.Show("User Have Already Issued 5 Books.", "Limit Cross"!, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (I.IsBookAlreadyIssuedOrNot() == false)
                            {
                                if (I.IssueBook() == 1)
                                {
                                    B.DescreaseQuantityOnHands();
                                    MessageBox.Show("Book Issues Successfully.", "Book Issued"!, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Unable To Issue A Book!", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else if (I.IsBookAlreadyIssuedOrNot() == true)
                            {
                                MessageBox.Show("This Book Is Already Issued By This User.!", "Already Issued", MessageBoxButton.OK, MessageBoxImage.Information);
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("Book Is Not Available In Out Library.", "Book Not Available"!, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("User Does Not Exists!", "Invalid User", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            DisplayAllRemaingFines();
            DisplayAllFines();
            ViewIssuedBookData();
            DisplayAllBooksInIssueBook();
            DisplayRemainingFines();
            txtIssueBookIdOrTitle.Text = "";

        }
        private void DisplayIssueBooksSearch() {
            ObservableCollection<Books> IssueBookDataList = new ObservableCollection<Books>();
            Books B = new Books();
            IssueBookDataList = B.DisplaySearchedBooksOnlyIdOrTitle(txtIssueBookIdOrTitle.Text.Trim().ToString());
            dgvIssueBookData.ItemsSource = IssueBookDataList;

            if (IssueBookDataList.Count == 1)
            {
                txtIssueBookAuthor.Text = IssueBookDataList[0].Author.ToString().Trim();
                txtIssueBookTitle.Text = IssueBookDataList[0].BookTitle.ToString().Trim();
                txtIssueBookQuantityOnHands.Text = IssueBookDataList[0].QuantityOnHand.ToString().Trim();
                if (IssueBookDataList[0].BookType.ToString().Trim() == "Fiction Book")
                {
                    txtIssueBookType.SelectedIndex = 0;
                }
                else if (IssueBookDataList[0].BookType.ToString().Trim() == "Science Book")
                {
                    txtIssueBookType.SelectedIndex = 1;
                }
                else if (IssueBookDataList[0].BookType.ToString().Trim() == "Drama Book")
                {
                    txtIssueBookType.SelectedIndex = 2;
                }
                else if (IssueBookDataList[0].BookType.ToString().Trim() == "History Book")
                {
                    txtIssueBookType.SelectedIndex = 3;
                }
                else if (IssueBookDataList[0].BookType.ToString().Trim() == "Biography Book")
                {
                    txtIssueBookType.SelectedIndex = 4;
                }
                txtUserIdForIssueBook.IsEnabled = true;
                btnIssueBook.IsEnabled = true;
            }
            else {
                txtIssueBookAuthor.Text = "Author Name";
                txtIssueBookTitle.Text = "Book Title";
                txtIssueBookQuantityOnHands.Text ="Available Book";
                txtIssueBookType.SelectedIndex = -1;
                btnIssueBook.IsEnabled = false;
                txtUserIdForIssueBook.IsEnabled = false;
            }
        }
        private void DisplayAllBooksInIssueBook() {
            ObservableCollection<Books> IssueBookDataList = new ObservableCollection<Books>();
            Books B = new Books();
            IssueBookDataList = B.DisplayBooks();
            dgvIssueBookData.ItemsSource = IssueBookDataList;

            txtIssueBookAuthor.Text = "Author Name";
            txtIssueBookTitle.Text = "Book Title";
            txtIssueBookQuantityOnHands.Text = "Available Book";
            txtIssueBookType.SelectedIndex = -1;
            btnIssueBook.IsEnabled = false;
            txtUserIdForIssueBook.IsEnabled = false;
            txtUserIdForIssueBook.Text = "User Id";
        }
        private void txtIssueBookIdOrTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtIssueBookIdOrTitle.Text.Trim().Length == 0)
            {
                DisplayAllBooksInIssueBook();
            }
            else
            {
                DisplayIssueBooksSearch();
            }
        }
        private void txtUserIdForIssueBook_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Return Book
        private void ViewIssuedBookData() {
            IssuedBooks I = new IssuedBooks();
            DataTable DT = I.ViewAllCurrentIssueBooks();
            dgvReturnBookData.ItemsSource = DT.DefaultView;
        }
        private void txtReturnBookIdOrTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtReturnBookIdOrTitle.Text.Trim().Length == 0)
            {
                ViewIssuedBookData();
                txtReturnBookTitle.Text = "Book Title";
                txtIssuedOn.Text = "Issued On";
                txtIssuedBy.Text = "Issued By";
                txtFine.Text = "0";
                btnReturnBook.IsEnabled = false;
            }
            else
            {
                int IId = 0;
                double Fine;
                int.TryParse(txtReturnBookIdOrTitle.Text.Trim().ToString(), out IId);
                IssuedBooks I = new IssuedBooks();
                I.IssuedId = IId;
                DataTable DT = I.DisplaySearchedIssueBooksOnlyIdOrTitle();
                dgvReturnBookData.ItemsSource = DT.DefaultView;

                if (DT.Rows.Count == 1)
                {
                    txtReturnBookTitle.Text = DT.Rows[0].Field<string>(1);
                    DateTime IssDate = DT.Rows[0].Field<DateTime>(3);
                    txtIssuedOn.Text = IssDate.ToString("d");
                    txtIssuedBy.Text = DT.Rows[0].Field<string>(2);
                    if ((DateTime.Today - IssDate).TotalDays > 10)
                    {
                        Fine = (DateTime.Today - IssDate).TotalDays * 5;
                        txtFine.Text = Fine.ToString();
                    }
                    else
                    {
                        txtFine.Text = "0";
                    }
                    btnReturnBook.IsEnabled = true;
                }
                else
                {
                    txtReturnBookTitle.Text = "Book Title";
                    txtIssuedOn.Text = "Issued On";
                    txtIssuedBy.Text = "Issued By";
                    txtFine.Text = "0";
                    btnReturnBook.IsEnabled = false;
                }
            }

        }

        private void btnReturnBook_Click(object sender, RoutedEventArgs e)
        {
            IssuedBooks I = new IssuedBooks();
            int IId;
            int.TryParse(txtReturnBookIdOrTitle.Text.Trim(), out IId);
            I.IssuedId = IId;
            bool IFined;
            if (txtFine.Text.Trim() == "0")
            {
                IFined = false;
            }
            else
            {
                IFined = true;
                Fines F = new Fines();
                F.IssueId = I.IssuedId;
                F.FineAmount = float.Parse(txtFine.Text.ToString().Trim());
                F.RemainingFineAmount = F.FineAmount;
                F.AddNewFineEntry();
            }
            I.IsFined = IFined;
            if (I.ReturnBook() == 1)
            {
                Books B = new Books();
                B.BookTitle = txtReturnBookTitle.Text.Trim();
                B.BookId = B.GetBookIdBasedOnBookTitle();
                B.IncreaseQuantityOnHands();
                if (IFined == true)
                {
                    MessageBox.Show("Book Returned Successfully.\nUser Have To Pay $" + txtFine.Text.ToString().Trim() + " As Penalty For Late Returning Book.", "Book Returned", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Book Returned Successfully.", "Book Returned", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Unable To Return A Book!", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ViewIssuedBookData();
            DisplayRemainingFines();
            DisplayAllFines();
            txtReturnBookTitle.Text = "Book Title";
            txtIssuedOn.Text = "Issued On";
            txtIssuedBy.Text = "Issued By";
            txtFine.Text = "0";
            btnReturnBook.IsEnabled = false;
            txtReturnBookIdOrTitle.Text = "";
        }

        private void DisplayPendingAllBooks()
        {
            IssuedBooks I = new IssuedBooks();
            DataTable DT = I.ViewAllCurrentIssueBooks();
            dgvViewPendingBooks.ItemsSource = DT.DefaultView;
        }
        private void txtSearchPendingBook_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchPendingBook.Text.Trim() == "Issue Id, User Name, Book Name" || txtSearchPendingBook.Text.Trim() == "")
            {
                DisplayPendingAllBooks();
            }
            else
            {
                IssuedBooks I = new IssuedBooks();
                dgvViewPendingBooks.ItemsSource = I.ViewSearchedAllCurrentIssueBooks(txtSearchPendingBook.Text.Trim()).DefaultView;
            }

        }

        private void txtSearchPendingBook_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchPendingBook.Text.Trim() == "")
            {
                txtSearchPendingBook.Text = "Issue Id, User Name, Book Name";
                txtSearchPendingBook.Foreground = Brushes.DarkGray;
            }
        }

        private void txtSearchPendingBook_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchPendingBook.Text.Trim() == "Issue Id, User Name, Book Name")
            {
                txtSearchPendingBook.Text = "";
                txtSearchPendingBook.Foreground = Brushes.Black;
            }
        }

        private void DisplayAllIssuedBooks()
        {
            IssuedBooks I = new IssuedBooks();
            DataTable DT = I.DisplayAllIssuedBooks();
            dgvViewAllIssuedBooks.ItemsSource = DT.DefaultView;
        }
        private void txtSearchAllIssuedBook_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchAllIssuedBook.Text.Trim() == "Issue Id, User Name, Book Name" || txtSearchAllIssuedBook.Text.Trim() == "")
            {
                DisplayAllIssuedBooks();
            }
            else
            {
                IssuedBooks I = new IssuedBooks();
                dgvViewAllIssuedBooks.ItemsSource = I.ViewSearchedAllIssueBooks(txtSearchAllIssuedBook.Text.Trim()).DefaultView;
            }

        }

        private void txtSearchAllIssuedBook_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllIssuedBook.Text.Trim() == "")
            {
                txtSearchAllIssuedBook.Text = "Issue Id, User Name, Book Name";
                txtSearchPendingBook.Foreground = Brushes.DarkGray;
            }
        }

        private void txtSearchAllIssuedBook_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllIssuedBook.Text.Trim() == "Issue Id, User Name, Book Name")
            {
                txtSearchAllIssuedBook.Text = "";
                txtSearchAllIssuedBook.Foreground = Brushes.Black;
            }
        }

        private void txtReturnBookIdOrTitle_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Manage Book Module Starts From Here
        private void txtAddBookTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddBookTitle.Text.Trim() == "Book Title") {
                txtAddBookTitle.Text = "";
                txtAddBookTitle.Foreground = Brushes.Black;
            }
        }

        private void txtAddBookAuthor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddBookAuthor.Text.Trim() == "Author Name")
            {
                txtAddBookAuthor.Text = "";
                txtAddBookAuthor.Foreground = Brushes.Black;
            }
        }

        private void txtAddBookQuantity_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddBookQuantity.Text.Trim() == "0")
            {
                txtAddBookQuantity.Text = "";
                txtAddBookQuantity.Foreground = Brushes.Black;
            }
        }

        private void txtAddBookTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddBookTitle.Text.Trim() == "" || txtAddBookTitle.Text.Trim() == null) {
                txtAddBookTitle.Text = "Book Title";
                txtAddBookTitle.Foreground = Brushes.DarkGray;
            }
        }

        private void txtAddBookAuthor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddBookAuthor.Text.Trim() == "" || txtAddBookAuthor.Text.Trim() == null)
            {
                txtAddBookAuthor.Text = "Author Name";
                txtAddBookAuthor.Foreground = Brushes.DarkGray;
            }
        }

        private void txtAddBookQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddBookQuantity.Text.Trim() == "" || txtAddBookQuantity.Text.Trim() == null)
            {
                txtAddBookQuantity.Text = "0";
                txtAddBookQuantity.Foreground = Brushes.DarkGray;
            }
        }

        //Add New Book
        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            Books B = new Books();
            string ValidUserInputOrNot = B.ValidateUserInput(
                BookTitle: txtAddBookTitle.Text.Trim(),
                BookType: txtAddBookType.Text.ToString(),
                Quantity: txtAddBookQuantity.Text.Trim(),
                AuthorName: txtAddBookAuthor.Text.Trim());

            if ( ValidUserInputOrNot=="") {
                //All User Input Are Valid
                B.BookTitle = txtAddBookTitle.Text.Trim();
                B.BookType = txtAddBookType.Text.ToString().Trim();
                B.Author = txtAddBookAuthor.Text.Trim();
                B.Quantity = int.Parse(txtAddBookQuantity.Text.Trim());
                B.QuantityOnHand = B.Quantity;

                int Result = B.AddNewBook();
                if (Result == 1)
                {
                    MessageBox.Show("New Book Added Successfully", "Book Added", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtUpdateBookIdList();
                    DisplayAllBooks();
                }
                else {
                    MessageBox.Show("Unable To Add New Book!", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ClearAddBookForm();
            }
            else
            {
                MessageBox.Show(ValidUserInputOrNot,"Input Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void ClearAddBookForm()
        {
            txtAddBookAuthor.Text = "Author Name";
            txtAddBookAuthor.Foreground = Brushes.DarkGray;

            txtAddBookTitle.Text = "Book Title";
            txtAddBookTitle.Foreground = Brushes.DarkGray;

            txtAddBookQuantity.Text = "0";
            txtAddBookQuantity.Foreground = Brushes.DarkGray;

            txtAddBookType.SelectedIndex = 0;
        }
        private void txtUpdateBookIdList() {
            Books B = new Books();
            List<int> BooksId = new List<int>();
            BooksId = B.getAllBookId();
            txtUpdateBookId.ItemsSource = BooksId;
            txtUpdateBookId.SelectedIndex = 0;
            txtDeleteBookId.ItemsSource = BooksId;
            txtDeleteBookId.SelectedIndex = 0;
        }
        private void txtAddBookType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        //Update Book
        private void btnFetchBookData_Click(object sender, RoutedEventArgs e)
        {
            int BookId = int.Parse(txtUpdateBookId.Text.Trim());
            Books B = new Books();
            B.BookId = BookId;
            B.getBookDetailsBasedOnBookId();
            txtUpdateBookTitle.Text = B.BookTitle;
            if (B.BookType== "Fiction Book") {
                txtUpdateBookType.SelectedIndex = 0;
            }
            else if (B.BookType == "Science Book")
            {
                txtUpdateBookType.SelectedIndex = 1;
            }
            else if (B.BookType == "Drama Book")
            {
                txtUpdateBookType.SelectedIndex = 2;
            }
            else if (B.BookType == "History Book")
            {
                txtUpdateBookType.SelectedIndex = 3;
            }
            else if (B.BookType == "Biography Book")
            {
                txtUpdateBookType.SelectedIndex = 4;
            }

            txtUpdateBookAuthor.Text = B.Author;
            txtUpdateBookQuantity.Text = B.Quantity.ToString();
            EnableAllUpdateControls();
        }
        public void EnableAllUpdateControls() {
            txtUpdateBookTitle.IsEnabled = true;
            txtUpdateBookType.IsEnabled = true;
            txtUpdateBookAuthor.IsEnabled = true;
            txtUpdateBookQuantity.IsEnabled = true;
            btnUpdateBook.IsEnabled = true;
            txtUpdateBookAuthor.Foreground = Brushes.Black;
            txtAddBookTitle.Foreground = Brushes.Black;
            txtAddBookQuantity.Foreground = Brushes.Black;
        }
        public void DisableAllUpdateControls()
        {
            txtUpdateBookTitle.IsEnabled = false ;
            txtUpdateBookType.IsEnabled = false;
            txtUpdateBookAuthor.IsEnabled = false;
            txtUpdateBookQuantity.IsEnabled = false;
            btnUpdateBook.IsEnabled = false;
        }
        private void btnUpdateBook_Click(object sender, RoutedEventArgs e)
        {
            Books B = new Books();
            B.BookId = int.Parse(txtUpdateBookId.Text);
            string ValidUserInputOrNot = B.ValidateUserInput(
                BookTitle: txtUpdateBookTitle.Text.Trim(),
                BookType: txtUpdateBookType.Text.ToString(),
                Quantity: txtUpdateBookQuantity.Text.Trim(),
                AuthorName: txtUpdateBookAuthor.Text.Trim());

            if (ValidUserInputOrNot == "")
            {
                //All User Input Are Valid
                B.BookTitle = txtUpdateBookTitle.Text.Trim();
                B.BookType = txtUpdateBookType.Text.ToString().Trim();
                B.Author = txtUpdateBookAuthor.Text.Trim();
                B.Quantity = int.Parse(txtUpdateBookQuantity.Text.Trim());

                int Result = B.UpdateBook();
                if (Result == 1)
                {
                    MessageBox.Show("Book Info Updated Successfully", "Book Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtUpdateBookIdList();
                }
                else
                {
                    MessageBox.Show("Unable To Update Book Info!", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ClearUpdateBookForm();
                DisableAllUpdateControls();
            }
            else
            {
                MessageBox.Show(ValidUserInputOrNot, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearUpdateBookForm()
        {
            txtUpdateBookAuthor.Text = "Author Name";
            txtUpdateBookAuthor.Foreground = Brushes.DarkGray;

            txtUpdateBookTitle.Text = "Book Title";
            txtAddBookTitle.Foreground = Brushes.DarkGray;

            txtUpdateBookQuantity.Text = "0";
            txtAddBookQuantity.Foreground = Brushes.DarkGray;

            txtUpdateBookType.SelectedIndex = 0;
        }

        //Delete Book
        private void ClearDeleteBookForm()
        {
            txtDeleteBookAuthor.Text = "Author Name";
            txtDeleteBookAuthor.Foreground = Brushes.DarkGray;

            txtDeleteBookTitle.Text = "Book Title";
            txtDeleteBookTitle.Foreground = Brushes.DarkGray;

            txtDeleteBookQuantity.Text = "0";
            txtDeleteBookQuantity.Foreground = Brushes.DarkGray;

            txtDeleteBookType.SelectedIndex = 0;
        }
        public void EnableAllDeleteControls()
        {
            txtDeleteBookTitle.IsEnabled = true;
            txtDeleteBookType.IsEnabled = true;
            txtDeleteBookAuthor.IsEnabled = true;
            txtDeleteBookQuantity.IsEnabled = true;
            btnDeleteBook.IsEnabled = true;
            txtDeleteBookAuthor.Foreground = Brushes.Black;
            txtDeleteBookTitle.Foreground = Brushes.Black;
            txtDeleteBookQuantity.Foreground = Brushes.Black;
        }
        public void DisableAllDeleteControls()
        {
            txtDeleteBookTitle.IsEnabled = false;
            txtDeleteBookType.IsEnabled = false;
            txtDeleteBookAuthor.IsEnabled = false;
            txtDeleteBookQuantity.IsEnabled = false;
            btnDeleteBook.IsEnabled = false;
        }
        private void btnFetchBookDataForDelete_Click(object sender, RoutedEventArgs e)
        {
            int BookId = int.Parse(txtDeleteBookId.Text.Trim());
            Books B = new Books();
            B.BookId = BookId;
            B.getBookDetailsBasedOnBookId();
            txtDeleteBookTitle.Text = B.BookTitle;
            if (B.BookType == "Fiction Book")
            {
                txtDeleteBookType.SelectedIndex = 0;
            }
            else if (B.BookType == "Science Book")
            {
                txtDeleteBookType.SelectedIndex = 1;
            }
            else if (B.BookType == "Drama Book")
            {
                txtDeleteBookType.SelectedIndex = 2;
            }
            else if (B.BookType == "History Book")
            {
                txtDeleteBookType.SelectedIndex = 3;
            }
            else if (B.BookType == "Biography Book")
            {
                txtDeleteBookType.SelectedIndex = 4;
            }

            txtDeleteBookAuthor.Text = B.Author;
            txtDeleteBookQuantity.Text = B.Quantity.ToString();
            EnableAllDeleteControls();
        }
        private void btnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            Books B = new Books();
            B.BookId = int.Parse(txtDeleteBookId.Text.Trim());
            int Result = B.DeleteBook();
            if (Result == 1)
            {
                ClearDeleteBookForm();
                MessageBox.Show("Book Info Deleted Successfully", "Book Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                txtUpdateBookIdList();
            }
            else if (Result == 2)
            {
                MessageBox.Show("This Book Is Issued By Someone.", "Book Not Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                txtUpdateBookIdList();
            }
            else
            {
                MessageBox.Show("Unable To Delete Book Info!", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        //Display Books
        private void DisplayAllBooks() {
            ObservableCollection<Books> BooksList = new ObservableCollection<Books>();
            Books B = new Books();
            BooksList = B.DisplayBooks();
            dgvViewBooks.ItemsSource = BooksList;
        }
        private void txtSearchBook_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchBook.Text.Trim() == "Book Title, Book Author, Book Type" || txtSearchBook.Text.Trim() == "")
            {
                DisplayAllBooks();
            }
            else {
                ObservableCollection<Books> BooksList = new ObservableCollection<Books>();
                Books B = new Books();
                BooksList = B.DisplaySearchedBooks(txtSearchBook.Text.Trim());
                dgvViewBooks.ItemsSource = BooksList;
            }
            
        }
        private void txtSearchBook_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchBook.Text.Trim() == "") {
                txtSearchBook.Text = "Book Title, Book Author, Book Type";
                txtSearchBook.Foreground = Brushes.DarkGray;
            }
        }
        private void txtSearchBook_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchBook.Text.Trim() == "Book Title, Book Author, Book Type") {
                txtSearchBook.Text = "";
                txtSearchBook.Foreground = Brushes.Black;
            }
        }

        //Issue Or Return Book Module

        private void ActiveManageIssueBooks()
        {
            btnManageIssueBooks.Background = Brushes.AliceBlue;
            btnManageIssueBooks.FontSize = 18;
            btnManageIssueBooks.Foreground = new SolidColorBrush(Color.FromRgb(41, 53, 65));
        }
        private void DeactiveManageIssueBooks()
        {
            btnManageIssueBooks.Background = new SolidColorBrush(Color.FromRgb(41, 53, 65));
            btnManageIssueBooks.FontSize = 15;
            btnManageIssueBooks.Foreground = Brushes.AliceBlue;
        }
        private void btnManageIssueBooks_MouseEnter(object sender, MouseEventArgs e)
        {
            ActiveManageIssueBooks();
        }
        private void btnManageIssueBooks_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ManageIssueBooksTabControl.Visibility == Visibility.Hidden) {

                DeactiveManageIssueBooks();
            }
        }

        private void ActiveManageBooks()
        {
            btnManageBooks.Background = Brushes.AliceBlue;
            btnManageBooks.FontSize = 18;
            btnManageBooks.Foreground = new SolidColorBrush(Color.FromRgb(41, 53, 65));
        }

        private void DeactiveManageBooks()
        {
            btnManageBooks.Background = new SolidColorBrush(Color.FromRgb(41, 53, 65));
            btnManageBooks.FontSize = 15;
            btnManageBooks.Foreground = Brushes.AliceBlue;
        }
        private void btnManageBooks_MouseEnter(object sender, MouseEventArgs e)
        {
            ActiveManageBooks();
        }

        private void btnManageBooks_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ManageBooksTabControl.Visibility==Visibility.Hidden) {
                DeactiveManageBooks();
            }
        }

        private void ActiveManageUsers()
        {
            btnManageUsers.Background = Brushes.AliceBlue;
            btnManageUsers.FontSize = 18;
            btnManageUsers.Foreground = new SolidColorBrush(Color.FromRgb(41, 53, 65));
        }

        private void DeactiveManageUsers()
        {
            btnManageUsers.Background = new SolidColorBrush(Color.FromRgb(41, 53, 65));
            btnManageUsers.FontSize = 15;
            btnManageUsers.Foreground = Brushes.AliceBlue;
        }

        private void btnManageUsers_MouseEnter(object sender, MouseEventArgs e)
        {
            ActiveManageUsers();
        }
        private void btnManageUsers_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ManageUsersTabControl.Visibility == Visibility.Hidden) {
                DeactiveManageUsers();
            }
        }
        private void ActiveManageFines()
        {
            btnManageFines.Background = Brushes.AliceBlue;
            btnManageFines.FontSize = 18;
            btnManageFines.Foreground = new SolidColorBrush(Color.FromRgb(41, 53, 65));
        }
        private void DeactiveManageFines()
        {
            btnManageFines.Background = new SolidColorBrush(Color.FromRgb(41, 53, 65));
            btnManageFines.FontSize = 15;
            btnManageFines.Foreground = Brushes.AliceBlue;
        }
        private void btnManageFines_MouseEnter(object sender, MouseEventArgs e)
        {
            ActiveManageFines();
        }
        private void btnManageFines_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ManageFineTabControl.Visibility == Visibility.Hidden) {
                DeactiveManageFines();
            }
        }
        private void ActiveManageSetting()
        {
            btnSetting.Background = Brushes.AliceBlue;
            btnSetting.FontSize = 18;
            btnSetting.Foreground = new SolidColorBrush(Color.FromRgb(41, 53, 65));
        }
        private void DeactiveManageSetting()
        {
            btnSetting.Background = new SolidColorBrush(Color.FromRgb(41, 53, 65));
            btnSetting.FontSize = 15;
            btnSetting.Foreground = Brushes.AliceBlue;
        }
        
        private void btnSetting_MouseEnter(object sender, MouseEventArgs e)
        {
            ActiveManageSetting();
        }
        private void btnSetting_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ManageSettingTabControl.Visibility == Visibility.Hidden)
            {
                DeactiveManageSetting();
            }
        }

        //View Of Manage Books
        private void btnManageBooks_Click(object sender, RoutedEventArgs e)
        {
            ManageBooksTabControl.Visibility = Visibility.Visible;
            ManageIssueBooksTabControl.Visibility = Visibility.Hidden;
            ManageUsersTabControl.Visibility = Visibility.Hidden;
            ManageFineTabControl.Visibility = Visibility.Hidden;
            ManageSettingTabControl.Visibility = Visibility.Hidden;
            ActiveManageBooks();
            DeactiveManageFines();
            DeactiveManageIssueBooks();
            DeactiveManageSetting();
            DeactiveManageUsers();
        }
        private void btnManageIssueBooks_Click(object sender, RoutedEventArgs e)
        {
            ManageBooksTabControl.Visibility = Visibility.Hidden;
            ManageIssueBooksTabControl.Visibility = Visibility.Visible;
            ManageUsersTabControl.Visibility = Visibility.Hidden;
            ManageFineTabControl.Visibility = Visibility.Hidden;
            ManageSettingTabControl.Visibility = Visibility.Hidden;
            DeactiveManageBooks();
            DeactiveManageFines();
            ActiveManageIssueBooks();
            DeactiveManageSetting();
            DeactiveManageUsers();

        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            ManageBooksTabControl.Visibility = Visibility.Hidden;
            ManageIssueBooksTabControl.Visibility = Visibility.Hidden;
            ManageUsersTabControl.Visibility = Visibility.Hidden;
            ManageSettingTabControl.Visibility = Visibility.Visible;
            ManageFineTabControl.Visibility = Visibility.Hidden;
            DeactiveManageBooks();
            DeactiveManageFines();
            DeactiveManageIssueBooks();
            ActiveManageSetting();
            DeactiveManageUsers();
        }

        private void btnManageFines_Click(object sender, RoutedEventArgs e)
        {
            ManageBooksTabControl.Visibility = Visibility.Hidden;
            ManageIssueBooksTabControl.Visibility = Visibility.Hidden;
            ManageUsersTabControl.Visibility = Visibility.Hidden;
            ManageSettingTabControl.Visibility = Visibility.Hidden;
            ManageFineTabControl.Visibility = Visibility.Visible;
            DeactiveManageBooks();
            ActiveManageFines();
            DeactiveManageIssueBooks();
            DeactiveManageSetting();
            DeactiveManageUsers();
        }

        private void btnManageUsers_Click(object sender, RoutedEventArgs e)
        {
            ManageUsersTabControl.Visibility = Visibility.Visible;
            ManageBooksTabControl.Visibility = Visibility.Hidden;
            ManageIssueBooksTabControl.Visibility = Visibility.Hidden;
            ManageFineTabControl.Visibility = Visibility.Hidden;
            ManageSettingTabControl.Visibility = Visibility.Hidden;

            DeactiveManageBooks();
            DeactiveManageFines();
            DeactiveManageIssueBooks();
            DeactiveManageSetting();
            ActiveManageUsers();
        }

        private void txtAddUserFirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserFirstName.Text.Trim() == "First Name")
            {
                txtAddUserFirstName.Text = "";
                txtAddUserFirstName.Foreground = Brushes.Black;
            }
        }

        private void txtAddUserLastName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserLastName.Text.Trim() == "Last Name") {
                txtAddUserLastName.Text = "";
                txtAddUserLastName.Foreground = Brushes.Black;
            }
        }

        private void txtAddUserEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserEmail.Text.Trim() == "Email Id")
            {
                txtAddUserEmail.Text = "";
                txtAddUserEmail.Foreground = Brushes.Black;
            }
        }

        private void txtAddUserContact_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserContact.Text.Trim() == "Contact Number")
            {
                txtAddUserContact.Text = "";
                txtAddUserContact.Foreground = Brushes.Black;
            }
        }

        private void txtAddUserFirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserFirstName.Text.Trim() == "")
            {
                txtAddUserFirstName.Text = "First Name";
                txtAddUserFirstName.Foreground = Brushes.DarkGray;
            }
        }

        private void txtAddUserLastName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserLastName.Text.Trim() == "")
            {
                txtAddUserLastName.Text = "Last Name";
                txtAddUserLastName.Foreground = Brushes.DarkGray;
            }
        }

        private void txtAddUserEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserEmail.Text.Trim() == "")
            {
                txtAddUserEmail.Text = "Email Id";
                txtAddUserEmail.Foreground = Brushes.DarkGray;
            }
        }

        private void txtAddUserContact_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddUserContact.Text.Trim() == "")
            {
                txtAddUserContact.Text = "Contact Number";
                txtAddUserContact.Foreground = Brushes.DarkGray;
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            Users U = new Users();
            U.FirstName = txtAddUserFirstName.Text.Trim();
            U.LastName = txtAddUserLastName.Text.Trim();
            U.EmailId = txtAddUserEmail.Text.Trim();
            U.ContactNumber=txtAddUserContact.Text.Trim();
            if (txtAddUserType.Text.Trim() == "Student")
            {
                U.Role = 'S';
            }
            else if (txtAddUserType.Text.Trim() == "Faculty")
            {
                U.Role = 'F';
            }
            else if (txtAddUserType.Text.Trim() == "Admin")
            {
                U.Role = 'A';
            }
            else if (txtAddUserType.Text.Trim() == "Librarian")
            {
                U.Role = 'L';
            }
            string Result = U.ValidateAddUserInput();
            if (Result == "")
            {
                if (U.CheckUserEmailOrContactExistsOrNot())
                {
                    //User Data Is Already Saved!
                    MessageBox.Show("User Exists Already!", "Already Exists", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    if (U.AddNewUser())
                    {
                        MessageBox.Show("User Info Added Successfully.", "User Added", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else {
                        MessageBox.Show("Unable To Add User Info!", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    ClearAddUserForm();
                }
            }
            else {
                MessageBox.Show(Result, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            DisplayAllActiveUsers();
            DisplayAllActiveUserDataInDeleteGridView();
        }
    
        private void ClearAddUserForm() {
            txtAddUserFirstName.Text = "First Name";
            txtAddUserLastName.Text = "Last Name";
            txtAddUserEmail.Text = "Email Id";
            txtAddUserContact.Text = "Contact Number";
            txtAddUserType.SelectedIndex = 0;

            txtAddUserFirstName.Foreground = Brushes.DarkGray;
            txtAddUserLastName.Foreground = Brushes.DarkGray;
            txtAddUserEmail.Foreground = Brushes.DarkGray;
            txtAddUserContact.Foreground = Brushes.DarkGray;

        }

        private void txtAddUserContact_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Active User Code
        private void DisplayAllActiveUsers()
        {
            Users U = new Users();
            DataTable DT = U.DisplayActiveUserList();
            dgvViewAllActiveUser.ItemsSource = DT.DefaultView;
        }

        private void txtSearchAllActiveUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchAllActiveUser.Text.Trim() == "User Name, User Email, Contact Number" || txtSearchAllActiveUser.Text.Trim() == "")
            {//Display All Users
                DisplayAllActiveUsers();
            }
            else
            {
                Users U = new Users();
                DataTable DT = U.DisplayActiveSearchUserList(txtSearchAllActiveUser.Text.Trim());
                dgvViewAllActiveUser.ItemsSource = DT.DefaultView;
            }

        }

        private void txtSearchAllActiveUser_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllActiveUser.Text.Trim() == "")
            {
                txtSearchAllActiveUser.Text = "User Name, User Email, Contact Number";
                txtSearchAllActiveUser.Foreground = Brushes.DarkGray;
            }
        }

        private void txtSearchAllActiveUser_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllActiveUser.Text.Trim() == "User Name, User Email, Contact Number")
            {
                txtSearchAllActiveUser.Text = "";
                txtSearchAllActiveUser.Foreground = Brushes.Black;
            }
        }

        //Deleted User Code
        private void DisplayAllDeletedUsers()
        {
            Users U = new Users();
            DataTable DT = U.DisplayDeletedUserList();
            dgvViewAllDeletedUser.ItemsSource = DT.DefaultView;
        }

        private void txtSearchAllDeletedUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchAllDeletedUser.Text.Trim() == "User Name, User Email, Contact Number" || txtSearchAllDeletedUser.Text.Trim() == "")
            {//Display All Users
                DisplayAllDeletedUsers();
            }
            else
            {
                Users U = new Users();
                DataTable DT = U.DisplayDeletedSearchUserList(txtSearchAllActiveUser.Text.Trim());
                dgvViewAllDeletedUser.ItemsSource = DT.DefaultView;
            }

        }

        private void txtSearchAllDeletedUser_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllDeletedUser.Text.Trim() == "")
            {
                txtSearchAllDeletedUser.Text = "User Name, User Email, Contact Number";
                txtSearchAllDeletedUser.Foreground = Brushes.DarkGray;
            }
        }

        private void txtSearchAllDeletedUser_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllDeletedUser.Text.Trim() == "User Name, User Email, Contact Number")
            {
                txtSearchAllDeletedUser.Text = "";
                txtSearchAllDeletedUser.Foreground = Brushes.Black;
            }
        }

        private void DisplayAllActiveUserDataInDeleteGridView() {
            Users U = new Users();
            DataTable DT = U.DisplayActiveUserList(); ;
            dgvActiveUsersData.ItemsSource = DT.DefaultView;
        }

        private void txtDeleteUserId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtDeleteUserId.Text.Trim().Length == 0)
            {
                DisplayAllActiveUserDataInDeleteGridView();
                txtDeleteUserContactNumber.Text = "0000000000";
                txtDeleteUserEmailId.Text = "Email Id";
                txtDeleteUserFirstName.Text = "First Name";
                txtDeleteUserLastName.Text = "Last Name";
                txtDeleteUserType.SelectedIndex = -1;
                btnDeleteUser.IsEnabled = false;
            }
            else
            {
                Users U = new Users();

                DataTable DT = U.DisplayActiveSearchUserList(txtDeleteUserId.Text.Trim()); ;
                dgvActiveUsersData.ItemsSource = DT.DefaultView;

                if (DT.Rows.Count == 1)
                {
                    txtDeleteUserFirstName.Text = DT.Rows[0].Field<string>(1) ;
                    txtDeleteUserLastName.Text = DT.Rows[0].Field<string>(2);
                    txtDeleteUserEmailId.Text = DT.Rows[0].Field<string>(3) ;
                    txtDeleteUserContactNumber.Text = DT.Rows[0].Field<string>(4);
                    string Role = DT.Rows[0].Field<string>(5).Trim();
                    char R;
                    char.TryParse(Role ,out R);
                    if (R == 'S')
                    {
                        txtDeleteUserType.SelectedIndex = 0;
                    }
                    else if (R == 'F') {
                        txtDeleteUserType.SelectedIndex = 1;
                    }
                    else if (R == 'L')
                    {
                        txtDeleteUserType.SelectedIndex = 2;
                    }
                    else if (R == 'A')
                    {
                        txtDeleteUserType.SelectedIndex = 3;
                    }
                    btnDeleteUser.IsEnabled = true;
                }
                else
                {
                    txtDeleteUserContactNumber.Text = "0000000000";
                    txtDeleteUserEmailId.Text = "Email Id";
                    txtDeleteUserFirstName.Text = "First Name";
                    txtDeleteUserLastName.Text = "Last Name";
                    txtDeleteUserType.SelectedIndex = -1;
                    btnDeleteUser.IsEnabled = false;
                }
            }

        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            Users U = new Users();
            U.EmailId = txtDeleteUserEmailId.Text.Trim();
            U.UserId = U.GetUserIdBasedOUserEmail();
            if (U.DeleteUser())
            {
                MessageBox.Show("User Deleted Successfully.", "User Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else {
                MessageBox.Show("Unable To Delete User.", "User Not Deleted", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            DisplayAllActiveUsers();
            DisplayAllActiveUserDataInDeleteGridView();
            txtDeleteUserContactNumber.Text = "0000000000";
            txtDeleteUserEmailId.Text = "Email Id";
            txtDeleteUserFirstName.Text = "First Name";
            txtDeleteUserLastName.Text = "Last Name";
            txtDeleteUserType.SelectedIndex = -1;
            btnDeleteUser.IsEnabled = false;


        }
        

        //Manage Setting Module
        

        private void txtOldPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtOldPassword.Password.Trim() == "")
            {
                txtOldPassword.Password = "Old Password";
                txtOldPassword.Foreground = Brushes.DarkGray;
            }
        }

        private void txtOldPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtOldPassword.Password.Trim() == "Old Password")
            {
                txtOldPassword.Password = "";
                txtOldPassword.Foreground = Brushes.Black;
            }
        }

        private void txtNewPassword1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNewPassword1.Password.Trim() == "")
            {
                txtNewPassword1.Password = "New Password";
                txtNewPassword1.Foreground = Brushes.DarkGray;
            }
        }

        private void txtNewPassword1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNewPassword1.Password.Trim() == "New Password")
            {
                txtNewPassword1.Password = "";
                txtNewPassword1.Foreground = Brushes.Black;
            }
        }
        private void txtNewPassword2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNewPassword2.Password.Trim() == "")
            {
                txtNewPassword2.Password = "Re-enter New Password";
                txtNewPassword2.Foreground = Brushes.DarkGray;
            }
        }

        private void txtNewPassword2_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNewPassword2.Password.Trim() == "Re-enter New Password")
            {
                txtNewPassword2.Password = "";
                txtNewPassword2.Foreground = Brushes.Black;
            }
        }

        private void txtNewPassword1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if ( txtNewPassword1.Password.Length < 4 || ( txtNewPassword1.Password.ToString() != txtNewPassword2.Password.ToString()))
            {
                btnChangePassword.IsEnabled = false;
            }
            else {
                btnChangePassword.IsEnabled = true;
            }
        }

        private void txtNewPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtNewPassword2.Password.Length < 4 || (txtNewPassword1.Password.ToString() != txtNewPassword2.Password.ToString()))
            {
                btnChangePassword.IsEnabled = false;
            }
            else
            {
                btnChangePassword.IsEnabled = true;
            }
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            Users U = new Users();
            U.UserId = currentUserId;
            if (U.CheckCurrentPassword(txtOldPassword.Password.ToString()))
            {
                if (U.ChangePassword(txtNewPassword1.Password.ToString()))
                {
                    MessageBox.Show("Password Changed Successfully", "Password Changed", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("Unable To Change Password", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                txtNewPassword1.Password = "";
                txtNewPassword2.Password = "";
                txtOldPassword.Password = "";
                btnChangePassword.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Please Enter Correct Your Current Password", "Incorrect Current Password", MessageBoxButton.OK, MessageBoxImage.Error);
                txtOldPassword.Password = "";
            }
            //Users U = new Users();
            //U.UserId = currentUserId;
            //if (U.CheckCurrentPassword(txtOldPassword.Text))
            //{
            //    if (U.ChangePassword(txtOldPassword.Text))
            //    {
            //        MessageBox.Show("Password Changed Successfully", "Password Changed", MessageBoxButton.OK, MessageBoxImage.Information);

            //    }
            //    else
            //    {
            //        MessageBox.Show("Unable To Change Password", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);

            //    }
            //    txtNewPassword1.Password = "";
            //    txtNewPassword2.Password = "";
            //    txtOldPassword.Text = "";
            //    btnChangePassword.IsEnabled = false;
            //}
            //else
            //{
            //    MessageBox.Show("Please Enter Correct Your Current Password", "Incorrect Current Password", MessageBoxButton.OK, MessageBoxImage.Error);
            //    txtOldPassword.Text = "";
            //}
        }



        //private void txtIssueBookIdOrTitle_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (txtIssueBookIdOrTitle.Text.Trim() == "") {
        //        txtIssueBookIdOrTitle.Text = "Book Id Or Title";
        //        txtIssueBookIdOrTitle.Foreground = Brushes.DarkGray;
        //    }
        //}

        //private void txtIssueBookIdOrTitle_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (txtIssueBookIdOrTitle.Text.Trim() == "Book Id Or Title") {
        //        txtIssueBookIdOrTitle.Text = "Book Id Or Title";
        //        txtIssueBookIdOrTitle.Foreground = Brushes.Black;
        //    }
        //}

        //View Of Manage Issue Books


        //ManageFines

        //Display All Fines
        private void DisplayAllFines() {
            Fines F = new Fines();
            dgvAllFines.ItemsSource = F.GetAllFineAmountList().DefaultView;
        }
        private void txtSearchAllFines_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchAllFines.Text.Trim() == "User Name, Book Title, Email Id, Issue Id" || txtSearchAllFines.Text.Trim() == "")
            {
                DisplayAllFines();
            }
            else
            {
                Fines F = new Fines();
                dgvAllFines.ItemsSource = F.GetAllFineAmountListSearch(txtSearchAllFines.Text.Trim().ToString()).DefaultView;
            }

        }

        private void txtSearchAllFines_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllFines.Text.Trim() == "")
            {
                txtSearchAllFines.Text = "User Name, Book Title, Email Id, Issue Id";
                txtSearchAllFines.Foreground = Brushes.DarkGray;
            }
        }

        private void txtSearchAllFines_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchAllFines.Text.Trim() == "User Name, Book Title, Email Id, Issue Id")
            {
                txtSearchAllFines.Text = "";
                txtSearchAllFines.Foreground = Brushes.Black;
            }
        }


        //Display Remaing fine
        private void DisplayRemainingFines()
        {
            Fines F = new Fines();
            dgvRemainingFine.ItemsSource = F.GetRemainingFineAmountList().DefaultView;
        }
        private void txtSearchRemainingFines_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchRemainingFines.Text.Trim() == "User Name, Book Title, Email Id, Issue Id" || txtSearchAllFines.Text.Trim() == "")
            {
                DisplayRemainingFines();
            }
            else
            {
                Fines F = new Fines();
                dgvRemainingFine.ItemsSource = F.GetRemainingFineAmountSearch(txtSearchRemainingFines.Text.Trim().ToString()).DefaultView;
            }

        }

        private void txtSearchRemainingFines_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchRemainingFines.Text.Trim() == "")
            {
                txtSearchRemainingFines.Text = "User Name, Book Title, Email Id, Issue Id";
                txtSearchRemainingFines.Foreground = Brushes.DarkGray;
            }
        }

        private void txtSearchRemainingFines_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearchRemainingFines.Text.Trim() == "User Name, Book Title, Email Id, Issue Id")
            {
                txtSearchRemainingFines.Text = "";
                txtSearchRemainingFines.Foreground = Brushes.Black;
            }
        }

        //Accept Fine

        private void txtAcceptFineSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAcceptFineSearch.Text.Trim().Length == 0)
            {
                DisplayAllRemaingFines();
                txtAcceptFineBookTitle.Text = "Book Title";
                txtAcceptFineIssuedBy.Text = "Issued By";
                txtAcceptFineTotalFine.Text = "0";
                txtAcceptFineRemainingFine.Text = "0";
                issuedIdForBinding = 0;
                txtFineAmountToPay.IsEnabled = false;
                btnAcceptFine.IsEnabled = false;
            }
            else
            {
                Fines F = new Fines();
                DataTable DT = F.GetRemainingFineAmountSearch(txtAcceptFineSearch.Text.Trim());
                dgvAcceptFineRemainingFine.ItemsSource = DT.DefaultView;

                if (DT.Rows.Count == 1)
                {
                    txtAcceptFineBookTitle.Text = DT.Rows[0].Field<string>(3); ;
                    txtAcceptFineIssuedBy.Text = DT.Rows[0].Field<string>(1) ;
                    txtAcceptFineTotalFine.Text = DT.Rows[0].Field<double>(6).ToString();
                    txtAcceptFineRemainingFine.Text = DT.Rows[0].Field<double>(5).ToString();
                    txtFineAmountToPay.IsEnabled = true;
                    int IssId;
                    int.TryParse(DT.Rows[0].Field<int>(0).ToString(),out IssId);
                    issuedIdForBinding = IssId;
                    MessageBox.Show(issuedIdForBinding.ToString());
                    btnAcceptFine.IsEnabled = true;
                }
                else {
                    txtAcceptFineBookTitle.Text = "Book Title";
                    txtAcceptFineIssuedBy.Text = "Issued By";
                    txtAcceptFineTotalFine.Text = "0";
                    txtAcceptFineRemainingFine.Text = "0";
                    txtFineAmountToPay.IsEnabled = false;
                    issuedIdForBinding = 0;
                    btnAcceptFine.IsEnabled = false;
                }

            }
        }

        private void DisplayAllRemaingFines()
        {
            Fines F = new Fines();
            dgvAcceptFineRemainingFine.ItemsSource = F.GetRemainingFineAmountList().DefaultView;
        }

        private void txtFineAmountToPay_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtFineAmountToPay.Text.Trim() == "0") {
                txtFineAmountToPay.Text = "";
                txtFineAmountToPay.Foreground = Brushes.Black;
            }
        }

        private void txtFineAmountToPay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtFineAmountToPay.Text.Trim() == "") {
                txtFineAmountToPay.Text = "0";
                txtFineAmountToPay.Foreground = Brushes.DarkGray;
            }
        }

        private void btnAcceptFine_Click(object sender, RoutedEventArgs e)
        {
            double EnteredAmount;
            double PendingAmount;

            if (double.TryParse(txtAcceptFineRemainingFine.Text.Trim(), out PendingAmount) &&
            double.TryParse(txtFineAmountToPay.Text.Trim(), out EnteredAmount))
            {
                if (EnteredAmount > PendingAmount)
                {
                    MessageBox.Show("Your Pending Amount Is " + PendingAmount, "Entered High Value", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtFineAmountToPay.Text = "";
                }
                else {
                    Fines F = new Fines();
                    F.IssueId = IssuedIdForBinding;
                    if (F.AcceptFine(EnteredAmount)) {
                        MessageBox.Show("Fine Received Successfully For Issue Id " + IssuedIdForBinding + "." ,"Fine Received",MessageBoxButton.OK,MessageBoxImage.Information);
                    }
                }
            }
            else {
                MessageBox.Show("Invalid Amount Entered!","Invalid Input",MessageBoxButton.OK,MessageBoxImage.Error);
                txtFineAmountToPay.Text = "";
            }
            DisplayAllRemaingFines();
            DisplayRemainingFines();
            DisplayAllFines();
            txtAcceptFineSearch.Text = "";
            txtAcceptFineBookTitle.Text = "Book Title";
            txtAcceptFineIssuedBy.Text = "Issued By";
            txtAcceptFineTotalFine.Text = "0";
            txtAcceptFineRemainingFine.Text = "0";
            issuedIdForBinding = 0;
            txtFineAmountToPay.IsEnabled = false;
            btnAcceptFine.IsEnabled = false;
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }
    }
}
