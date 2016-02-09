' ----------------------------------------------------------------------------------------------------------------------------
' CConsole
' Author: Donivan Jacob Hughes
' Created: 06-30-2015
' Last Updated: 09-28-2015
' Copyright 2015 Donivan Jacob Hughes
' ----------------------------------------------------------------------------------------------------------------------------
' Changelog
' ----------------------------------------------------------------------------------------------------------------------------
' Version 0.1:
'           - Initial build
' Version 0.1.1: 
'           - Added first draft of the Performance Monitor
' Version 0.1.2:
'           - Added mouse information toggleable tab.
'           - Added partial logging functionality, but throw a NotImplemented exception unless in Dev Mode
'           - Added Always On Top toggleable button
'           - Changed FPerformanceViewer and FConsole from Public to Friend, so they are inaccessible outside of DebugTools
'           - Commented out the entire class and updated all the documentation.
' Version 0.2:
'           - Converted the entire form into a WPF. This is still accessible from a winform,
'             and may work with WPF applications as well now
'           - The Status bar functions (such as 'Always on Top') are now in the menu on the top of the form,
'             and are now checkable, vs the old highlighted method.
'           - The new form and functionality are accessible via the WPF sub class
' Version 0.3:
'           - Added multitab functionality. This enables multiple streams of output to be visually separate.
' Version 0.3.0.1:;
'           - Added write interval for a continuous checking of something.
' Version 0.3.1:
'           - Added DataSourceViewer. Allows quick viewing of datatables and objects.
' Version 0.4:
'           - Removed WinForm Version functionality. Left the form in however.
'           - Corrected write for IEnumerables to work with WPf version
'           - Added functionality to clean up lingering write tasks every 1 second.
' Version 0.4.0.1:
'           - Reorganized code into regions
'           - Fixed WriteClass to only write to console once, ensuring the call will be displayed
'             as a block even when the volume of calls to write is high.
'           - Did the same for the IEnumerable overload of Write
'           - Added tab overloads for both of them, as well as a interval overload for the IEnumerable overload of Write
' Version 0.4.1:
'           - Added tabbing to the DataSourceViewer. It now creates a new tab, rather than a new window, when you set a new source.
'             This will enable visual tracking of changes in the datatables.
' Version 0.4.2.1:
'           - Added a tools menu to DataSourceViewer that uses EPPlus to write the current tabs datatable to an excel file and allow you to save it.
' Version 0.4.2.2:
'           - Added scroll bar to console window textbox
' Version 0.5:
'           - Converted the windows output area from a Textbox to a RichTextbox
'           - It now writes to a single paragraph until you specify otherwise via NewParagraph
'             which, like everything else, works on a per window basis. If at any point you use any of the new 
'             functions , such as SetFontSize, it will affect the entire current paragraph.
'           - Added functions to manipulate the current paragraph:
'               SetStyle - Let you Bold, Italicize, Underline,, Strike and a few other things
'               SetFontSize - Sets the current paragraphs font size to the number specified
'           - Changed the write functionality to use a queue system, so while it may be slower now, all of your
'             writes and changes should be in order.
' Version 0.5.0.1:
'           - Shortened the update interval
'           - Removed a couple wait loops
' Version 0.6:
'           - Added Error check to IsNullOrEmpty
'           - Changed AddNewWindow to be in a new thread so I can set Apartmentstate to STA
'           - Changed structure of windows so that each window(with the exception of overflow windows) is its own queue, meaning two
'             windows will write simultaneously instead of in order.
'           - Added functionality to prevent stack overflows. It now creates a new window, which is still referenced by the same key
'             and writes to the new window instead. Hopefully this will solve issues writing huge amounts of text.
'             The limit is currently set to 10k characters.
'           - Fixed a bug where if the write process finished too quickly, the wait line would throw an error.
'           - Fixed a bug where IsNullOrEmpty would throw an error due to the fact that items were still being added to the queue
' Version 0.6.1:
'           - Added possible fix for writes not queuing correctly
' Version 0.6.2:
'           - Added Total Writes field in status bar
'           - Added some info for the CConsole itself into a new menu object:
'               - If main console is processing
'               - How many queued items there are between all queues
'               - How many queues are still processing items
'           - Corrected some spelling mistakes
' Version 0.7:
'           - Added some functionality for the new DebugPlatform, a locally running instance of CConsole that any application can send
'             debug information too.
'           - Changed all write overloads to funnel through the fully parameterized version for instantiation purposes
' Version 0.7.1:
'           - Added a compatibility version for 3.5 projects
' Version 1.0:
'           - Finally functional enough to call a full version
'           - Adding functionality for a status view to keep tabs on verious things.
' ----------------------------------------------------------------------------------------------------------------------------
' Tasks
' ----------------------------------------------------------------------------------------------------------------------------
' CONTENTTODO: Add datatable change tracking (Might be too resource intensive)
' CONTENTTODO: Possibly add a generic way to track changes in an object
' CONTENTTODO: Add more text manipulation functionality
' ----------------------------------------------------------------------------------------------------------------------------
' Imports
' ----------------------------------------------------------------------------------------------------------------------------
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Forms.Integration


