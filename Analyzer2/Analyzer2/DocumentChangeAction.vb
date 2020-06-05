Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeActions

Friend Class DocumentChangeAction
    Inherits SimpleCodeAction

    Private ReadOnly _createChangedDocument As Func(Of CancellationToken, Task(Of Document))

    Public Sub New(ByVal title As String, ByVal createChangedDocument As Func(Of CancellationToken, Task(Of Document)), ByVal Optional equivalenceKey As String = Nothing)
        MyBase.New(title, equivalenceKey)
        _createChangedDocument = createChangedDocument
    End Sub

    Protected Overrides Function GetChangedDocumentAsync(ByVal cancellationToken As CancellationToken) As Task(Of Document)
        Return _createChangedDocument(cancellationToken)
    End Function
End Class


Friend MustInherit Class SimpleCodeAction
    Inherits CodeAction

    Public Sub New(title As String, equivalenceKey As String)
        Me.Title = title
        Me.EquivalenceKey = equivalenceKey
    End Sub
    Public NotOverridable Overrides ReadOnly Property Title As String
    Public NotOverridable Overrides ReadOnly Property EquivalenceKey As String

    Protected Overrides Function GetChangedDocumentAsync(ByVal cancellationToken As CancellationToken) As Task(Of Document)
        Return Task.FromResult(Of Document)(Nothing)
    End Function

End Class
