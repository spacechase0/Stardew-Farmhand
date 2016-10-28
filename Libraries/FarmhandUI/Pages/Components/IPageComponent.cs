using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.UI.Pages.Controls;

namespace Farmhand.UI.Pages.Components
{
    public interface IPageComponent : IComponent
    {
        Type GetParamsType();
    }
}
