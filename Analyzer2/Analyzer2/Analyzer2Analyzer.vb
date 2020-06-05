Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Diagnostics

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class Analyzer2Analyzer
    Inherits DiagnosticAnalyzer

    Private Shared ReadOnly Rule As New DiagnosticDescriptor("IDE0078", "Unnecessary ByVal", "The modifier ByVal is unnecessary and can be removed.", "CodeStyle", DiagnosticSeverity.Hidden, isEnabledByDefault:=True, description:="Unnecessary ByVal can be removed.", "https://github.com/Youssef1313", WellKnownDiagnosticTags.Unnecessary, WellKnownDiagnosticTags.Telemetry)

    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
        Get
            Return ImmutableArray.Create(Rule)
        End Get
    End Property

    Public Overrides Sub Initialize(context As AnalysisContext)
        context.EnableConcurrentExecution()
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)
        context.RegisterSyntaxNodeAction(
            Sub(syntaxContext As SyntaxNodeAnalysisContext)
                Dim parameterSyntax = TryCast(syntaxContext.Node, ParameterSyntax)
                If parameterSyntax Is Nothing Then
                    Return
                End If
                For Each modifier In parameterSyntax.Modifiers
                    If modifier.IsKind(SyntaxKind.ByValKeyword) Then
                        syntaxContext.ReportDiagnostic(Diagnostic.Create(Rule, modifier.GetLocation()))
                    End If
                Next

            End Sub, SyntaxKind.Parameter)

    End Sub

End Class
