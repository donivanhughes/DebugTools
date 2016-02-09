Public Class WStatusViewer
    Private lclsStatusObjects As New List(Of StatusObject)
    Private GetWomsServiceStatus As Func(Of StatusObject.StateData)
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        GetWomsServiceStatus = Function()
                                   Dim clsStateData As New StatusObject.StateData
                                   Try

                                   Catch ex As Exception

                                   End Try
                               End Function

    End Sub
    Private Sub WStatusViewer_Loaded(sender As Object, e As Windows.RoutedEventArgs) Handles Me.Loaded

        Dim clsStatusObject As New StatusObject
        clsStatusObject.UpdateInterval = 15
        clsStatusObject.Header = "WOMs Update Service"

    End Sub
End Class
