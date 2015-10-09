Imports System.IO
Imports System.Windows.Controls
Imports Microsoft.Win32
Imports OfficeOpenXml
Public Class WDataSourceViewer
    Public Sub WriteTabToExcelFile(intTabIndex As Integer)
        Dim tiBuffer As TabItem = tctlWindowFrame.Items(intTabIndex)
        Dim dgToWrite As DataGrid = tiBuffer.Content.Children(0)
        Dim pkgWrite As New ExcelPackage
        Dim wbWrite As ExcelWorkbook = pkgWrite.Workbook
        Dim wsWrite As ExcelWorksheet = wbWrite.Worksheets.Add(tiBuffer.Header)
        Dim sfdPath As New SaveFileDialog
        Dim fiFile As FileInfo
        Dim blnResult As Boolean?
        wsWrite.Cells(1, 1).LoadFromDataTable(DirectCast(dgToWrite.ItemsSource, System.Data.DataView).ToTable, True)
        sfdPath.DefaultExt = ".xlsx"
        sfdPath.AddExtension = True
        sfdPath.ValidateNames = True
        sfdPath.OverwritePrompt = True
        blnResult = sfdPath.ShowDialog()
        If blnResult = True Then
            fiFile = New FileInfo(sfdPath.FileName)
            pkgWrite.SaveAs(fiFile)
        End If
        

    End Sub
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

    Private Sub mnuToolsWriteToExcel_Click(sender As Object, e As Windows.RoutedEventArgs) Handles mnuToolsWriteToExcel.Click
        WriteTabToExcelFile(tctlWindowFrame.SelectedIndex)
    End Sub
End Class
