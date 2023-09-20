Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Runtime.Remoting.Messaging
Imports Newtonsoft.Json

Public Class Message
    Inherits DependencyObject



    Public Shared ReadOnly messagesFilePath As String = Path.Combine("HistoricMsg", DateTime.Now.ToString("ddMMyyyy") & ".json")

    <JsonProperty("Name")>
    Public Property Name As String

    <JsonProperty("Sender")>
    Public Property Sender As String

    <JsonProperty("Room")>
    Public Property Room As String

    <JsonProperty("Content")>
    Public Property Content As String

    <JsonProperty("Timestamp")>
    Public Property Timestamp As DateTime

    <JsonProperty("Avatar")>
    Public Property Avatar As String

    <JsonProperty("IsAlignedRight")>
    Public Property IsAlignedRight As Boolean
        Get
            Return DirectCast(GetValue(IsAlignedRightProperty), Boolean)
        End Get
        Set(ByVal value As Boolean)
            SetValue(IsAlignedRightProperty, value)
        End Set
    End Property


    Public Shared ReadOnly IsAlignedRightProperty As DependencyProperty = DependencyProperty.Register("IsAlignedRight", GetType(Boolean), GetType(Message), New PropertyMetadata(False))



    Public Sub New()
        ' Par défaut, aligner le message à gauche
        IsAlignedRight = False

        ' Définir le timestamp du message au moment de sa création
        Timestamp = DateTime.Now
    End Sub

    ' Chargement des messages à partir du fichier JSON
    Public Shared Function LoadMessagesFromJson() As ObservableCollection(Of Message)
        Dim messages As ObservableCollection(Of Message) = Nothing

        If File.Exists(messagesFilePath) Then
            Using streamReader As New StreamReader(messagesFilePath)
                Using jsonReader As New JsonTextReader(streamReader)
                    Dim serializer As New JsonSerializer()
                    messages = serializer.Deserialize(Of ObservableCollection(Of Message))(jsonReader)

                    For Each message In messages
                        If (message.Sender = My.Settings.UserName) Then
                            message.IsAlignedRight = True
                        Else
                            message.IsAlignedRight = False
                        End If
                    Next
                End Using
            End Using
        End If

        Return messages
    End Function

    ' Enregistrement des messages dans le fichier JSON
    Public Shared Sub SaveMessagesToJson(ByVal messages As ObservableCollection(Of Message))

        Dim dossier As String = "HistoricMsg"
        ' Vérifier si le dossier existe
        If Not Directory.Exists(dossier) Then
            ' Créer le dossier s'il n'existe pas
            Directory.CreateDirectory(dossier)
        End If

        Dim serializedMessages As String = "[" & String.Join("," & Environment.NewLine, messages.Select(Function(m) JsonConvert.SerializeObject(New With {m.Name, m.Sender, m.Room, m.Content, m.Avatar, m.IsAlignedRight, m.Timestamp}))) & "]"
        File.WriteAllText(messagesFilePath, serializedMessages)

    End Sub



End Class