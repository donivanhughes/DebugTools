Imports System.Windows.Controls

Public Class WDataSourceViewer

    Public Sub AddSource(objSource As Object)
        Dim strWindowName As String
        strWindowName = "NO NAME"
        If TypeOf (objSource) Is DataTable Then
            If DirectCast(objSource, DataTable).TableName.IsNullOrEmpty = False Then
                strWindowName = DirectCast(objSource, DataTable).TableName
            Else
                strWindowName = Now.ToShortTimeString
            End If

            objSource = DirectCast(objSource, DataTable).AsDataView

        End If

        Dim dgNew = AddWindow(strWindowName)
        dgNew.ItemsSource = objSource
    End Sub
    Private Function AddWindow(strWindow As String) As DataGrid
        Dim tabCopy As TabItem = New TabItem()
        Dim dgNew As New DataGrid
        tabCopy.Header = strWindow
        Dim dkpContainer As New DockPanel
        dkpContainer.Children.Add(dgNew)
        tabCopy.Content = dkpContainer
        Dim intNewTabIndex As Integer = tctlWindowFrame.Items.Add(tabCopy)
        tctlWindowFrame.SelectedIndex = intNewTabIndex

        Return dgNew
    End Function
End Class
