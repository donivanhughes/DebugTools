Imports System.Drawing
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports DebugTools.ConsoleTools


Friend Class FPerformanceViewer
    Private lgrpDisplays As New List(Of PerformanceDisplay)
    Private WithEvents stpwCheck As New Stopwatch
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Try

            ' Add any initialization after the InitializeComponent() call.
            Dim ndeCurrentCategory As TreeNode
            Dim Categories() As PerformanceCounterCategory = PerformanceCounterCategory.GetCategories()
            Dim Counters As New ArrayList
            CConsole.WriteLine("Adding categories...")
            For intCategoryIndex As Integer = 0 To 10 'Categories.Length - 1
                If intCategoryIndex Mod 10 = 0 Then
                    CConsole.WriteLine(intCategoryIndex & " Categories Added")
                End If
                ndeCurrentCategory = tvCategories.Nodes.Add(Categories(intCategoryIndex).CategoryName)
                'Counters = Categories(intCategoryIndex).GetCounters
                'For intCounterIndex As Integer = 0 To Counters.Length - 1
                '    ndeCurrentCategory.Nodes.Add(Counters(intCounterIndex).CounterName)
                'Next

                Dim mycat As New PerformanceCounterCategory(ndeCurrentCategory.Text)
                ' Remove the current contents of the list. 

                ' Retrieve the counters. 
                Try
                    Dim instanceNames As String() = mycat.GetInstanceNames()
                    If (instanceNames.Length = 0) Then
                        Counters.AddRange(mycat.GetCounters())
                    Else
                        Dim i As Integer
                        For i = 0 To instanceNames.Length - 1

                            Counters.AddRange(mycat.GetCounters(instanceNames(i)))
                        Next
                    End If
                    ' Add the retrieved counters to the list. 
                    Dim counter As PerformanceCounter
                    For Each counter In Counters

                        If PerformanceCounterCategory.CounterExists(counter.CounterName, ndeCurrentCategory.Text) Then
                            Try
                                Dim Try1 = New PerformanceCounter(ndeCurrentCategory.Text, counter.CounterName)
                                Try1.NextValue()


                                ndeCurrentCategory.Nodes.Add(counter.CounterName)
                            Catch ex As Exception

                            End Try





                        End If

                    Next
                Catch ex As Exception
                    CConsole.WriteLine(ex.Message)
                End Try
            Next

        Catch ex As Exception
            CConsole.WriteLine(ex.Message)
        End Try
        CConsole.WriteLine("Cleaning up unused Categories...")
        For intIndex As Integer = tvCategories.Nodes.Count - 1 To 0 Step -1
            If tvCategories.Nodes.Item(intIndex).Nodes.Count = 0 Then
                tvCategories.Nodes.RemoveAt(intIndex)
            End If
        Next

    End Sub

    Private Sub FPerformanceViewer_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub FPerformanceViewer_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Application.DoEvents()
    End Sub

    Private Sub tvCategories_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles tvCategories.AfterCheck
        If e.Node.Checked = True Then
            If e.Node.Parent IsNot Nothing Then
                Try


                    e.Node.Tag = New PerformanceDisplay(New PerformanceCounter(e.Node.Parent.Text, e.Node.Text, New PerformanceCounterCategory(e.Node.Parent.Text).GetInstanceNames()(0), True))
                Catch ex As Exception
                    e.Node.Tag = New PerformanceDisplay(New PerformanceCounter(e.Node.Parent.Text, e.Node.Text))

                End Try

                lgrpDisplays.Add(e.Node.Tag)
                flpCounters.Controls.Add(e.Node.Tag)

            End If
        Else

            If e.Node.Tag IsNot Nothing Then
                lgrpDisplays.Remove(e.Node.Tag)
                e.Node.Tag.Dispose()
                e.Node.Tag = Nothing
            End If

        End If
    End Sub



    Private Sub tvCategories_BeforeCheck(sender As Object, e As TreeViewCancelEventArgs) Handles tvCategories.BeforeCheck
        For Each ndeChild As TreeNode In e.Node.Nodes
            ndeChild.Checked = Not e.Node.Checked
        Next
    End Sub


    Public Class PerformanceDisplay
        Inherits GroupBox

        Public Property PerformanceCounter As PerformanceCounter
        Private WithEvents tmrCheck As New Timer
        Public Property CheckEnabled As Boolean = False
        Public txtDisplay As New TextBox

        Public Sub New(pcToDisplay As PerformanceCounter)
            tmrCheck.Interval = 100
            tmrCheck.Start()
            Me.Size = New Size(200, 100)
            PerformanceCounter = pcToDisplay
            Text = pcToDisplay.CounterName
            Text = Text.Replace(vbNewLine, "")
            txtDisplay.Text = "Value"
            txtDisplay.Multiline = True
            txtDisplay.Dock = DockStyle.Fill
            Controls.Add(txtDisplay)
        End Sub

        Public Async Sub UpdateValue() Handles tmrCheck.Tick
            'PerformanceCounter.InstanceLifetime = PerformanceCounterInstanceLifetime.Global
            Dim test = PerformanceCounter



            'Dim tskNextValue As Task(Of Single) = Task.Run(Function()
            'Return test.NextValue
            '                                        End Function)

            txtDisplay.Text = GetNextValue()
        End Sub

        Private Async Sub WaitForTask(tskToWaitFor As Task(Of Single))
            Await tskToWaitFor
            txtDisplay.Text = tskToWaitFor.Result
        End Sub

        Private Function GetNextValue() As Single
            Return PerformanceCounter.NextValue
        End Function

    End Class
End Class