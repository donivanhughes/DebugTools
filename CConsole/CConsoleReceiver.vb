Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports DebugTools.ConsoleTools


Public Class CConsoleReciever
    Public Event MessageReceived(clsMessage As ConsoleTools.Message)
    Public intMessagesRun As Integer
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub


    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = NativeMethods.WM_COPYDATA Then
            intMessagesRun += 1
            ' Extract the file name
            Dim copyData As NativeMethods.COPYDATASTRUCT = DirectCast(Marshal.PtrToStructure(m.LParam, GetType(NativeMethods.COPYDATASTRUCT)), NativeMethods.COPYDATASTRUCT)
            Dim dataType As Integer = CInt(copyData.dwData)
            If dataType = 2 Then
                Dim strMessage As String = Marshal.PtrToStringAnsi(copyData.lpData)
                Dim clsMessage As ConsoleTools.Message
                Try
                    clsMessage = DebugTools.Serialization.JSON.Deserialize(Of ConsoleTools.Message)(strMessage)

                Catch ex As Exception
                    If strMessage Is Nothing Then
                        strMessage = ""
                    End If
                    clsMessage = New ConsoleTools.Message(strMessage, "Message Error", 0)
                End Try
                RaiseEvent MessageReceived(clsMessage)

            Else
                Windows.MessageBox.Show([String].Format("Unrecognized data type = {0}.", dataType), "SendMessageDemo", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            End If
        Else
            MyBase.WndProc(m)
        End If
    End Sub

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================

End Class