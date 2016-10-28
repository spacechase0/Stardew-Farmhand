using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Farmhand.UI.Pages
{
    public class ComponentDefinition
    {
        public string Name { get; set; }

        public string CodeBehindTypeName { get; set; }

        public string CodeBehindParamsTypeName { get; set; }

        public string DesignerFile { get; set; }

        public bool HasCodeBehind => string.IsNullOrWhiteSpace(CodeBehindTypeName);

        public bool HasCodeBehindParams => string.IsNullOrWhiteSpace(CodeBehindTypeName);

        public bool HasDesignerFile => string.IsNullOrWhiteSpace(CodeBehindParamsTypeName);

        public IComponent CreateCodeBehindInstance()
        {
            var cbType = Type.GetType(CodeBehindTypeName);
            if(cbType == null)
                throw new Exception($"Type {CodeBehindTypeName} used in CodeBehindTypeName does not exist");

            return Activator.CreateInstance(cbType) as IComponent;
        }

        public IComponent CreateCodeBehindInstance(object parameters)
        {
            var cbType = Type.GetType(CodeBehindTypeName);
            if (cbType == null)
                throw new Exception($"Type {CodeBehindTypeName} used in CodeBehindTypeName does not exist");

            return Activator.CreateInstance(cbType, parameters) as IComponent;
        }
    }
}

//{
//  "Name": "ClickableTexture",
//  "CodeBehindType": "Farmhand.UI.PageDefinitions.Controls.ClickableTexture",
//  "CodeBehindParams": "Farmhand.UI.PageDefinitions.Controls.ClickableTextureParameters",
//  "DesignerFile": ""
//}