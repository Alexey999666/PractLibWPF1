using System;
using System.Collections.Generic;

namespace PractLibWPF1.ModelsDB;

public partial class Book
{
    public int BookId { get; set; }

    public string Author { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int? YearPublished { get; set; }

    public decimal? Price { get; set; }

    public string? Annotation { get; set; }

    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}