Namespace ConsoleTools
    ''' ------------------------------------------------------------------------------------------
    ''' Name: CConsole
    ''' ------------------------------------------------------------------------------------------
    ''' <summary>
    '''     A static class acting as a replacement for the Windows Console window, allowing
    '''     a console window for debugging purposes, even in a WinForm application. It also contains
    '''     an array of useful debugging tools.
    ''' </summary>
    ''' <remarks>Note: All writes are done asynchronously</remarks>
    ''' ------------------------------------------------------------------------------------------
    Public Class CConsole

#Region "Private Variables"
        Private Shared WithEvents wpfConsole As WConsole
        Private Shared blnSendToPlatformFailure As Boolean = False
        Private Shared WithEvents frmSender As New Form
        Private Shared objSource As Object
        Private Shared m_blnIsDevMode As Boolean = False
        Private Shared m_blnEnabled As Boolean = True
        Private Shared m_strSeparator As String = "--------------------------"
        Private Shared ReadOnly m_strDebugLogLocation As String = "C:\Dev\DebugLogFiles\" & Now.ToString("MM-dd-yyyy") & ".txt"
        Private Shared m_fiLogFile As FileInfo
        Private Shared m_swLogFile As StreamWriter
        Private Shared ReadOnly m_lstrAlreadyWritten As New List(Of String)
        Private Shared WithEvents evlLogConfiguration As EventLog
        Private Shared blnIsOpen As Boolean = False
        Private Shared blnAlwaysOnTop As Boolean = False
        Private Shared ptrLocalWindow As IntPtr

#End Region

#Region "Public Properties"
        ''' ------------------------------------------------------------------------------------------
        ''' <summary>Gets or sets the separator the CConsole will use for <see cref="CConsole.WriteSeparator">WriteSeperator</see>. </summary>
        ''' <value>The new separator to use.</value>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Property Separator As String
            Get
                Return m_strSeparator
            End Get
            Set(value As String)
                m_strSeparator = value
            End Set
        End Property

        ''' ------------------------------------------------------------------------------------------
        ''' <summary>Gets or sets a value indicating whether this <see cref="CConsole" /> is enabled. </summary>
        ''' <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        ''' <remarks>
        '''     If the <see cref="CConsole" /> is disabled, no instance of FConsole will be created and none of the functions
        '''     will do anything allowing you to enable and disable for debugging and production.
        '''     This is overridden by the application setting ToolsEnabled
        ''' </remarks>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Property Enabled As Boolean
            Get
                Dim blnReturnValue As Boolean
                If My.Settings.ToolsEnabled = True Then
                    blnReturnValue = m_blnEnabled
                Else
                    blnReturnValue = False
                End If
                Return blnReturnValue
            End Get
            Set(value As Boolean)
                m_blnEnabled = value
            End Set
        End Property

        ''' ------------------------------------------------------------------------------------------
        ''' <summary>Gets or sets a value indicating whether this instance is in development mode. </summary>
        ''' <value><c>true</c> if this instance is in development mode; otherwise, <c>false</c>.</value>
        ''' <remarks>When set to true, removes errors from NotImplemented features.</remarks>
        '''  ------------------------------------------------------------------------------------------
        Public Property IsDevMode As Boolean
            Get
                Return m_blnIsDevMode
            End Get
            Set(value As Boolean)
                m_blnIsDevMode = value
            End Set
        End Property

        Public Shared Property Source As Object
            Get
                Return objSource
            End Get
            Set(value As Object)
                objSource = value
                Open()
                wpfConsole.OpenDataSourceViewer()
            End Set
        End Property

        Public Shared Property AlwaysOnTop As Boolean
            Get
                Return blnAlwaysOnTop
            End Get
            Set(value As Boolean)
                blnAlwaysOnTop = value
            End Set
        End Property

        Public Shared ReadOnly Property TextLength(strWindow As String) As Double
            Get
                Dim dblReturnValue As Double = -1
                If Window IsNot Nothing Then
                    dblReturnValue = wpfConsole.TextLength(strWindow)
                End If
                Return dblReturnValue
            End Get
        End Property

        Public Shared Property Window As WConsole
            Get
                Return wpfConsole
            End Get
            Set(value As WConsole)
                If wpfConsole IsNot Nothing AndAlso wpfConsole IsNot value Then
                    wpfConsole.Close()
                End If

                wpfConsole = value
            End Set
        End Property

