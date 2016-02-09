Imports System.Runtime.InteropServices
Imports System.Windows.Forms


Public Class NativeMethods



    Private Sub New()
    End Sub
    ''' <summary>
    ''' Retrieves a handle to the top-level window whose class name and window name match 
    ''' the specified strings.
    ''' This function does not search child windows.
    ''' This function does not perform a case-sensitive search.
    ''' </summary>
    ''' <param name="lpClassName">If lpClassName is null, it finds any window whose title matches
    ''' the lpWindowName parameter.</param>
    ''' <param name="lpWindowName">The window name (the window's title). If this parameter is null,
    ''' all window names match.</param>
    ''' <returns>If the function succeeds, the return value is a handle to the window 
    ''' that has the specified class name and window name.</returns>
    <DllImport("user32.dll", SetLastError:=True)> _
    Public Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    End Function

    ''' <summary>
    ''' Handle used to send the message to all windows
    ''' </summary>
    Public Shared HWND_BROADCAST As New IntPtr(&HFFFF)

    ''' <summary>
    ''' An application sends the WM_COPYDATA message to pass data to another application.
    ''' </summary>
    Public Shared WM_COPYDATA As UInteger = &H4A

    ''' <summary>
    ''' Contains data to be passed to another application by the WM_COPYDATA message.
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure COPYDATASTRUCT
        ''' <summary>
        ''' User defined data to be passed to the receiving application.
        ''' </summary>
        Public dwData As IntPtr

        ''' <summary>
        ''' The size, in bytes, of the data pointed to by the lpData member.
        ''' </summary>
        Public cbData As Integer

        ''' <summary>
        ''' The data to be passed to the receiving application. This member can be IntPtr.Zero.
        ''' </summary>
        Public lpData As IntPtr
    End Structure

    ''' <summary>
    ''' Sends the specified message to a window or windows.
    ''' </summary>
    ''' <param name="hWnd">A handle to the window whose window procedure will receive the message.
    ''' If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level
    ''' windows in the system.</param>
    ''' <param name="Msg">The message to be sent.</param>
    ''' <param name="wParam">Additional message-specific information.</param>
    ''' <param name="lParam">Additional message-specific information.</param>
    ''' <returns>The return value specifies the result of the message processing; 
    ''' it depends on the message sent.</returns>
    <DllImport("user32.dll", CharSet:=CharSet.Unicode)> _
    Public Shared Function SendMessage(hWnd As IntPtr, Msg As UInt32, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    ''' <summary>
    ''' Values used in the struct CHANGEFILTERSTRUCT
    ''' </summary>
    Public Enum MessageFilterInfo As UInteger
        ''' <summary>
        ''' Certain messages whose value is smaller than WM_USER are required to pass 
        ''' through the filter, regardless of the filter setting. 
        ''' There will be no effect when you attempt to use this function to 
        ''' allow or block such messages.
        ''' </summary>
        None = 0

        ''' <summary>
        ''' The message has already been allowed by this window's message filter, 
        ''' and the function thus succeeded with no change to the window's message filter. 
        ''' Applies to MSGFLT_ALLOW.
        ''' </summary>
        AlreadyAllowed = 1

        ''' <summary>
        ''' The message has already been blocked by this window's message filter, 
        ''' and the function thus succeeded with no change to the window's message filter. 
        ''' Applies to MSGFLT_DISALLOW.
        ''' </summary>
        AlreadyDisAllowed = 2

        ''' <summary>
        ''' The message is allowed at a scope higher than the window.
        ''' Applies to MSGFLT_DISALLOW.
        ''' </summary>
        AllowedHigher = 3
    End Enum

    ''' <summary>
    ''' Values used by ChangeWindowMessageFilterEx
    ''' </summary>
    Public Enum ChangeWindowMessageFilterExAction As UInteger
        ''' <summary>
        ''' Resets the window message filter for hWnd to the default.
        ''' Any message allowed globally or process-wide will get through,
        ''' but any message not included in those two categories,
        ''' and which comes from a lower privileged process, will be blocked.
        ''' </summary>
        Reset = 0

        ''' <summary>
        ''' Allows the message through the filter. 
        ''' This enables the message to be received by hWnd, 
        ''' regardless of the source of the message, 
        ''' even it comes from a lower privileged process.
        ''' </summary>
        Allow = 1

        ''' <summary>
        ''' Blocks the message to be delivered to hWnd if it comes from
        ''' a lower privileged process, unless the message is allowed process-wide 
        ''' by using the ChangeWindowMessageFilter function or globally.
        ''' </summary>
        DisAllow = 2
    End Enum

    ''' <summary>
    ''' Contains extended result information obtained by calling 
    ''' the ChangeWindowMessageFilterEx function.
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure CHANGEFILTERSTRUCT
        ''' <summary>
        ''' The size of the structure, in bytes. Must be set to sizeof(CHANGEFILTERSTRUCT), 
        ''' otherwise the function fails with ERROR_INVALID_PARAMETER.
        ''' </summary>
        Public size As UInteger

        ''' <summary>
        ''' If the function succeeds, this field contains one of the following values, 
        ''' <see cref="MessageFilterInfo"/>
        ''' </summary>
        Public info As MessageFilterInfo
    End Structure

    ''' <summary>
    ''' Modifies the User Interface Privilege Isolation (UIPI) message filter for a specified window
    ''' </summary>
    ''' <param name="hWnd">
    ''' A handle to the window whose UIPI message filter is to be modified.</param>
    ''' <param name="msg">The message that the message filter allows through or blocks.</param>
    ''' <param name="action">The action to be performed, and can take one of the following values
    ''' <see cref="MessageFilterInfo"/></param>
    ''' <param name="changeInfo">Optional pointer to a 
    ''' <see cref="CHANGEFILTERSTRUCT"/> structure.</param>
    ''' <returns>If the function succeeds, it returns TRUE; otherwise, it returns FALSE. 
    ''' To get extended error information, call GetLastError.</returns>
    <DllImport("user32.dll", SetLastError:=True)> _
    Public Shared Function ChangeWindowMessageFilterEx(hWnd As IntPtr, msg As UInteger, action As ChangeWindowMessageFilterExAction, ByRef changeInfo As CHANGEFILTERSTRUCT) As Boolean
    End Function


    'Declare the mouse hook constant.
    'For other hook types, obtain these values from Winuser.h in Microsoft SDK.
    Shared WH_MOUSE As Integer = 4
    Shared hHook As Integer = 0
    Public Delegate Function Callback(ByVal nCode As Integer, _
   ByVal wParam As IntPtr, _
   ByVal lParam As IntPtr) As Integer
    'Keep the reference so that the delegate is not garbage collected.
    Private Shared hookproc As Callback

    'Import for the SetWindowsHookEx function.
    <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)> _
    Public Overloads Shared Function SetWindowsHookEx _
          (ByVal idHook As Integer, ByVal HookProc1 As Callback, _
           ByVal hInstance As IntPtr, ByVal wParam As Integer) As Integer
    End Function

    'Import for the CallNextHookEx function.
    <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)> _
    Public Overloads Shared Function CallNextHookEx _
          (ByVal idHook As Integer, ByVal nCode As Integer, _
           ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
    End Function
    'Import for the UnhookWindowsHookEx function.
    <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)> _
    Public Overloads Shared Function UnhookWindowsHookEx _
              (ByVal idHook As Integer) As Boolean
    End Function

    'Point structure declaration.
    <StructLayout(LayoutKind.Sequential)> Public Structure Point
        Public x As Integer
        Public y As Integer
    End Structure

    'MouseHookStruct structure declaration.
    <StructLayout(LayoutKind.Sequential)> Public Structure MouseHookStruct
        Public pt As Point
        Public hwnd As Integer
        Public wHitTestCode As Integer
        Public dwExtraInfo As Integer
    End Structure
    Public Shared Sub HookUp()
        hookproc = AddressOf MouseHookProc
        hHook = SetWindowsHookEx(WH_MOUSE, _
                                 hookproc, _
                                 IntPtr.Zero, _
System.Threading.Thread.CurrentThread.ManagedThreadId())
        If hHook.Equals(0) Then
            MsgBox("SetWindowsHookEx Failed")
            Return
        End If
    End Sub
   

    Public Shared Function MouseHookProc( _
   ByVal nCode As Integer, _
   ByVal wParam As IntPtr, _
   ByVal lParam As IntPtr) As Integer
        Dim MyMouseHookStruct As New MouseHookStruct()

        Dim ret As Integer

        If (nCode < 0) Then
            Return CallNextHookEx(hHook, nCode, wParam, lParam)
        End If

        MyMouseHookStruct = CType(Marshal.PtrToStructure(lParam, MyMouseHookStruct.GetType()), MouseHookStruct)

        Dim tempForm As Form
        tempForm = Form.ActiveForm()

        Dim strCaption As String
        strCaption = "x = " & MyMouseHookStruct.pt.x & " y = " & MyMouseHookStruct.pt.y

        tempForm.Text = strCaption
        Return CallNextHookEx(hHook, nCode, wParam, lParam)

    End Function
End Class





