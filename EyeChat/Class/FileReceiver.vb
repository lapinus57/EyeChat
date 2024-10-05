Imports System.Net.Sockets

Public Class FileReceiver

    Public Sub ReceiveFile(savePath As String, port As Integer)
        Dim listener As New TcpListener(System.Net.IPAddress.Any, port)
        listener.Start()

        Dim client As TcpClient = listener.AcceptTcpClient()
        Dim data(client.ReceiveBufferSize) As Byte

        Using stream As NetworkStream = client.GetStream()
            Dim bytesRead As Integer = stream.Read(data, 0, data.Length)
            Using fileStream As New System.IO.FileStream(savePath, System.IO.FileMode.Create)
                fileStream.Write(data, 0, bytesRead)
                fileStream.Close()
            End Using
            stream.Close()
        End Using

        client.Close()
        listener.Stop()
    End Sub

End Class
