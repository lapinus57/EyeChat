Imports System.Collections.ObjectModel
Imports MahApps.Metro.Controls.Dialogs

Class MainWindow

    Public Property UserList As New ObservableCollection(Of User)()
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        Me.ShowCloseButton = True
        UserlistCtrl.AddUser("Nouvel Utilisateur", "avatar4.jpg", "En ligne", "Actif")





    End Sub

    Private Async Function LaunchGitHubSiteAsync() As Task
        ' Exemple : Ouvrir le site GitHub dans le navigateur par défaut
        Dim url As String = "https://github.com/lapinus57/EyeChat"
        Process.Start(url)
        Await ShowMessageAsync("This is the title", "Some message")
    End Function



End Class


