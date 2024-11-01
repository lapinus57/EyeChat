Imports System.Net.Sockets
Imports System.Text
Imports log4net

Public Class SendManager
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Private Shared sendingClient As UdpClient
    Private Const BroadcastAddress As String = "255.255.255.255"
    Private Const MulticastAddress As String = "224.0.0.1"
    Private Const Port As Integer = 50545 ' Utilise la constante de port partagée

    ' Initialisation du client d'envoi
    Public Shared Sub InitializeSender()
        Try
            'sendingClient = New UdpClient(BroadcastAddress, Port) With {
            '.EnableBroadcast = True
            '}
            sendingClient = New UdpClient With {
    .EnableBroadcast = False
}
            sendingClient.Connect(MulticastAddress, Port)

            logger.Debug("Client d'envoi UDP initialisé avec succès")
        Catch ex As Exception
            logger.Error("Erreur lors de l'initialisation du client d'envoi UDP : " & ex.Message)
            MsgBox("Erreur lors de l'initialisation du client d'envoi UDP : " & ex.Message)
        End Try
    End Sub

    ' Fonction permettant d'envoyer un message complet 
    Public Shared Sub SendMessage(message As String)
        logger.Debug("Envoi d'un message ")
        Try
            ' Vérifier si le message n'est pas vide
            If Not String.IsNullOrWhiteSpace(message) Then
                ' Vérifie si le client d'envoi est initialiser
                If sendingClient IsNot Nothing Then

                    ' Convertir le message en tableau d'octets en utilisant l'encodage UTF-8
                    Dim DataMessage As Byte() = Encoding.UTF8.GetBytes(message)

                    ' Envoyer les octets du message à travers le client d'envoi
                    If DataMessage IsNot Nothing AndAlso sendingClient IsNot Nothing Then

                        ' Envoyer les octets du message à travers le client d'envoi
                        sendingClient.Send(DataMessage, DataMessage.Length)

                        ' Afficher le message dans la console
                        logger.Debug("Envoi d'un message : " & message)
                    End If
                End If
            End If
        Catch ex As Exception
            logger.Error($"Erreur lors de l'envoi d'un message : {message}. L'erreur suivante est apparue : {ex.Message}")
        End Try
    End Sub

    ' Fonction permettant d'envoyer un code suivi d'un contenu spécifié
    Public Shared Sub SendMessageWithCode(code As String, content As String)
        logger.Debug("Envoi d'un message avec code ")
        Try
            ' Vérifier si le code et le contenu ne sont pas nuls
            If code IsNot Nothing And content IsNot Nothing Then
                ' Concaténer le code et le contenu pour former le message complet
                Dim message As String = code + content

                ' Convertir le message en tableau d'octets en utilisant l'encodage UTF-8
                Dim messageBytes As Byte() = Encoding.UTF8.GetBytes(message)

                ' Envoyer les octets du message à travers le client d'envoi
                sendingClient.Send(messageBytes, messageBytes.Length)

                ' Afficher le message dans la console
                logger.Debug("Envoi d'un message avec code : " & code & " et message : " & content)
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de l'envoi d'un message avec code : " & code & " et message : " & content & " l'erreur suivante est apparue : " & ex.Message)
        End Try

    End Sub
End Class
