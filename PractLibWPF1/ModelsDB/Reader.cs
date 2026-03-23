using System;
using System.Collections.Generic;

namespace PractLibWPF1.ModelsDB;

public partial class Reader
{
    public int ReaderId { get; set; }

    public string LibraryCardNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