#End Region

#Region "Events"

        ''' ------------------------------------------------------------------------------------------
        ''' Name: ConsoleWindowClosed
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Whenever the console is closed, delete the instance of it. </summary>
        ''' ------------------------------------------------------------------------------------------
        Private Shared Sub ConsoleWindowClosed() Handles wpfConsole.Closed
            'Clear instance reference
            wpfConsole = Nothing
        End Sub

#End Region

#Region "Enums"
        Public Enum TextStyles
            Bold
            Italic
            Underline
            BaseLine
            OverLine
            Strike
        End Enum

#End Region

#Region "Write Overloads"
        ''' ------------------------------------------------------------------------------------------
        '''  Name: Write
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified text to the CConsole output. </summary>
        ''' <param name="strText">The text to write to the CConsole output.</param>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub Write(strText As String)
            If Enabled AndAlso My.Settings.ToolsEnabled Then

                Write(strText, "Main", 0)
            End If
            Forms.Application.DoEvents()
        End Sub

        ''' ------------------------------------------------------------------------------------------
        '''  Name: Write
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified text to the CConsole output. </summary>
        ''' <param name="strText">The text to write to the CConsole output.</param>
        ''' <param name="strWindowName">The name of the window to write the output into.</param>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub Write(strText As String, strWindowName As String)
            If Enabled Then

                Write(strText, strWindowName, 0)
            End If
            Forms.Application.DoEvents()
        End Sub

        ''' ------------------------------------------------------------------------------------------
        '''  Name: Write
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified text to the CConsole output. </summary>
        ''' <param name="strText">The text to write to the CConsole output.</param>
        ''' <param name="strWindowName">The name of the window to write the output into.</param>
        ''' <param name="dblInterval">The minimum time in seconds allowed between writes to the output window</param>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub Write(strText As String, strWindowName As String, dblInterval As Double)
            If blnSendToPlatformFailure = False Then
                Try
                    blnSendToPlatformFailure = Not SendMessageToPlatform(strText, strWindowName, dblInterval)
                Catch ex As Exception
                    blnSendToPlatformFailure = True
                End Try
            End If

            If Enabled AndAlso blnSendToPlatformFailure = True Then
                Initialize()
                wpfConsole.Write(strText, strWindowName, dblInterval)
            End If
            Forms.Application.DoEvents()
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: Write
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified IEnumerable to the CConsole output. </summary>
        ''' <param name="ienuOutput">The IEnumberable to write to the CConsole output.</param>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub Write(ienuOutput As IEnumerable)
            Write(ienuOutput, "Main", 0)
        End Sub
        ''' ------------------------------------------------------------------------------------------
        ''' Name: Write
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified IEnumerable to the CConsole output. </summary>
        ''' <param name="xelOutput">The XElement to write to the CConsole output.</param>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub Write(xelOutput As XElement)
            Write(xelOutput.ToString, "Main", 0)
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: Write
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified IEnumerable to the CConsole output. </summary>
        ''' <param name="ienuOutput">The IEnumberable to write to the CConsole output.</param>
        ''' <param name="strWindow">The name of the window on which to write</param>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub Write(ienuOutput As IEnumerable, strWindow As String)
            Write(ienuOutput, strWindow, 0)
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: Write
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified IEnumerable to the CConsole output. </summary>
        ''' <param name="ienuOutput">The IEnumberable to write to the CConsole output.</param>
        ''' <param name="strWindow">The name of the window on which to write</param>
        ''' <param name="dblInterval">The minimum time between writes to the console</param>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteEnumberable(ienuOutput As IEnumerable, strWindow As String, dblInterval As Double)
            If Enabled = True Then
                'Initialize()
                Dim strTextToPrint As String = ""
                strTextToPrint &= "List Type: " & ienuOutput.GetType().ToString & vbNewLine
                Dim intItemIndex As Double = 0
                If ienuOutput.IsNullOrEmpty = False Then
                    For Each Item In ienuOutput
                        strTextToPrint &= "Item " & intItemIndex & ": " & Item.ToString & vbNewLine
                        intItemIndex += 1
                    Next
                Else
                    strTextToPrint &= "Empty" & vbNewLine
                End If
                Write(strTextToPrint, strWindow, dblInterval)
            End If
        End Sub
