Imports System.Collections.ObjectModel
Imports MahApps.Metro.Controls.Dialogs
Imports EyeChat.Message
Imports EyeChat.User
Imports System.ComponentModel
Imports log4net.Repository.Hierarchy
Imports log4net
Imports Newtonsoft.Json.Linq
Imports log4net.Core
Imports EyeChat.SettingsViewModel
Imports log4net.Layout
Imports EyeChat.Patient
Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text
Imports System.Web.UI.WebControls
Imports Octokit

Class MainWindow
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Shared selectedUserName As String
    Public Shared Property Computers As ObservableCollection(Of Computer)
    Public Shared Property Patients As ObservableCollection(Of Patient)
    Public Shared Property Messages As ObservableCollection(Of Message)
    Public Shared Property Users As ObservableCollection(Of User)
    Public Shared Property SelectedUserMessages As ObservableCollection(Of Message)
    Public Shared Property SelectedUser As String

    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public Property DebugLevels As New ObservableCollection(Of String)() From {"DEBUG", "INFO", "WARN", "ERROR"}


    Delegate Sub AddReceivedMessage(ByRef message As String)
    Private Const port As Integer = 50545
    Private Const broadcastAddress As String = "255.255.255.255"
    Private receivingClient As UdpClient
    Private sendingClient As UdpClient
    Private receivingThread As Thread

    Private suggestions As List(Of String) = New List(Of String)() From {"/Suggestion1", "/Suggestion2", "/Suggestion3"}




    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        Me.ShowCloseButton = True


        If String.IsNullOrWhiteSpace(My.Settings.WindowsName) Then
            My.Settings.WindowsName = Environment.UserName
            My.Settings.Save()
        End If
        If String.IsNullOrWhiteSpace(My.Settings.ComputerName) Then
            My.Settings.ComputerName = Environment.MachineName
            My.Settings.Save()
        End If
        If String.IsNullOrWhiteSpace(My.Settings.UniqueId) Then
            My.Settings.UniqueId = GenerateUniqueId()
            My.Settings.Id = GetUniqueIdHashCode()
            My.Settings.Save()
        End If
        My.Settings.RoomNameDisplayUsers = Visibility.Visible
        My.Settings.NameRoomDisplayUsers = Visibility.Collapsed
        My.Settings.NameDisplayUsers = Visibility.Collapsed

        ' Initialise la collection de messages
        Messages = If(LoadMessagesFromJson(), New ObservableCollection(Of Message)())
        ' Initialise la collection des users
        Users = If(LoadUsersFromJson(), New ObservableCollection(Of User)())
        ' Initialise la collection des patients
        Patients = If(LoadPatientsFromJson(), New ObservableCollection(Of Patient)())
        ' Initialise la collection des ordinateurs 
        Computers = New ObservableCollection(Of Computer)
        ' Initialisez la collection de messages selectionné
        SelectedUserMessages = New ObservableCollection(Of Message)()


        ' Définissez le DataContext sur la fenêtre elle-même
        Me.DataContext = Me
        Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "avataaars.png")


        'AddMessage("Benoit", "John", "Hello! 💖😁🐨🐱‍🐉👩🏿‍👩🏻‍👦🏽 lol", True, avatarPath)
        'AddMessage("Benoit", "Benoit", "Hello! 💖😁🐨🐱‍🐉👩🏿‍👩🏻‍👦🏽 lol", False, avatarPath)

        SelectUser("Benoit")
        'SaveUsersToJson(Users)

        InitializeSender()
        InitializeReceiver()

        'SavePatientsToJson(Patients)
        'SelectUserList("Benoit")
        'SelectUserList("benoit")
        ListUseres.SelectedItem = Users.FirstOrDefault(Function(user) user.Name = "Benoit")



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
        SelectedUserMessages.Clear()
        If Messages IsNot Nothing Then ' Vérifiez si la collection Messages est non nulle
            ' Utilisez LINQ pour filtrer les messages de l'utilisateur sélectionné
            Dim userMessages = Messages.Where(Function(m) m.Name = name)
            ' Ajoutez chaque message filtré à la nouvelle ObservableCollection
            For Each message In userMessages
                SelectedUserMessages.Add(message)
            Next
        End If
    End Sub







    Public Sub updatemsglist()
        SelectUser(SelectedUser)
        MahApps.Metro.Controls.HeaderedControlHelper.SetHeaderFontSize(TabCtrl, My.Settings.AppSizeDisplay)
    End Sub


