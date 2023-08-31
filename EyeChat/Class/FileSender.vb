Imports System.Net.Sockets
Imports System.Text

Public Class FileSender

    Public Sub SendFile(filePath As String, ipAddress As String, port As Integer)
        Dim client As New TcpClient(ipAddress, port)
        Dim data As Byte() = System.IO.File.ReadAllBytes(filePath)

        Using stream As NetworkStream = client.GetStream()
            stream.Write(data, 0, data.Length)
            stream.Close()
        End Using

        client.Close()
    End Sub

End Class