#End Region

#Region "WriteLine Overloads"
        Public Shared Sub WriteLine(strText As String)
            If Enabled Then
                Write(strText & vbNewLine, "Main")
            End If
            Forms.Application.DoEvents()
        End Sub

        Public Shared Sub WriteLine(strText As String, strWindowName As String)
            If Enabled Then
                Write(strText & vbNewLine, strWindowName)
            End If
            Forms.Application.DoEvents()
        End Sub

        Public Shared Sub WriteLine(strText As String, strWindowName As String, dblInterval As Double)
            If Enabled Then
                Write(strText & vbNewLine, strWindowName, dblInterval)
            End If
            Forms.Application.DoEvents()

        End Sub

#End Region
#Region "RichTextBox Functionality"
        Public Shared Sub NewParagraph()
            If Enabled Then
                Initialize()
                wpfConsole.NewParagraph("Main")
            End If
        End Sub
        Public Shared Sub NewParagraph(strWindow As String)
            If Enabled Then
                Initialize()
                wpfConsole.NewParagraph(strWindow)
            End If
        End Sub
        Public Shared Sub SetStyle(TextStyle As TextStyles)
            If Enabled Then
                Initialize()
                wpfConsole.SetStyling("Main", TextStyle)
            End If
        End Sub
        Public Shared Sub SetStyle(strWindow As String, TextStyle As TextStyles)
            If Enabled Then
                Initialize()
                wpfConsole.SetStyling(strWindow, TextStyle)
            End If
        End Sub
        Public Shared Sub SetFontSize(dblFontSize As Double)
            If Enabled Then
                Initialize()
                wpfConsole.SetStyling("Main", dblFontSize)
            End If
        End Sub
        Public Shared Sub SetFontSize(strWindow As String, dblFontSize As Double)
            If Enabled Then
                Initialize()
                wpfConsole.SetStyling(strWindow, dblFontSize)
            End If
        End Sub
#End Region
#Region "Secondary Functionality"
        Private Shared Sub Initialize()
            If Enabled Then
                If wpfConsole Is Nothing Then
                    wpfConsole = New WConsole
                    ElementHost.EnableModelessKeyboardInterop(Window)
                    wpfConsole.Show()
                End If
            End If
        End Sub

        ''' ------------------------------------------------------------------------------------------
        '''  Name: Open
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Opens the CConsole window </summary>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub Open()
            If Enabled Then
                Initialize()
            End If
        End Sub
        ''' ------------------------------------------------------------------------------------------
        '''  Name: NewWindow
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Adds a new output window </summary>
        ''' <param name="strName">The name of the window.</param>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub NewWindow(strName As String)
            If Enabled Then
                wpfConsole.Write("", strName)
            End If
            Forms.Application.DoEvents()
        End Sub

        ''' ------------------------------------------------------------------------------------------
        '''  Name: Clear
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Clears the main output window. </summary>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub Clear()
            If Enabled Then
                Initialize()
                wpfConsole.ClearWindow("Main")
            End If

        End Sub
        ''' ------------------------------------------------------------------------------------------
        '''  Name: Clear
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Clears the specified output window. </summary>
        ''' <param name="strWindow">The name of the window to clear.</param>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub Clear(strWindow)
            If Enabled Then
                Initialize()
                wpfConsole.ClearWindow(strWindow)
            End If
        End Sub
#End Region

