
Imports System.Net.Http
Imports System.Text
Imports MahApps.Metro.Controls.Dialogs
Imports Newtonsoft.Json.Linq

Public Class GithubWindows


    'Déclaration des variables d'information A propos
    Public Property Title As String
    Public Property AssemblyVersion As String
    Public Property FileVersion As String
    Public Property Description As String
    Public Property Company As String
    Public Property Copyright As String
    Public Property Trademark As String

    Private ReadOnly httpClient As HttpClient
    Private Property DialogCoordinator As IDialogCoordinator


    Public Sub New()
        InitializeComponent()
        DialogCoordinator = New DialogCoordinator() ' Instancier DialogCoordinator
        Me.DataContext = New GitHubViewModel(DialogCoordinator)
        DataContext.GetAssemblyInfos()
        DataContext = DataContext
    End Sub


    Private Sub GithubWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        'DataContext = Me



    End Sub



    Public Async Function CreateGitHubIssueAsync(ByVal title As String, ByVal body As String) As Task
        ' Déclaration variable pour le dépôt GitHub
        Dim apiUrl As String = "https://api.github.com/repos/{owner}/{repo}/issues"
        Dim owner As String = "lapinus57"
        Dim repo As String = "EyeChat"
        Dim personalAccessToken As String = "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"

        Dim url As String = apiUrl.Replace("{owner}", owner).Replace("{repo}", repo)

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "VotreApplication")
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {personalAccessToken}")

            Dim requestBody As New JObject(
            New JProperty("title", title),
            New JProperty("body", body)
        )

            Dim requestBodyString As String = requestBody.ToString()

            Dim content As HttpContent = New StringContent(requestBodyString, Encoding.UTF8, "application/json")

            Dim response As HttpResponseMessage = Await client.PostAsync(url, content)

            Dim viewModel As GitHubViewModel = TryCast(Me.DataContext, GitHubViewModel)

            If viewModel IsNot Nothing Then

                If response.IsSuccessStatusCode Then
                    Console.WriteLine("Issue created successfully!")
                    Await viewModel.DisplayMessageAsync("EyeChat - Github", "Issue created successfully!")
                Else
                    Console.WriteLine("Failed to create issue. Status Code: " & response.StatusCode.ToString())
                    Await viewModel.DisplayMessageAsync("EyeChat - Github", "Failed to create issue. Status Code: " & response.StatusCode.ToString())
                End If

            End If
        End Using
    End Function

    Private Async Sub SendReport_Click(sender As Object, e As RoutedEventArgs)

        ' Récupérer le texte du DescriptionTextBox
        Dim descriptionText As String = DescriptionTextBox.Text.Trim()

        ' Récupérer la catégorie sélectionnée dans la categoryComboBox
        Dim selectedCategoryItem As ComboBoxItem = DirectCast(categoryComboBox.SelectedItem, ComboBoxItem)
        Dim selectedCategory As String = If(selectedCategoryItem IsNot Nothing, selectedCategoryItem.Tag.ToString(), "")

        ' Créer une instance de la classe GitHubViewModel

        ' Vérifier si le texte de la description n'est pas vide
        If Not String.IsNullOrWhiteSpace(descriptionText) Then


            ' Utiliser la catégorie sélectionnée pour prendre des mesures appropriées
            Select Case selectedCategory
                Case "problem"
                    ' Traiter le problème signalé
                    Await CreateGitHubIssueAsync(MainWindow._userSettingsMain.UserName & " a rencontré un problème", descriptionText)

                Case "idea"
                    ' Traiter l'idée proposée
                    Await CreateGitHubIssueAsync(MainWindow._userSettingsMain.UserName & " a une idée", descriptionText)

                Case Else
                    ' Catégorie non reconnue
                    ' Gérer le cas où la catégorie n'est pas reconnue
            End Select
        Else
            ' Le DescriptionTextBox est vide ou ne contient que des espaces
            ' Afficher un message d'erreur ou prendre une autre action appropriée

        End If

    End Sub

    Private Sub WikiButton_Click(sender As Object, e As RoutedEventArgs)
        'Dim viewModel As GitHubViewModel = TryCast(Me.DataContext, GitHubViewModel)
        'If viewModel IsNot Nothing Then
        'Await viewModel.DisplayMessageAsync("Erreur", "Le champ de description est vide.")
        'End If
    End Sub

    Private Sub HomePageButton_Click(sender As Object, e As RoutedEventArgs)





    End Sub



End Class
