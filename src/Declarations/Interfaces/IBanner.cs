using System;

namespace Declarations.Interfaces
{
    public interface IBanner
    {
        int Id { get; set; }
        string Html { get; set; }
        DateTime Created { get; set; }
        DateTime? Modified { get; set; }
    }
}
