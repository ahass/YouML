using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouML.Models
{
    public class ClassModel
    {
        public NameSyntax Namespace { get; set; }

        public ClassDeclarationSyntax ClassDeclarationSyntax { get; set; }

        public IEnumerable<MethodDeclarationSyntax> MethodDeclarationSyntax { get; set; }

        public IEnumerable<PropertyDeclarationSyntax> PropertyDeclarationSyntax { get; set; }

        public IEnumerable<FieldDeclarationSyntax> FieldDeclarationSyntax { get; set; }
        
        public IEnumerable<ConstructorDeclarationSyntax> ConstructorDeclarationSyntax { get; set; }
        
        public IEnumerable<EventFieldDeclarationSyntax> EventDeclarationSyntax { get; set; }

        public IEnumerable<EnumDeclarationSyntax> EnumDeclarationSyntax { get; set; }

    }
}
