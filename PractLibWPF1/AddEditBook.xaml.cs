using System;
using System.Text;
using System.Windows;
using PractLibWPF1.ModelsDB;

namespace PractLibWPF1
{
    public partial class AddEditBook : Window
    {
        public AddEditBook()
        {
            InitializeComponent();
        }

        Pract4Libraly1Context _db = new Pract4Libraly1Context();
        Book _book;

        private void AddOrEditBook_Loaded(object sender, RoutedEventArgs e)
        {
            if (Data.book == null)
            {
                BookAddEdit.Title = "Добавление книги";
                btnAddEditBook.Content = "Добавить";
                _book = new Book();
            }
            else
            {
                BookAddEdit.Title = "Редактирование книги";
                btnAddEditBook.Content = "Редактировать";
                _book = _db.Books.Find(Data.book.BookId);
            }
            BookAddEdit.DataContext = _book;
        }

        private void btnCancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddEditBook_Clicked(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (tbAuthor.Text.Length == 0) error.AppendLine("Введите автора");
            if (tbTitle.Text.Length == 0) error.AppendLine("Введите название");

            if (tbYear.Text.Length > 0)
            {
                if (!int.TryParse(tbYear.Text, out int year))
                {
                    error.AppendLine("Некорректный год");
                }
                else
                {
                    _book.YearPublished = year;
                }
            }
            else
            {
                _book.YearPublished = null;
            }

            if (tbPrice.Text.Length > 0)
            {
                if (!decimal.TryParse(tbPrice.Text, out decimal price))
                {
                    error.AppendLine("Некорректная цена");
                }
                else
                {
                    _book.Price = price;
                }
            }
            else
            {
                _book.Price = null;
            }

            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString());
                return;
            }

            try
            {
                if (Data.book == null)
                {
                    _db.Books.Add(_book);
                    _db.SaveChanges();
                }
                else
                {
                    _db.SaveChanges();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}