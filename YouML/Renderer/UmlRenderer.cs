using YouML.Models;
using System.Text;
using YouML.Tools;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace YouML.Renderer
{
    public class UmlRenderer
    {
        private const string BackgroundColor = "#GreenYellow/LightGoldenRodYellow";

        public static string Render(ClassModel classModel)
        {
            var sb = new StringBuilder();

            sb.AppendLine("@startuml");//desc start
            sb.AppendFormat("Frame {0} {1}", classModel.Namespace, BackgroundColor).Append("{").AppendLine();//namespace start
            AddBaseAndClassObjects(classModel, sb);
            AddConstructors(classModel, sb);
            AddEnums(classModel, sb);
            AddFields(classModel, sb);
            AddProps(classModel, sb);
            AddMethods(classModel, sb);
            AddEvents(classModel, sb);

            sb.AppendLine("}");//class close
            sb.AppendLine("}");//namespace close
            
            sb.AppendLine("@enduml");//desc close

            return sb.ToString();
        }

        private static void AddBaseAndClassObjects(ClassModel classModel, StringBuilder sb)
        {

            if (classModel.ClassDeclarationSyntax is null)
            {
                sb.Append("class null{");
                return;
            }

            if (classModel.ClassDeclarationSyntax.BaseList != null)
            {
                
                    

                foreach (var baseType in classModel.ClassDeclarationSyntax.BaseList.Types)
                {
                    string baseObject = string.Empty;


                    if (baseType.Type.ToString().StartsWith("I")) //todo improve interface recognition
                    {
                        baseObject = string.Format("interface \"{0}\" as {1}", baseType.Type.ToString(), baseType.Type.ToString().Replace('<','_').Replace('>', ' '));
                    }
                    else
                    {
                        baseObject = string.Format("class \"{0}\" as {1}", baseType.Type.ToString(), baseType.Type.ToString().Replace('<', '_').Replace('>', ' '));
                    }

                    sb.AppendFormat(baseObject).AppendLine();
                    
                }

                sb.AppendFormat("class {0} ", classModel.ClassDeclarationSyntax.Identifier.ValueText);

                if (classModel.ClassDeclarationSyntax.BaseList.Types.Count > 0) sb.Append("extends ");


                string extends = string.Empty;
                foreach (var baseType in classModel.ClassDeclarationSyntax.BaseList.Types)
                {

                    extends += string.Format(baseType.Type.ToString().Replace('<', '_').Replace('>', ' ') + ", ");
                }

                extends = extends.Remove(extends.Length - 2);

                sb.Append(extends);

                sb.Append("{").AppendLine();//class start


            }
        }
        

        private static void AddConstructors(ClassModel classModel, StringBuilder sb)
        {

            if (classModel.ClassDeclarationSyntax is null) return;

            using (var e = classModel.ConstructorDeclarationSyntax.GetEnumerator())
            {
                var allowCaption = true;
                
                while (e.MoveNext().Equals(true))
                {
                    if (e.Current != null)
                    {
                        if (allowCaption)
                        {
                            sb.AppendLine(".. Constructors ..");
                            allowCaption = false;
                        }

                        sb.AppendFormat("{0}{1}", e.Current.Modifiers.GetUmlModifiers(), e.Current.Identifier)
                            .Append("()").AppendLine();
                    }
                        
                }
            }
        }
        
        private static void AddEvents(ClassModel classModel, StringBuilder sb)
        {
            if (classModel.ClassDeclarationSyntax is null) return;

            using (var e = classModel.EventDeclarationSyntax.GetEnumerator())
            {
                var allowCaption = true;

                while (e.MoveNext().Equals(true))
                {
                    if (e.Current != null)
                    {
                        if (allowCaption)
                        {
                            sb.AppendLine(".. Events ..");
                            allowCaption = false;
                        }

                        sb.AppendFormat("{0} {1}", e.Current.Modifiers.GetUmlModifiers(), e.Current.Declaration)
                            .AppendLine();
                    }
                }
            }
        }

        private static void AddFields(ClassModel classModel, StringBuilder sb)
        {
            if (classModel.ClassDeclarationSyntax is null) return;

            using (var e = classModel.FieldDeclarationSyntax.GetEnumerator())
            {
                var allowCaption = true;

                while (e.MoveNext().Equals(true))
                {
                    if (e.Current != null)
                    {
                        if (allowCaption)
                        {
                            sb.AppendLine(".. Fields ..");
                            allowCaption = false;
                        }

                        sb.AppendFormat("{0}{1}", e.Current.Modifiers.GetUmlModifiers(), 
                                e.Current.Declaration.ToString().Split('=')[0].TrimEnd())
                            .AppendLine();
                    }
                }
            }
        }

        private static void AddProps(ClassModel classModel, StringBuilder sb)
        {
            if (classModel.ClassDeclarationSyntax is null) return;

            using (var e = classModel.PropertyDeclarationSyntax.GetEnumerator())
            {
                var allowCaption = true;

                while (e.MoveNext().Equals(true))
                {
                    if (e.Current != null)
                    {
                        if (allowCaption)
                        {
                            sb.AppendLine(".. Properties ..");
                            allowCaption = false;
                        }

                        sb.AppendFormat("{0} {1} {2}", 
                                e.Current.Modifiers.GetUmlModifiers(), 
                                e.Current.Type, 
                                e.Current.Identifier)
                            .AppendLine();
                    }

                }
            }
        }

        private static void AddMethods(ClassModel classModel, StringBuilder sb)
        {
            if (classModel.ClassDeclarationSyntax is null) return;

            using (var e = classModel.MethodDeclarationSyntax.GetEnumerator())
            {
                var allowCaption = true;

                while (e.MoveNext().Equals(true))
                {
                    if (e.Current != null)
                    {
                        if (allowCaption)
                        {
                            sb.AppendLine(".. Methods ..");
                            allowCaption = false;
                        }

                        sb.AppendFormat("{0}{1} {2}", 
                                e.Current.Modifiers.GetUmlModifiers(), 
                                e.Current.ReturnType, 
                                e.Current.Identifier)
                            .Append("()").AppendLine();
                    }

                }
            }
        }

        private static void AddEnums(ClassModel classModel, StringBuilder sb)
        {
            if (classModel.ClassDeclarationSyntax is null) return;

            using (var e = classModel.EnumDeclarationSyntax.GetEnumerator())
            {
                var allowCaption = true;

                while (e.MoveNext().Equals(true))
                {
                    if (e.Current != null)
                    {
                        if (allowCaption)
                        {
                            sb.AppendLine(".. Enums ..");
                            allowCaption = false;
                        }

                        sb.AppendFormat("{0} {1} {2}",
                                e.Current.Modifiers.GetUmlModifiers(),
                                e.Current.EnumKeyword,
                                e.Current.Identifier)
                            .AppendLine();
                    }
                }
            }
        }
    }
}
