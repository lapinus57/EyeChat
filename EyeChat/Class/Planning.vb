Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports EyeChat.MainWindow
Imports log4net
Imports Newtonsoft.Json

Public Class Planning
    Implements INotifyPropertyChanged

    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Private _Day As String
    Public Property Day As String
        Get
            Return _Day
        End Get
        Set(value As String)
            If _Day <> value Then
                _Day = value
                NotifyPropertyChanged("Day")
            End If
        End Set
    End Property

    Private _User As String
    Public Property User As String
        Get
            Return _User
        End Get
        Set(value As String)
            If _User <> value Then
                _User = value
                NotifyPropertyChanged("User")
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    ' Charge le Planning à partir d'un fichier JSON
    Public Shared Sub LoadPlanningFromJson()
        Try
            If File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Planning.json")) Then
                Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Planning.json"))
                Plannings = JsonConvert.DeserializeObject(Of ObservableCollection(Of Planning))(json)
            Else
                Plannings = New ObservableCollection(Of Planning)()
                SavePlanningToJson(Plannings)
            End If
        Catch ex As Exception
            ' Gérer les erreurs de chargement (par exemple, journaliser l'erreur)
            logger.Error($"Erreur lors du chargement du Planning : {ex.Message}")
        End Try

    End Sub

    Public Shared Sub SavePlanningToJson(ByVal exams As ObservableCollection(Of Planning))
        Try
            Dim jsonFilePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Planning.json")

            ' Convertir les options d'examen en format JSON
            Dim optionsJson As String = JsonConvert.SerializeObject(exams.ToList(), Formatting.Indented)

            ' Écrire le JSON dans le fichier
            File.WriteAllText(jsonFilePath, optionsJson)
        Catch ex As Exception
            ' Gérer les erreurs de sauvegarde (par exemple, journaliser l'erreur)
            logger.Error($"Erreur lors de la sauvegarde du Planning : {ex.Message}")
        End Try
    End Sub

End Class