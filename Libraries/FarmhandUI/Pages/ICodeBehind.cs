using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Farmhand.UI.Pages
{
    public class CodeBehind
    {
        private readonly List<IComponent> _controls = new List<IComponent>();

        public IEnumerable<IComponent> Controls => _controls;
        
    }
}
