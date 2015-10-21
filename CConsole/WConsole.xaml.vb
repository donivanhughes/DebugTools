Imports System.Drawing
Imports System.Text
Imports System.Threading.Tasks
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Forms
Imports System.Windows.Forms.Integration
Imports System.Windows.Input
Imports DebugTools.ConsoleTools




Public Class WConsole
    Delegate Sub SetTextCallback(strText As String)


    'Delegate Sub SetPerformanceCallback(frmLocalPerformanceViewer As FPerformanceViewer)
    Private wpfPerformanceViewer As WPerformanceViewer
    Private wpfDataSourceViewer As WDataSourceViewer
    Private dicWindows As Dictionary(Of String, Windows.Controls.RichTextBox)
    Private WithEvents tmrUpdateInfo As New Timers.Timer
    Private WithEvents tmrCleanUp As New Timers.Timer
    Private WithEvents tmrRunTask As New Timers.Timer
    Private dblTimeSinceWrite As Double = 0
    Private lkeyPressedKeys As New List(Of Key)
    Private ltskToCleanUp As New List(Of Task)
    Private blnProcessing As Boolean = False
    Private qtskActions As New Queue(Of Task)




    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        dicWindows = New Dictionary(Of String, Windows.Controls.RichTextBox) From {{"Main", txtConsoleWindow}}

        Dim parNew As New Paragraph
        txtConsoleWindow.Document.Blocks.Clear()
        txtConsoleWindow.Document.Blocks.Add(parNew)
        txtConsoleWindow.Tag = parNew


        If CConsole.AlwaysOnTop = True Then
            mnuSettingsAlwaysOnTop.IsChecked = True
        End If
        tmrUpdateInfo.Interval = 10
        tmrCleanUp.Interval = 1000
        tmrRunTask.Interval = 0.00001
        tmrUpdateInfo.Start()
        tmrCleanUp.Start()
        tmrRunTask.Start()
    End Sub

    Private Sub SetText(strText As String)
        txtConsoleWindow.AppendText(strText)

        Forms.Application.DoEvents()
    End Sub


    'Friend Sub Write(strText As String)
    '    Dim SuppressWarning = New Task(Sub()
    '                                       Dispatcher.InvokeAsync(New Action(Sub()
    '                                                                             txtConsoleWindow.AppendText(strText)
    '                                                                             tmrRunTask.Start()
    '                                                                             Application.DoEvents()

    '                                                                         End Sub), Windows.Threading.DispatcherPriority.Background)
    '                                   End Sub)
    '    qtskActions.Enqueue(SuppressWarning)
    '    'ltskToCleanUp.Add(SuppressWarning)
    '    Application.DoEvents()


    'End Sub

    Friend Sub Write(strText As String, strWindow As String, Optional dblInterval As Double = 0)
        If dblInterval = 0 OrElse dblTimeSinceWrite > dblInterval Then
            dblTimeSinceWrite = 0

            If dicWindows.ContainsKey(strWindow) = False Then
                AddNewWindow(strWindow)
            End If
            Dim SuppressWarning = New Task(Sub()
                                               Dispatcher.InvokeAsync(New Action(Sub()
                                                                                     Try
                                                                                         WaitForProcessing()
                                                                                         blnProcessing = True
                                                                                         With dicWindows(strWindow)
                                                                                             Dim parCurrent As Paragraph = .Tag
                                                                                             parCurrent.Inlines.Add(strText)
                                                                                             .ScrollToEnd()
                                                                                         End With
                                                                                         blnProcessing = False
                                                                                         Forms.Application.DoEvents()
                                                                                     Catch ex As StackOverflowException
                                                                                         dicWindows(strWindow) = AddNewWindow(strWindow + "Overflow")
                                                                                     Catch ex As Exception
                                                                                         CConsole.WriteException(ex)
                                                                                     End Try
                                                                                 End Sub), Windows.Threading.DispatcherPriority.Background)
                                           End Sub)


            qtskActions.Enqueue(SuppressWarning)
            'ltskToCleanUp.Add(SuppressWarning)
            tmrRunTask.Start()
        End If
        Forms.Application.DoEvents()
    End Sub
    Friend Sub NewParagraph(strWindow As String)
        Dim SuppressWarning = New Task(Sub()
                                           Dispatcher.InvokeAsync(New Action(Sub()
                                                                                 Try
                                                                                     WaitForProcessing()
                                                                                     blnProcessing = True
                                                                                     Dim parNew As New Paragraph
                                                                                     With dicWindows(strWindow)
                                                                                         .Document.Blocks.Add(parNew)
                                                                                         .Tag = parNew
                                                                                     End With
                                                                                     blnProcessing = False

                                                                                     Forms.Application.DoEvents()
                                                                                 Catch ex As StackOverflowException
                                                                                     dicWindows(strWindow) = AddNewWindow(strWindow + "Overflow")
                                                                                 Catch ex As Exception
                                                                                     CConsole.WriteException(ex)
                                                                                 End Try
                                                                             End Sub), Windows.Threading.DispatcherPriority.Background)
                                       End Sub)
        qtskActions.Enqueue(SuppressWarning)
        tmrRunTask.Start()

    End Sub
    Friend Sub SetStyling(strWindow As String, TextStyle As CConsole.TextStyles)
        Dim SuppressWarning = New Task(Sub()
                                           Dispatcher.InvokeAsync(New Action(Sub()
                                                                                 With DirectCast(dicWindows(strWindow).Tag, Paragraph)


                                                                                     Select Case TextStyle
                                                                                         Case CConsole.TextStyles.Bold
                                                                                             .FontWeight = FontWeights.Bold
                                                                                         Case CConsole.TextStyles.Italic
                                                                                             .FontStyle = FontStyles.Italic
                                                                                         Case CConsole.TextStyles.Underline
                                                                                             .TextDecorations.Add(TextDecorations.Underline)
                                                                                         Case CConsole.TextStyles.OverLine
                                                                                             .TextDecorations.Add(TextDecorations.OverLine)
                                                                                         Case CConsole.TextStyles.Strike
                                                                                             .TextDecorations.Add(TextDecorations.Strikethrough)
                                                                                         Case CConsole.TextStyles.BaseLine
                                                                                             .TextDecorations.Add(TextDecorations.Baseline)
                                                                                     End Select

                                                                                 End With
                                                                             End Sub), Windows.Threading.DispatcherPriority.Background)
                                       End Sub)
        qtskActions.Enqueue(SuppressWarning)
        tmrRunTask.Start()
    End Sub
    Friend Sub SetStyling(strWindow As String, NewFontSize As Double)
        Dim SuppressWarning = New Task(Sub()
                                           Dispatcher.InvokeAsync(New Action(Sub()
                                                                                 With DirectCast(dicWindows(strWindow).Tag, Paragraph)
                                                                                     .FontSize = NewFontSize
                                                                                     End With
                                                                             End Sub), Windows.Threading.DispatcherPriority.Background)
                                       End Sub)
        qtskActions.Enqueue(SuppressWarning)
        tmrRunTask.Start()
    End Sub
    Private Sub WaitForProcessing()
        While blnProcessing = True
        End While
    End Sub
    Friend Sub ClearWindow(strWindow)
        dicWindows(strWindow).Document.Blocks.Clear()
    End Sub
    Private Function AddNewWindow(strWindowName As String) As Windows.Controls.RichTextBox
        Dim tabCopy As TabItem = New TabItem()

        tabCopy.Header = strWindowName
        Dim txtNewConsole As New Windows.Controls.RichTextBox
        txtNewConsole.Name = "txt" & strWindowName.Replace(" ", "")
        txtNewConsole.Foreground = Windows.Media.Brushes.White
        txtNewConsole.Background = Windows.Media.Brushes.Black
        txtNewConsole.FontFamily = txtConsoleWindow.FontFamily
        txtNewConsole.FontSize = txtConsoleWindow.FontSize
        txtNewConsole.FontStyle = txtConsoleWindow.FontStyle
        txtNewConsole.BorderThickness = txtConsoleWindow.BorderThickness
        txtNewConsole.Padding = txtConsoleWindow.Padding

        txtConsoleWindow.Margin = txtConsoleWindow.Margin
        txtNewConsole.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        txtNewConsole.Document.Blocks.Clear()
        Dim parNew As New Paragraph
        txtNewConsole.Document.Blocks.Add(parNew)
        txtNewConsole.Tag = parNew
        dicWindows.Add(strWindowName, txtNewConsole)
        Dim dkpContainer As New DockPanel
        dkpContainer.Children.Add(txtNewConsole)
        tabCopy.Content = dkpContainer

        Dim intNewTabIndex As Integer = tctlWindowFrame.Items.Add(tabCopy)
        tctlWindowFrame.SelectedIndex = intNewTabIndex
        Forms.Application.DoEvents()

        Return txtNewConsole
    End Function


    Private Sub tmrUpdateInfo_Tick() Handles tmrUpdateInfo.Elapsed
        dblTimeSinceWrite += tmrUpdateInfo.Interval
        Dispatcher.InvokeAsync(AddressOf UpdateInputData)
    End Sub
    Private Sub tmrCleanUp_Tick() Handles tmrCleanUp.Elapsed
        If ltskToCleanUp.Count > 0 Then
            For Each tskRunning As Task In ltskToCleanUp
                If tskRunning.IsCompleted Then
                    tskRunning.Dispose()
                End If

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
        If wpfDataSourceViewer Is Nothing Then
            wpfDataSourceViewer = New WDataSourceViewer()
        End If


        wpfDataSourceViewer.AddSource(CConsole.Source)
        wpfDataSourceViewer.Show()
    End Sub

    Private Sub mnuToolsDataSourceViewer_Unchecked(sender As Object, e As Windows.RoutedEventArgs) Handles mnuToolsDataSourceViewer.Unchecked
        wpfDataSourceViewer.Close()
        wpfDataSourceViewer = Nothing
    End Sub

    Private Sub tmrRunTask_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles tmrRunTask.Elapsed
        If blnProcessing = True Then
            Exit Sub
        End If
        If qtskActions.IsNullOrEmpty = False Then
            Dim tskNext As Task = qtskActions.Dequeue

            tskNext.Start()
            tskNext.Wait()

            tskNext.Dispose()
        Else
            tmrRunTask.Stop()
        End If

    End Sub
End Class
