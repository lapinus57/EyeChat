Imports System.Collections.ObjectModel

Public Class UserListUserControl
    Public Property UserList As ObservableCollection(Of User)

    Public Sub New()
        InitializeComponent()
        UserList = New ObservableCollection(Of User)()
        UserList.Add(New User() With {.Name = "A Tous", .Avatar = Nothing, .Status = "En ligne"})
        UserList.Add(New User() With {.Name = "Secrétariat", .Avatar = Nothing, .Status = "Absent"})
        UserList.Add(New User() With {.Name = "MédecinRDC", .Avatar = Nothing, .Status = "En ligne"})
        UserList.Add(New User() With {.Name = "Benoît", .Avatar = Nothing, .Status = "En ligne"})
        UserList.Add(New User() With {.Name = "Alicia", .Avatar = Nothing, .Status = "En ligne"})
        UserList.Add(New User() With {.Name = "Christelle", .Avatar = Nothing, .Status = "En ligne"})
        UserList.Add(New User() With {.Name = "Alix", .Avatar = Nothing, .Status = "En ligne"})
        UserList.Add(New User() With {.Name = "Caroline", .Avatar = Nothing, .Status = "En ligne"})
        UserList.Add(New User() With {.Name = "Esra", .Avatar = Nothing, .Status = "En ligne"})

        DataContext = Me
    End Sub

    Public Sub AddUser(name As String, avatar As String, status As String, userStatus As String)
        Dim newUser As New User() With {
            .Name = name,
            .Avatar = avatar,
            .Status = status
        }
        UserList.Add(newUser)
    End Sub
End Class