#Region "WriteClass"


        ''' ------------------------------------------------------------------------------------------
        ''' Name: WriteClass
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Breaks down the class, and prints all visible fields and properties to the console. </summary>
        ''' <param name="objObject">The object to write out.</param>
        ''' <param name="strWindowName">The name of the window on which to write</param>
        ''' <remarks> Also breaks out lists in the object and prints them as well.
        ''' Defaults to printing to the Main Tab</remarks>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteClass(objObject As Object, strWindowName As String)
            'Check if console window is enabled
            If Enabled = True Then
                Dim strTextToPrint As String = ""
                'Get the object type
                Dim typObject As Type = objObject.GetType()
                Dim objMemberValue As Object
                'Write the object's type to the console window
                strTextToPrint &= typObject.Name & vbNewLine
                'Iterate through all members of public visible elements in the object
                'Get and Set for each element counts as separate members
                For Each member As MemberInfo In typObject.GetMembers
                    'Get Member type
                    Dim mType = member.MemberType

                    'Handle according to the type
                    If mType = MemberTypes.Property Then
                        objMemberValue = GetPropertyValue(objObject, member.Name)
                        'If we were able to get a value out, print member name and value
                        If objMemberValue IsNot Nothing Then
                            strTextToPrint &= vbTab & member.Name & ": " & objMemberValue.ToString & vbNewLine
                        End If
                    ElseIf mType = MemberTypes.Field Then
                        objMemberValue = GetFieldValue(objObject, member.Name)
                        'If we were able to get a value out, print member name and value
                        If objMemberValue IsNot Nothing Then
                            strTextToPrint &= vbTab & member.Name & ": " & objMemberValue.ToString & vbNewLine
                        End If
                    End If

                Next
                CConsole.WriteLine(strTextToPrint, strWindowName)
            End If
        End Sub
        ''' ------------------------------------------------------------------------------------------
        ''' Name: WriteClass
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Breaks down the class, and prints all visible fields and properties to the console. </summary>
        ''' <param name="objObject">The object to write out.</param>
        ''' <remarks> Also breaks out lists in the object and prints them as well.
        ''' Defaults to printing to the Main Tab</remarks>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteClass(objObject As Object)
            'Check if console window is enabled
            If Enabled = True Then
                Dim strTextToPrint As String = ""
                'Get the object type
                Dim typObject As Type = objObject.GetType()
                Dim objMemberValue As Object
                'Write the object's type to the console window
                strTextToPrint &= typObject.Name & vbNewLine
                'Iterate through all members of public visible elements in the object
                'Get and Set for each element counts as separate members
                For Each member As MemberInfo In typObject.GetMembers
                    'Get Membet type
                    Dim mType = member.MemberType

                    'Handle according to the type
                    If mType = MemberTypes.Property Then
                        objMemberValue = GetPropertyValue(objObject, member.Name)
                        'If we were able to get a value out, print member name and value
                        If objMemberValue IsNot Nothing Then
                            strTextToPrint &= vbTab & member.Name & ": " & objMemberValue.ToString & vbNewLine
                        End If
                    ElseIf mType = MemberTypes.Field Then
                        objMemberValue = GetFieldValue(objObject, member.Name)
                        'If we were able to get a value out, print member name and value
                        If objMemberValue IsNot Nothing Then
                            strTextToPrint &= vbTab & member.Name & ": " & objMemberValue.ToString & vbNewLine
                        End If
                    End If

                Next
                CConsole.WriteLine(strTextToPrint)
            End If
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: GetPropertyValue
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Gets the specified properties value. </summary>
        ''' <param name="objObject">The containing object.</param>
        ''' <param name="strPropertyName">The name of the property.</param>
        ''' <returns>System.Object.</returns>
        ''' ------------------------------------------------------------------------------------------
        Private Shared Function GetPropertyValue(objObject As Object, strPropertyName As String) As Object
            'Get the property type
            Dim objType As Type = objObject.GetType()
            Dim objValue As Object
            Dim pInfo As PropertyInfo
            Dim strPropertyValue As Object = Nothing
            'Attempt to get the properties info
            Try
                pInfo = objType.GetProperty(strPropertyName)
            Catch ex As Exception
                'If unable to get the info, return a string stating that as the value
                strPropertyValue = "Unable to determine value"
                Return strPropertyValue
            End Try

            Dim ienuValue As IEnumerable
            'Ignore setter, as they contain no value
            If Not strPropertyName.Contains("set_") AndAlso pInfo IsNot Nothing Then
                'Remove the gets from getters, so it is formatted correctly for GetValue
                strPropertyName = strPropertyName.Replace("get_", "")
                Try
                    'Get the value
                    objValue = pInfo.GetValue(objObject, Nothing)

                    'If the value is enummerable, build a single string from the enummerables values
                    ienuValue = TryCast(objValue, IEnumerable)
                    'Value will be nothing if not enumberable
                    If TypeOf (objValue) Is String OrElse (ienuValue Is Nothing) Then
                        strPropertyValue = objValue
                    Else
                        If Not ienuValue.IsNullOrEmpty Then
                            strPropertyValue += vbNewLine
                            For Each Item In ienuValue
                                strPropertyValue += vbTab & vbTab & Item.ToString & vbNewLine
                            Next
                        Else
                            'If the enummerable has no values, send that information back as the value
                            strPropertyValue = "Empty"
                        End If

                    End If
                Catch ex As Exception
                    'If anything breaks, give up and move on
                    strPropertyValue = "Unable to determine value"
                End Try

            End If

            Return strPropertyValue
        End Function

        ''' ------------------------------------------------------------------------------------------
        ''' Name: GetFieldValue
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Gets the specified fields value. </summary>
        ''' <param name="objObject">The containing object.</param>
        ''' <param name="strFieldName">The name of the field.</param>
        ''' <returns>System.Object.</returns>
        ''' ------------------------------------------------------------------------------------------
        Private Shared Function GetFieldValue(objObject As Object, strFieldName As String) As Object
            Dim objType As Type = objObject.GetType()
            Dim objValue As Object

            Dim fInfo As FieldInfo = objType.GetField(strFieldName)
            Dim ienuValue As IEnumerable
            Dim strFieldValue As Object = Nothing
            If fInfo IsNot Nothing Then
                Try
                    'Get value as object
                    objValue = fInfo.GetValue(objObject)
                    'check if the value is an enumerable list
                    ienuValue = TryCast(objValue, IEnumerable)
                    'Value will be nothing if not enumberable
                    If TypeOf (objValue) Is String OrElse ienuValue Is Nothing Then
                        strFieldValue = objValue
                    Else
                        If Not ienuValue.IsNullOrEmpty Then
                            For Each Item In ienuValue
                                strFieldValue += vbTab & vbTab & Item.ToString & vbNewLine
                            Next
                        Else
                            strFieldValue = "Empty"
                        End If

                    End If

                Catch ex As Exception
                    strFieldValue = "Unable to determine value"
                End Try

            End If

            Return strFieldValue
        End Function
#End Region

#Region "Specific Write Functions"
        ''' ------------------------------------------------------------------------------------------
        ''' Name: WriteSeparator
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes the current <see cref="Separator" /> with a return character at the end. </summary>
        ''' <remarks>Write to the Main window</remarks>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteSeparator()
            'Check if console window is enabled
            If Enabled = True Then
                WriteLine(m_strSeparator)
            End If
        End Sub
        ''' ------------------------------------------------------------------------------------------
        ''' Name: WriteSeparator
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes the current <see cref="Separator" /> with a return character at the end. </summary>
        ''' <param name="strWindow">The name of the window on which to write</param>
        ''' <remarks>Write to the Main window</remarks>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteSeparator(strWindow As String)
            'Check if console window is enabled
            If Enabled = True Then
                WriteLine(m_strSeparator, strWindow)
            End If
        End Sub
        ''' ------------------------------------------------------------------------------------------
        ''' Name: WriteDataError
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Attachable handler for DataGridView DataErrors </summary>
        ''' <param name="sender">The sender.</param>
        ''' <param name="e">The <see cref="Windows.Forms.DataGridViewDataErrorEventArgs" /> instance containing the event data.</param>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteDataError(sender As Object, e As DataGridViewDataErrorEventArgs)
            WriteLine(vbNewLine & m_strSeparator & vbNewLine &
                      sender.Name & vbNewLine &
                      m_strSeparator & vbNewLine &
                      "Context:" & e.Context & vbNewLine &
                      "Exception:" & e.Exception.Message & vbNewLine &
                      "ColumnIndex:" & e.ColumnIndex & vbNewLine &
                      "RowIndex:" & e.RowIndex & vbNewLine &
                      m_strSeparator, DirectCast(sender, DataGridView).Name & " Data Error", sender.Name & "Data Error")
        End Sub
        ''' ------------------------------------------------------------------------------------------
        '''  Name: WriteException
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Writes the specified exception to the Erorr window. </summary>
        ''' <param name="ex">The exception to write.</param>
        '''  ------------------------------------------------------------------------------------------
        Public Shared Sub WriteException(ex As Exception)
            If Enabled Then
                Initialize()


                Forms.Application.DoEvents()

                Dim strMessage As String
                strMessage &= "Error: " & ex.Message & vbNewLine
                If ex.InnerException IsNot Nothing Then
                    strMessage &= "InnerException: " & ex.InnerException.Message & vbNewLine
                End If
                strMessage &= "Stack Trace: " & ex.StackTrace & vbNewLine & m_strSeparator
                wpfConsole.Write(strMessage & vbNewLine, "Error", blnIsException:=True)
            End If
        End Sub
