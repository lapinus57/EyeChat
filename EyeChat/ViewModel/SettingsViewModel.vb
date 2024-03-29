﻿Imports System.ComponentModel
Imports ControlzEx.Theming
Imports System.Drawing
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices
Imports log4net
Imports Newtonsoft.Json.Linq
Imports log4net.Core
Imports EyeChat.MainWindow
Imports EyeChat.User
Imports EyeChat
Imports System.IO
Imports Newtonsoft.Json


Public Class SettingsViewModel
    Implements INotifyPropertyChanged

    Public Property ColorItems As New ObservableCollection(Of ColorItemViewModel)()
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public Property DebugLevels As New ObservableCollection(Of String)() From {"DEBUG", "INFO", "WARN", "ERROR"}

    Private _examOptions As New ObservableCollection(Of ExamOption)()
    Private _SpeedMessage As New ObservableCollection(Of SpeedMessage)()
    Private _Planning As New ObservableCollection(Of Planning)()



    Public Event PropertyChanged As PropertyChangedEventHandler _
    Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New()
        _examOptions = New ObservableCollection(Of ExamOption)()

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
                Return My.Settings.UserName

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété UserName : {ex.Message}")
                Return "EyeChat"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.UserName = value
                My.Settings.Save()
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
                Return My.Settings.AppTheme

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppTheme : {ex.Message}")
                Return "White"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.AppTheme = value
                My.Settings.Save()
                NotifyPropertyChanged("AppTheme")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppTheme : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppSizeDisplay As Integer
        Get
            Try
                Return My.Settings.AppSizeDisplay

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppSizeDisplay : {ex.Message}")
                Return "14"
            End Try

        End Get
        Set(ByVal value As Integer)
            Try
                My.Settings.AppSizeDisplay = value
                My.Settings.Save()


                'TabCtrl.FontSize = value
                NotifyPropertyChanged("AppSizeDisplay")
                NotifyPropertyChanged("ArrowSize")
                SelectUser(SelectedUser)
                Users.Clear()



                Dim loadedUsers = LoadUsersFromJson()
                For Each user In loadedUsers
                    Users.Add(user)
                Next
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppSizeDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppColorString As String
        Get
            Try
                Dim storedValue As String = My.Settings.AppColorString
                ' Supprimer les guillemets simples si présents
                Return If(Not String.IsNullOrEmpty(storedValue), storedValue.Trim("'"c), "Blue")

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppColorString : {ex.Message}")
                Return "Blue"
            End Try

        End Get
        Set(ByVal value As String)

            Try
                value = If(Not String.IsNullOrEmpty(value), value.Trim("'"c), value)

                Dim converter As New System.Windows.Media.ColorConverter()
                Dim color As System.Windows.Media.Color = CType(converter.ConvertFromString(value), System.Windows.Media.Color)

                My.Settings.AppColorString = value
                My.Settings.AppColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
                My.Settings.Save()
                NotifyPropertyChanged("AppColorString")
                SelectUser(SelectedUser)
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppColorString : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppColor As Color
        Get
            Try
                Return My.Settings.AppColor

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppColor : {ex.Message}")
                Return Color.Blue
            End Try

        End Get
        Set(ByVal value As Color)
            Try
                My.Settings.AppColor = value
                My.Settings.Save()
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
    End Sub

    Public Shared Sub SetTheme()
        Try
            'Récupération de la couleur de base et traduction
            Dim themeName As String = If(My.Settings.AppTheme = "Clair", "Light", "Dark")

            Dim mediaColor As System.Windows.Media.Color = System.Windows.Media.Color.FromArgb(
        My.Settings.AppColor.A,
        My.Settings.AppColor.R,
        My.Settings.AppColor.G,
        My.Settings.AppColor.B)

            Dim brush As New System.Windows.Media.SolidColorBrush(mediaColor)

            ' Créer un nouvel objet Theme en utilisant les valeurs des paramètres stockées dans les Settings par défaut
            Dim newTheme As New Theme(
        "AppTheme",                                      ' Nom du thème
        "AppTheme",                                      ' Nom affiché du thème
        themeName,                                       ' Nom du thème de base 
        My.Settings.AppColorString,                      ' Nom de la couleur d'accent stockée dans les Settings
        mediaColor,                                      ' Couleur d'accent stockée dans les Settings
        New SolidColorBrush(mediaColor),                 ' Pinceau pour la couleur d'accent
        True,                                            ' Utiliser les couleurs système
        False                                            ' Appliquer uniquement aux contrôles MahApps.Metro
    )

            ' Changer le thème de l'application en utilisant le nouvel objet Theme
            ThemeManager.Current.ChangeTheme(System.Windows.Application.Current, newTheme)
        Catch ex As Exception
            ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
            logger.Error($"Erreur lors de la modification de la propriété ChangeTheme : {ex.Message}")
        End Try

    End Sub

    Public Property SelectedDebugLevel As String
        Get
            Try
                Return My.Settings.DebugLevel
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété SelectedDebugLevel : {ex.Message}")
                Return "DEBUG"
            End Try

        End Get
        Set(ByVal value As String)
            My.Settings.DebugLevel = value
            My.Settings.Save()

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

            logger.Info($"Le niveau de débogage a été modifié : {value}")
            NotifyPropertyChanged("SelectedDebugLevel")
        End Set
    End Property

    Public Property CtrlF9 As String
        Get
            Try
                Return My.Settings.CtrlF9
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF9 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.CtrlF9 = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF9")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF9 : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property CtrlF9Enabled As Boolean
        Get
            Try
                Return My.Settings.CtrlF9Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF9Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.CtrlF9Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF9Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF9Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property CtrlF10 As String
        Get
            Try
                Return My.Settings.CtrlF10
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF10 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.CtrlF10 = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF10")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF10 : {ex.Message}")
            End Try
        End Set
    End Property



    Public Property CtrlF10Enabled As Boolean
        Get
            Try
                Return My.Settings.CtrlF10Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF10Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.CtrlF10Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF10Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF10Enabled : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property CtrlF11 As String
        Get
            Try
                Return My.Settings.CtrlF11

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF11 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.CtrlF11 = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF11")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF11 : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property CtrlF11Enabled As Boolean
        Get
            Try
                Return My.Settings.CtrlF11Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF11Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.CtrlF11Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF11Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF11Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property CtrlF12 As String
        Get
            Try
                Return My.Settings.CtrlF12
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF12 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.CtrlF12 = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF12")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF12 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property CtrlF12Enabled As Boolean

        Get
            Try
                Return My.Settings.CtrlF12Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété CtrlF12Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.CtrlF12Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("CtrlF12Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété CtrlF12Enabled : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property ShiftF9 As String
        Get
            Try
                Return My.Settings.ShiftF9
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF9 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.ShiftF9 = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF9")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShifttF9 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF9Enabled As Boolean
        Get
            Try
                Return My.Settings.ShiftF9Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF9Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.ShiftF9Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF9Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF9Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF10 As String
        Get
            Try
                Return My.Settings.ShiftF10
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF10 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.ShiftF10 = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF10")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF10 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF10Enabled As Boolean
        Get
            Try
                Return My.Settings.ShiftF10Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF10Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.ShiftF10Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF10Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF10Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF11 As String
        Get
            Try
                Return My.Settings.ShiftF11
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF11 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.ShiftF11 = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF11")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF11 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF11Enabled As Boolean
        Get
            Try
                Return My.Settings.ShiftF11Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF11Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.ShiftF11Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF11Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF11Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ShiftF12 As String
        Get
            Try
                Return My.Settings.ShiftF12
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF12 : {ex.Message}")
                Return String.Empty
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.ShiftF12 = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF12")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF12 : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property ShiftF12Enabled As Boolean
        Get
            Try
                Return My.Settings.ShiftF12Enabled
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ShiftF12Enabled : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.ShiftF12Enabled = value
                My.Settings.Save()
                NotifyPropertyChanged("ShiftF12Enabled")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété ShiftF12Enabled : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property PlanningMode As Boolean
        Get
            Try
                Return My.Settings.PlanningMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété PlanningMode : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.PlanningMode = value
                My.Settings.Save()
                NotifyPropertyChanged("PlanningMode")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété PlanningMode : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property PlanningMode2 As Boolean
        Get
            Try
                Return My.Settings.PlanningMode2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété PlanningMode2 : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.PlanningMode2 = value
                My.Settings.Save()
                NotifyPropertyChanged("PlanningMode2")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété PlanningMode2 : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property PlanningName2 As String
        Get
            Try
                Return My.Settings.PlanningName2
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété PlanningName2 : {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As String)
            Try
                My.Settings.PlanningName2 = value
                My.Settings.Save()
                NotifyPropertyChanged("PlanningName2")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété PlanningName2 : {ex.Message}")
            End Try
        End Set
    End Property


    Public Property SecretaryMode As Boolean
        Get
            Try
                Return My.Settings.SecretaryMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété SecretaryMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.SecretaryMode = value
                My.Settings.Save()
                NotifyPropertyChanged("SecretaryMode")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété SecretaryMode : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property DoctorMode As Boolean
        Get
            Try
                Return My.Settings.DoctorMode
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété DoctorMode: {ex.Message}")
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            Try
                My.Settings.DoctorMode = value
                My.Settings.Save()
                NotifyPropertyChanged("DoctorMode")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété DoctorMode : {ex.Message}")
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
            Return My.Settings.RoomDisplay
        End Get
        Set(value As Boolean)
            Try
                My.Settings.RoomDisplay = value
                If value = True Then
                    My.Settings.RoomDisplayStr = "Visible"
                Else
                    My.Settings.RoomDisplayStr = "Collapsed"
                End If
                My.Settings.Save()
                NotifyPropertyChanged("RoomDisplay")
                NotifyPropertyChanged("RoomDisplayStr")
            Catch ex As Exception
                logger.Error($"Erreur lors de la modification de la propriété RoomDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property RoomDisplayStr As String
        Get
            Return My.Settings.RoomDisplayStr
        End Get
        Set(value As String)
            Try
                My.Settings.RoomDisplayStr = value
                NotifyPropertyChanged("RoomDisplayStr")
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

End Class


