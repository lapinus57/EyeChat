Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Drawing
Imports System.Windows.Threading
Imports log4net
Imports System.Media
Imports System.Windows.Media.Animation

Public Class ReceiveManager

    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Private receivingClient As UdpClient
    Private Const MulticastAddress As String = "224.0.0.1"
    Private Const Port As Integer = 50545 ' Port défini comme constante
    Private ReadOnly dispatcher As Dispatcher ' Utilisé pour accéder au thread de l'interface utilisateur
    Private mainWindow As MainWindow

    ' Dictionnaire pour mapper les codes de message aux méthodes de traitement
    Private ReadOnly messageHandlers As Dictionary(Of String, Action(Of String))

    Public Sub New(dispatcher As Dispatcher, mainWin As MainWindow)
        Me.dispatcher = dispatcher
        Me.mainWindow = mainWin

        ' Initialisation du dictionnaire des gestionnaires de messages
        messageHandlers = New Dictionary(Of String, Action(Of String)) From {
            {"USR01", AddressOf HandleUserConnection},
            {"USR02", AddressOf HandleUserDisconnection},
            {"USR03", AddressOf HandleUserNameChange},
            {"USR04", AddressOf HandleUserColorChange},
            {"USR05", AddressOf HandleUserUpdate},
            {"USR11", AddressOf HandleUserAvatarUpdate},
            {"NFC01", AddressOf HandleNFCInsertion},
            {"NFC02", AddressOf HandleNFCRemoval},
            {"NFC11", AddressOf HandleNFCUserAddition},
            {"NFC12", AddressOf HandleNFCUserRemoval},
            {"SMF01", AddressOf HandleSpeedMessage},
            {"PTN01", AddressOf HandlePatientAdd},
            {"PTN02", AddressOf HandlePatientCheckPass},
            {"PTN03", AddressOf HandlePatientRemove},
            {"PTN04", AddressOf HandlePatientUpdate},
            {"PTN05", AddressOf HandlePatientUndoPass},
            {"SYS01", AddressOf HandleRemoteShutdown},
            {"SYS02", AddressOf HandleRemoteDisconnect},
            {"SYS20", AddressOf HandleFileReception},
            {"MSG01", AddressOf handlemessageAddition},
            {"MSG02", AddressOf HandleMessageRemoval},
            {"SLN01", AddressOf HandleRoomCreation},
            {"SLN02", AddressOf HandleRoomRemoval},
            {"SLN03", AddressOf HandleRoomUseraddition},
            {"SLN04", AddressOf HandleRoomUserRemoval},
            {"POP01", AddressOf HandlePopupnotification}
        }
    End Sub

    ' Démarre l'écoute des messages entrants
    Public Sub StartReceiving()
        Try
            ' Créer un nouveau thread pour écouter les messages UDP
            Dim receiveThread As New Thread(AddressOf ReceiveMessages) With {
            .IsBackground = True
             }

            receiveThread.Start()
        Catch ex As Exception
            logger.Error("Erreur lors du démarrage de la réception de messages : " & ex.Message)
            MsgBox("Erreur lors du démarrage de la réception de messages : " & ex.Message)
        End Try

    End Sub

    ' Boucle de réception de messages UDP
    Private Sub ReceiveMessages()
        Try
            ' Initialisation du client UDP
            receivingClient = New UdpClient(Port)
            'receivingClient = New UdpClient(New IPEndPoint(IPAddress.Parse("192.168.1.210"), Port)) ' Adresse IP locale

            'receivingClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, True)

            receivingClient.JoinMulticastGroup(IPAddress.Parse(MulticastAddress))
            Dim endPoint As New IPEndPoint(IPAddress.Any, Port)

            ' Boucle infinie pour écouter les messages entrants
            While True
                Try


                    Dim receiveBytes As Byte() = receivingClient.Receive(endPoint)
                    Dim receivedMessage As String = Encoding.UTF8.GetString(receiveBytes)

                    ' Exécute la méthode MessageReceived sur le thread de l'interface utilisateur
                    dispatcher?.Invoke(Sub() MessageReceived(receivedMessage))


                Catch ex As SocketException
                    ' Gérer l'exception de socket ici
                    MessageBox.Show("Erreur de socket : " & ex.Message)
                End Try
            End While

        Catch ex As Exception
            ' Gestion des autres exceptions générales
            MessageBox.Show("Erreur lors de la configuration du client UDP : " & ex.Message)
        Finally
            ' Libérer les ressources si nécessaire
            receivingClient?.Close()
        End Try
    End Sub

    Private Sub MessageReceived(ByVal receivedMessage As String)
        ' Validation du message et extraction du code
        If String.IsNullOrWhiteSpace(receivedMessage) OrElse receivedMessage.Length < 5 Then
            logger.Warn("Message reçu invalide.")
            Return
        End If

        Console.WriteLine("Message reçu : " & receivedMessage)
        Dim messageCode As String = receivedMessage.Substring(0, 5)
        Dim messageContent As String = receivedMessage.Substring(5)

        ' Utilisation du dictionnaire pour trouver le bon gestionnaire
        If messageHandlers.ContainsKey(messageCode) Then
            messageHandlers(messageCode).Invoke(messageContent)
        Else
            logger.Warn("Code de message non reconnu : " & messageCode)
        End If
    End Sub

    Private Sub HandleUserConnection(ByVal content As String)
        Try
            ' Découper le contenu en parties
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de connexion utilisateur invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim identifiantPC As String = parts(1)

            ' Rechercher l'utilisateur par nom
            Dim userToUpdate As User = MainWindow.Users.FirstOrDefault(Function(user) user.Name = userName)

            ' Si l'utilisateur n'existe pas, l'ajouter
            If userToUpdate Is Nothing Then
                mainWindow.Addusers(userName) ' Ajout de l'utilisateur s'il n'existe pas
                userToUpdate = MainWindow.Users.FirstOrDefault(Function(user) user.Name = userName)
                If userToUpdate Is Nothing Then
                    logger.Error("Impossible d'ajouter l'utilisateur.")
                    Return
                End If
            End If

            ' Mettre à jour l'état de l'utilisateur
            UpdateUserStatus(userToUpdate, identifiantPC, True) ' isConnection = True

            ' Sauvegarder les utilisateurs et rafraîchir l'interface
            SaveAndRefreshUsers()
            If userName <> MainWindow._userSettingsMain.UserName Then
                ' Envoyer un message USR05 avec les informations de l'utilisateur actuel
                SendManager.SendMessage("USR05" & MainWindow._userSettingsMain.UserName & "|" & Environment.UserName & "|/Avatar/" & MainWindow._userSettingsMain.UserAvatar)

            End If
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la connexion utilisateur : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleUserDisconnection(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de déconnexion utilisateur invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim identifiantPC As String = parts(1)

            Dim userToUpdate As User = MainWindow.Users.FirstOrDefault(Function(user) user.Name = userName)
            If userToUpdate IsNot Nothing Then
                UpdateUserStatus(userToUpdate, identifiantPC, False) ' isConnection = False
                SaveAndRefreshUsers()
            End If
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la déconnexion utilisateur : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleUserNameChange(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de changement de nom d'utilisateur invalide.")
                Return
            End If

            Dim oldUserName As String = parts(0)
            Dim newUserName As String = parts(1)

            Dim userToUpdate As User = MainWindow.Users.FirstOrDefault(Function(user) user.Name = oldUserName)
            If userToUpdate IsNot Nothing Then
                userToUpdate.Name = newUserName
                SaveAndRefreshUsers()
            End If
        Catch ex As Exception
            logger.Error("Erreur lors du traitement du changement de nom d'utilisateur : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleUserColorChange(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de changement de couleur d'utilisateur invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim newColorString As String = parts(1)
            Dim newColor As Color = ColorTranslator.FromHtml(newColorString)

            Dim userToUpdate As User = MainWindow.Users.FirstOrDefault(Function(user) user.Name = userName)
            If userToUpdate IsNot Nothing Then
                userToUpdate.ColorUser = newColor
                SaveAndRefreshUsers()
            End If
        Catch ex As Exception
            logger.Error("Erreur lors du traitement du changement de couleur d'utilisateur : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleUserAvatarUpdate(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de mise à jour d'avatar invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim avatarTag As String = parts(1)

            Dim userToUpdate As User = MainWindow.Users.FirstOrDefault(Function(user) user.Name = userName)
            If userToUpdate IsNot Nothing Then
                userToUpdate.Avatar = avatarTag
                SaveAndRefreshUsers()
                SendManager.SendMessage("ACK11" & userName & "|AvatarUpdated")
                logger.Debug("Confirmation de mise à jour envoyée à l'utilisateur " & userName)
            End If
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la mise à jour d'avatar : " & ex.Message)
        End Try
    End Sub

    ' Ajoutez cette méthode pour gérer le message USR05
    Private Sub HandleUserUpdate(ByVal content As String)
        Try
            ' Découper le contenu en parties
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 3 Then
                logger.Warn("Contenu de mise à jour utilisateur invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim identifiantPC As String = parts(1)
            Dim userAvatar As String = parts(2)

            ' Rechercher l'utilisateur par nom
            Dim userToUpdate As User = MainWindow.Users.FirstOrDefault(Function(user) user.Name = userName)

            ' Si l'utilisateur n'existe pas, l'ajouter
            If userToUpdate Is Nothing Then
                mainWindow.Addusers(userName) ' Ajout de l'utilisateur s'il n'existe pas
                userToUpdate = MainWindow.Users.FirstOrDefault(Function(user) user.Name = userName)
                If userToUpdate Is Nothing Then
                    logger.Error("Impossible d'ajouter l'utilisateur.")
                    Return
                End If
            End If

            ' Mettre à jour les informations de l'utilisateur
            userToUpdate.Status = identifiantPC
            userToUpdate.Avatar = userAvatar


            ' Sauvegarder les utilisateurs et rafraîchir l'interface
            SaveAndRefreshUsers()
        Catch ex As Exception
            logger.Error("Erreur lors de la mise à jour de l'utilisateur : " & ex.Message)
        End Try
    End Sub
    Private Sub HandleNFCInsertion(ByVal content As String)
        Try
            'Not implemented yet

            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu d'insertion NFC invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim uid As String = parts(1)

            ' Ajouter l'utilisateur à la carte NFC
            'NfcManager.AddUserToCard(userName, uid)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de l'insertion NFC : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleNFCRemoval(ByVal content As String)
        Try
            'Not implemented yet

            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de suppression NFC invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim uid As String = parts(1)

            ' Supprimer l'utilisateur de la carte NFC
            'NfcManager.RemoveUserFromCard(userName, uid)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la suppression NFC : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleNFCUserAddition(ByVal content As String)
        Try
            'Not implemented yet

            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu d'ajout d'utilisateur NFC invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim uid As String = parts(1)

            ' Ajouter la carte NFC à l'utilisateur
            'NfcManager.AddCardToUser(uid, userName)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de l'ajout d'utilisateur NFC : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleNFCUserRemoval(ByVal content As String)
        Try
            'Not implemented yet

            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de suppression d'utilisateur NFC invalide.")
                Return
            End If

            Dim userName As String = parts(0)
            Dim uid As String = parts(1)

            ' Supprimer la carte NFC de l'utilisateur
            'NfcManager.RemoveCardFromUser(uid, userName)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la suppression d'utilisateur NFC : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleSpeedMessage(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 6 Then
                logger.Warn("Contenu de message SpeedMessage invalide.")
                Return
            End If

            Dim userSend As String = parts(0)
            Dim destinataire As String = parts(1)
            Dim message As String = parts(2)
            Dim option1 As String = parts(3)
            Dim option2 As String = parts(4)
            Dim option3 As String = parts(5)

            If destinataire = MainWindow._userSettingsMain.UserName Then
                dispatcher.Invoke(Sub()
                                      mainWindow.WindowState = WindowState.Normal
                                      mainWindow.Topmost = True
                                      mainWindow.Focus()
                                  End Sub)
                mainWindow.SpeedMessageDialog(userSend, userSend & message, option1, option2, option3)
            End If
        Catch ex As Exception
            logger.Error("Erreur lors du traitement du message SpeedMessage : " & ex.Message)
        End Try
    End Sub

    Private Sub HandlePatientAdd(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 8 Then
                logger.Warn("Contenu d'ajout de patient invalide.")
                Return
            End If

            Dim title As String = parts(0)
            Dim lastName As String = parts(1)
            Dim firstName As String = parts(2)
            Dim exam As String = parts(3)
            Dim comments As String = parts(4)
            Dim floor As String = parts(5)
            Dim examinator As String = parts(6)
            Dim holdTime As String = parts(7)

            PatientManager.PatientAdd(title, lastName, firstName, exam, comments, floor, examinator, holdTime)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de l'ajout de patient : " & ex.Message)
        End Try
    End Sub

    Private Sub HandlePatientCheckPass(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 9 Then
                logger.Warn("Contenu de vérification de patient invalide.")
                Return
            End If

            Dim title As String = parts(0)
            Dim lastName As String = parts(1)
            Dim firstName As String = parts(2)
            Dim exam As String = parts(3)
            Dim comments As String = parts(4)
            Dim floor As String = parts(5)
            Dim examinator As String = parts(6)
            Dim holdTime As String = parts(7)
            Dim OperatorName As String = parts(8)

            PatientManager.PatientCheckPass(title, lastName, firstName, exam, comments, floor, examinator, holdTime, OperatorName)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la vérification de patient : " & ex.Message)
        End Try
    End Sub

    Private Sub HandlePatientRemove(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 8 Then
                logger.Warn("Contenu de suppression de patient invalide.")
                Return
            End If

            Dim title As String = parts(0)
            Dim lastName As String = parts(1)
            Dim firstName As String = parts(2)
            Dim exam As String = parts(3)
            Dim comments As String = parts(4)
            Dim floor As String = parts(5)
            Dim examinator As String = parts(6)
            Dim holdTime As String = parts(7)

            PatientManager.PatientRemove(title, lastName, firstName, exam, comments, floor, examinator, holdTime)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la suppression de patient : " & ex.Message)
        End Try
    End Sub

    Private Sub HandlePatientUpdate(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 9 Then
                logger.Warn("Contenu de mise à jour de patient invalide.")
                Return
            End If

            Dim title As String = parts(0)
            Dim lastName As String = parts(1)
            Dim firstName As String = parts(2)
            Dim exam As String = parts(3)
            Dim comments As String = parts(4)
            Dim floor As String = parts(5)
            Dim examinator As String = parts(6)
            Dim oldHoldTime As String = parts(7)
            Dim newHoldTime As String = parts(8)

            PatientManager.PatientUpdate(title, lastName, firstName, exam, comments, floor, examinator, oldHoldTime, newHoldTime)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la mise à jour de patient : " & ex.Message)
        End Try
    End Sub

    Private Sub HandlePatientUndoPass(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 9 Then
                logger.Warn("Contenu d'annulation de validation de patient invalide.")
                Return
            End If

            Dim title As String = parts(0)
            Dim lastName As String = parts(1)
            Dim firstName As String = parts(2)
            Dim exam As String = parts(3)
            Dim comments As String = parts(4)
            Dim floor As String = parts(5)
            Dim examinator As String = parts(6)
            Dim holdTime As String = parts(7)
            Dim OperatorName As String = parts(8)

            PatientManager.PatientUndoPass(title, lastName, firstName, exam, comments, floor, examinator, holdTime, OperatorName)
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de l'annulation de validation de patient : " & ex.Message)
        End Try

    End Sub

    Private Sub HandleRemoteShutdown(ByVal content As String)

        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de fermeture à distance invalide.")
                Return
            End If

            Dim targetPC As String = parts(0)
            Dim authorPC As String = parts(1)

            If targetPC = My.Settings.UniqueId Then
                logger.Debug("Réception d'un message SYS01 pour fermeture à distance.")
                Message.SaveMessagesToJson(MainWindow.Messages)
                User.SaveUsersToJson(MainWindow.Users)
                dispatcher.Invoke(Sub()
                                      mainWindow.Close()
                                  End Sub)
            End If
        Catch ex As Exception
            logger.Error("Erreur lors du traitement de la fermeture à distance : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleRemoteDisconnect(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 2 Then
                logger.Warn("Contenu de déconnexion à distance invalide.")
                Return
            End If

            Dim targetPC As String = parts(0)
            Dim authorPC As String = parts(1)

            If targetPC = My.Settings.UniqueId Then
                logger.Debug("Réception d'un message SYS02 pour déconnexion à distance.")
                Message.SaveMessagesToJson(MainWindow.Messages)
                User.SaveUsersToJson(MainWindow.Users)
                MainWindow.SelectedUserMessages.Clear()
                MainWindow.Users.Clear()
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la déconnexion à distance : " & ex.Message)
        End Try
    End Sub

    Private Sub HandleFileReception(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 4 Then
                logger.Warn("Contenu de réception de fichier invalide.")
                Return
            End If

            Dim targetPC As String = parts(0)
            Dim authorPC As String = parts(1)
            Dim folder As String = parts(2)
            Dim fileName As String = parts(3)
            Dim port As Integer = 12345 ' Remplacez par le même port que celui utilisé pour l'envoi

            If targetPC = My.Settings.UniqueId And authorPC <> My.Settings.UniqueId Then
                ' Mettre le client en réception de fichier
                Dim receiver As New FileReceiver()
                Dim savePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, fileName)
                ' Remplacez par l'emplacement où vous souhaitez sauvegarder le fichier
                receiver.ReceiveFile(savePath, port)
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la réception de fichier :  " & ex.Message)
        End Try
    End Sub

    Private Sub handlemessageAddition(ByVal content As String)
        Try
            Dim parts As String() = content.Split("|"c)
            If parts.Length < 5 Then
                logger.Warn("Contenu d'ajout de message invalide.")
                Return
            End If

            Dim author As String = parts(0)
            Dim destinataire As String = parts(1)
            Dim room As String = parts(2)
            Dim message As String = parts(3)
            Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", parts(4))

            Dim messageAdded As Boolean = False ' Variable pour suivre si un message a été ajouté

            Select Case destinataire
                Case "A Tous"

                    If author = MainWindow._userSettingsMain.UserName Then
                        ' Auteur envoie un message "A Tous"
                        mainWindow.AddMessage("A Tous", author, room, message, True, avatarPath)
                        mainWindow.SelectUserByName("A Tous")
                        messageAdded = True
                    Else
                        ' Autre utilisateur envoie un message "A Tous"
                        mainWindow.AddMessage("A Tous", author, room, message, False, avatarPath)
                        mainWindow.SelectUserByName("A Tous")
                        messageAdded = True
                    End If

                Case "Secrétariat"
                    If author = MainWindow._userSettingsMain.UserName Then
                        ' Auteur envoie un message à "Secrétaria"
                        mainWindow.AddMessage("Secrétariat", author, room, message, True, avatarPath)
                        mainWindow.SelectUserByName("Secrétariat")
                        messageAdded = True
                    End If

                    If MainWindow._userSettingsMain.SecretaryMode = True Then
                        ' Secrétaria mode est activé
                        mainWindow.AddMessage("Secrétariat", author, room, message, False, avatarPath)
                        mainWindow.AddMessage(author, author, room, message, False, avatarPath)
                        mainWindow.SelectUserByName(author)
                        messageAdded = True
                    End If

                Case Else
                    If author = MainWindow._userSettingsMain.UserName Then
                        ' Auteur envoie un message à "quelqu'un"
                        mainWindow.AddMessage(destinataire, author, room, message, True, avatarPath)
                        mainWindow.SelectUserByName(destinataire)
                        messageAdded = True
                    End If

                    If destinataire = MainWindow._userSettingsMain.UserName Then
                        ' Destinataire est l'auteur
                        mainWindow.AddMessage(author, author, room, message, False, avatarPath)
                        mainWindow.SelectUserByName(author)
                        messageAdded = True
                    End If
            End Select

            ' Mettre à jour la fenêtre si un message a été ajouté
            If messageAdded Then
                dispatcher.Invoke(Sub()
                                      mainWindow.WindowState = WindowState.Normal
                                      mainWindow.Topmost = True
                                      mainWindow.Focus()
                                  End Sub)
            End If




        Catch ex As Exception
            logger.Error("Erreur de réception d'un message MSG01 " & ex.Message)
        End Try
        ' Destinataire est l'auteur
    End Sub

    Private Sub HandleMessageRemoval(ByVal content As String)
        'Not implemented yet
    End Sub

    Private Sub HandleRoomCreation(ByVal content As String)
        'Not implemented yet
    End Sub
    Private Sub HandleRoomRemoval(ByVal content As String)
        'Not implemented yet
    End Sub
    Private Sub HandleRoomInvitation(ByVal content As String)
        'Not implemented yet
    End Sub

    Private Sub HandleRoomUseraddition(ByVal content As String)
        'Not implemented yet
    End Sub

    Private Sub HandleRoomUserRemoval(ByVal content As String)
        'Not implemented yet
    End Sub

    ' Méthode pour gérer les notifications POP01
    Private Sub HandlePopupNotification(message As String)
        ' Vérifier que le message est suffisamment long

        ' Extraire le nom de l'utilisateur du message
        Dim userName As String = message

        ' Vérifier si le nom d'utilisateur correspond à celui de l'utilisateur actuel
        If userName = MainWindow._userSettingsMain.UserName Then
                ' Faire apparaître la MainWindow
                Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

            ' Restaurer la fenêtre si elle est agrandie ou minimisée
            If mainWindow.WindowState = WindowState.Maximized OrElse mainWindow.WindowState = WindowState.Minimized Then
                mainWindow.WindowState = WindowState.Normal
            End If

            mainWindow.Topmost = True
            ' Cette ligne est nécessaire pour s'assurer que la fenêtre est restaurée correctement
            mainWindow.WindowState = WindowState.Normal
            mainWindow.Focus()

            ' Émettre un bip sonore
            SystemSounds.Beep.Play()

            ' Déclencher l'animation de secousse sur la MainWindow
            'TriggerShakeAnimation(mainWindow)
            ' Déclencher l'animation de déformation sur la MainWindow
            'TriggerDistortAnimation(mainWindow)
            ' Déclencher l'animation de déformation permanente sur la MainWindow
            'TriggerPermanentDistortAnimation(mainWindow)
            ' Déclencher l'effet miroir sur la MainWindow
            TriggerMirrorEffect(mainWindow)
        End If

    End Sub


    ' Méthode pour envoyer l'ID du PC local
    Private Sub SendLocalPCInfo()
        Try
            Dim localIPAddress As IPAddress = FilesTransferManager.GetLocalIPAddress()
            SendManager.SendMessageWithCode("DBG02", My.Settings.UniqueId & "|" & Environment.UserName & "|" & localIPAddress.ToString)
            logger.Debug("Envoi de l'ID du PC aux autres applications")
        Catch ex As Exception
            logger.Error("Erreur lors de l'envoi de l'ID du PC aux autres applications : " & ex.Message)
        End Try
    End Sub

    ' Méthode pour mettre à jour le statut de l'utilisateur
    Private Sub UpdateUserStatus(user As User, identifiantPC As String, isConnection As Boolean)
        Try
            If isConnection Then
                ' Gestion de la connexion
                If user.Status = "Offline" Then
                    user.Status = identifiantPC
                    logger.Info("Statut utilisateur mis à jour pour un seul identifiant")
                ElseIf Not user.Status.Contains(identifiantPC) Then
                    user.Status &= " | " & identifiantPC
                    logger.Info("Statut utilisateur mis à jour pour plusieurs identifiants")
                Else
                    logger.Debug("Aucune mise à jour requise, utilisateur déjà à jour")
                End If
            Else
                ' Gestion de la déconnexion
                If user.Status = identifiantPC Then
                    user.Status = "Offline"
                ElseIf user.Status.Contains("|") Then
                    Dim statusParts As String() = user.Status.Split("|"c)
                    user.Status = String.Join(" | ", statusParts.Where(Function(part) part <> identifiantPC))
                End If
                logger.Info($"Déconnexion de l'utilisateur {user.Name}, statut mis à jour")
            End If

            ' Envoyer les informations locales à jour si la connexion est établie
            If isConnection Then
                SendLocalPCInfo()
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la mise à jour du statut utilisateur : " & ex.Message)
        End Try
    End Sub

    ' Méthode pour sauvegarder et recharger les utilisateurs
    Private Sub SaveAndRefreshUsers()
        ' Stocker l'utilisateur sélectionné
        Dim selectedUser As String = mainWindow.GetSelectedUserName()
        User.SaveUsersToJson(MainWindow.Users)
        MainWindow.Users = User.LoadUsersFromJson()
        dispatcher.Invoke(Sub() mainWindow.ListUseres.ItemsSource = MainWindow.Users)
        mainWindow.SelectUserByName(selectedUser)
    End Sub

    ' Méthode pour déclencher l'animation de secousse
    Private Sub TriggerShakeAnimation(window As Window)
        Dim originalLeft As Double = window.Left
        Dim originalTop As Double = window.Top

        Dim shakeAnimation As New Storyboard()
        Dim animationX As New DoubleAnimation() With {
            .From = originalLeft - 20,
            .To = originalLeft + 20,
            .Duration = New Duration(TimeSpan.FromMilliseconds(100)),
            .AutoReverse = True,
            .RepeatBehavior = New RepeatBehavior(5)
        }
        Dim animationY As New DoubleAnimation() With {
            .From = originalTop - 20,
            .To = originalTop + 20,
            .Duration = New Duration(TimeSpan.FromMilliseconds(100)),
            .AutoReverse = True,
            .RepeatBehavior = New RepeatBehavior(5)
        }

        Storyboard.SetTarget(animationX, window)
        Storyboard.SetTargetProperty(animationX, New PropertyPath(Window.LeftProperty))
        Storyboard.SetTarget(animationY, window)
        Storyboard.SetTargetProperty(animationY, New PropertyPath(Window.TopProperty))

        shakeAnimation.Children.Add(animationX)
        shakeAnimation.Children.Add(animationY)

        shakeAnimation.Begin()
    End Sub

    ' Méthode pour déclencher l'animation de déformation
    Private Sub TriggerDistortAnimation(window As Window)
        Dim scaleTransform As New ScaleTransform(1.0, 1.0)
        window.RenderTransform = scaleTransform
        window.RenderTransformOrigin = New System.Windows.Point(0.5, 0.5)

        Dim scaleXAnimation As New DoubleAnimation() With {
            .From = 1.0,
            .To = 1.2,
            .Duration = New Duration(TimeSpan.FromMilliseconds(100)),
            .AutoReverse = True,
            .RepeatBehavior = New RepeatBehavior(3)
        }
        Dim scaleYAnimation As New DoubleAnimation() With {
            .From = 1.0,
            .To = 1.2,
            .Duration = New Duration(TimeSpan.FromMilliseconds(100)),
            .AutoReverse = True,
            .RepeatBehavior = New RepeatBehavior(3)
        }

        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation)
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation)
    End Sub


    ' Méthode pour déclencher l'animation de déformation permanente
    Private Sub TriggerPermanentDistortAnimation(window As Window)
        Dim skewTransform As New SkewTransform(0, 0)
        window.RenderTransform = skewTransform
        window.RenderTransformOrigin = New System.Windows.Point(0.5, 0.5)

        Dim skewXAnimation As New DoubleAnimation() With {
            .From = 0,
            .To = 20,
            .Duration = New Duration(TimeSpan.FromMilliseconds(500)),
            .AutoReverse = False,
            .RepeatBehavior = New RepeatBehavior(1)
        }
        Dim skewYAnimation As New DoubleAnimation() With {
            .From = 0,
            .To = 20,
            .Duration = New Duration(TimeSpan.FromMilliseconds(500)),
            .AutoReverse = False,
            .RepeatBehavior = New RepeatBehavior(1)
        }

        skewTransform.BeginAnimation(SkewTransform.AngleXProperty, skewXAnimation)
        skewTransform.BeginAnimation(SkewTransform.AngleYProperty, skewYAnimation)
    End Sub

    ' Méthode pour déclencher l'effet miroir
    Private Sub TriggerMirrorEffect(window As Window)
        ' Vérifier si la fenêtre est prête pour la transformation
        If window Is Nothing Then
            Return
        End If

        ' Obtenir l'élément racine de la fenêtre
        Dim rootElement As FrameworkElement = TryCast(window.Content, FrameworkElement)
        If rootElement Is Nothing Then
            Return
        End If

        ' Appliquer la transformation de mise à l'échelle pour l'effet miroir
        Dim scaleTransform As New ScaleTransform(-1, 1)
        rootElement.RenderTransform = scaleTransform
        rootElement.RenderTransformOrigin = New System.Windows.Point(0.5, 0.5)
    End Sub
End Class
