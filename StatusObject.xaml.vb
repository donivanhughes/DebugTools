Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Public Class StatusObject
    Public Shared StateBrushes As New Dictionary(Of Status, Brush) From
 {{Status.Up, Brushes.Green},
  {Status.Unknown, Brushes.Yellow},
  {Status.Down, Brushes.Red}}
    Private ReadOnly tmrUpdate As New Forms.Timer
    Private enuState As Status = Status.Unknown
    Private funcCheckAction As Func(Of StateData) = (Function()
                                                         Return New StateData
                                                     End Function)
    Public Event StateChanged(sender As StatusObject, ByRef e As StatusChangedArgs)
    Public Event TextChanged(sender As StatusObject, ByRef e As TextChangedArgs)
    ''' ------------------------------------------------------------------------------------------
    ''' <summary>Gets or sets how often the UpdateAction is called in seconds. </summary>
    ''' <value>How often the UpdateAction is called in seconds</value>
    '''  ------------------------------------------------------------------------------------------
    Public Property UpdateInterval As Double
        Get
            Return tmrUpdate.Interval
        End Get
        Set(value As Double)
            tmrUpdate.Interval = value * 1000
        End Set
    End Property
    Public Property Header As String
        Get
            Return lblHeader.Content
        End Get
        Set(value As String)
            lblHeader.Content = value
        End Set
    End Property
    Public Property Text As String
        Get
            Return rtxtStatusInfo.Text
        End Get
        Set(value As String)
            If rtxtStatusInfo.Text <> value Then
                Dim e As New TextChangedArgs(rtxtStatusInfo.Text, value)
                RaiseEvent TextChanged(Me, e)
                If e.Cancel = False Then
                    rtxtStatusInfo.Document.Blocks.Clear()
                    Dim parText As New Paragraph
                    parText.Inlines.Add(value)
                    rtxtStatusInfo.Document.Blocks.Add(parText)
                End If
            End If
        End Set
    End Property
    Public Property State As Status
        Get
            Return enuState
        End Get
        Set(value As Status)
            If enuState <> value Then
                Dim e As New StatusChangedArgs(enuState, value)
                RaiseEvent StateChanged(Me, e)
                If e.Cancel = False Then
                    enuState = value
                    arcStatusColor.Fill = StateBrushes(value)
                    arcStatusColor.Stroke = StateBrushes(value)
                End If
            End If
        End Set
    End Property

    Public WriteOnly Property CheckAction As Func(Of StateData)
        Set(value As Func(Of StateData))
            funcCheckAction = value
        End Set
    End Property


    Public Enum Status
        Up
        Unknown
        Down
    End Enum

    Public Sub New()
        MyBase.New()

        Me.InitializeComponent()
        AddHandler tmrUpdate.Tick, AddressOf RunUpdateCheck

    End Sub
    Public Sub Start()
        tmrUpdate.Start()
    End Sub
    Public Sub [Stop]()
        tmrUpdate.Stop()
    End Sub
    Public Sub RunUpdateCheck()
        Dim clsStateData = funcCheckAction()
        If clsStateData Is Nothing Then
            State = clsStateData.State
            Text = clsStateData.Information
        End If
    End Sub

    Public Class StateData
        Public State As Status = Status.Unknown
        Public Information As String = "StateData.Information Not Set"
        Public Sub New()
        End Sub
        Public Sub New(enuState As Status, strInformation As String)
            State = enuState
            Information = strInformation
        End Sub
        Public Sub New(enuState As Status)
            State = enuState
        End Sub
        Public Sub New(strInformation As String)
            Information = strInformation
        End Sub
    End Class

    Public Structure StatusChangedArgs
        Public ReadOnly OldState As Status
        Public ReadOnly NewState As Status
        Public Cancel As Boolean
        Friend Sub New(enuOldState As Status, enuNewState As Status)
            OldState = enuOldState
            NewState = enuNewState
            Cancel = False
        End Sub
    End Structure
    Public Structure TextChangedArgs
        Public ReadOnly OldText As String
        Public ReadOnly NewText As String
        Public Cancel As Boolean
        Friend Sub New(enuOldText As String, enuNewText As String)
            OldText = enuOldText
            NewText = enuNewText
            Cancel = False
        End Sub
        
    End Structure
End Class
