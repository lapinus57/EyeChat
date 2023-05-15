Public Class LoginResult
    Public Property Username As String
    Public Property Password As String

    Public Sub New(username As String, password As String)
        Me.Username = username
        Me.Password = password
    End Sub
End Class
