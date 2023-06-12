Imports System.Collections.ObjectModel
Imports MahApps.Metro.Controls.Dialogs
Imports EyeChat.Message
Imports EyeChat.User
Imports EyeChat.UsersListBubbleCtrl
Imports System.ComponentModel

Class MainWindow
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Shared Property Messages As ObservableCollection(Of Message)
    Public Shared Property Users As ObservableCollection(Of User)
    Public Shared Property SelectedUser As String
    Public Shared Property SelectedUserMessages As ObservableCollection(Of Message)

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        Me.ShowCloseButton = True


        If String.IsNullOrWhiteSpace(My.Settings.UniqueId) Then
            My.Settings.UniqueId = GenerateUniqueId()
            My.Settings.Id = GetUniqueIdHashCode()
            My.Settings.Save()
        End If
        My.Settings.RoomNameDisplayUsers = Visibility.Visible
        My.Settings.NameRoomDisplayUsers = Visibility.Collapsed
        My.Settings.NameDisplayUsers = Visibility.Collapsed

        ' Initialisez la collection de messages
        Messages = If(LoadMessagesFromJson(), New ObservableCollection(Of Message)())
        Users = If(LoadUsersFromJson(), New ObservableCollection(Of User)())
        ' Initialisez les propriétés du code-behind ici
        'Users = New ObservableCollection(Of User)()


        ' Initialisez la collection de messages selectionné
        SelectedUserMessages = New ObservableCollection(Of Message)()

        ' Définissez le DataContext sur la fenêtre elle-même
        Me.DataContext = Me
        Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "avataaars.png")


        AddMessage("Benoit", "John", "Hello! 💖😁🐨🐱‍🐉👩🏿‍👩🏻‍👦🏽 lol", True, avatarPath)
        AddMessage("Benoit", "John", "Hello! 💖😁🐨🐱‍🐉👩🏿‍👩🏻‍👦🏽 lol", False, avatarPath)
        SelectUser("John")
        SaveUsersToJson(Users)
        SelectUserList("Benoit")


    End Sub

    Private Async Function LaunchGitHubSiteAsync() As Task
        ' Exemple : Ouvrir le site GitHub dans le navigateur par défaut
        Dim url As String = "https://github.com/lapinus57/EyeChat"
        Process.Start(url)
        Await ShowMessageAsync("This is the title", "Some message")
    End Function

    Public Function GenerateUniqueId() As String

        ' Génération d'un identifiant unique basé sur le nom d'utilisateur de Windows et le nom du PC
        Dim windowsUser As String = Environment.UserName
        Dim computerName As String = Environment.MachineName

        Return $"{windowsUser}_{computerName}"
    End Function

    Public Function GetUniqueIdHashCode() As Integer
        ' Génération d'un integer unique basé sur le UniqueId
        Return My.Settings.UniqueId.GetHashCode()
    End Function


    ' Méthode pour ajouter un nouveau message
    Public Sub AddMessage(ByVal name As String, ByVal sender As String, ByVal content As String, ByVal isAlignedRight As Boolean, ByVal avatar As String)
        If Messages IsNot Nothing Then

            Messages.Add(New Message With {.Name = name, .Sender = sender, .Content = content, .IsAlignedRight = isAlignedRight, .Timestamp = DateTime.Now, .Avatar = avatar})
            SaveMessagesToJson(Messages)
        End If
    End Sub

    ' Méthode pour sélectionner un utilisateur et afficher uniquement ses messages
    Public Shared Sub SelectUser(ByVal name As String)
        SelectedUser = name
        SelectedUserMessages.Clear()

        If Messages IsNot Nothing Then ' Vérifiez si la collection Messages est non nulle
            ' Utilisez LINQ pour filtrer les messages de l'utilisateur sélectionné
            Dim userMessages = Messages.Where(Function(m) m.Name = SelectedUser)
            ' Ajoutez chaque message filtré à la nouvelle ObservableCollection
            For Each message In userMessages
                SelectedUserMessages.Add(message)
            Next
        End If
    End Sub
    Public Sub SelectUserList(userName As String)

        'usersListCtrl.SelectUser(userName)

    End Sub


    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        SelectUser(SelectedUser)
        Dim index As Integer = test.Items.Cast(Of User)().ToList().FindIndex(Function(item) item.Name = "Benoit")
        If index >= 0 Then
            test.SelectedIndex = index
        End If

    End Sub

    Public Function updatemsglist()
        SelectUser(SelectedUser)
    End Function


End Class