#End Region

#Region "File Writing"

        ''' ------------------------------------------------------------------------------------------
        ''' Name: WriteToFile
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes a string to a debug file. </summary>
        ''' <param name="strOutput">The string to write.</param>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteToFile(strOutput As String)
            'Check if console window is enabled
            If Enabled = True Then
                'Check if debug file has already been created during this run
                If m_fiLogFile Is Nothing Then
                    'If it hasn't delete any existing version and create a new one
                    m_fiLogFile = New FileInfo(m_strDebugLogLocation)
                    If m_fiLogFile.Exists Then
                        m_fiLogFile.Delete()
                    End If
                    'If directory doesn't exist, create it
                    If m_fiLogFile.Directory.Exists = False Then
                        m_fiLogFile.Directory.Create()
                    End If
                    'Set up the file, and turn on autoflush, so it always go directly to the file
                    m_swLogFile = m_fiLogFile.CreateText
                    m_swLogFile.AutoFlush = True
                End If
                'Write to the file
                m_swLogFile.Write(strOutput)
            End If
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: WriteToFile
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Writes a string to a debug file. </summary>
        ''' <param name="strOutput">The string to write.</param>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub WriteDistinctToFile(strOutput As String)
            'Check if console window is enabled
            If Enabled = True Then
                'Check if debug file has already been created during this run
                If m_fiLogFile Is Nothing Then
                    'If it hasn't delete any existing version and create a new one
                    m_fiLogFile = New FileInfo(m_strDebugLogLocation)
                    If m_fiLogFile.Exists Then
                        m_fiLogFile.Delete()
                    End If
                    'If directory doesn't exist, create it
                    If m_fiLogFile.Directory.Exists = False Then
                        m_fiLogFile.Directory.Create()
                    End If
                    'Set up the file, and turn on autoflush, so it always go directly to the file
                    m_swLogFile = m_fiLogFile.CreateText
                    m_swLogFile.AutoFlush = True
                End If
                'If the exact string has already been written to the file, don't write again
                If m_lstrAlreadyWritten.Contains(strOutput) = False Then
                    m_lstrAlreadyWritten.Add(strOutput)
                    m_swLogFile.Write(strOutput)
                End If
            End If
        End Sub
