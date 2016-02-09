Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization

Namespace DebugTools
    Public Class Serialization

        Public Class JSON
            Public Shared Function Serialize(objToSerialize As Object) As String
                Dim jssSerializer As New JavaScriptSerializer
                jssSerializer.MaxJsonLength = Integer.MaxValue
                Dim strReturnValue As String = jssSerializer.Serialize(objToSerialize)
                Return strReturnValue
            End Function

            Public Shared Function Deserialize(Of T)(strJSON As String) As T
                Dim objReturnValue As T

                Dim jssSerializer As New JavaScriptSerializer
                jssSerializer.MaxJsonLength = Integer.MaxValue
                objReturnValue = jssSerializer.Deserialize(Of T)(strJSON)

                Return objReturnValue
            End Function
            Public Overloads Shared Function DeserializeFile(Of T)(strPath As String) As T
                Dim jssSerializer As New JavaScriptSerializer
                jssSerializer.MaxJsonLength = Integer.MaxValue
                Dim strJSON = File.ReadAllText(strPath)
                Dim objReturnValue As T = jssSerializer.Deserialize(Of T)(strJSON)
                Return objReturnValue
            End Function

            Public Shared Sub CacheObject(objToCache As Object, strPath As String)
                Dim strJSON As String = Serialize(objToCache)
                File.WriteAllText(strPath, strJSON, Encoding.GetEncoding(1252))
            End Sub

        End Class
    End Class
End Namespace

