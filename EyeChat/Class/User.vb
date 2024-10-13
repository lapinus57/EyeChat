Imports System.Collections.ObjectModel
Imports System.IO
Imports Newtonsoft.Json

Public Class User
    Inherits DependencyObject

    Public Shared ReadOnly UsersFilePath As String = Path.Combine("Users", "Users.json")

    <JsonProperty("Name")>
    Public Property Name As String

    <JsonProperty("Status")>
    Public Property Status As String

    <JsonProperty("Avatar")>
    Public Property Avatar As String

    <JsonProperty("ColorUser")>
    Public Property ColorUser As System.Drawing.Color

    <JsonProperty("AuxiliaireTilte")>
    Public Property AuxiliaireTilte As String

    <JsonProperty("initials")>
    Public Property Initials As String

    <JsonProperty("UUID")>
    Public Property UUID As String



    Public Sub New()
        ' Par défaut, définir un avatar vide
        'Avatar = String.Empty
        Avatar = "/Avatar/avataaars.png"
        ' Par défaut, définir une room vide
        AuxiliaireTilte = String.Empty
        ColorUser = System.Drawing.Color.White
        ' Par défaut, définir un status offline
        Status = "Offline"
        Initials = String.Empty
        UUID = String.Empty
    End Sub

    Public Shared Function LoadUsersFromJson(Optional userNameToExclude As String = Nothing) As ObservableCollection(Of User)
        Dim Users As ObservableCollection(Of User) = Nothing

        If File.Exists(UsersFilePath) Then
            Using streamReader As New StreamReader(UsersFilePath)
                Using jsonReader As New JsonTextReader(streamReader)
                    Dim serializer As New JsonSerializer()
                    Users = serializer.Deserialize(Of ObservableCollection(Of User))(jsonReader)
                End Using
            End Using

            ' Si un nom d'utilisateur est spécifié pour exclusion
            If Not String.IsNullOrEmpty(userNameToExclude) Then
                ' Vérifier si l'utilisateur à exclure est présent dans la liste
                Dim userToRemove As User = Users.FirstOrDefault(Function(user) user.Name = userNameToExclude)

                If userToRemove IsNot Nothing Then
                    Users.Remove(userToRemove) ' Supprimer l'utilisateur de la liste
                End If
            End If
        Else

        End If

        Return Users
    End Function


    ' Enregistrement des messages dans le fichier JSON
    Public Shared Sub SaveUsersToJson(ByVal Users As ObservableCollection(Of User))

        Dim dossier As String = "Users"
        ' Vérifier si le dossier existe
        If Not Directory.Exists(dossier) Then
            ' Créer le dossier s'il n'existe pas
            Directory.CreateDirectory(dossier)
        End If

        Dim serializedUsers As String = "[" & String.Join("," & Environment.NewLine, Users.Select(Function(m) JsonConvert.SerializeObject(New With {m.Name, m.AuxiliaireTilte, m.Status, m.Avatar, m.Initials, m.UUID}))) & "]"
        File.WriteAllText(UsersFilePath, serializedUsers)

    End Sub
End Class
