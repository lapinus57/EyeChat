Imports System.Collections.ObjectModel
Imports System.IO
Imports EyeChat.MainWindow
Imports Newtonsoft.Json
Public Class Computer

    Public Property ComputerID As String

    Public Property ComputerUser As String

    Public Property ComputerIp As String


    Public Shared Sub SaveComputersToJson()

        Dim json As String = JsonConvert.SerializeObject(Computers, Formatting.Indented)
        File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Computerlist.json"), json)
    End Sub


    ' Charge les ordinateurs à partir d'un fichier JSON
    Public Shared Sub LoadComputersFromJson()
        If File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Computerlist.json")) Then
            Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Computerlist.json"))
            Computers = JsonConvert.DeserializeObject(Of ObservableCollection(Of Computer))(json)
        Else
            Computers = New ObservableCollection(Of Computer)()
            SaveComputersToJson()
        End If
    End Sub

End Class
