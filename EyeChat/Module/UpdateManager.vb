Imports System.Net
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports Newtonsoft.Json.Linq
Imports NuGet.Versioning
Imports MahApps.Metro.Controls.Dialogs
Imports log4net.Core
Imports log4net

Public Class UpdateManager
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Private Const RepoOwner As String = "lapinus57"
    Private Const RepoName As String = "EyeChat"
    Private Const Version As String = "0.0.1" ' Remplacez par votre version actuelle
    Private mainWindow As MainWindow

    Public Sub New(mainWindow As MainWindow)
        Me.mainWindow = mainWindow
    End Sub
    Public Async Sub CheckForUpdates(channel As String)
        ' Obtient la dernière version du serveur dans le canal spécifié
        logger.Info("Vérification des mises à jour disponibles dans le canal " & channel)
        Dim serverVersion As String = Await GetServerVersion(channel)

        If Not String.IsNullOrEmpty(serverVersion) AndAlso CompareVersions(serverVersion, Version) > 0 Then

            ' Récupère la version actuelle et la version de mise à jour
            Dim currentAppVersion As String = Version
            Dim updateAppVersion As String = serverVersion

            ' Configure les boutons de la boîte de dialogue
            Dim dialogSettings As New MetroDialogSettings With {
                .AffirmativeButtonText = "Mettre à jour",
                .NegativeButtonText = "Annuler"
            }

            ' Construit le message de la boîte de dialogue avec les versions
            Dim message As String = $"Votre version actuelle : {currentAppVersion}{Environment.NewLine}" &
                                    $"Version disponible : {updateAppVersion}{Environment.NewLine}" &
                                    $"Une nouvelle mise à jour est disponible dans le canal {channel}. Voulez-vous mettre à jour maintenant?"

            ' Ajoute un message de débogage pour indiquer que la mise à jour est disponible
            logger.Debug("Mise à jour disponible. Version actuelle : " & currentAppVersion & ", Version disponible : " & updateAppVersion)

            ' Affiche la boîte de dialogue et attend la réponse de l'utilisateur
            Dim result As MessageDialogResult = Await mainWindow.ShowMessageAsync("Mise à jour disponible", message, MessageDialogStyle.AffirmativeAndNegative, dialogSettings)

            If result = MessageDialogResult.Affirmative Then
                ' Si l'utilisateur accepte, lance le téléchargement et l'installation de la mise à jour
                Await DownloadAndInstallUpdate(channel)
            End If
        Else
            ' Affiche un message si aucune mise à jour n'est disponible
            Await mainWindow.ShowMessageAsync("Pas de mise à jour", "Vous utilisez la dernière version de l'application.")

            ' Ajoute un message de débogage pour indiquer qu'aucune mise à jour n'est disponible
            logger.Debug("Aucune mise à jour disponible. Version actuelle : " & Version)
        End If
    End Sub

    Private Async Function GetServerVersion(channel As String) As Task(Of String)
        logger.Info("Récupération de la version du serveur dans le canal " & channel)
        Try
            Dim apiUrl As String = $"https://raw.githubusercontent.com/{RepoOwner}/{RepoName}/master/releases/{channel}/version.json"
            Dim webClient As New WebClient()
            Dim versionJson As String = Await webClient.DownloadStringTaskAsync(apiUrl)
            Dim versionObject As JObject = JObject.Parse(versionJson)
            Dim serverVersion As String = versionObject("version").ToString()
            logger.Debug($"Version du serveur obtenue avec succès : {serverVersion}")
            Return serverVersion
        Catch ex As Exception
            logger.Error($"Erreur lors de la récupération de la version du serveur : {ex.Message}")
            Return Nothing
        End Try
    End Function

    Private Function CompareVersions(version1 As String, version2 As String) As Integer
        logger.Info("Comparaison des versions " & version1 & " et " & version2)
        Try
            Dim nugetVersion1 As NuGetVersion = NuGetVersion.Parse(version1)
            Dim nugetVersion2 As NuGetVersion = NuGetVersion.Parse(version2)
            Dim result As Integer = nugetVersion1.CompareTo(nugetVersion2)

            If result > 0 Then
                logger.Debug("La version " & version1 & " est supérieure à la version " & version2)
            ElseIf result < 0 Then
                logger.Debug("La version " & version1 & " est inférieure à la version " & version2)
            Else
                logger.Debug("La version " & version1 & " est égale à la version " & version2)
            End If

            Return result
        Catch ex As Exception
            logger.Error("Erreur lors de la comparaison des versions : " & ex.Message)
            Return 0
        End Try
    End Function

    Private Async Function DownloadAndInstallUpdate(channel As String) As Task
        logger.Info("Téléchargement et installation de la mise à jour depuis le canal " & channel)
        Dim webClient As New WebClient()
        Dim updateUrl As String = Await GetUpdateUrl(channel)

        Try
            Dim progressController = Await mainWindow.ShowProgressAsync("Téléchargement de la mise à jour", "Téléchargement en cours...")
            webClient.DownloadFileAsync(New Uri(updateUrl), "update.zip")

            AddHandler webClient.DownloadFileCompleted, Async Sub(sender, args)
                                                            progressController.SetIndeterminate()
                                                            Await progressController.CloseAsync()

                                                            Try
                                                                logger.Debug("Début de l'extraction et de l'installation des fichiers de mise à jour.")
                                                                Dim updateZipPath As String = "update.zip"
                                                                Dim updateExtractPath As String = "update_temp"
                                                                Dim updateExePath As String = Path.Combine(updateExtractPath, "update.exe")
                                                                Dim appDirectory As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

                                                                ZipFile.ExtractToDirectory(updateZipPath, updateExtractPath)

                                                                If File.Exists(updateExePath) Then
                                                                    mainWindow.Close()
                                                                    Process.Start(updateExePath)
                                                                    File.Delete(updateZipPath)
                                                                    logger.Debug("Mise à jour installée avec succès.")
                                                                Else
                                                                    Await mainWindow.ShowMessageAsync("Erreur de mise à jour", "Le fichier de mise à jour est incomplet ou endommagé.")
                                                                    logger.Error("Le fichier de mise à jour est incomplet ou endommagé.")
                                                                End If
                                                            Catch ex As Exception
                                                                logger.Error("Erreur lors de l'extraction et de l'installation des fichiers de mise à jour : " & ex.Message)
                                                            End Try
                                                        End Sub
        Catch ex As Exception
            logger.Error("Erreur lors du téléchargement et de l'installation de la mise à jour : " & ex.Message)
        End Try
    End Function

    Private Async Function GetUpdateUrl(channel As String) As Task(Of String)
        logger.Info("Obtention de l'URL de mise à jour depuis le canal " & channel)
        Try
            Dim apiUrl As String = $"https://raw.githubusercontent.com/{RepoOwner}/{RepoName}/master/releases/{channel}/version.json"
            Dim webClient As New WebClient()
            Dim versionJson As String = Await webClient.DownloadStringTaskAsync(apiUrl)
            Dim versionObject As JObject = JObject.Parse(versionJson)
            Dim updateUrl As String = versionObject("updateUrl").ToString()
            logger.Debug($"URL de mise à jour obtenue avec succès : {updateUrl}")
            Return updateUrl
        Catch ex As Exception
            logger.Error($"Erreur lors de la récupération de l'URL de mise à jour : {ex.Message}")
            Return Nothing
        End Try
    End Function
End Class

