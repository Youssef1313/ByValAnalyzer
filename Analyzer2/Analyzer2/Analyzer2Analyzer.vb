Imports System.Collections.Immutable
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Diagnostics

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class Analyzer2Analyzer
    Inherits DiagnosticAnalyzer

    Private Shared ReadOnly Rule As New DiagnosticDescriptor("IDE0078", "Unnecessary ByVal", "The modifier ByVal is unnecessary and can be removed.", "CodeStyle", DiagnosticSeverity.Hidden, isEnabledByDefault:=True, description:="Unnecessary ByVal can be removed.", "https://github.com/Youssef1313", WellKnownDiagnosticTags.Unnecessary, WellKnownDiagnosticTags.Telemetry)

    Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor) = ImmutableArray.Create(Rule)

    Public Overrides Sub Initialize(context As AnalysisContext)
        context.EnableConcurrentExecution()
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)
        context.RegisterSyntaxNodeAction(
            Sub(syntaxContext As SyntaxNodeAnalysisContext)
                Dim parameterSyntax = DirectCast(syntaxContext.Node, ParameterSyntax)
                For Each modifier In parameterSyntax.Modifiers
                    If modifier.IsKind(SyntaxKind.ByValKeyword) Then
                        syntaxContext.ReportDiagnostic(Diagnostic.Create(Rule, modifier.GetLocation()))
                    End If
                Next
            End Sub, SyntaxKind.Parameter)
    End Sub

End Class
