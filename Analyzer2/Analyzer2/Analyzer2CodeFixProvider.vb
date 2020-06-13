Imports System.Collections.Immutable
Imports System.Composition
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.CodeActions
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Editing

<ExportCodeFixProvider(LanguageNames.VisualBasic, Name:=NameOf(Analyzer2CodeFixProvider)), [Shared]>
Public Class Analyzer2CodeFixProvider
    Inherits CodeFixProvider

    Public NotOverridable Overrides ReadOnly Property FixableDiagnosticIds As ImmutableArray(Of String) = ImmutableArray.Create("IDE0078")

    Public NotOverridable Overrides Function GetFixAllProvider() As FixAllProvider
        Return WellKnownFixAllProviders.BatchFixer
    End Function

    Public Overrides Async Function RegisterCodeFixesAsync(context As CodeFixContext) As Task
        Dim root = Await context.Document.GetSyntaxRootAsync(context.CancellationToken)
        Dim editor As New SyntaxEditor(root, context.Document.Project.Solution.Workspace)
        For Each diagnostic In context.Diagnostics
            context.RegisterCodeFix(New MyCodeAction(Function(ct) FixAsync(context.Document, diagnostic, editor, ct)), context.Diagnostics)
        Next
    End Function

    Private Async Function FixAsync(document As Document, diagnostic As Diagnostic, editor As SyntaxEditor, cancellationToken As CancellationToken) As Task(Of Document)
        Dim root = Await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(False)
        Dim node = DirectCast(root.FindNode(diagnostic.Location.SourceSpan), ParameterSyntax)
        Dim tokenList = SyntaxFactory.TokenList(node.Modifiers.Where(Function(m) Not m.IsKind(SyntaxKind.ByValKeyword)))
        editor.ReplaceNode(node, node.WithModifiers(tokenList))
        Dim newRoot = editor.GetChangedRoot()
        Return document.WithSyntaxRoot(newRoot)
    End Function


    Private Class MyCodeAction
        Inherits CodeAction

        Public Overrides ReadOnly Property Title As String = "Remove ByVal modifier"

        Private ReadOnly _createChangedDocument As Func(Of CancellationToken, Task(Of Document))

        Public Sub New(createChangedDocument As Func(Of CancellationToken, Task(Of Document)))
            _createChangedDocument = createChangedDocument
        End Sub

        Protected Overrides Function GetChangedDocumentAsync(cancellationToken As CancellationToken) As Task(Of Document)
            Return _createChangedDocument(cancellationToken)
        End Function
    End Class
End Class
