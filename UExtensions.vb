Imports System.Runtime.CompilerServices

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

        If ienuToExtend IsNot Nothing Then
            For Each Item In ienuToExtend
                blnReturnValue = False
                Exit For
            Next
        End If

        Return blnReturnValue

    End Function

End Module
