Imports System.Runtime.CompilerServices
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports DebugTools.ConsoleTools


Module MMain
    ''' ------------------------------------------------------------------------------------------
    '''  Name: IsNullOrEmpty
    '''  ------------------------------------------------------------------------------------------
    ''' <summary> Determines whether this Ienumerable is nothing and has no members . </summary>
    ''' <returns><c>true</c> if [is null or empty] returns true; otherwise, <c>false</c>.</returns>
    '''  ------------------------------------------------------------------------------------------
    <Extension()> _
    Public Function IsNullOrEmpty(ienuToExtend As IEnumerable) As Boolean
        Dim blnReturnValue As Boolean = True
        Try
            If ienuToExtend IsNot Nothing Then
                For Each Item In ienuToExtend
                    blnReturnValue = False
                    Exit For
                Next
            End If
        Catch ex As Exception
            CConsole.WriteException(ex)
        End Try
        Return blnReturnValue

    End Function

    <Extension()> _
    Public Function Text(rtxtToExtend As RichTextbox) As String
        Dim strReturnValue As String = ""
        Try
            With rtxtToExtend.Document
                strReturnValue = New TextRange(.ContentStart, .ContentEnd).Text
            End With
        Catch ex As Exception
            CConsole.WriteException(ex)
        End Try
        Return strReturnValue

    End Function

    <Extension()> _
    Public Function Length(rtxtToExtend As RichTextBox) As Double
        Dim dblReturnValue As String = -1
        Try
            With rtxtToExtend.Document
                dblReturnValue = New TextRange(.ContentStart, .ContentEnd).Text.Length
            End With
        Catch ex As Exception
            CConsole.WriteException(ex)
        End Try
        Return dblReturnValue

    End Function

End Module
