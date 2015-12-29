' ----------------------------------------------------------------------------------------------------------------------------
' CConsole
' Author: Donivan Jacob Hughes
' Created: 06-30-2015
' Last Updated: 12-29-2015
' Copyright 2015 Donivan Jacob Hughes
' ----------------------------------------------------------------------------------------------------------------------------
' This is part of the CConsole tool set.
' ----------------------------------------------------------------------------------------------------------------------------
Imports System.Drawing
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports DebugTools.ConsoleTools


Friend Class FConsole
    Delegate Sub SetTextCallback(strText As String)


    Delegate Sub SetPerformanceCallback(frmLocalPerformanceViewer As FPerformanceViewer)


    Private WithEvents tmrUpdateInfo As New Timer
    Private frmPerformanceViewer As FPerformanceViewer
    Private clrDisabled As Color

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        tsRight.Visible = False
        tmrUpdateInfo.Interval = 10
        tmrUpdateInfo.Start()
    End Sub

    Private Sub SetText(strText As String)
        rtxtConsole.AppendText(strText)

        Application.DoEvents()
    End Sub

    Private Sub SetPerformanceViewer(frmLocalPerformanceViewer As FPerformanceViewer)
        Try

            frmPerformanceViewer = frmLocalPerformanceViewer
            CConsole.WriteLine("Please Wait while we open the Performance View...")
            SetBusy(True)
            frmPerformanceViewer.Show()
            SetBusy(False)

            btnViewPerformance.BackColor = Color.DodgerBlue
            btnViewPerformance.Enabled = True
            Application.DoEvents()
        Catch ex As Exception
            CConsole.Write(ex.Message)
        End Try
    End Sub

    Friend Async Function Write(strText As String) As Task
        Await Task.Run(Sub()

            Dim d As New SetTextCallback(AddressOf SetText)
            Me.Invoke(d, New Object() {strText})
                          End Sub)
    End Function

    Private Async Sub btnViewPerformance_Click(sender As Object, e As EventArgs) Handles btnViewPerformance.Click

        If frmPerformanceViewer IsNot Nothing Then
            CConsole.WriteLine("Closing Performance Viewer...")
            SetBusy(True)
            frmPerformanceViewer.Close()
            frmPerformanceViewer.Dispose()
            frmPerformanceViewer = Nothing
            btnViewPerformance.BackColor = DefaultBackColor
            SetBusy(False)

        Else
            CConsole.WriteLine("Building Performance View...")
            btnViewPerformance.Enabled = False
            Await Task.Run(Sub()
                Dim frmLocalPerformanceViewer As New FPerformanceViewer

                Application.DoEvents()
                Dim d As New SetPerformanceCallback(AddressOf SetPerformanceViewer)
                Me.Invoke(d, New Object() {frmLocalPerformanceViewer})
                              End Sub)
        End If
    End Sub

    Private Sub SetBusy(blnBusy As Boolean)
        If blnBusy = True Then
            Cursor = Cursors.WaitCursor
            Enabled = False
        Else
            Cursor = Cursors.Default
            Enabled = True
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        CConsole.Clear()
    End Sub

    Private Sub tsbMouseData_Click(sender As Object, e As EventArgs) Handles tsbMouseData.Click
        tsRight.Visible = Not tsRight.Visible
        If tsRight.Visible = True Then
            tsbMouseData.BackColor = Color.DodgerBlue
            Me.Size = New Size(New Point(Me.Size.Width + tsRight.Width, Me.Size.Height))
        Else
            tsbMouseData.BackColor = DefaultBackColor
            Me.Size = New Size(New Point(Me.Size.Width - tsRight.Width, Me.Size.Height))
        End If
    End Sub

    Private Sub tmrUpdateInfo_Tick(sender As Object, e As EventArgs) Handles tmrUpdateInfo.Tick
        If tsRight.Visible = True Then
            UpdateMouseData()

        End If
    End Sub

    Private Sub UpdateMouseData()
        'Mouse data is displayed in the right toolbar. it is enabled and disabled via the mousedata button
        'Mouse Button
        Select Case MouseButtons
            Case 0
                lblMouseStatus.Text = "None"
            Case MouseButtons.Left
                lblMouseStatus.Text = "Left"
            Case MouseButtons.Right
                lblMouseStatus.Text = "Right"
            Case MouseButtons.Middle
                lblMouseStatus.Text = "Middle"
            Case MouseButtons.Right + MouseButtons.Left
                lblMouseStatus.Text = "L+R"
            Case MouseButtons.Right + MouseButtons.Middle
                lblMouseStatus.Text = "R+M"
            Case MouseButtons.Left + MouseButtons.Middle
                lblMouseStatus.Text = "L+M"
            Case MouseButtons.Right + MouseButtons.Left + MouseButtons.Middle
                lblMouseStatus.Text = "L+R+M"
            Case MouseButtons.XButton1
                lblMouseStatus.Text = "X1"
            Case MouseButtons.XButton2
                lblMouseStatus.Text = "X2"
        End Select

        'Mouse Position
        lblMousePosition.Text = MousePosition.ToString
    End Sub

    Private Sub tsbAlwaysOnTop_Click(sender As Object, e As EventArgs) Handles tsbAlwaysOnTop.Click
        Me.TopMost = Not Me.TopMost
        If TopMost = True Then
            tsbAlwaysOnTop.BackColor = Color.DodgerBlue
        Else
            tsbAlwaysOnTop.BackColor = DefaultBackColor
        End If
    End Sub
End Class