using System;
using System.Collections.Generic;

namespace PractLibWPF1.ModelsDB;

public partial class BookCopy
{
    public int CopyId { get; set; }

    public int BookId { get; set; }

    public bool IsAvailable { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
