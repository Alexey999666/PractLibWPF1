using System;
using System.Collections.Generic;

namespace PractLibWPF1.ModelsDB;

public partial class Issue
{
    public int IssueId { get; set; }

    public int CopyId { get; set; }

    public int ReaderId { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public virtual BookCopy Copy { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
