Public Class WDataSourceViewer
    Public Property Source As Object
        Get
            Return dgvMain.ItemsSource
        End Get
        Set(value As Object)
            Dim objSource
            If TypeOf (value) Is DataTable Then
                '    Dim liaBuffer As New List(Of Object)
                '    For Each drRow As DataRow In DirectCast(value, DataTable).Rows
                '        liaBuffer.Add(drRow.ItemArray.ToArray)
                '    Next
                '    objSource = liaBuffer
                'Else
                objSource = DirectCast(value, DataTable).AsDataView
            End If
            'dgvMain.DataContext = objSource

            dgvMain.ItemsSource = objSource



        End Set
    End Property
End Class