#End Region

#Region "Event Logging(Unfinished)"


        ''' ------------------------------------------------------------------------------------------
        ''' Name: CreateSource
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Creates the specified source in the Amerisolution Log. </summary>
        ''' <param name="strLogName">Name of the source to add.</param>
        ''' <remarks>Errors out without admin privileges.</remarks>
        ''' <exception>NotImplemented</exception>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub CreateSource(strLogName As String)
            'Are we testing the console window?
            If m_blnIsDevMode = False Then
                'No, throw not implemented exception
                Throw New NotImplementedException("This feature is not ready for use")
            End If
            Try

                EventLog.CreateEventSource(strLogName, "AmeriSolutions")

            Catch ex As Exception
                WriteLine("Log " & strLogName & " was unable to be created." & vbNewLine &
                          "Error: " & ex.Message)
            End Try
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: Log
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Logs the specified text into the event log as an entry. </summary>
        ''' <param name="strLogText">The text to log.</param>
        ''' <exception>NotImplemented</exception>
        ''' ------------------------------------------------------------------------------------------
        Private Shared Sub Log(strLogText As String)
            'Are we testing the console window?
            If m_blnIsDevMode = False Then
                'No, throw not implemented exception
                Throw New NotImplementedException("This feature is not ready for use")
            End If
            If evlLogConfiguration Is Nothing Then
                Dim strLogName As String = EventLog.LogNameFromSourceName("", ".")
                evlLogConfiguration = New EventLog(strLogName, ".", "")
            End If
            evlLogConfiguration.WriteEntry("Point Of Origin: " & My.Application.Info.ProductName & " - Version " & My.Application.Info.Version.ToString & vbNewLine &
                                           "Message: " & strLogText)
            WriteLine("Logged: " & strLogText)
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: ReadLogs
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Prints the logs from the Amerisoultions eventlog to the console window. </summary>
        ''' <exception>NotImplemented</exception>
        ''' ------------------------------------------------------------------------------------------
        Private Shared Sub ReadLogs()
            'Are we testing the console window?
            If m_blnIsDevMode = False Then
                'No, throw not implemented exception
                Throw New NotImplementedException("This feature is not ready for use")
            End If
            If evlLogConfiguration Is Nothing Then
                evlLogConfiguration = New EventLog("Amerisolutions")
            End If

            WriteLine(Separator & "Event Logs" & Separator)
            WriteSeparator()
            For Each Entry As EventLogEntry In evlLogConfiguration.Entries
                WriteLine(Entry.Message & vbNewLine & Separator)
            Next
        End Sub

        ''' ------------------------------------------------------------------------------------------
        ''' Name: Log
        ''' ------------------------------------------------------------------------------------------
        ''' <summary> Logs the specified text into the event log as an entry. </summary>
        ''' <param name="objToLog">Object to write to log.</param>
        ''' <remarks> Calls ToString on the specified object</remarks>
        ''' <exception>NotImplemented</exception>
        ''' ------------------------------------------------------------------------------------------
        Public Shared Sub Log(objToLog As Object)
            'Are we testing the console window?
            If m_blnIsDevMode = False Then
                'No, throw not implemented exception
                Throw New NotImplementedException("This feature is not ready for use")
            End If
            Log(objToLog.ToString)
        End Sub


