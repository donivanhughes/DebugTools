Imports System.Drawing
Imports System.Text
Imports System.Threading.Tasks
Imports System.Threading
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports System.Windows.Forms.Integration
Imports System.Windows.Input
Imports DebugTools.ConsoleTools




Public Class WConsole
    Delegate Sub SetTextCallback(strText As String)


    'Delegate Sub SetPerformanceCallback(frmLocalPerformanceViewer As FPerformanceViewer)
    Private wpfPerformanceViewer As WPerformanceViewer
    Private wpfDataSourceViewer As WDataSourceViewer
    Private dicWindows As Dictionary(Of String, Windows.Controls.TextBox)
    Private WithEvents tmrUpdateInfo As New Timers.Timer
    Private WithEvents tmrCleanUp As New Timers.Timer
    Private dblTimeSinceWrite As Double = 0
    Private lkeyPressedKeys As New List(Of Key)
    Private ltskToCleanUp As New List(Of Task)
   


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        dicWindows = New Dictionary(Of String, Windows.Controls.TextBox) From {{"Main", txtConsoleWindow}}


        If CConsole.AlwaysOnTop = True Then
            mnuSettingsAlwaysOnTop.IsChecked = True
        End If
        tmrUpdateInfo.Interval = 10
        tmrCleanUp.Interval = 1000
        tmrUpdateInfo.Start()
        tmrCleanUp.Start()
    End Sub

    Private Sub SetText(strText As String)
        txtConsoleWindow.AppendText(strText)

        Application.DoEvents()
    End Sub


    Friend Sub Write(strText As String)
        Dim SuppressWarning = Task.Run(Sub()
                                           Dispatcher.InvokeAsync(New Action(Sub()
                                                                                 txtConsoleWindow.AppendText(strText)
                                                                                 Application.DoEvents()

                                                                             End Sub), Windows.Threading.DispatcherPriority.Background)
                                       End Sub)

        ltskToCleanUp.Add(SuppressWarning)
        Application.DoEvents()


    End Sub

    Friend Sub Write(strText As String, strWindow As String, Optional dblInterval As Double = 0)
        If dblInterval = 0 OrElse dblTimeSinceWrite > dblInterval Then
            dblTimeSinceWrite = 0

            If dicWindows.ContainsKey(strWindow) = False Then
                AddNewWindow(strWindow)
            End If
            Dim SuppressWarning = Task.Run(Sub()
                                               Dispatcher.InvokeAsync(New Action(Sub()
                                                                                     Try


                                                                                         dicWindows(strWindow).AppendText(strText)
                                                                                         dicWindows(strWindow).ScrollToLine(dicWindows(strWindow).LineCount - 1)
                                                                                         Application.DoEvents()
                                                                                     Catch ex As StackOverflowException
                                                                                         dicWindows(strWindow) = AddNewWindow(strWindow + "Overflow")
                                                                                     Catch ex As Exception

                                                                                     End Try
                                                                                 End Sub), Windows.Threading.DispatcherPriority.Background)
                                           End Sub)



            ltskToCleanUp.Add(SuppressWarning)
        End If
        Application.DoEvents()
    End Sub

    Friend Sub ClearWindow(strWindow)
        dicWindows(strWindow).Text = ""
    End Sub
    Private Function AddNewWindow(strWindowName As String) As Windows.Controls.TextBox
        Dim tabCopy As TabItem = New TabItem()

        tabCopy.Header = strWindowName
        Dim txtNewConsole As New Windows.Controls.TextBox
        txtNewConsole.Name = "txt" & strWindowName.Replace(" ", "")
        txtNewConsole.Foreground = Windows.Media.Brushes.White
        txtNewConsole.Background = Windows.Media.Brushes.Black
        txtNewConsole.FontFamily = txtConsoleWindow.FontFamily
        txtNewConsole.FontSize = txtConsoleWindow.FontSize
        txtNewConsole.FontStyle = txtConsoleWindow.FontStyle
        txtNewConsole.BorderThickness = txtConsoleWindow.BorderThickness
        txtNewConsole.Padding = txtConsoleWindow.Padding
        txtNewConsole.TextWrapping = txtConsoleWindow.TextWrapping
        txtConsoleWindow.Margin = txtConsoleWindow.Margin
        dicWindows.Add(strWindowName, txtNewConsole)
        Dim dkpContainer As New DockPanel
        dkpContainer.Children.Add(txtNewConsole)
        tabCopy.Content = dkpContainer

        Dim intNewTabIndex As Integer = tctlWindowFrame.Items.Add(tabCopy)
        tctlWindowFrame.SelectedIndex = intNewTabIndex
        Application.DoEvents()

        Return txtNewConsole
    End Function
    'Private Async Sub btnViewPerformance_Click(sender As Object, e As EventArgs) Handles btnViewPerformance.Click

    '    If frmPerformanceViewer IsNot Nothing Then
    '        CConsole.WriteLine("Closing Performance Viewer...")
    '        SetBusy(True)
    '        frmPerformanceViewer.Close()
    '        frmPerformanceViewer.Dispose()
    '        frmPerformanceViewer = Nothing
    '        btnViewPerformance.BackColor = DefaultBackColor
    '        SetBusy(False)

    '    Else
    '        CConsole.WriteLine("Building Performance View...")
    '        btnViewPerformance.Enabled = False
    '        Await Task.Run(Sub()
    '                           Dim frmLocalPerformanceViewer As New FPerformanceViewer

    '                           Application.DoEvents()
    '                           Dim d As New SetPerformanceCallback(AddressOf SetPerformanceViewer)
    '                           Me.Invoke(d, New Object() {frmLocalPerformanceViewer})
    '                       End Sub)
    '    End If
    'End Sub

    'Private Sub SetBusy(blnBusy As Boolean)
    '    If blnBusy = True Then
    '        Cursor = Cursors.WaitCursor
    '        IsEnabled = False
    '    Else
    '        Cursor = Cursors.Default
    '        IsEnabled = True
    '    End If
    'End Sub

    'Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
    '    CConsole.Clear()
    'End Sub

    'Private Sub tsbMouseData_Click(sender As Object, e As EventArgs) Handles tsbMouseData.Click
    '    tsRight.Visible = Not tsRight.Visible
    '    If tsRight.Visible = True Then
    '        tsbMouseData.BackColor = Color.DodgerBlue
    '        Me.Size = New Size(New Point(Me.Size.Width + tsRight.Width, Me.Size.Height))
    '    Else
    '        tsbMouseData.BackColor = DefaultBackColor
    '        Me.Size = New Size(New Point(Me.Size.Width - tsRight.Width, Me.Size.Height))
    '    End If
    'End Sub

    Private Sub tmrUpdateInfo_Tick() Handles tmrUpdateInfo.Elapsed
        dblTimeSinceWrite += tmrUpdateInfo.Interval
        Dispatcher.InvokeAsync(AddressOf UpdateInputData)

        'Dispatcher.InvokeAsync(AddressOf GetKeyboardData)
        'Dispatcher.InvokeAsync(AddressOf UpdateKeyboardData)


    End Sub
    Private Sub tmrCleanUp_Tick() Handles tmrCleanUp.Elapsed
        If ltskToCleanUp.Count > 0 Then
            For Each tskRunning As Task In ltskToCleanUp
                tskRunning.Dispose()
            Next
            ltskToCleanUp.Clear()
        End If

    End Sub

    Private Sub UpdateInputData()
        'Mouse data is displayed in the right toolbar. it is enabled and disabled via the mousedata button
        'Mouse Button
        If mnuToolsMouse.IsChecked = True Then
            Select Case System.Windows.Forms.Control.MouseButtons
                Case 0
                    lblMouseStatus.Content = "None"
                Case MouseButtons.Left
                    lblMouseStatus.Content = "Left"
                Case MouseButtons.Right
                    lblMouseStatus.Content = "Right"
                Case MouseButtons.Middle
                    lblMouseStatus.Content = "Middle"
                Case MouseButtons.Right + MouseButtons.Left
                    lblMouseStatus.Content = "L+R"
                Case MouseButtons.Right + MouseButtons.Middle
                    lblMouseStatus.Content = "R+M"
                Case MouseButtons.Left + MouseButtons.Middle
                    lblMouseStatus.Content = "L+M"
                Case MouseButtons.Right + MouseButtons.Left + MouseButtons.Middle
                    lblMouseStatus.Content = "L+R+M"
                Case MouseButtons.XButton1
                    lblMouseStatus.Content = "X1"
                Case MouseButtons.XButton2
                    lblMouseStatus.Content = "X2"
            End Select

            'Mouse Position
            lblMousePosition.Content = System.Windows.Forms.Control.MousePosition


            UpdateKeyboardData()

        End If

    End Sub
    Private Sub GetKeyboardData()
        Dim lkeyBuffer As New List(Of Key)
        For Each Key As Key In Keys.GetValues(GetType(Key))
            If Key <> Keys.None AndAlso Key < 200 AndAlso Key > 0 Then
                If Keyboard.IsKeyDown(Key) = True Then
                    If lkeyBuffer.Contains(Key) = False Then
                        lkeyBuffer.Add(Key)
                    End If


                End If
            End If
        Next
        lkeyPressedKeys = lkeyBuffer
    End Sub

    Private Sub UpdateKeyboardData()
        GetKeyboardData()
        Dim strKeyString As New StringBuilder
        For Each Key As Key In lkeyPressedKeys
            If strKeyString.Length = 0 Then
                strKeyString.Append(Key.ToString)
            Else
                strKeyString.Append("," & Key.ToString)
            End If
        Next
        lblKeyboardKeysPressed.Content = strKeyString.ToString
    End Sub


    Private Sub mnuSettingsAlwaysOnTop_CheckChanged(sender As Object, e As Windows.RoutedEventArgs) Handles mnuSettingsAlwaysOnTop.Checked, mnuSettingsAlwaysOnTop.Unchecked
        Me.Topmost = mnuSettingsAlwaysOnTop.IsChecked
    End Sub

    Private Sub mnuToolsPerformanceViewer_Checked(sender As Object, e As Windows.RoutedEventArgs) Handles mnuToolsPerformanceViewer.Checked
        wpfPerformanceViewer = New WPerformanceViewer
        ElementHost.EnableModelessKeyboardInterop(wpfPerformanceViewer)
        wpfPerformanceViewer.Show()

    End Sub

    Private Sub mnuToolsPerformanceViewer_Unchecked(sender As Object, e As Windows.RoutedEventArgs) Handles mnuToolsPerformanceViewer.Unchecked
        wpfPerformanceViewer.Close()

    End Sub

    Private Sub mnuToolsMouse_CheckChanged(sender As Object, e As Windows.RoutedEventArgs) Handles mnuToolsMouse.Checked, mnuToolsMouse.Unchecked
        If mnuToolsMouse.IsChecked = True Then
            cdivMouseInformation.Width = New Windows.GridLength(115)
            Me.Width += 115
        Else
            cdivMouseInformation.Width = New Windows.GridLength(0)
            Me.Width -= 115
        End If
    End Sub

    Private Sub CloseWindow() Handles mnuFileExit.Click
        Me.Close()
    End Sub

    Private Sub WConsole_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        GC.Collect()
        tmrCleanUp_Tick()
    End Sub
   
    Public Sub OpenDataSourceViewer()
        mnuToolsDataSourceViewer_Checked()
    End Sub
    Private Sub mnuToolsDataSourceViewer_Checked() Handles mnuToolsDataSourceViewer.Checked
        wpfDataSourceViewer = New WDataSourceViewer()
        wpfDataSourceViewer.Source = CConsole.Source
        wpfDataSourceViewer.Show()
    End Sub

    Private Sub mnuToolsDataSourceViewer_Unchecked(sender As Object, e As Windows.RoutedEventArgs) Handles mnuToolsDataSourceViewer.Unchecked
        wpfDataSourceViewer.Close()
        wpfDataSourceViewer = Nothing
    End Sub
End Class