#Region "Gestion de l'ajout, suppression et modification d'un patient"

    Public Sub PatientAdd(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Annotation As String, ByVal Position As String, ByVal Colors As String, ByVal Examinator As String)
        Patients.Add(New Patient With {.Title = Title, .LastName = LastName, .FirstName = FirstName, .Exams = Exams, .Annotation = Annotation, .Position = Position, .Hold_Time = Date.Now, .IsTaken = False, .Colors = Colors, .Examinator = Examinator})
    End Sub
    Public Sub PatientRemove()
        Dim patientToRemove As Patient = Patients.FirstOrDefault(Function(patient) patient.FirstName = "benoit")
        If patientToRemove IsNot Nothing Then
            Patients.Remove(patientToRemove)
        End If
    End Sub

    Public Sub ModifyPatient(ByVal lastName As String, ByVal updatedPatient As Patient)
        For index As Integer = 0 To Patients.Count - 1
            If Patients(index).LastName = lastName Then
                Patients(index) = updatedPatient
                Exit For
            End If
        Next
    End Sub

#End Region

#Region "Gestion UDP et messages"
    Private Sub InitializeSender()
        sendingClient = New UdpClient(broadcastAddress, port)
        sendingClient.EnableBroadcast = True
    End Sub

    Private Sub InitializeReceiver()
        Dim receiveThread As New Thread(AddressOf ReceiveMessages)
        receiveThread.IsBackground = True
        receiveThread.Start()
    End Sub

    Private Sub ReceiveMessages()
        receivingClient = New UdpClient(port)
        Dim endPoint As IPEndPoint = New IPEndPoint(IPAddress.Any, port)

        While True
            Dim receiveBytes As Byte() = receivingClient.Receive(endPoint)
            Dim receivedMessage As String = Encoding.UTF8.GetString(receiveBytes)

            ' Utilisation de Dispatcher.Invoke pour exécuter la méthode MessageReceived sur le thread de l'interface utilisateur
            Dispatcher.Invoke(Sub() MessageReceived(receivedMessage))
        End While
    End Sub

    Private Sub MessageReceived(ByVal receivedMessage As String)
        ' Traitez le message reçu en fonction du code
        Dim messageCode As String = receivedMessage.Substring(0, 5)


        Select Case messageCode
            Case "USR01"
                ' Code de message de connexion 
                ' "USR01|UserName|IdentifiantPC|Color"
            Case "USR02"
                ' Code message de déconnection
                ' "USR01|UserName|IdentifiantPC"
            Case "USR03"
                ' Code message pour le changement de nom
                '"USR01|UserName Ancien|UserName new"
            Case "USR04"
                 ' Code message pour le changement de coleur
                '"USR01|UserName|NewColor"

            Case "USR11"
                ' Code message




            Case "PTN01"
                ' Code de message pour ajouter un patient au RDC
                ' "PTN01|Nom|Prénom|Autres informations"
                PatientAdd("Mr", "muller", "benoit", "FO", Nothing, "RDC", "Green", "benoit")

            Case "PTN02"
                ' Code de message pour la mise à jour d'un patient au RDC
                ' "PTN02|Nom|Prénom|Autres informations"

            Case "PTN03"
                ' Code de message pour supprimer un patient au RDC
                ' "PTN03|Nom|Prénom"


            Case "SYS01"
                ' Code pour fermer la boite à distance
                ' Exemple du message envoyé : "SYS01|IdentifiantPC(cible)|IdentifiantPC(auteur)"

                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim targetPC As String = parts(0)
                    Dim authorPC As String = parts(1)

                    ' Vérification de l'identifiant PC de la cible
                    If targetPC = My.Settings.UniqueId Then
                        logger.Debug("Réception d'un message SYS01 pour fermeture à distance.")
                        SaveMessagesToJson(Messages)
                        SaveUsersToJson(Users)
                        Me.Close() ' Fermer la fenêtre
                    End If
                Catch ex As Exception
                    ' Gestion de l'exception
                    logger.Error("Une erreur s'est produite lors du traitement du message SYS01 : " & ex.Message)
                End Try


            Case "SYS02"
                ' Code pour déconnecter l'utilisateur à distance
                ' Exemple du message envoyé : "SYS02|IdentifiantPC(cible)|IdentifiantPC(auteur)"

                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim targetPC As String = parts(0)
                    Dim authorPC As String = parts(1)
                    If targetPC = My.Settings.UniqueId Then
                        logger.Debug("Réception d'un message SYS02 pour déconnexion à distance.")
                        SaveMessagesToJson(Messages)
                        SaveUsersToJson(Users)
                        SelectedUserMessages.Clear()
                        Users.Clear()
                    End If
                Catch ex As Exception
                    logger.Error("Erreur lors de la déconnexion à distance : " & ex.Message)
                End Try

            Case "MSG01"

                Dim avatarPathd As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
                AddMessage("Marvin", "Marvin", "ll", False, avatarPathd)
                'Ajouter un message
                '"MSG01{My.Settings.UserName}|{selectedUser.Name}|{Message}|{Avatar}"
                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim destinataire As String = parts(0)
                    Dim author As String = parts(1)
                    Dim message As String = parts(2)
                    Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", parts(3))

                    If destinataire = My.Settings.UserName Then

                        AddMessage(author, author, message, False, avatarPath)

                    Else
                        AddMessage(destinataire, author, message, False, avatarPath)
                    End If



                Catch ex As Exception

                End Try

                'SelectUserList("Benoit")
                'SelectUser("Benoit")
                'PatientRemove()

            Case "MSG02"
                'Pour supprimer un message



            Case "SLN01"


            Case "DBG01"
                'Code pour envoyer l'ID du PC au autres application
                Dim IDmessage As String = My.Settings.UniqueId

                SendMessageWithCode("DBG02", My.Settings.UniqueId)
            Case "DBG02"
                'Code pour enregistrer l'ID des autre PC dans la collection computer
                Dim messageContent As String = receivedMessage.Substring(5)
                If Not Computers.Any(Function(c) c.ComputerID = messageContent) Then
                    Computers.Add(New Computer With {.ComputerID = messageContent})
                End If

            Case Else
                ' Code pour les messages non reconnus
                ' ...
        End Select
    End Sub

    'Fonction permettant l'envoie d'un code suivit d'un message
    Private Sub SendMessageWithCode(code As String, content As String)
        If code IsNot Nothing And content IsNot Nothing Then
            Dim message As String = code + content
            Dim messageBytes As Byte() = Encoding.UTF8.GetBytes(message)
            sendingClient.Send(messageBytes, messageBytes.Length)
        End If

    End Sub

    ' Fonction premettant l'envoie de message
    Public Sub Sendmessage(message As String)
        If message <> "" Then
            Dim DataMessage() As Byte = Encoding.UTF8.GetBytes(message)
            If DataMessage IsNot Nothing Then
                Try
                    sendingClient.Send(DataMessage, DataMessage.Length)
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

