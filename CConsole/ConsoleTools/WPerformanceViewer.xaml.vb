Public Class WPerformanceViewer
    Private WithEvents tmrUpdateInfo As New Timers.Timer

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        tmrUpdateInfo.Interval = 100
        Me.Width = 200
        Me.Height = 250
        tmrUpdateInfo.Start()
    End Sub
    Private Sub tmrUpdateInfo_Tick(sender As Object, e As EventArgs) Handles tmrUpdateInfo.Elapsed

        Dispatcher.InvokeAsync(AddressOf UpdateMemory)
    


    End Sub

    Private Sub UpdateMemory()
        Dim proCurrentProcess As Process = Process.GetCurrentProcess()
        lblPagedMemory.Content = (proCurrentProcess.PagedSystemMemorySize64 / 1024).ToString("0.00") & " K"
        lblUnPagedMemory.Content = (proCurrentProcess.NonpagedSystemMemorySize64 / 1024).ToString("0.00") & " K"
        lblWorkingSet.Content = (proCurrentProcess.WorkingSet64 / 1024).ToString("0.00") & " K"
        'lblOther.Content = New PerformanceCounter("Processor", "% Process Time", proCurrentProcess.ProcessName).NextValue
    End Sub
End Class
