using System;
using System.Text;
using System.Windows;
using PractLibWPF1.ModelsDB;

namespace PractLibWPF1
{
    public partial class AddEditReader : Window
    {
        public AddEditReader()
        {
            InitializeComponent();
        }

        Pract4Libraly1Context _db = new Pract4Libraly1Context();
        Reader _reader;

        private void AddOrEditReader_Loaded(object sender, RoutedEventArgs e)
        {
            if (Data.reader == null)
            {
                ReaderAddEdit.Title = "Добавление читателя";
                btnAddEditReader.Content = "Добавить";
                _reader = new Reader();
                _reader.IsDeleted = false;
            }
            else
            {
                ReaderAddEdit.Title = "Редактирование читателя";
                btnAddEditReader.Content = "Редактировать";
                _reader = _db.Readers.Find(Data.reader.ReaderId);
            }
            ReaderAddEdit.DataContext = _reader;
        }

        private void btnCancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddEditReader_Clicked(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (tbCardNumber.Text.Length == 0) error.AppendLine("Введите номер читательского билета");
            if (tbFullName.Text.Length == 0) error.AppendLine("Введите ФИО");

            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString());
                return;
            }

            try
            {
                // Обновляем значения из полей
                _reader.LibraryCardNumber = tbCardNumber.Text.Trim();
                _reader.FullName = tbFullName.Text.Trim();
                _reader.Address = string.IsNullOrWhiteSpace(tbAddress.Text) ? null : tbAddress.Text.Trim();
                _reader.Phone = string.IsNullOrWhiteSpace(tbPhone.Text) ? null : tbPhone.Text.Trim();
                // IsDeleted уже привязан через Binding

                if (Data.reader == null)
                {
                    _db.Readers.Add(_reader);
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