#End Region

#Region "Write To Platform"
        Public Shared Function SendMessageToPlatform(strText As String, strWindow As String, dblInterval As Double) As Boolean
            Dim blnSuccess = True
            Dim clsMessage As New Message(strText, strWindow, dblInterval)
            Dim strMessage As String = clsMessage.ToString
            Dim changeFilter As New NativeMethods.CHANGEFILTERSTRUCT()
            changeFilter.size = CUInt(Marshal.SizeOf(changeFilter))
            changeFilter.info = 0
            If Not NativeMethods.ChangeWindowMessageFilterEx(frmSender.Handle, NativeMethods.WM_COPYDATA, NativeMethods.ChangeWindowMessageFilterExAction.Allow, changeFilter) Then
                Dim [error] As Integer = Marshal.GetLastWin32Error()
                blnSuccess = False
            End If



            Dim windowTitle As String = "CConsoleReceiver"
            ' Find the window with the name of the main form
            If ptrLocalWindow = IntPtr.Zero Then
                ptrLocalWindow = NativeMethods.FindWindow(Nothing, windowTitle)
            End If
            If ptrLocalWindow = IntPtr.Zero Then
                blnSuccess = False
            Else
                Dim ptrCopyData As IntPtr = IntPtr.Zero
                Try
                    ' Create the data structure and fill with data
                    Dim copyData As New NativeMethods.COPYDATASTRUCT()
                    copyData.dwData = New IntPtr(2)
                    ' Just a number to identify the data type
                    copyData.cbData = strMessage.Length + 1
                    ' One extra byte for the \0 character
                    copyData.lpData = Marshal.StringToHGlobalAnsi(strMessage)

                    ' Allocate memory for the data and copy
                    ptrCopyData = Marshal.AllocCoTaskMem(Marshal.SizeOf(copyData))
                    Marshal.StructureToPtr(copyData, ptrCopyData, False)

                    ' Send the message
                    NativeMethods.SendMessage(ptrLocalWindow, NativeMethods.WM_COPYDATA, IntPtr.Zero, ptrCopyData)
                Catch ex As Exception

                    blnSuccess = False
                Finally
                    ' Free the allocated memory after the control has been returned
                    If ptrCopyData <> IntPtr.Zero Then
                        Marshal.FreeCoTaskMem(ptrCopyData)
                    End If
                End Try
            End If
            Return blnSuccess
        End Function
        Public Shared Function PreparePlatform() As Boolean
            Dim blnSuccess = False
            If SystemInformation.UserName = "hughes.dh.1" Then
                ptrLocalWindow = NativeMethods.FindWindow(Nothing, "CConsoleReceiver")
                If ptrLocalWindow = IntPtr.Zero Then
                    Dim app As String = "file://C:/Users/hughes.dh.1/OneDrive/Personal Projects/GitHub/DebugPlatform/publish/DebugPlatform.application"
                    Process.Start("rundll32.exe", Convert.ToString("dfshim.dll,ShOpenVerbApplication ") & app)
                    ptrLocalWindow = NativeMethods.FindWindow(Nothing, "CConsoleReceiver")
                End If
                If ptrLocalWindow = IntPtr.Zero Then
                    blnSuccess = True
                End If

            End If
            Return blnSuccess
        End Function
#End Region
       

       
    End Class
    Public Class Message
        Public Property Text As String = "{EMPTY}"
        Public Property Window As String = "Main"
        Public Property Interval As Integer = 0
        Public Sub New()

        End Sub
        Public Sub New(strText As String, strWindow As String, dblInteval As Double)
            Text = strText
            Window = strWindow
            Interval = dblInteval
        End Sub

        ''' ------------------------------------------------------------------------------------------
        '''  Name: ToString
        '''  ------------------------------------------------------------------------------------------
        ''' <summary> Returns a <see cref="System.String" /> that represents this instance. </summary>
        ''' <returns>A <see cref="System.String" /> that represents this instance.</returns>
        '''  ------------------------------------------------------------------------------------------
        Public Overrides Function ToString() As String

            Return DebugTools.Serialization.JSON.Serialize(Me)
        End Function
    End Class
End Namespace
