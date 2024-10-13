Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports ControlzEx.Theming
Imports EyeChat.MainWindow
Imports EyeChat.User
Imports log4net
Imports Newtonsoft.Json
Imports MahApps.Metro
Imports MahApps.Metro.Theming
Imports System.Configuration



Public Class SettingsViewModel
    Implements INotifyPropertyChanged

    Public Property ColorItems As New ObservableCollection(Of ColorItemViewModel)()
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public Property DebugLevels As New ObservableCollection(Of String)() From {"DEBUG", "INFO", "WARN", "ERROR"}

    Private _examOptions As New ObservableCollection(Of ExamOption)()
    Private _SpeedMessage As New ObservableCollection(Of SpeedMessage)()
    Private _Planning As New ObservableCollection(Of Planning)()
    Private _settings As Dictionary(Of String, Boolean)
    Private Shared _userSettings As UserSettings


    Private _mainWindow As MainWindow
    Private _appSizeDisplay As Double

    Public Event PropertyChanged As PropertyChangedEventHandler _
    Implements INotifyPropertyChanged.PropertyChanged


    Public Sub New()



        _examOptions = New ObservableCollection(Of ExamOption)()

        _userSettings = UserSettings.Load()

        ' Charger les options d'examen à partir du JSON
        Dim jsonFilePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "examOptions.json")
        If File.Exists(jsonFilePath) Then
            Dim json As String = File.ReadAllText(jsonFilePath)
            Dim ptions As List(Of ExamOption) = JsonConvert.DeserializeObject(Of List(Of ExamOption))(json)

            For Each ption As ExamOption In ptions
                _examOptions.Add(ption)
            Next
        End If

        _SpeedMessage = New ObservableCollection(Of SpeedMessage)()

        ' Charger les options d'examen à partir du JSON
        jsonFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "SpeedMessage.json")
        If File.Exists(jsonFilePath) Then
            Dim json As String = File.ReadAllText(jsonFilePath)
            Dim ptions As List(Of SpeedMessage) = JsonConvert.DeserializeObject(Of List(Of SpeedMessage))(json)

            For Each ption As SpeedMessage In ptions
                _SpeedMessage.Add(ption)
            Next
        End If

        _Planning = New ObservableCollection(Of Planning)()

        ' Charger le Planning à partir du JSON
        jsonFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Planning.json")
        If File.Exists(jsonFilePath) Then
            Dim json As String = File.ReadAllText(jsonFilePath)
            Dim ptions As List(Of Planning) = JsonConvert.DeserializeObject(Of List(Of Planning))(json)

            For Each ption As Planning In ptions
                _Planning.Add(ption)
            Next
        End If


    End Sub

    Public ReadOnly Property UserSettings As UserSettings
        Get
            Return _userSettings
        End Get
    End Property

    Public Property AppNameDisplay As String
        Get
            Try
                Return My.Settings.AppNameDisplay

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété  AppNameDisplay : {ex.Message}")
                Return "EyeChat"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.AppNameDisplay = value
                My.Settings.Save()
                NotifyPropertyChanged("AppNameDisplay")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppNameDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ComputerName As String
        Get
            Try
                Return My.Settings.ComputerName

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ComputerName : {ex.Message}")
                Return "NoName"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.ComputerName = value
                My.Settings.Save()
                NotifyPropertyChanged("ComputerName")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété ComputerName : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property WindowsName As String
        Get
            Try
                Return My.Settings.WindowsName

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété WindowsName : {ex.Message}")
                Return "Bender"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.WindowsName = value
                My.Settings.Save()
                NotifyPropertyChanged("WindowsName")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété WindowsName : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property UserName As String
        Get
            Try
                Return _userSettings.UserName

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété UserName : {ex.Message}")
                Return "EyeChat"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                _userSettings.UserName = value
                _userSettings.Save()
                NotifyPropertyChanged("UserName")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété UserName : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppTheme As String
        Get
            Try
                Return _userSettings.AppTheme
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppTheme : {ex.Message}")
                Return "White"
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Éviter les doublons avant de modifier AppTheme
                If _userSettings.AppTheme = value Then
                    logger.Info("AppTheme n'a pas changé, aucun besoin de mise à jour.")
                    Return
                End If

                ' Mise à jour des paramètres
                _userSettings.AppTheme = value
                _userSettings.Save()
                NotifyPropertyChanged("AppTheme")
                logger.Error($"Le thème a été modifié : {value}")
            Catch ex As Exception
                ' Gérer l'exception
                logger.Error($"Erreur lors de la modification de la propriété AppTheme : {ex.Message}")
            End Try
        End Set
    End Property





    Public Property AppSizeDisplay As Double
        Get
            Try
                logger.Info("Accès à la propriété AppSizeDisplay")
                Return _userSettings.AppSizeDisplay
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppSizeDisplay : {ex.Message}")
                Return 1.0 ' Valeur par défaut en cas d'erreur
            End Try
        End Get
        Set(ByVal value As Double)
            Try
                If _userSettings.AppSizeDisplay <> value Then
                    _userSettings.AppSizeDisplay = value
                    _userSettings.Save() ' Sauvegarder les paramètres
                    NotifyPropertyChanged("AppSizeDisplay")
                    Dim mainWindow = CType(Application.Current.MainWindow, MainWindow)
                    mainWindow.SetHeaderFontSize(CInt(value))
                    mainWindow.UpdateLayout()
                    'My.Application.MainWindow.SetHeaderFontSize(CInt(value))
                    'PatientTabCtrl.FontSize = value
                    NotifyPropertyChanged("ArrowSize")
                    SelectUser("A Tous")
                    Users.Clear()
                    Dim loadedUsers = LoadUsersFromJson()
                    For Each user In loadedUsers
                        Users.Add(user)
                    Next
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété AppSizeDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppColorString As String
        Get
            Try

                Dim storedValue As String = _userSettings.AppColorString
                ' Supprimer les guillemets simples si présents
                Return If(Not String.IsNullOrEmpty(storedValue), storedValue.Trim("'"c), "Blue")
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppColorString : {ex.Message}")
                Return "Blue"
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Éviter les doublons avant de modifier AppColorString
                If _userSettings.AppColorString = value Then
                    logger.Info("AppColorString n'a pas changé, aucun besoin de mise à jour.")
                    Return
                End If

                value = If(Not String.IsNullOrEmpty(value), value.Trim("'"c), value)

                Dim converter As New System.Windows.Media.ColorConverter()
                Dim Color As System.Windows.Media.Color = CType(System.Windows.Media.ColorConverter.ConvertFromString(value), System.Windows.Media.Color)


                ' Mise à jour des paramètres
                _userSettings.AppColorString = value
                _userSettings.AppColor = System.Drawing.Color.FromArgb(Color.A, Color.R, Color.G, Color.B)
                _userSettings.Save()
                NotifyPropertyChanged("AppColorString")
                SelectUser(SelectedUser)
            Catch ex As Exception
                ' Gérer l'exception
                logger.Error($"Erreur lors de la modification de la propriété AppColorString : {ex.Message}")
            End Try
        End Set
    End Property


    Public Property AppColor As Color
        Get
            Try
                logger.Debug("Lecture de la propriété AppColor")
                Dim storedValue As System.Drawing.Color = _userSettings.AppColor

                ' Vérifiez si storedValue est null ou vide
                If storedValue = Nothing Then
                    Return Color.Blue ' Valeur par défaut
                End If

                Return storedValue

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppColor : {ex.Message}")
                Return Color.Blue
            End Try

        End Get
        Set(ByVal value As Color)
            Try
                ' Éviter les doublons avant de modifier AppColor
                If MainWindow._userSettingsMain.AppColor = value Then
                    logger.Info("AppColor n'a pas changé, aucun besoin de mise à jour.")
                    Return
                End If

                MainWindow._userSettingsMain.AppColor = value
                MainWindow._userSettingsMain.Save()
                NotifyPropertyChanged("AppColor")
                SelectUser(SelectedUser)
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppColor : {ex.Message}")
            End Try
        End Set
    End Property

    Public Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        My.Application.MainWindow.UpdateLayout()

    End Sub






    Public Shared Sub SetTheme()
        Try
            ' Récupérer le nom du thème (clair ou sombre)
            Dim themeName As String = If(_userSettings.AppTheme = "Clair", "Light", "Dark")

            ' Récupérer la couleur sélectionnée à partir de My.Settings.AppColor
            Dim mediaColor As System.Windows.Media.Color = System.Windows.Media.Color.FromArgb(
            _userSettings.AppColor.A,
            _userSettings.AppColor.R,
            _userSettings.AppColor.G,
            _userSettings.AppColor.B)

            ' Détecter l'application actuelle
            Dim application = System.Windows.Application.Current

            ' Vérifier si l'application et ses dictionnaires de ressources existent
            If application IsNot Nothing AndAlso application.Resources IsNot Nothing Then
                ' Nettoyer les thèmes existants en double
                CleanExistingThemes()

                ' Regénérer un nouveau thème si nécessaire
                Dim newTheme As Theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(themeName, mediaColor)
                ' Changer le thème de l'application en utilisant le nouvel objet Theme
                ThemeManager.Current.ChangeTheme(System.Windows.Application.Current, newTheme)

                ' Rechercher si un dictionnaire de ressources existe déjà avec ce thème
                Dim existingResourceDictionary = application.Resources.MergedDictionaries.FirstOrDefault(Function(rd) rd.Source IsNot Nothing AndAlso rd.Source.ToString().Contains("AppTheme"))

                ' Si un dictionnaire de ressources existe avec le même thème, ne pas ajouter
                If existingResourceDictionary IsNot Nothing Then
                    logger.Info("Le thème existe déjà, aucun besoin de le regénérer.")
                Else
                    ' Ajouter le nouveau thème
                    application.Resources.MergedDictionaries.Add(newTheme.Resources)
                    ThemeManager.Current.ChangeTheme(application, newTheme)
                End If
            End If

        Catch ex As Exception
            ' Gérer l'erreur
            logger.Error($"Erreur lors de la modification du thème : {ex.Message}")
        End Try
    End Sub

    ' Méthode pour nettoyer les thèmes en double dans le ResourceDictionary
    Private Shared Sub CleanExistingThemes()
        Dim application = System.Windows.Application.Current

        If application IsNot Nothing AndAlso application.Resources.MergedDictionaries IsNot Nothing Then
            ' Supprimer les dictionnaires de ressources en double qui contiennent "AppTheme"
            Dim themesToRemove = application.Resources.MergedDictionaries.Where(Function(rd) rd.Source IsNot Nothing AndAlso rd.Source.ToString().Contains("AppTheme")).ToList()

            For Each theme In themesToRemove
                application.Resources.MergedDictionaries.Remove(theme)
            Next
        End If
    End Sub



    Public Property SelectedDebugLevel As String
        Get
            Try
                logger.Debug("Lecture de la propriété SelectedDebugLevel")
                Return _userSettings.DebugLevel
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété SelectedDebugLevel : {ex.Message}")
                Return "DEBUG"
            End Try

        End Get
        Set(ByVal value As String)
            ' Mise à jour des paramètres uniquement si la valeur change
            If _userSettings.DebugLevel <> value Then
                _userSettings.DebugLevel = value
                _userSettings.Save()
                NotifyPropertyChanged("SelectedDebugLevel")
                logger.Info($"Le niveau de débogage a été modifié : {value}")
                ' Vérifier le niveau de débogage sélectionné
                Select Case value
                    Case "DEBUG"
                        logger.Logger.Repository.Threshold = log4net.Core.Level.Debug
                    Case "INFO"
                        logger.Logger.Repository.Threshold = log4net.Core.Level.Info
                    Case "WARN"
                        logger.Logger.Repository.Threshold = log4net.Core.Level.Warn
                    Case "ERROR"
                        logger.Logger.Repository.Threshold = log4net.Core.Level.Error
                End Select
            End If


            NotifyPropertyChanged("SelectedDebugLevel")
        End Set
    End Property


    Public Property CtrlF9 As String
        Get
            Try
                logger.Debug("Lecture de la propriété CtrlF9")
                Return _userSettings.CtrlF9
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF9 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Mise à jour des paramètres si la valeur change
                If _userSettings.CtrlF9 <> value Then
                    _userSettings.CtrlF9 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("CtrlF9")
                    logger.Info($"La valeur de CtrlF9 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF9 : {ex.Message}")
            End Try
        End Set
    End Property


    Public Property CtrlF9Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété CtrlF9Enabled")
                Return _userSettings.CtrlF9Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF9Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(value As Boolean)
            Try
                If _userSettings.CtrlF9Enabled <> value Then
                    _userSettings.CtrlF9Enabled = value
                    _userSettings.Save()
                    OnPropertyChanged(NameOf(CtrlF9Enabled))
                    logger.Info($"La valeur de CtrlF9Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF9Enabled1 : {ex.Message}")
            End Try


        End Set
    End Property


    Public Property CtrlF10 As String
        Get
            Try
                logger.Debug("Lecture de la propriété CtrlF10")
                Return _userSettings.CtrlF10
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF10 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.CtrlF10 <> value Then
                    _userSettings.CtrlF10 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("CtrlF10")
                    logger.Info($"La valeur de CtrlF10 a été modifiée : {value}")
                End If

            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF10 : {ex.Message}")
            End Try
        End Set
    End Property



    Public Property CtrlF10Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété CtrlF10Enabled")
                Return _userSettings.CtrlF10Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF10Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.CtrlF10Enabled <> value Then
                    _userSettings.CtrlF10Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("CtrlF10Enabled")
                    logger.Info($"La valeur de CtrlF10Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF10Enabled : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property CtrlF11 As String
        Get
            Try
                logger.Debug("Lecture de la propriété CtrlF11")
                Return _userSettings.CtrlF11
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF11 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.CtrlF11 <> value Then
                    _userSettings.CtrlF11 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("CtrlF11")
                    logger.Info($"La valeur de CtrlF11 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF11 : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property CtrlF11Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété CtrlF11Enabled")
                Return _userSettings.CtrlF11Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF11Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.CtrlF11Enabled <> value Then
                    _userSettings.CtrlF11Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("CtrlF11Enabled")
                    logger.Info($"La valeur de CtrlF11Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF11Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property CtrlF12Enabled As Boolean

        Get
            Try
                logger.Debug("Lecture de la propriété CtrlF12Enabled")
                Return _userSettings.CtrlF12Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF12Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.CtrlF12Enabled <> value Then
                    _userSettings.CtrlF12Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("CtrlF12Enabled")
                    logger.Info($"La valeur de CtrlF12Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF12Enabled : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property ShiftF9 As String
        Get
            Try
                logger.Debug("Lecture de la propriété ShiftF9")
                Return _userSettings.ShiftF9
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF9 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.ShiftF9 <> value Then
                    _userSettings.ShiftF9 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("ShiftF9")
                    logger.Info($"La valeur de ShiftF9 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShifttF9 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF9Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété ShiftF9Enabled")
                Return _userSettings.ShiftF9Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF9Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.ShiftF9Enabled <> value Then
                    _userSettings.ShiftF9Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("ShiftF9Enabled")
                    logger.Info($"La valeur de ShiftF9Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF9Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF10 As String
        Get
            Try
                logger.Debug("Lecture de la propriété ShiftF10")
                Return _userSettings.ShiftF10
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF10 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.ShiftF10 <> value Then
                    _userSettings.ShiftF10 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("ShiftF10")
                    logger.Info($"La valeur de ShiftF10 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF10 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF10Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété ShiftF10Enabled")
                Return _userSettings.ShiftF10Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF10Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.ShiftF10Enabled <> value Then
                    _userSettings.ShiftF10Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("ShiftF10Enabled")
                    logger.Info($"La valeur de ShiftF10Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF10Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF11 As String
        Get
            Try
                logger.Debug("Lecture de la propriété ShiftF11")
                Return _userSettings.ShiftF11
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF11 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.ShiftF11 <> value Then
                    _userSettings.ShiftF11 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("ShiftF11")
                    logger.Info($"La valeur de ShiftF11 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF11 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF11Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété ShiftF11Enabled")
                Return _userSettings.ShiftF11Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF11Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.ShiftF11Enabled <> value Then
                    _userSettings.ShiftF11Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("ShiftF11Enabled")
                    logger.Info($"La valeur de ShiftF11Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF11Enabled : {ex.Message}")
            End Try
        End Set
    End Property


    Public Property PlanningMode As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété PlanningMode")
                Return _userSettings.PlanningMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété PlanningMode : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.PlanningMode <> value Then
                    _userSettings.PlanningMode = value
                    _userSettings.Save()
                    NotifyPropertyChanged("PlanningMode")
                    logger.Info($"La valeur de PlanningMode a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété PlanningMode : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property PlanningMode2 As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété PlanningMode2")
                Return _userSettings.PlanningMode2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété PlanningMode2 : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.PlanningMode2 <> value Then
                    _userSettings.PlanningMode2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("PlanningMode2")
                    logger.Info($"La valeur de PlanningMode2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété PlanningMode2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property PlanningName2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété PlanningName2")
                Return _userSettings.PlanningName2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété PlanningName2 : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As String)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.PlanningName2 <> value Then
                    _userSettings.PlanningName2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("PlanningName2")
                    logger.Info($"La valeur de PlanningName2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété PlanningName2 : {ex.Message}")
            End Try
        End Set
    End Property


    Public Property SecretaryMode As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété SecretaryMode")
                Return _userSettings.SecretaryMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété SecretaryMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.SecretaryMode <> value Then
                    _userSettings.SecretaryMode = value
                    _userSettings.Save()
                    NotifyPropertyChanged("SecretaryMode")
                    logger.Info($"La valeur de SecretaryMode a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété SecretaryMode : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property DoctorMode As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété DoctorMode")
                Return _userSettings.DoctorMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété DoctorMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.DoctorMode <> value Then
                    _userSettings.DoctorMode = value
                    _userSettings.Save()
                    NotifyPropertyChanged("DoctorMode")
                    logger.Info($"La valeur de DoctorMode a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété DoctorMode : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property OrthoMode As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété OrthoMode")
                Return _userSettings.OrthoMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété OrthoMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.OrthoMode <> value Then
                    _userSettings.OrthoMode = value
                    _userSettings.Save()
                    NotifyPropertyChanged("OrthoMode")
                    logger.Info($"La valeur de OrthoMode a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété OrthoMode : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AdvanvedMode As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété AdvanvedMode")
                Return _userSettings.AdvanvedMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AdvanvedMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.AdvanvedMode <> value Then
                    _userSettings.AdvanvedMode = value
                    _userSettings.Save()
                    NotifyPropertyChanged("AdvanvedMode")
                    logger.Info($"La valeur de AdvanvedMode a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété AdvanvedMode : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AdminMode As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété AdminMode")
                Return _userSettings.AdminMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AdminMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.AdminMode <> value Then
                    _userSettings.AdminMode = value
                    _userSettings.Save()
                    NotifyPropertyChanged("AdminMode")
                    logger.Info($"La valeur de AdminMode a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété AdminMode : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property NFCMode As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété NFCMode")
                Return _userSettings.NFCMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété NFCMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                ' Mise à jour des paramètres uniquement si la valeur change
                If _userSettings.NFCMode <> value Then
                    _userSettings.NFCMode = value
                    _userSettings.Save()
                    NotifyPropertyChanged("NFCMode")
                    logger.Info($"La valeur de NFCMode a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété NFCMode : {ex.Message}")
            End Try
        End Set
    End Property

    Public ReadOnly Property ArrowSize As Double
        Get
            Try
                Return AppSizeDisplay * 0.8 ' Ajustez le coefficient selon vos préférences
            Catch ex As Exception
                logger.Error($"Erreur lors de la récupération de la propriété ArrowSize : {ex.Message}")
                Return 0 ' ou une valeur par défaut appropriée en cas d'erreur
            End Try
        End Get
    End Property

    Public Property RoomDisplay As Boolean
        Get
            logger.Debug("Lecture de la propriété RoomDisplay")
            Return _userSettings.RoomDisplay
        End Get
        Set(value As Boolean)
            Try
                If _userSettings.RoomDisplay <> value Then
                    _userSettings.RoomDisplay = value
                    If value = True Then
                        _userSettings.RoomDisplayStr = "Visible"
                    Else
                        _userSettings.RoomDisplayStr = "Collapsed"
                    End If
                    _userSettings.Save()
                    NotifyPropertyChanged("RoomDisplay")
                    NotifyPropertyChanged("RoomDisplayStr")
                    logger.Info($"La valeur de RoomDisplay a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété RoomDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property RoomDisplayStr As String
        Get
            logger.Debug("Lecture de la propriété RoomDisplayStr")
            Return _userSettings.RoomDisplayStr
        End Get
        Set(value As String)
            Try
                If _userSettings.RoomDisplayStr <> value Then
                    _userSettings.RoomDisplayStr = value
                    NotifyPropertyChanged("RoomDisplay")
                    NotifyPropertyChanged("RoomDisplayStr")
                    logger.Info($"La valeur de RoomDisplayStr a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété RoomDisplayStr : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ExamOptions As ObservableCollection(Of ExamOption)
        Get
            Return _examOptions
        End Get
        Set(value As ObservableCollection(Of ExamOption))
            Try
                _examOptions = value
                NotifyPropertyChanged("ExamOptions")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ExamOptions : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property SpeedMessage As ObservableCollection(Of SpeedMessage)
        Get
            Return _SpeedMessage
        End Get
        Set(value As ObservableCollection(Of SpeedMessage))
            Try
                _SpeedMessage = value
                NotifyPropertyChanged("SpeedMessage")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété SpeedMessage : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property Planning As ObservableCollection(Of Planning)
        Get
            Return _Planning
        End Get
        Set(value As ObservableCollection(Of Planning))
            Try
                _Planning = value
                NotifyPropertyChanged("Planning")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété Planning : {ex.Message}")
            End Try
        End Set
    End Property

#Region "Raccourcis clavier consultation"
    Public Property F5Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété F5Enabled")
                Return _userSettings.F5Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                If _userSettings.F5Enabled <> value Then
                    _userSettings.F5Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Enabled")
                    logger.Info($"La valeur de F5Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Page1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Page1")
                Return _userSettings.F5Page1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Page1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Page1 <> value Then
                    _userSettings.F5Page1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Page1")
                    logger.Info($"La valeur de F5Page1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Page1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Page2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Page2")
                Return _userSettings.F5Page2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Page2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Page2 <> value Then
                    _userSettings.F5Page2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Page2")
                    logger.Info($"La valeur de F5Page2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Page2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Page3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Page3")
                Return _userSettings.F5Page3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Page3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Page3 <> value Then
                    _userSettings.F5Page3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Page3")
                    logger.Info($"La valeur de F5Page3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Page3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Page4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Page4")
                Return _userSettings.F5Page4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Page4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Page4 <> value Then
                    _userSettings.F5Page4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Page4")
                    logger.Info($"La valeur de F5Page4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Page4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Page5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Page5")
                Return _userSettings.F5Page5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Page5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Page5 <> value Then
                    _userSettings.F5Page5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Page5")
                    logger.Info($"La valeur de F5Page5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Page5 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Text1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Text1")
                Return _userSettings.F5Text1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Text1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Text1 <> value Then
                    _userSettings.F5Text1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Text1")
                    logger.Info($"La valeur de F5Text1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Text1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Text2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Text2")
                Return _userSettings.F5Text2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Text2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Text2 <> value Then
                    _userSettings.F5Text2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Text2")
                    logger.Info($"La valeur de F5Text2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Text2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Text3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Text3")
                Return _userSettings.F5Text3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Text3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Text3 <> value Then
                    _userSettings.F5Text3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Text3")
                    logger.Info($"La valeur de F5Text3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Text3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Text4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Text4")
                Return _userSettings.F5Text4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Text4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Text4 <> value Then
                    _userSettings.F5Text4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Text4")
                    logger.Info($"La valeur de F5Text4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Text4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F5Text5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F5Text5")
                Return _userSettings.F5Text5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F5Text5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F5Text5 <> value Then
                    _userSettings.F5Text5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F5Text5")
                    logger.Info($"La valeur de F5Text5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F5Text5 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété F6Enabled")
                Return _userSettings.F6Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                If _userSettings.F6Enabled <> value Then
                    _userSettings.F6Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Enabled")
                    logger.Info($"La valeur de F6Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Page1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Page1")
                Return _userSettings.F6Page1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Page1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Page1 <> value Then
                    _userSettings.F6Page1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Page1")
                    logger.Info($"La valeur de F6Page1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Page1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Page2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Page2")
                Return _userSettings.F6Page2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Page2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Page2 <> value Then
                    _userSettings.F6Page2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Page2")
                    logger.Info($"La valeur de F6Page2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Page2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Page3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Page3")
                Return _userSettings.F6Page3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Page3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Page3 <> value Then
                    _userSettings.F6Page3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Page3")
                    logger.Info($"La valeur de F6Page3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Page3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Page4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Page4")
                Return _userSettings.F6Page4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Page4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Page4 <> value Then
                    _userSettings.F6Page4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Page4")
                    logger.Info($"La valeur de F6Page4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Page4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Page5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Page5")
                Return _userSettings.F6Page5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Page5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Page5 <> value Then
                    _userSettings.F6Page5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Page5")
                    logger.Info($"La valeur de F6Page5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Page5 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Text1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Text1")
                Return _userSettings.F6Text1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Text1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Text1 <> value Then
                    _userSettings.F6Text1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Text1")
                    logger.Info($"La valeur de F6Text1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Text1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Text2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Text2")
                Return _userSettings.F6Text2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Text2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Text2 <> value Then
                    _userSettings.F6Text2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Text2")
                    logger.Info($"La valeur de F6Text2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Text2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Text3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Text3")
                Return _userSettings.F6Text3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Text3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Text3 <> value Then
                    _userSettings.F6Text3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Text3")
                    logger.Info($"La valeur de F6Text3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Text3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Text4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Text4")
                Return _userSettings.F6Text4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Text4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Text4 <> value Then
                    _userSettings.F6Text4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Text4")
                    logger.Info($"La valeur de F6Text4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Text4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F6Text5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F6Text5")
                Return _userSettings.F6Text5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F6Text5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F6Text5 <> value Then
                    _userSettings.F6Text5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F6Text5")
                    logger.Info($"La valeur de F6Text5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F6Text5 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété F7Enabled")
                Return _userSettings.F7Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                If _userSettings.F7Enabled <> value Then
                    _userSettings.F7Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Enabled")
                    logger.Info($"La valeur de F7Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Page1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Page1")
                Return _userSettings.F7Page1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Page1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Page1 <> value Then
                    _userSettings.F7Page1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Page1")
                    logger.Info($"La valeur de F7Page1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Page1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Page2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Page2")
                Return _userSettings.F7Page2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Page2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Page2 <> value Then
                    _userSettings.F7Page2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Page2")
                    logger.Info($"La valeur de F7Page2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Page2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Page3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Page3")
                Return _userSettings.F7Page3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Page3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Page3 <> value Then
                    _userSettings.F7Page3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Page3")
                    logger.Info($"La valeur de F7Page3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Page3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Page4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Page4")
                Return _userSettings.F7Page4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Page4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Page4 <> value Then
                    _userSettings.F7Page4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Page4")
                    logger.Info($"La valeur de F7Page4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Page4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Page5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Page5")
                Return _userSettings.F7Page5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Page5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Page5 <> value Then
                    _userSettings.F7Page5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Page5")
                    logger.Info($"La valeur de F7Page5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Page5 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Text1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Text1")
                Return _userSettings.F7Text1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Text1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Text1 <> value Then
                    _userSettings.F7Text1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Text1")
                    logger.Info($"La valeur de F7Text1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Text1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Text2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Text2")
                Return _userSettings.F7Text2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Text2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Text2 <> value Then
                    _userSettings.F7Text2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Text2")
                    logger.Info($"La valeur de F7Text2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Text2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Text3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Text3")
                Return _userSettings.F7Text3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Text3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Text3 <> value Then
                    _userSettings.F7Text3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Text3")
                    logger.Info($"La valeur de F7Text3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Text3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Text4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Text4")
                Return _userSettings.F7Text4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Text4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Text4 <> value Then
                    _userSettings.F7Text4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Text4")
                    logger.Info($"La valeur de F7Text4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Text4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F7Text5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F7Text5")
                Return _userSettings.F7Text5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F7Text5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F7Text5 <> value Then
                    _userSettings.F7Text5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F7Text5")
                    logger.Info($"La valeur de F7Text5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F7Text5 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Enabled As Boolean
        Get
            Try
                logger.Debug("Lecture de la propriété F8Enabled")
                Return _userSettings.F8Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                If _userSettings.F8Enabled <> value Then
                    _userSettings.F8Enabled = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Enabled")
                    logger.Info($"La valeur de F8Enabled a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Page1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Page1")
                Return _userSettings.F8Page1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Page1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Page1 <> value Then
                    _userSettings.F8Page1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Page1")
                    logger.Info($"La valeur de F8Page1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Page1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Page2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Page2")
                Return _userSettings.F8Page2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Page2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Page2 <> value Then
                    _userSettings.F8Page2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Page2")
                    logger.Info($"La valeur de F8Page2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Page2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Page3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Page3")
                Return _userSettings.F8Page3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Page3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Page3 <> value Then
                    _userSettings.F8Page3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Page3")
                    logger.Info($"La valeur de F8Page3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Page3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Page4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Page4")
                Return _userSettings.F8Page4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Page4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Page4 <> value Then
                    _userSettings.F8Page4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Page4")
                    logger.Info($"La valeur de F8Page4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Page4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Page5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Page5")
                Return _userSettings.F8Page5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Page5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Page5 <> value Then
                    _userSettings.F8Page5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Page5")
                    logger.Info($"La valeur de F8Page5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Page5 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Text1 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Text1")
                Return _userSettings.F8Text1
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Text1 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Text1 <> value Then
                    _userSettings.F8Text1 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Text1")
                    logger.Info($"La valeur de F8Text1 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Text1 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Text2 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Text2")
                Return _userSettings.F8Text2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Text2 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Text2 <> value Then
                    _userSettings.F8Text2 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Text2")
                    logger.Info($"La valeur de F8Text2 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Text2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Text3 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Text3")
                Return _userSettings.F8Text3
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Text3 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Text3 <> value Then
                    _userSettings.F8Text3 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Text3")
                    logger.Info($"La valeur de F8Text3 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Text3 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Text4 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Text4")
                Return _userSettings.F8Text4
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Text4 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Text4 <> value Then
                    _userSettings.F8Text4 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Text4")
                    logger.Info($"La valeur de F8Text4 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Text4 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property F8Text5 As String
        Get
            Try
                logger.Debug("Lecture de la propriété F8Text5")
                Return _userSettings.F8Text5
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété F8Text5 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                If _userSettings.F8Text5 <> value Then
                    _userSettings.F8Text5 = value
                    _userSettings.Save()
                    NotifyPropertyChanged("F8Text5")
                    logger.Info($"La valeur de F8Text5 a été modifiée : {value}")
                End If
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété F8Text5 : {ex.Message}")
            End Try
        End Set
    End Property

#End Region


    Public Shared Sub SaveExamOptionsToJson(ByVal exams As ObservableCollection(Of ExamOption))
        Try
            Dim jsonFilePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "examOptions.json")

            ' Convertir les options d'examen en format JSON
            Dim optionsJson As String = JsonConvert.SerializeObject(exams.ToList(), Formatting.Indented)

            ' Écrire le JSON dans le fichier
            File.WriteAllText(jsonFilePath, optionsJson)
        Catch ex As Exception
            ' Gérer les erreurs de sauvegarde (par exemple, journaliser l'erreur)
            logger.Error($"Erreur lors de la sauvegarde des ExamOptions : {ex.Message}")
        End Try
    End Sub

    Public Shared Sub SaveSpeedMessageToJson(ByVal exams As ObservableCollection(Of SpeedMessage))
        Try
            Dim jsonFilePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "SpeedMessage.json")

            ' Convertir les options d'examen en format JSON
            Dim optionsJson As String = JsonConvert.SerializeObject(exams.ToList(), Formatting.Indented)

            ' Écrire le JSON dans le fichier
            File.WriteAllText(jsonFilePath, optionsJson)
        Catch ex As Exception
            ' Gérer les erreurs de sauvegarde (par exemple, journaliser l'erreur)
            logger.Error($"Erreur lors de la sauvegarde des SpeedMessage : {ex.Message}")
        End Try
    End Sub




    Public ReadOnly Property PredefinedColors As List(Of Color)
        Get
            For Each colorProperty As PropertyInfo In GetType(Colors).GetProperties()
                Dim color As Color = DirectCast(colorProperty.GetValue(Nothing), Color)
                Call New List(Of Color)().Add(color)
            Next
            Return New List(Of Color)()
        End Get
    End Property


    Private Sub SaveSettingsIfChanged(Of T)(ByRef setting As T, ByVal newValue As T, ByVal propertyName As String)
        If Not EqualityComparer(Of T).Default.Equals(setting, newValue) Then
            setting = newValue
            My.Settings.Save()
            NotifyPropertyChanged(propertyName)
        End If
    End Sub

    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

End Class


