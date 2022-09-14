using System.Linq;
using YouML.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YouML.Parser
{
    public static class ClassParser
    {
        public static ClassModel Parse(string fileContent)
        {
            try
            {

                if (fileContent.Equals(string.Empty)) return new ClassModel();

                var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);

                var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

                var namespaceSyntax = root?.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();// Todo error on apply to cs files with no namespace

                var fcnamespaceSyntax = root?.Members.OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();

                var classModel = new ClassModel();

                // Todo current limitation, only one class in file
                ClassDeclarationSyntax classSyntax;

                if (fcnamespaceSyntax != null)
                    classSyntax = fcnamespaceSyntax?.Members.OfType<ClassDeclarationSyntax>().First(); // Todo error on apply to interface
                else if (namespaceSyntax != null)
                    classSyntax = namespaceSyntax?.Members.OfType<ClassDeclarationSyntax>().First(); // Todo error on apply to interface
                else return new ClassModel();

                var methodSyntax = classSyntax?.Members.OfType<MethodDeclarationSyntax>();

                if (classSyntax != null)
                {
                    var propertySyntax = classSyntax.Members.OfType<PropertyDeclarationSyntax>();

                    var fieldSyntax = classSyntax.Members.OfType<FieldDeclarationSyntax>();

                    var constructorSyntax = classSyntax.Members.OfType<ConstructorDeclarationSyntax>();

                    var eventSyntax = classSyntax.Members.OfType<EventFieldDeclarationSyntax>();

                    var enumSyntax = classSyntax.Members.OfType<EnumDeclarationSyntax>();

                    classModel.Namespace = namespaceSyntax != null ? namespaceSyntax.Name : fcnamespaceSyntax.Name;
                    classModel.ClassDeclarationSyntax = classSyntax;
                    classModel.MethodDeclarationSyntax = methodSyntax;
                    classModel.PropertyDeclarationSyntax = propertySyntax;
                    classModel.FieldDeclarationSyntax = fieldSyntax;
                    classModel.ConstructorDeclarationSyntax = constructorSyntax;
                    classModel.EventDeclarationSyntax = eventSyntax;
                    classModel.EnumDeclarationSyntax = enumSyntax;

                }

                return classModel;
            }
            catch
            {
                return new ClassModel();
            }

        }
    }
}
