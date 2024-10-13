Imports System.IO
Imports Newtonsoft.Json

Public Class Salle

    <JsonProperty("Name")>
    Public Property Nom As String ' Nom de la salle
    <JsonProperty("description")>
    Public Property Description As String ' description de la salle

End Class

' Classe qui contient la liste de toutes les salles dans le fichier JSON
Public Class SallesData
    Public Property Salles As List(Of Salle) ' Liste de toutes les salles
    Public Class SallesData
        Public Property Salles As List(Of Salle) ' Liste de toutes les salles

        ' Méthode pour sauvegarder les salles dans un fichier JSON
        Public Sub Sauvegarder(cheminFichier As String)
            ' Sérialiser les données en JSON
            Dim jsonString As String = JsonConvert.SerializeObject(Me, Formatting.Indented)

            ' Sauvegarder dans le fichier spécifié
            File.WriteAllText(cheminFichier, jsonString)

            ' Optionnel : afficher un message pour confirmer la sauvegarde
            MessageBox.Show("Les salles ont été sauvegardées avec succès dans le fichier " & cheminFichier)
        End Sub

        ' Méthode pour lire les salles à partir d'un fichier JSON
        Public Shared Function Charger(cheminFichier As String) As SallesData
            ' Lire le contenu du fichier JSON
            Dim jsonString As String = File.ReadAllText(cheminFichier)

            ' Désérialiser le JSON en objets
            Return JsonConvert.DeserializeObject(Of SallesData)(jsonString)
        End Function
    End Class
End Class

' Méthode pour sauvegarder les salles dans un fichier JSON
