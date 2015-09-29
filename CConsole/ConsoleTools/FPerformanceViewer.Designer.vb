<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FPerformanceViewer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.tmrCheck = New System.Windows.Forms.Timer(Me.components)
        Me.tvCategories = New System.Windows.Forms.TreeView()
        Me.pcMain = New System.Diagnostics.PerformanceCounter()
        Me.flpCounters = New System.Windows.Forms.FlowLayoutPanel()
        CType(Me.pcMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrCheck
        '
        Me.tmrCheck.Enabled = True
        '
        'tvCategories
        '
        Me.tvCategories.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.tvCategories.CheckBoxes = True
        Me.tvCategories.Dock = System.Windows.Forms.DockStyle.Left
        Me.tvCategories.Location = New System.Drawing.Point(0, 0)
        Me.tvCategories.Name = "tvCategories"
        Me.tvCategories.Size = New System.Drawing.Size(169, 391)
        Me.tvCategories.TabIndex = 0
        '
        'pcMain
        '
        Me.pcMain.CategoryName = ".NET Memory Cache 4.0"
        Me.pcMain.CounterName = "Cache Hits"
        Me.pcMain.InstanceName = "pcMain"
        '
        'flpCounters
        '
        Me.flpCounters.AutoScroll = True
        Me.flpCounters.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpCounters.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flpCounters.Location = New System.Drawing.Point(169, 0)
        Me.flpCounters.Name = "flpCounters"
        Me.flpCounters.Size = New System.Drawing.Size(349, 391)
        Me.flpCounters.TabIndex = 1
        '
        'FPerformanceViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(518, 391)
        Me.Controls.Add(Me.flpCounters)
        Me.Controls.Add(Me.tvCategories)
        Me.Name = "FPerformanceViewer"
        Me.Text = "FPerformanceViewer"
        CType(Me.pcMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrCheck As System.Windows.Forms.Timer
    Friend WithEvents tvCategories As System.Windows.Forms.TreeView
    Friend WithEvents pcMain As System.Diagnostics.PerformanceCounter
    Friend WithEvents flpCounters As System.Windows.Forms.FlowLayoutPanel
End Class
