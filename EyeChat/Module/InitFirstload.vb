Imports System.IO
Imports log4net

Public Class InitFirstload
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Shared Sub Load()
        logger.Info("Initialisation du premier chargement et vérification")
        Try
            HandleFirstLoad()
            logger.Info("Initialisation du premier chargement terminé")
        Catch ex As Exception
            logger.Error($"Erreur lors de l'initialisation du premier chargement : {ex.Message}")
        End Try

        Try
            Filetest()
            logger.Info("Vérification des fichiers terminée")
        Catch ex As Exception
            logger.Error($"Erreur lors de la vérification des fichiers : {ex.Message}")
        End Try
    End Sub

    Public Shared Sub HandleFirstLoad()

        'test si les paramètres sont présent
        logger.Info("Verification des paramètres")
        Try
            ' Test si My.Settings.windowsName est vide
            logger.Debug("Test si le paramètre My.Settings.WindowsName est présent")
            If String.IsNullOrWhiteSpace(My.Settings.WindowsName) Then
                My.Settings.WindowsName = Environment.UserName
                My.Settings.Save()
                logger.Info($"Nom de l'utilisateur windows : {My.Settings.WindowsName}")
            End If
        Catch ex As Exception
            logger.Error($"Erreur lors de la récupération du nom de l'utilisateur windows : {ex.Message}")
        End Try

        Try
            ' test si My.Settings.ComputerName est vide
            logger.Debug("Test si le paramètre My.Settings.ComputerName est présent")
            If String.IsNullOrWhiteSpace(My.Settings.ComputerName) Then
                My.Settings.ComputerName = Environment.MachineName
                My.Settings.Save()
                logger.Info($"Nom de l'ordinateur : {My.Settings.ComputerName}")
            End If
        Catch ex As Exception
            logger.Error($"Erreur lors de la récupération du nom de l'ordinateur : {ex.Message}")
        End Try

        Try
            ' test si My.Settings.UniqueId est vide
            logger.Debug("Test si le paramètre My.Settings.UniqueId est présent")
            If String.IsNullOrWhiteSpace(My.Settings.UniqueId) Then
                My.Settings.UniqueId = UIDManager.GenerateUniqueId()
                My.Settings.Id = UIDManager.GetUniqueIdHashCode()
                My.Settings.Save()
                logger.Info($"UniqueId : {My.Settings.UniqueId}")
            End If
        Catch ex As Exception
            logger.Error($"Erreur lors de la récupération du UniqueId : {ex.Message}")
        End Try
    End Sub

    Public Shared Sub Filetest()
        logger.Info("Verification des fichiers")
        Try
            Dim dossier As String = "HistoricPatient"
            ' Vérifier si le dossier existe
            logger.Debug("Verification du dossier HistoricPatient")
            If Not Directory.Exists(dossier) Then
                ' Créer le dossier s'il n'existe pas
                Directory.CreateDirectory(dossier)
                logger.Debug("Creation du dossier HistoricPatient")
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la creation du dossier HistoricPatient")
        End Try

        Try
            Dim dossier As String = "HistoricMsg"
            ' Vérifier si le dossier existe
            logger.Debug("Verification du dossier HistoricMsg")
            If Not Directory.Exists(dossier) Then
                ' Créer le dossier s'il n'existe pas
                Directory.CreateDirectory(dossier)
                logger.Debug("Creation du dossier HistoricMsg")
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la creation du dossier HistoricMsg")
        End Try

        Try
            Dim dossier As String = "Core"
            ' Vérifier si le dossier existe
            logger.Debug("Verification du dossier Core")
            If Not Directory.Exists(dossier) Then
                ' Créer le dossier s'il n'existe pas
                Directory.CreateDirectory(dossier)
                logger.Debug("Creation du dossier Core")
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la creation du dossier Core")
        End Try

        Try
            Dim dossier As String = "Logs"
            ' Vérifier si le dossier existe
            logger.Debug("Verification du dossier Logs")
            If Not Directory.Exists(dossier) Then
                ' Créer le dossier s'il n'existe pas
                Directory.CreateDirectory(dossier)
                logger.Debug("Creation du dossier Logs")
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la creation du dossier Logs")
        End Try

        Try
            Dim dossier As String = "Users"
            ' Vérifier si le dossier existe
            logger.Debug("Verification du dossier Users")
            If Not Directory.Exists(dossier) Then
                ' Créer le dossier s'il n'existe pas
                Directory.CreateDirectory(dossier)
                logger.Debug("Creation du dossier Users")
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la creation du dossier Users")
        End Try

    End Sub

End Class