#End Region

    Private Sub SendTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles SendTextBox.KeyDown
        If e.Key = Key.Enter Then
            SendTextBox.Text = SendTextBox.Text.TrimEnd()
            If Not String.IsNullOrEmpty(SendTextBox.Text) Then
                Select Case SendTextBox.Text
                    Case "/DEBUG"
                        Try
                            Dim UserToAdd As User = Users.FirstOrDefault(Function(user) user.Name = "Marvin")
                            If UserToAdd Is Nothing Then
                                Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
                                Users.Add(New User With {.Name = "Marvin", .Room = "Sytem", .Avatar = avatarPath, .Status = "Don't Panic"})
                            End If

                        Catch ex As Exception

                        End Try

                    Case "/ENDDEBUG"
                        Try
                            Dim UserToRemove As User = Users.FirstOrDefault(Function(user) user.Name = "Marvin")
                            If UserToRemove IsNot Nothing Then
                                Users.Remove(UserToRemove)
                            End If
                        Catch ex As Exception

                        End Try

                    Case "/LSTCOMPUTER"
                        Sendmessage("DBG01")

                    Case "/DISPCOMPUTER"
                        Dim computerString As New StringBuilder()
                        computerString.AppendLine("Actuellement il y a :")
                        For Each computer In Computers
                            computerString.AppendLine(computer.ComputerID)
                        Next
                        Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
                        AddMessage("Marvin", "Marvin", computerString.ToString, False, avatarPath)


                    Case Else

                        'Formate le message sous la forme "MSG01{My.Settings.UserName}|{selectedUser.Name}|{Message}|{Avatar}"
                        Try
                            Dim text As String
                            Dim selectedUser As User = TryCast(ListUseres.SelectedItem, User)
                            If selectedUser IsNot Nothing Then
                                Dim selectedUserName As String = selectedUser.Name
                                ' Faites quelque chose avec le nom de l'utilisateur sélectionné
                                ' ...
                                text = "MSG01" & My.Settings.UserName & "|" & selectedUserName & "|" & SendTextBox.Text & "|system.png"
                                Sendmessage(text)

                            End If

                        Catch ex As Exception

                        End Try




                End Select

            End If

            SendTextBox.Clear()
        End If
    End Sub


    Private Sub ListUseres_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListUseres.SelectionChanged
        Dim selectedUser As User = TryCast(ListUseres.SelectedItem, User)
        If selectedUser IsNot Nothing Then
            selectedUserName = selectedUser.Name
            ' Faites quelque chose avec le nom de l'utilisateur sélectionné
            ' ...
            'Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
            'AddMessage("Marvin", "Marvin", selectedUser.Name, False, avatarPath)
        End If
    End Sub






    Private Sub ConnectionButon_Click(sender As Object, e As RoutedEventArgs)
        SelectUserByName("Marvin")


        'If ConnectionButon.Content = "Connection" Then
        'ShowLoginDialogPreview()
        ' Else
        'ConnectionButon.Content = "Connection"
        'logger.Debug($" {My.Settings.UserName} s'est déconnecté du poste.")
        'SaveMessagesToJson(Messages)
        'SaveUsersToJson(Users)
        'SelectedUserMessages.Clear()
        'Users.Clear()
        'My.Settings.UserName = ""
        'My.Settings.Save()

        'End If
    End Sub
    Private Async Sub ShowLoginDialogPreview()

        Dim result = Await Me.ShowInputAsync("Connection", "lapin", New MetroDialogSettings() With {.NegativeButtonText = "Annuler", .AffirmativeButtonText = "Connection"})
        'Dim result = Await Me.ShowLoginAsync("Eyechat connection", "", New LoginDialogSettings() With {.NegativeButtonVisibility = True, .NegativeButtonText = "Annuler"})
        If result Is Nothing Then
            ' L'utilisateur a appuyé sur Annuler ou a fermé la boîte de dialogue
        Else
            If result = "benoit" Then
                My.Settings.UserName = result
                My.Settings.Save()
                ConnectionButon.Content = "Deconnection"
                Dim loadedUsers = LoadUsersFromJson()
                Users.Clear()
                For Each user In loadedUsers
                    Users.Add(user)
                Next
            End If
        End If

    End Sub


    Private Sub SelectUserByName(userName As String)
        Dim userToSelect As User = Users.FirstOrDefault(Function(u) u.Name = userName)
        If userToSelect IsNot Nothing Then
            ListUseres.SelectedItem = userToSelect
            SelectUser(userToSelect.Name)
        End If
    End Sub

    Private _selectedListUser As User
    Public Property SelectedListUser As User
        Get
            Return _selectedListUser
        End Get
        Set(value As User)
            _selectedListUser = value
            ' Faites quelque chose avec l'utilisateur sélectionné (par exemple, récupérer le nom)
            If _selectedListUser IsNot Nothing Then
                Dim selectedUserName As String = _selectedListUser.Name
                ' Utilisez le nom de l'utilisateur sélectionné comme vous le souhaitez
            End If
        End Set
    End Property

End Class


