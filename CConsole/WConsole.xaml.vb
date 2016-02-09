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
Imports Telerik.Windows.Controls


Public Class WConsole
    Delegate Sub SetTextCallback(strText As String)


    Private WithEvents frmReceiver As New CConsoleReciever
    Private wpfPerformanceViewer As WPerformanceViewer
    Private wpfDataSourceViewer As WDataSourceViewer
    Private dicWindows As Dictionary(Of String, WindowObject)
    Private dtmStart As DateTime
    Private dtmEnd As DateTime
    Private WithEvents tmrUpdateInfo As New Timers.Timer
    Private WithEvents tmrCleanUp As New Timers.Timer
    Private ldtmTimepoints As New List(Of Item)
    Private dblTimeSinceWrite As Double = 0
    Private lkeyPressedKeys As New List(Of Key)
    Private ltskToCleanUp As New List(Of Task)
    Private blnProcessing As Boolean = False
    Private intTotalWrites As Integer
    Private dblTimeSinceLastTaskRun
    Private tabToModify As TabItem
    Private niTaskBarIcon As New NotifyIcon
    Private blnQuit As Boolean = False
    Public Class Track
        Public Property Data() As IEnumerable(Of Item)

        Public Property StartDate() As Date
        Public Property EndDate() As Date
    End Class

    Public Class Item
        Public Property Duration() As TimeSpan
        Public Property [Date]() As Date
    End Class



    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        dicWindows = New Dictionary(Of String, WindowObject) From {{"Main", New WindowObject(txtConsoleWindow)}}
        dicWindows("Main").Name = "Main"
        Dim parNew As New Paragraph
        txtConsoleWindow.Document.Blocks.Clear()
        txtConsoleWindow.Document.Blocks.Add(parNew)
        txtConsoleWindow.Tag = parNew


        If CConsole.AlwaysOnTop = True Then
            mnuSettingsAlwaysOnTop.IsChecked = True
        End If
        tmrUpdateInfo.Interval = 10
        tmrCleanUp.Interval = 1000

        tmrUpdateInfo.Start()
        tmrCleanUp.Start()

        CConsole.Window = Me

        frmReceiver.Show()
        frmReceiver.Hide()

        dtmStart = DateTime.Now

        tctlWindowFrame.Tag = tabMain.ContextMenu

        niTaskBarIcon.Icon = My.Resources.ConsoleIcon
        Dim ShowWindow = (Sub(sender As Object, args As EventArgs)
                              Me.Show()
                              Me.WindowState = WindowState.Normal
                              niTaskBarIcon.Visible = False
                          End Sub)
        AddHandler niTaskBarIcon.DoubleClick, ShowWindow



    End Sub

    Public ReadOnly Property TextLength(strWindowName) As Double
        Get
            Dim dblReturnValue As Double = -1
            If dicWindows.ContainsKey(strWindowName) Then
                dblReturnValue = dicWindows(strWindowName).Textbox.Length
            End If
            Return dblReturnValue
        End Get
    End Property

    Private Sub SetText(strText As String)
        txtConsoleWindow.AppendText(strText)

        Forms.Application.DoEvents()
    End Sub

    Public Sub ReceiveMessage(clsMessage As ConsoleTools.Message) Handles frmReceiver.MessageReceived

        If Me.WindowState = Windows.WindowState.Minimized Then
            Me.Show()
            Me.WindowState = WindowState.Normal
            niTaskBarIcon.Visible = False
        End If
        Write(clsMessage.Text, clsMessage.Window, clsMessage.Interval)
    End Sub


    Friend Sub Write(strText As String, strWindow As String, Optional dblInterval As Double = 0, Optional ByVal blnIsException As Boolean = False)



        If Math.Abs(dblInterval - 0) <= 0 OrElse dblTimeSinceWrite > dblInterval Then
            intTotalWrites += 1
            dblTimeSinceWrite = 0

            If dicWindows.ContainsKey(strWindow) = False Then
                AddNewWindow(strWindow)
            End If
            Dim SuppressWarning = New Task(Sub()
                                               Dispatcher.InvokeAsync(New Action(Sub()
                                                                                     Try
                                                                                         WaitForProcessing()
                                                                                         blnProcessing = True
                                                                                         Dim parCurrent As Paragraph
                                                                                         If dicWindows(strWindow).Textbox.Length > Short.MaxValue Then
                                                                                             parCurrent = dicWindows(strWindow).Textbox.Tag
                                                                                             parCurrent.Inlines.Add("Handling OverFlow")
                                                                                             HandleOverFlow(dicWindows(strWindow))
                                                                                         End If
                                                                                         With dicWindows(strWindow).Textbox
                                                                                             parCurrent = .Tag
                                                                                             parCurrent.Inlines.Add(strText)
                                                                                             .ScrollToEnd()
                                                                                         End With
                                                                                         blnProcessing = False
                                                                                         Forms.Application.DoEvents()


                                                                                     Catch ex As Exception
                                                                                         blnProcessing = False
                                                                                         CConsole.WriteException(ex)
                                                                                     End Try
                                                                                 End Sub), Windows.Threading.DispatcherPriority.Background)
                                           End Sub)

            If blnIsException = False Then
                WaitForProcessing()

                dicWindows(strWindow).QueueTask(SuppressWarning)
            Else
                SuppressWarning.Start()
            End If

            'ltskToCleanUp.Add(SuppressWarning)

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
                                                                                     With dicWindows(strWindow).Textbox
                                                                                         .Document.Blocks.Add(parNew)
                                                                                         .Tag = parNew
                                                                                     End With
                                                                                     blnProcessing = False

                                                                                     Forms.Application.DoEvents()
                                                                                 Catch ex As Exception
                                                                                     blnProcessing = False
                                                                                     CConsole.WriteException(ex)
                                                                                 End Try
                                                                             End Sub), Windows.Threading.DispatcherPriority.Background)
                                       End Sub)
        WaitForProcessing()
        dicWindows(strWindow).QueueTask(SuppressWarning)


    End Sub
    Friend Sub SetStyling(strWindow As String, TextStyle As CConsole.TextStyles)
        Dim SuppressWarning = New Task(Sub()
                                           Dispatcher.InvokeAsync(New Action(Sub()
                                                                                 With DirectCast(dicWindows(strWindow).Textbox.Tag, Paragraph)


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
        WaitForProcessing()
        dicWindows(strWindow).QueueTask(SuppressWarning)


    End Sub
    Friend Sub SetStyling(strWindow As String, NewFontSize As Double)
        Dim SuppressWarning = New Task(Sub()
                                           Dispatcher.InvokeAsync(New Action(Sub()
                                                                                 With DirectCast(dicWindows(strWindow).Textbox.Tag, Paragraph)
                                                                                     .FontSize = NewFontSize
                                                                                 End With
                                                                             End Sub), Windows.Threading.DispatcherPriority.Background)
                                       End Sub)
        WaitForProcessing()
        dicWindows(strWindow).QueueTask(SuppressWarning)

    End Sub
    Private Sub WaitForProcessing()
        While blnProcessing = True
            If dblTimeSinceWrite > 1 Then
                blnProcessing = False
            End If
        End While
    End Sub
    Private Sub HandleOverFlow(clsWindowObject As WindowObject)

        Dispatcher.Invoke(Sub()


                              Try
                                  clsWindowObject.Processing = True
                                  Dim strOldWindowHeader As String
                                  Dim tabCopy As TabItem = New TabItem()
                                  Dim txtNewConsole As Windows.Controls.RichTextBox = Nothing


                                  clsWindowObject.OverflowCount += 1

                                  tabCopy.Header = clsWindowObject.Name & "(" & clsWindowObject.OverflowCount & ")"
                                  tabCopy.ContextMenu = tctlWindowFrame.Tag
                                  AddHandler tabCopy.ContextMenuOpening, AddressOf TabContextMenuOpening

                                  txtNewConsole = New Windows.Controls.RichTextBox
                                  txtNewConsole.Name = "txt" & clsWindowObject.Textbox.Name & clsWindowObject.OverflowCount

                                  txtNewConsole.Foreground = Windows.Media.Brushes.White
                                  txtNewConsole.Background = Windows.Media.Brushes.Black
                                  txtNewConsole.FontFamily = txtConsoleWindow.FontFamily
                                  txtNewConsole.FontSize = txtConsoleWindow.FontSize
                                  txtNewConsole.FontStyle = txtConsoleWindow.FontStyle
                                  txtNewConsole.BorderThickness = txtConsoleWindow.BorderThickness
                                  txtNewConsole.Padding = txtConsoleWindow.Padding

                                  txtNewConsole.Margin = txtConsoleWindow.Margin
                                  txtNewConsole.VerticalScrollBarVisibility = txtConsoleWindow.VerticalScrollBarVisibility
                                  txtNewConsole.Document.Blocks.Clear()
                                  Dim parNew As New Paragraph
                                  txtNewConsole.Document.Blocks.Add(parNew)
                                  txtNewConsole.Tag = parNew

                                  clsWindowObject.Textbox = txtNewConsole
                                  Dim dkpContainer As New DockPanel
                                  dkpContainer.Children.Add(txtNewConsole)
                                  tabCopy.Content = dkpContainer

                                  Dim intNewTabIndex As Integer = tctlWindowFrame.Items.Add(tabCopy)
                                  tctlWindowFrame.SelectedIndex = intNewTabIndex
                                  Forms.Application.DoEvents()
                                  clsWindowObject.Processing = False
                              Catch ex As Exception
                                  blnProcessing = False
                                  clsWindowObject.Processing = False
                                  CConsole.WriteException(ex)
                              End Try

                          End Sub
        )
    End Sub
    Friend Sub ClearWindow(strWindow As String)
        Try

            If dicWindows.ContainsKey(strWindow) Then
                With dicWindows(strWindow).Textbox
                    .Document.Blocks.Clear()
                    .Document.Blocks.Add(New Paragraph)
                    .Tag = .Document.Blocks(0)
                End With
            End If
        Catch ex As Exception
            blnProcessing = False
            CConsole.WriteException(ex)
        End Try
    End Sub
    <STAThread>
    Private Function AddNewWindow(strWindowName As String) As WindowObject

        Return Dispatcher.Invoke(Of WindowObject)(Function()
                                                      Dim clsNewContainer As WindowObject = Nothing


                                                      WaitForProcessing()
                                                      blnProcessing = True
                                                      Dim tabCopy As TabItem = New TabItem()
                                                      Dim txtNewConsole As Windows.Controls.RichTextBox = Nothing
                                                      tabCopy.Header = strWindowName
                                                      tabCopy.ContextMenu = tctlWindowFrame.Tag
                                                      AddHandler tabCopy.ContextMenuOpening, AddressOf TabContextMenuOpening

                                                      txtNewConsole = New Windows.Controls.RichTextBox
                                                      txtNewConsole.Name = "txt" & strWindowName.Replace(" ", "").Replace("-", "_").Replace(":", "_")
                                                      txtNewConsole.Foreground = Windows.Media.Brushes.White
                                                      txtNewConsole.Background = Windows.Media.Brushes.Black
                                                      txtNewConsole.FontFamily = txtConsoleWindow.FontFamily
                                                      txtNewConsole.FontSize = txtConsoleWindow.FontSize
                                                      txtNewConsole.FontStyle = txtConsoleWindow.FontStyle
                                                      txtNewConsole.BorderThickness = txtConsoleWindow.BorderThickness
                                                      txtNewConsole.Padding = txtConsoleWindow.Padding

                                                      txtNewConsole.Margin = txtConsoleWindow.Margin
                                                      txtNewConsole.VerticalScrollBarVisibility = txtConsoleWindow.VerticalScrollBarVisibility
                                                      txtNewConsole.Document.Blocks.Clear()

                                                      Dim parNew As New Paragraph
                                                      txtNewConsole.Document.Blocks.Add(parNew)
                                                      txtNewConsole.Tag = parNew
                                                      clsNewContainer = New WindowObject(txtNewConsole)
                                                      dicWindows.Add(strWindowName, clsNewContainer)
                                                      Dim dkpContainer As New DockPanel
                                                      dkpContainer.Children.Add(txtNewConsole)
                                                      tabCopy.Content = dkpContainer
                                                      clsNewContainer.Name = strWindowName
                                                      Dim intNewTabIndex As Integer = tctlWindowFrame.Items.Add(tabCopy)
                                                      tctlWindowFrame.SelectedIndex = intNewTabIndex
                                                      Forms.Application.DoEvents()
                                                      blnProcessing = False


                                                      Return clsNewContainer
                                                  End Function
        )


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
        Dim intTotalItemsQueued = 0
        Dim intTotalQueuesProcessing = 0
        For Each Key In dicWindows.Keys
            intTotalItemsQueued += dicWindows(Key).QueueCount
            If dicWindows(Key).Processing = True Then
                intTotalQueuesProcessing += 1
            End If
        Next
        lblQueuedItems.Content = "Items Queued: " & intTotalItemsQueued
        lblIsProcessing.Content = "Processing: " & blnProcessing
        lblQueuesProcessing.Content = "Queues Processing: " & intTotalQueuesProcessing
        lblTotalWrites.Content = "Total Writes Called: " & intTotalWrites
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
    Protected Overrides Sub OnStateChanged(e As EventArgs)
        If WindowState = WindowState.Minimized Then

            Me.Hide()
            niTaskBarIcon.Visible = True
        End If

        MyBase.OnStateChanged(e)
    End Sub
    Private Sub CloseWindow() Handles mnuFileExit.Click
        blnQuit = True
        Me.Close()
    End Sub

    Private Sub WConsole_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        If blnQuit = False Then
            e.Cancel = True
            Me.WindowState = WindowState.Minimized
        Else

            GC.Collect()
            tmrCleanUp_Tick()
            niTaskBarIcon.Visible = False
        End If

        
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


    Private Class WindowObject
        Public Property Processing As Boolean
        Public Property Textbox As Controls.RichTextBox
        Private WithEvents tmrRunTask As New Timers.Timer
        Private qtskToDo As New Queue(Of Task)
        Public Property OverflowCount As Integer


        Public ReadOnly Property QueueCount As Integer
            Get
                Return qtskToDo.Count
            End Get
        End Property
        Public Property Name As String
        Public Sub New(rtxtWindow As Controls.RichTextBox)
            Textbox = rtxtWindow
            tmrRunTask.Interval = 0.0001
            tmrRunTask.Start()
        End Sub
        Private Sub WaitForProcessing()
            While Processing = True
            End While
        End Sub
        Private Sub tmrRunTask_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles tmrRunTask.Elapsed
            WaitForProcessing()
            If qtskToDo.Count > 0 Then
                Processing = True
                Try


                    Dim tskNext As Task = qtskToDo.Dequeue
                    If tskNext IsNot Nothing Then

                        If tskNext.Status = TaskStatus.Created Then
                            tskNext.Start()

                        End If
                        If tskNext.IsCompleted = False Then
                            tskNext.Wait()
                        End If

                    End If

                    tskNext.Dispose()
                Catch ex As Exception
                    CConsole.WriteException(ex)
                End Try
                Processing = False
            Else
                'tmrRunTask.Stop()
            End If

        End Sub
        Public Sub QueueTask(tskToQueue As task)
            qtskToDo.Enqueue(tskToQueue)

            tmrRunTask.Start()
        End Sub

    End Class

    Private Sub ctmCloseTab_Click(sender As Object, e As RoutedEventArgs) Handles ctmCloseTab.Click
        dicWindows.Remove(tabToModify.Header)
        DirectCast(tabToModify.Parent, Controls.TabControl).Items.Remove(tabToModify)

    End Sub

    Private Sub ctmClearTab_Click(sender As Object, e As RoutedEventArgs) Handles ctmClearTab.Click
        ClearWindow(tabToModify.Header)
    End Sub

    Private Sub TabContextMenuOpening(sender As Object, e As ContextMenuEventArgs) Handles tabMain.ContextMenuOpening
        tabToModify = sender
    End Sub

   
    Private Sub mnuToolsStatusViewer_Click(sender As Object, e As RoutedEventArgs) Handles mnuToolsStatusViewer.Click
        Dim test As New WStatusViewer
        test.ShowDialog()
    End Sub
End Class
