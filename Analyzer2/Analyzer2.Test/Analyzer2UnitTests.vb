Imports Analyzer2
Imports Analyzer2.Test.TestHelper
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Analyzer2.Test
    <TestClass>
    Public Class UnitTest
        Inherits CodeFixVerifier

        'No diagnostics expected to show up
        <TestMethod>
        Public Sub TestMethod1()
            Dim test = ""
            VerifyBasicDiagnostic(test)
        End Sub

        'Diagnostic And CodeFix both triggered And checked for
        <TestMethod>
        Public Sub TestMethod2()

            Dim test = "
Module Module1

    Sub Main(ByVal x As String)

    End Sub

End Module"
            Dim expected = New DiagnosticResult With {.Id = "IDE0078",
                .Message = "The modifier ByVal is unnecessary and can be removed.",
                .Severity = DiagnosticSeverity.Hidden,
                .Locations = New DiagnosticResultLocation() {
                        New DiagnosticResultLocation("Test0.vb", 4, 14)
                    }
            }


            VerifyBasicDiagnostic(test, expected)

            Dim fixtest = "
Module Module1

    Sub Main(x As String)

    End Sub

End Module"
            VerifyBasicFix(test, fixtest)
        End Sub

        Protected Overrides Function GetBasicCodeFixProvider() As CodeFixProvider
            Return New Analyzer2CodeFixProvider()
        End Function

        Protected Overrides Function GetBasicDiagnosticAnalyzer() As DiagnosticAnalyzer
            Return New Analyzer2Analyzer()
        End Function

    End Class
End Namespace
