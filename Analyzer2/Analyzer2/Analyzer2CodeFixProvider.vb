Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Composition
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.CodeActions
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Rename
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.Editing

<ExportCodeFixProvider(LanguageNames.VisualBasic, Name:=NameOf(Analyzer2CodeFixProvider)), [Shared]>
Public Class Analyzer2CodeFixProvider
    Inherits CodeFixProvider

    Private Const title As String = "Remove ByVal modifier"

    Public NotOverridable Overrides ReadOnly Property FixableDiagnosticIds As ImmutableArray(Of String)
        Get
            Return ImmutableArray.Create("IDE0078")
        End Get
    End Property

    Public NotOverridable Overrides Function GetFixAllProvider() As FixAllProvider
        ' See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        Return WellKnownFixAllProviders.BatchFixer
    End Function


    Public Overrides Async Function RegisterCodeFixesAsync(ByVal context As CodeFixContext) As Task
        Dim root = Await context.Document.GetSyntaxRootAsync(context.CancellationToken)
        Dim editor As New SyntaxEditor(root, context.Document.Project.Solution.Workspace)
        context.RegisterCodeFix(New MyCodeAction(
                title,
                Function(ct) FixAsync(context.Document, context.Diagnostics.First(), editor, ct)),
                context.Diagnostics)
    End Function


    Private Async Function FixAsync(document As Document, diagnostic As Diagnostic, editor As SyntaxEditor, cancellationToken As CancellationToken) As Task(Of Document)
        Dim root = Await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(False)
        Dim node = DirectCast(root.FindNode(diagnostic.Location.SourceSpan), ParameterSyntax)
        Dim tokenList = SyntaxFactory.TokenList(node.Modifiers.Where(Function(m) Not m.IsKind(SyntaxKind.ByValKeyword)))
        editor.ReplaceNode(node, node.WithModifiers(tokenList))
        If editor Is Nothing Then Return document
        Dim newRoot = editor.GetChangedRoot()
        If document Is Nothing Then Return document
        If newRoot Is Nothing Then Return document
        Return document.WithSyntaxRoot(newRoot)
    End Function



    Private Class MyCodeAction
        Inherits DocumentChangeAction

        Friend Sub New(title As String, createChangedDocument As Func(Of CancellationToken, Task(Of Document)))
            MyBase.New(title, createChangedDocument)
        End Sub
    End Class

End Class
