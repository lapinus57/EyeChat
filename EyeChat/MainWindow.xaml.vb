Imports MahApps.Metro.Controls.Dialogs

Class MainWindow
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        Me.ShowCloseButton = True
    End Sub

    Private Async Function LaunchGitHubSiteAsync() As Task
        ' Exemple : Ouvrir le site GitHub dans le navigateur par défaut
        Dim url As String = "https://github.com/lapinus57/EyeChat"
        Process.Start(url)
        Await Me.ShowMessageAsync("This is the title", "Some message")
    End Function



End Class
