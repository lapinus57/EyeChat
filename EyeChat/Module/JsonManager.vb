Imports System.Collections.ObjectModel
Imports System.IO
Imports log4net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class JsonManager
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public Shared Sub loadExamOption()
        ' Chargement de la liste des examens
        logger.Debug("Chargement des options d'examen")
        Try
            Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "examOptions.json"))
            MainWindow.ExamOptions = JsonConvert.DeserializeObject(Of ObservableCollection(Of ExamOption))(json)

        Catch ex As Exception
            logger.Error("Erreur lors du chargement des options d'examen : " & ex.Message)
        End Try
    End Sub

    Public Shared Sub loadSpeedMessage()
        ' Chargement des messages de rapide
        logger.Debug("Chargement des messages de vitesse")
        Try
            Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "SpeedMessage.json"))
            MainWindow.speedMessages = JsonConvert.DeserializeObject(Of List(Of SpeedMessage))(json)
        Catch ex As Exception
            logger.Error("Erreur lors du chargement des messages de vitesse : " & ex.Message)
        End Try
    End Sub

    Public Shared Sub EggPhrases()
        ' Chargement des phrases d'oeuf
        logger.Debug("Chargement des phrases d'oeuf")
        Try
            Dim jsonData As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "dataphrases.json"))
            MainWindow.phrasesData = JsonConvert.DeserializeObject(Of EggPhrasesData)(jsonData)
        Catch ex As Exception
            logger.Error("Erreur lors du chargement des phrases d'oeuf : " & ex.Message)
        End Try
    End Sub

End Class
