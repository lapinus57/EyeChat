
Imports System.Net.Http
Imports System.Reflection
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports MahApps.Metro.Controls
Imports MahApps.Metro.Controls.Dialogs

Public Class GithubWindows
    Inherits UserControl

    'Déclaration des variables d'information A propos
    Public Property Title As String
    Public Property AssemblyVersion As String
    Public Property FileVersion As String
    Public Property Description As String
    Public Property Company As String
    Public Property Copyright As String
    Public Property Trademark As String

    Public Sub GetAssemblyInfos()
        Dim assembly As Assembly = Assembly.GetExecutingAssembly()
        Dim assemblyName As AssemblyName = assembly.GetName()

        ' Récupérer les informations de version
        Dim assemblyVersionAttribut As Version = assembly.GetName().Version
        AssemblyVersion = assemblyVersionAttribut.ToString()

        ' Obtenir la version du fichier
        FileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion

        ' Récupérer le titre de l'application
        Title = assemblyName.Name

        Dim CopyrightAttribute As AssemblyCopyrightAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyCopyrightAttribute)), AssemblyCopyrightAttribute)
        Copyright = CopyrightAttribute.Copyright

        ' Récupérer la description de l'application
        Dim descriptionAttribute As AssemblyDescriptionAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
        Description = descriptionAttribute.Description

        ' Récupérer les informations du développeur
        Dim companyAttribute As AssemblyCompanyAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
        Company = companyAttribute.Company

        Trademark = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyTrademarkAttribute)), AssemblyTrademarkAttribute).Trademark
    End Sub

    Private Sub GithubWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        GetAssemblyInfos()
        DataContext = Me
    End Sub

    Public Async Function CreateGitHubIssueAsync(ByVal title As String, ByVal body As String) As Task
        ' Déclaration variable pour le dépôt GitHub
        Dim apiUrl As String = "https://api.github.com/repos/{owner}/{repo}/issues"
        Dim owner As String = "lapinus57"
        Dim repo As String = "EyeChat"
        Dim personalAccessToken As String = "ghp"

        Dim url As String = apiUrl.Replace("{owner}", owner).Replace("{repo}", repo)

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "VotreApplication")
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {personalAccessToken}")

            Dim requestBody As JObject = New JObject(
            New JProperty("title", title),
            New JProperty("body", body)
        )

            Dim requestBodyString As String = requestBody.ToString()

            Dim content As HttpContent = New StringContent(requestBodyString, Encoding.UTF8, "application/json")

            Dim response As HttpResponseMessage = Await client.PostAsync(url, content)

            If response.IsSuccessStatusCode Then
                Console.WriteLine("Issue created successfully!")
            Else
                Console.WriteLine("Failed to create issue. Status Code: " & response.StatusCode.ToString())
            End If
        End Using
    End Function

    Private Async Sub SendReport_Click(sender As Object, e As RoutedEventArgs)
        ' Récupérer le texte du DescriptionTextBox
        Dim descriptionText As String = DescriptionTextBox.Text.Trim()

        ' Récupérer la catégorie sélectionnée dans la categoryComboBox
        Dim selectedCategoryItem As ComboBoxItem = DirectCast(categoryComboBox.SelectedItem, ComboBoxItem)
        Dim selectedCategory As String = If(selectedCategoryItem IsNot Nothing, selectedCategoryItem.Tag.ToString(), "")

        ' Vérifier si le texte de la description n'est pas vide
        If Not String.IsNullOrWhiteSpace(descriptionText) Then
            ' Utiliser la catégorie sélectionnée pour prendre des mesures appropriées
            Select Case selectedCategory
                Case "problem"
                    ' Traiter le problème signalé
                    Await CreateGitHubIssueAsync(My.Settings.UserName & " a rencotré un problème", descriptionText)
                Case "idea"
                    ' Traiter l'idée proposée
                    Await CreateGitHubIssueAsync(My.Settings.UserName & " a une idée", descriptionText)
                Case Else
                    ' Catégorie non reconnue
                    ' Gérer le cas où la catégorie n'est pas reconnue
            End Select
        Else
            ' Le DescriptionTextBox est vide ou ne contient que des espaces
            ' Afficher un message d'erreur ou prendre une autre action appropriée
        End If

    End Sub

End Class
