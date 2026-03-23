using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PractLibWPF1.ModelsDB;

namespace PractLibWPF1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DBLoaded_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooksInDG();
            LoadReadersInDG();
        }

        void LoadBooksInDG()
        {
            using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
            {
                var books = _db.Books
                    .Select(b => new
                    {
                        b.BookId,
                        b.Author,
                        b.Title,
                        b.YearPublished,
                        b.Price,
                        b.Annotation,
                        TotalCopies = _db.BookCopies.Count(c => c.BookId == b.BookId),
                        AvailableCopies = _db.BookCopies.Count(c => c.BookId == b.BookId && c.IsAvailable == true)
                    })
                    .ToList();
                DGBooks.ItemsSource = books;
            }
        }

        void LoadReadersInDG()
        {
            using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
            {
                DGReaders.ItemsSource = _db.Readers.ToList();
            }
        }

        // ЗАПРОС 1: Книга с максимальным количеством экземпляров на полках
        private void btnMostCopiesOnShelf_Clicked(object sender, RoutedEventArgs e)
        {
            using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
            {
                var result = _db.Books
                    .Select(b => new
                    {
                        b.BookId,
                        b.Author,
                        b.Title,
                        b.YearPublished,
                        b.Price,
                        AvailableCopies = _db.BookCopies.Count(c => c.BookId == b.BookId && c.IsAvailable == true),
                        TotalCopies = _db.BookCopies.Count(c => c.BookId == b.BookId)
                    })
                    .Where(b => b.AvailableCopies > 0)
                    .OrderByDescending(b => b.AvailableCopies)
                    .FirstOrDefault();

                if (result != null)
                {
                    MessageBox.Show($"Книга с наибольшим количеством экземпляров на полках:\n\n" +
                                    $"Название: {result.Title}\n" +
                                    $"Автор: {result.Author}\n" +
                                    $"Всего экземпляров: {result.TotalCopies}\n" +
                                    $"На полках: {result.AvailableCopies}",
                                    "Результат запроса",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    var book = _db.Books
                        .Select(b => new
                        {
                            b.BookId,
                            b.Author,
                            b.Title,
                            b.YearPublished,
                            b.Price,
                            b.Annotation,
                            TotalCopies = _db.BookCopies.Count(c => c.BookId == b.BookId),
                            AvailableCopies = _db.BookCopies.Count(c => c.BookId == b.BookId && c.IsAvailable == true)
                        })
                        .Where(b => b.BookId == result.BookId)
                        .ToList();

                    DGBooks.ItemsSource = book;
                }
                else
                {
                    MessageBox.Show("Книги не найдены", "Результат запроса", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // ЗАПРОС 3: Самая популярная книга весной 2000 года
        private void btnMostPopularSpring2000_Clicked(object sender, RoutedEventArgs e)
        {
            using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
            {
                var result = _db.Books
                    .Select(b => new
                    {
                        b.BookId,
                        b.Author,
                        b.Title,
                        b.YearPublished,
                        b.Price,
                        IssuesCount = _db.Issues.Count(i =>
                            i.Copy.BookId == b.BookId &&
                            i.IssueDate >= new DateOnly(2000, 3, 1) &&
                            i.IssueDate <= new DateOnly(2000, 5, 31))
                    })
                    .Where(b => b.IssuesCount > 0)
                    .OrderByDescending(b => b.IssuesCount)
                    .FirstOrDefault();

                if (result != null)
                {
                    MessageBox.Show($"Самая популярная книга весной 2000 года:\n\n" +
                                    $"Название: {result.Title}\n" +
                                    $"Автор: {result.Author}\n" +
                                    $"Количество выдач: {result.IssuesCount}",
                                    "Результат запроса",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    var book = _db.Books
                        .Select(b => new
                        {
                            b.BookId,
                            b.Author,
                            b.Title,
                            b.YearPublished,
                            b.Price,
                            b.Annotation,
                            TotalCopies = _db.BookCopies.Count(c => c.BookId == b.BookId),
                            AvailableCopies = _db.BookCopies.Count(c => c.BookId == b.BookId && c.IsAvailable == true)
                        })
                        .Where(b => b.BookId == result.BookId)
                        .ToList();

                    DGBooks.ItemsSource = book;
                }
                else
                {
                    MessageBox.Show("Книги за весну 2000 года не найдены", "Результат запроса", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // ЗАПРОС 2: Читатели с задолженностью более 4 месяцев
        private void btnDebtors4Months_Clicked(object sender, RoutedEventArgs e)
        {
            using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
            {
                // Вычисляем дату 4 месяца назад на C#
                DateOnly fourMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));

                // Получаем должников из БД
                var debtorsData = _db.Issues
                    .Where(i => i.ReturnDate == null && i.IssueDate <= fourMonthsAgo)
                    .Select(i => new
                    {
                        i.Reader.ReaderId,
                        i.Reader.LibraryCardNumber,
                        i.Reader.FullName,
                        i.Reader.Address,
                        i.Reader.Phone,
                        i.Reader.IsDeleted,
                        IssueDate = i.IssueDate
                    })
                    .ToList(); // Выполняем запрос здесь

                // Вычисляем дни просрочки на клиенте
                var debtors = debtorsData
                    .Select(d => new
                    {
                        d.ReaderId,
                        d.LibraryCardNumber,
                        d.FullName,
                        d.Address,
                        d.Phone,
                        d.IsDeleted,
                        d.IssueDate,
                        DaysOverdue = DateOnly.FromDateTime(DateTime.Now).DayNumber - d.IssueDate.DayNumber
                    })
                    .OrderByDescending(d => d.DaysOverdue)
                    .ToList();

                if (debtors.Count > 0)
                {
                    string message = "Читатели с задолженностью более 4 месяцев:\n\n";
                    foreach (var debtor in debtors)
                    {
                        message += $"ФИО: {debtor.FullName}\n" +
                                  $"№ билета: {debtor.LibraryCardNumber}\n" +
                                  $"Телефон: {debtor.Phone ?? "не указан"}\n" +
                                  $"Дата выдачи: {debtor.IssueDate:dd.MM.yyyy}\n" +
                                  $"Дней просрочки: {debtor.DaysOverdue}\n" +
                                  $"----------------------------------------\n";
                    }

                    MessageBox.Show(message, "Результат запроса", MessageBoxButton.OK, MessageBoxImage.Information);

                    var debtorsList = debtors.Select(d => new Reader
                    {
                        ReaderId = d.ReaderId,
                        LibraryCardNumber = d.LibraryCardNumber,
                        FullName = d.FullName,
                        Address = d.Address,
                        Phone = d.Phone,
                        IsDeleted = d.IsDeleted
                    }).ToList();

                    DGReaders.ItemsSource = debtorsList;
                }
                else
                {
                    MessageBox.Show("Читателей с задолженностью более 4 месяцев не найдено",
                                  "Результат запроса", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // ЗАПРОС 4: Читатели с книгами на сумму более 100 руб.
        private void btnTotalPriceMore100_Clicked(object sender, RoutedEventArgs e)
        {
            using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
            {
                var readersWithBooks = _db.Issues
                    .Where(i => i.ReturnDate == null)
                    .GroupBy(i => i.Reader)
                    .Select(g => new
                    {
                        Reader = g.Key,
                        TotalPrice = g.Sum(i => i.Copy.Book.Price ?? 0),
                        BooksCount = g.Count()
                    })
                    .Where(r => r.TotalPrice > 100)
                    .OrderByDescending(r => r.TotalPrice)
                    .ToList();

                if (readersWithBooks.Count > 0)
                {
                    string message = "Читатели с книгами на сумму более 100 руб.:\n\n";
                    foreach (var item in readersWithBooks)
                    {
                        message += $"ФИО: {item.Reader.FullName}\n" +
                                  $"№ билета: {item.Reader.LibraryCardNumber}\n" +
                                  $"Телефон: {item.Reader.Phone ?? "не указан"}\n" +
                                  $"Общая сумма: {item.TotalPrice:C}\n" +
                                  $"Книг на руках: {item.BooksCount}\n" +
                                  $"----------------------------------------\n";
                    }

                    MessageBox.Show(message, "Результат запроса", MessageBoxButton.OK, MessageBoxImage.Information);

                    var readersList = readersWithBooks.Select(r => r.Reader).ToList();
                    DGReaders.ItemsSource = readersList;
                }
                else
                {
                    MessageBox.Show("Читателей с книгами на сумму более 100 руб. не найдено",
                                  "Результат запроса", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Быстрый поиск по книгам (по названию)
        private void tbBookFindTitle_Changed(object sender, TextChangedEventArgs e)
        {
            List<dynamic> books = (List<dynamic>)DGBooks.ItemsSource;
            var findBook = books.FirstOrDefault(p => p.Title?.Contains(tbBookFindTitle.Text) == true);
            if (findBook != null)
            {
                DGBooks.SelectedItem = findBook;
                DGBooks.ScrollIntoView(findBook);
            }
        }

        // Быстрый поиск по читателям (по ФИО)
        private void tbReaderFindName_Changed(object sender, TextChangedEventArgs e)
        {
            List<Reader> readers = (List<Reader>)DGReaders.ItemsSource;
            var findReader = readers.FirstOrDefault(p => p.FullName?.Contains(tbReaderFindName.Text) == true);
            if (findReader != null)
            {
                DGReaders.SelectedItem = findReader;
                DGReaders.ScrollIntoView(findReader);
            }
        }

        // Кнопки для книг
        private void btnAddBook_Clicked(object sender, RoutedEventArgs e)
        {
            Data.book = null;
            AddEditBook addEditBook = new AddEditBook();
            addEditBook.Owner = this;
            addEditBook.ShowDialog();
            LoadBooksInDG();
        }

        private void btnUpdateBook_Clicked(object sender, RoutedEventArgs e)
        {
            if (DGBooks.SelectedItem != null)
            {
                var selectedBook = (dynamic)DGBooks.SelectedItem;
                using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
                {
                    Data.book = _db.Books.Find(selectedBook.BookId);
                    AddEditBook addEditBook = new AddEditBook();
                    addEditBook.Owner = this;
                    addEditBook.ShowDialog();
                    LoadBooksInDG();
                }
            }
        }

        private void btnDeleteBook_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show("Удалить книгу?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var selectedBook = (dynamic)DGBooks.SelectedItem;
                    if (selectedBook != null)
                    {
                        using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
                        {
                            var book = _db.Books.Find(selectedBook.BookId);
                            if (book != null)
                            {
                                _db.Books.Remove(book);
                                _db.SaveChanges();
                            }
                        }
                        LoadBooksInDG();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
            else DGBooks.Focus();
        }

       
        private void btnAddReader_Clicked(object sender, RoutedEventArgs e)
        {
            Data.reader = null;
            AddEditReader addEditReader = new AddEditReader();
            addEditReader.Owner = this;
            addEditReader.ShowDialog();
            LoadReadersInDG();
        }

        private void btnUpdateReader_Clicked(object sender, RoutedEventArgs e)
        {
            if (DGReaders.SelectedItem != null)
            {
                Data.reader = (Reader)DGReaders.SelectedItem;
                AddEditReader addEditReader = new AddEditReader();
                addEditReader.Owner = this;
                addEditReader.ShowDialog();
                LoadReadersInDG();
            }
        }

        private void btnDeleteReader_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show("Удалить читателя?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Reader row = (Reader)DGReaders.SelectedItem;
                    if (row != null)
                    {
                        using (Pract4Libraly1Context _db = new Pract4Libraly1Context())
                        {
                            var readerToDelete = _db.Readers.Find(row.ReaderId);
                            if (readerToDelete != null)
                            {
                                _db.Readers.Remove(readerToDelete); 
                                _db.SaveChanges();
                            }
                        }
                        LoadReadersInDG();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка удаления");
                }
            }
            else DGReaders.Focus();
        }


    }
}