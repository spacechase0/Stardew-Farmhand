using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Farmhand.UI.Pages.Properties
{
    [Flags]
    public enum Alignment
    {
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3
    }
}
