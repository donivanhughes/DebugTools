<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FConsole
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FConsole))
        Me.bwWriter = New System.ComponentModel.BackgroundWorker()
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.tsBottom = New System.Windows.Forms.ToolStrip()
        Me.btnViewPerformance = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsbMouseData = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnClear = New System.Windows.Forms.ToolStripButton()
        Me.rtxtConsole = New System.Windows.Forms.TextBox()
        Me.tsRight = New System.Windows.Forms.ToolStrip()
        Me.lblMouseStatus = New System.Windows.Forms.ToolStripLabel()
        Me.lblMousePosition = New System.Windows.Forms.ToolStripLabel()
        Me.tsbAlwaysOnTop = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripContainer1.BottomToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.RightToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.tsBottom.SuspendLayout()
        Me.tsRight.SuspendLayout()
        Me.SuspendLayout()
        '
        'bwWriter
        '
        Me.bwWriter.WorkerReportsProgress = True
        Me.bwWriter.WorkerSupportsCancellation = True
        '
        'ToolStripContainer1
        '
        '
        'ToolStripContainer1.BottomToolStripPanel
        '
        Me.ToolStripContainer1.BottomToolStripPanel.Controls.Add(Me.tsBottom)
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.rtxtConsole)
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(589, 250)
        Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStripContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        '
        'ToolStripContainer1.RightToolStripPanel
        '
        Me.ToolStripContainer1.RightToolStripPanel.Controls.Add(Me.tsRight)
        Me.ToolStripContainer1.Size = New System.Drawing.Size(661, 300)
        Me.ToolStripContainer1.TabIndex = 1
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        '
        'tsBottom
        '
        Me.tsBottom.Dock = System.Windows.Forms.DockStyle.None
        Me.tsBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnViewPerformance, Me.ToolStripSeparator1, Me.tsbMouseData, Me.ToolStripSeparator2, Me.tsbAlwaysOnTop, Me.ToolStripSeparator3, Me.btnClear})
        Me.tsBottom.Location = New System.Drawing.Point(9, 0)
        Me.tsBottom.Name = "tsBottom"
        Me.tsBottom.Size = New System.Drawing.Size(375, 25)
        Me.tsBottom.TabIndex = 0
        '
        'btnViewPerformance
        '
        Me.btnViewPerformance.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnViewPerformance.Image = CType(resources.GetObject("btnViewPerformance.Image"), System.Drawing.Image)
        Me.btnViewPerformance.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnViewPerformance.Name = "btnViewPerformance"
        Me.btnViewPerformance.Size = New System.Drawing.Size(79, 22)
        Me.btnViewPerformance.Text = "Performance"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'tsbMouseData
        '
        Me.tsbMouseData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbMouseData.Image = CType(resources.GetObject("tsbMouseData.Image"), System.Drawing.Image)
        Me.tsbMouseData.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbMouseData.Name = "tsbMouseData"
        Me.tsbMouseData.Size = New System.Drawing.Size(106, 22)
        Me.tsbMouseData.Text = "Show Mouse Data"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'btnClear
        '
        Me.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnClear.Image = CType(resources.GetObject("btnClear.Image"), System.Drawing.Image)
        Me.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(38, 22)
        Me.btnClear.Text = "Clear"
        '
        'rtxtConsole
        '
        Me.rtxtConsole.BackColor = System.Drawing.SystemColors.MenuText
        Me.rtxtConsole.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtxtConsole.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtxtConsole.ForeColor = System.Drawing.SystemColors.MenuBar
        Me.rtxtConsole.Location = New System.Drawing.Point(0, 0)
        Me.rtxtConsole.Multiline = True
        Me.rtxtConsole.Name = "rtxtConsole"
        Me.rtxtConsole.ReadOnly = True
        Me.rtxtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.rtxtConsole.Size = New System.Drawing.Size(589, 250)
        Me.rtxtConsole.TabIndex = 0
        '
        'tsRight
        '
        Me.tsRight.Dock = System.Windows.Forms.DockStyle.None
        Me.tsRight.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblMouseStatus, Me.lblMousePosition})
        Me.tsRight.Location = New System.Drawing.Point(0, 3)
        Me.tsRight.Name = "tsRight"
        Me.tsRight.Size = New System.Drawing.Size(72, 47)
        Me.tsRight.TabIndex = 0
        '
        'lblMouseStatus
        '
        Me.lblMouseStatus.AutoSize = False
        Me.lblMouseStatus.Name = "lblMouseStatus"
        Me.lblMouseStatus.Size = New System.Drawing.Size(70, 15)
        Me.lblMouseStatus.Text = "--------"
        '
        'lblMousePosition
        '
        Me.lblMousePosition.AutoSize = False
        Me.lblMousePosition.Name = "lblMousePosition"
        Me.lblMousePosition.Size = New System.Drawing.Size(71, 15)
        Me.lblMousePosition.Text = "-------------"
        '
        'tsbAlwaysOnTop
        '
        Me.tsbAlwaysOnTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbAlwaysOnTop.Image = CType(resources.GetObject("tsbAlwaysOnTop.Image"), System.Drawing.Image)
        Me.tsbAlwaysOnTop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbAlwaysOnTop.Name = "tsbAlwaysOnTop"
        Me.tsbAlwaysOnTop.Size = New System.Drawing.Size(91, 22)
        Me.tsbAlwaysOnTop.Text = "Always On Top"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'FConsole
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(661, 300)
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "FConsole"
        Me.Text = "Console"
        Me.ToolStripContainer1.BottomToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.BottomToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.PerformLayout()
        Me.ToolStripContainer1.RightToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.RightToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.tsBottom.ResumeLayout(False)
        Me.tsBottom.PerformLayout()
        Me.tsRight.ResumeLayout(False)
        Me.tsRight.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bwWriter As System.ComponentModel.BackgroundWorker
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tsBottom As System.Windows.Forms.ToolStrip
    Friend WithEvents btnViewPerformance As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnClear As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsbMouseData As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents rtxtConsole As System.Windows.Forms.TextBox
    Friend WithEvents tsRight As System.Windows.Forms.ToolStrip
    Friend WithEvents lblMouseStatus As System.Windows.Forms.ToolStripLabel
    Friend WithEvents lblMousePosition As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tsbAlwaysOnTop As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
End Class
