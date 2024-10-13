Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports log4net
Imports Microsoft.VisualBasic.Devices

Public Class FilesTransferManager
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Shared Sub SendFileOverNetwork(folder As String, fileToSend As String, Optional user As String = "")
        logger.Info("Envoi du fichier sur le réseau")
        Try
            ' Vérifie si un utilisateur a été spécifié
            If String.IsNullOrEmpty(user) Then
                ' Aucun utilisateur spécifié, envoyez le message à tous les ordinateurs répertoriés
                For Each computer In MainWindow.Computers
                    ' Assurez-vous de ne pas vous envoyer un message à vous-même
                    If computer.ComputerID <> My.Settings.UniqueId Then
                        Dim message As String = $"SYS20{computer.ComputerID}|{My.Settings.UniqueId}|{folder}|{fileToSend}"
                        ' Envoyer le message
                        SendManager.SendMessage(message)
                        ' Envoyer le fichier à l'ordinateur actuel
                        Dim senderFile As New FileSender()
                        Dim filePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", fileToSend)
                        Dim ipAddress As String = computer.ComputerIp ' Remplacez par l'adresse IP du destinataire
                        Dim port As Integer = 12345 ' Remplacez par le port que vous souhaitez utiliser
                        senderFile.SendFile(filePath, ipAddress, port)
                        logger.Info($"Fichier envoyé à l'ordinateur {computer.ComputerID}")
                    End If
                Next
            Else
                ' Un utilisateur a été spécifié, envoyez le message uniquement à cet utilisateur
                Dim targetComputer As Computer = MainWindow.Computers.FirstOrDefault(Function(comp) comp.ComputerUser = user)
                If targetComputer IsNot Nothing Then
                    Dim message As String = $"SYS20{targetComputer.ComputerID}|{My.Settings.UniqueId}|{folder}|{fileToSend}"
                    ' Envoyer le message à l'utilisateur spécifié
                    SendManager.SendMessage(message)
                    ' Envoyer le fichier à l'utilisateur spécifié
                    Dim senderFile As New FileSender()
                    Dim filePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", fileToSend)
                    Dim ipAddress As String = targetComputer.ComputerIp ' Remplacez par l'adresse IP du destinataire
                    Dim port As Integer = 12345 ' Remplacez par le port que vous souhaitez utiliser
                    senderFile.SendFile(filePath, ipAddress, port)
                    logger.Info($"Fichier envoyé à l'ordinateur {targetComputer.ComputerID}")
                Else
                    ' Gérer le cas où l'utilisateur spécifié n'existe pas
                    logger.Error("L'utilisateur spécifié n'existe pas.")
                End If
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de l'envoi du fichier : " & ex.Message)
        End Try

    End Sub

    Public Shared Function GetLocalIPAddress() As IPAddress
        logger.Info("Obtention de l'adresse IP locale")
        Try
            ' Récupère toutes les interfaces réseau disponibles sur la machine de l'utilisateur
            Dim networkInterfaces As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()

            ' Itère à travers chaque interface réseau
            For Each networkInterface As NetworkInterface In networkInterfaces
                ' Vérifie si l'interface réseau est opérationnelle (en état de fonctionnement)
                If networkInterface.OperationalStatus = OperationalStatus.Up Then
                    ' Obtient les propriétés IP de l'interface réseau
                    Dim ipProperties As IPInterfaceProperties = networkInterface.GetIPProperties()

                    ' Récupère la collection des adresses IP unicast associées à cette interface réseau
                    Dim unicastIPAddresses As UnicastIPAddressInformationCollection = ipProperties.UnicastAddresses

                    ' Itère à travers chaque adresse IP unicast
                    For Each ipAddressInfo As UnicastIPAddressInformation In unicastIPAddresses
                        ' Vérifie si l'adresse IP est de type IPv4 (InterNetwork)
                        If ipAddressInfo.Address.AddressFamily = AddressFamily.InterNetwork Then
                            ' Enregistre un message de débogage indiquant l'adresse IP trouvée
                            logger.Debug("L'adresse IP de l'utilisateur est : " & ipAddressInfo.Address.ToString)

                            ' Renvoie l'adresse IP trouvée
                            Return ipAddressInfo.Address
                        End If
                    Next
                End If
            Next

            ' Si aucune adresse IPv4 n'est trouvée dans aucune interface réseau, renvoie Nothing
            Return Nothing
        Catch ex As Exception
            ' En cas d'erreur lors de l'exécution du code, capture l'exception
            logger.Error("Erreur sur GetLocalIPAddress : " & ex.Message)

            ' Renvoie également Nothing en cas d'exception
            Return Nothing
        End Try
    End Function
End Class
