﻿Imports System.Collections.ObjectModel
Imports MahApps.Metro.Controls.Dialogs
Imports EyeChat.Message
Imports EyeChat.User
Imports System.ComponentModel
Imports log4net
Imports log4net.Config
Imports Newtonsoft.Json.Linq
Imports EyeChat.Patient
Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text
Imports System.Web.UI.WebControls
Imports Newtonsoft.Json
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Interop
Imports MahApps.Metro.SimpleChildWindow
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports NuGet.Versioning
Imports System.Reflection
Imports System.IO.Compression
Imports EyeChat.PatientBubbleCtrl
Imports System.Net.NetworkInformation
Imports EyeChat.Computer
Imports EyeChat.Planning
Imports System.Windows.Threading
Imports System.Random


Public Class EggPhrasesData
    Public Property MarvinPhrases As List(Of String)
    Public Property JoyPhrases As List(Of String)
End Class


Class MainWindow
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Shared selectedUserName As String
    Public Shared Property Computers As ObservableCollection(Of Computer)

    'Liste des patients du RDC
    Public Shared Property PatientsRDC As ObservableCollection(Of Patient)
    'Liste des parients du 1er
    Public Shared Property Patients1er As ObservableCollection(Of Patient)
    'liste de tous les patients
    Public Shared Property PatientsALL As ObservableCollection(Of Patient)
    Public Shared Property ExamOptions As New List(Of ExamOption)()

    Public Shared Property speedMessages As New List(Of SpeedMessage)()

    Public Shared Property Plannings As New ObservableCollection(Of Planning)()
    Public Shared Property Messages As ObservableCollection(Of Message)
    Public Shared Property Users As ObservableCollection(Of User)
    Public Shared Property SelectedUserMessages As ObservableCollection(Of Message)
    Public Shared Property SelectedUser As String

    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public Property DebugLevels As New ObservableCollection(Of String)() From {"DEBUG", "INFO", "WARN", "ERROR"}

    Private timer As DispatcherTimer

    Delegate Sub AddReceivedMessage(ByRef message As String)
    Private Const port As Integer = 50545
    Private Const broadcastAddress As String = "255.255.255.255"
    Private receivingClient As UdpClient
    Public Shared sendingClient As UdpClient
    Private receivingThread As Thread
    Private Shared ReadOnly SuggestionValues As String() = {"/DEBUG", "/ENDDEBUG", "/LSTCOMPUTER", "/DISPCOMPUTER", "/TESTPATIENT", "/TESTSENDFILE", "/TESTMARVIN"}

    Dim jsonData As String
    Dim phrasesData As EggPhrasesData

    Private Const RepoOwner As String = "lapinus57"
    Private Const RepoName As String = "EyeChat"
    Private Const Version As String = "0.0.1" ' Remplacez par votre version actuelle



    Structure UserUpdateInfo
        Dim UserName As String
        Dim LastIdentifiantPC As String
    End Structure

    ' Stocker les informations de mise à jour pour chaque utilisateur
    Dim userUpdates As New Dictionary(Of String, UserUpdateInfo)()





#Region "Gestion Capture nom des fêntre"
    <DllImport("user32.dll")>
    Private Shared Function EnumWindows(ByVal lpEnumFunc As EnumWindowsDelegate, ByVal lParam As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowText(ByVal hWnd As IntPtr, ByVal lpString As StringBuilder, ByVal nMaxCount As Integer) As Integer
    End Function

    Private Delegate Function EnumWindowsDelegate(ByVal hWnd As IntPtr, ByVal lParam As IntPtr) As Boolean

#End Region

#Region "Gestion des raccourcis clavier"

    <DllImport("User32.dll")>
    Private Shared Function RegisterHotKey(<[In]> hWnd As IntPtr, <[In]> id As Integer, <[In]> fsModifiers As UInteger, <[In]> vk As UInteger) As Boolean
    End Function

    <DllImport("User32.dll")>
    Private Shared Function UnregisterHotKey(<[In]> hWnd As IntPtr, <[In]> id As Integer) As Boolean
    End Function


    Private _source As HwndSource
    Private Const HOTKEY_ID As Integer = 9000
    Private Const HOTKEY_ID1 As Integer = 9001
    Private Const HOTKEY_ID2 As Integer = 9002
    Private Const HOTKEY_ID3 As Integer = 9003
    Private Const HOTKEY_ID4 As Integer = 9004
    Private Const HOTKEY_ID5 As Integer = 9005
    Private Const HOTKEY_ID6 As Integer = 9006
    Private Const HOTKEY_ID7 As Integer = 9007
    Private Const HOTKEY_ID8 As Integer = 9008
    Private Const HOTKEY_ID9 As Integer = 9009


    Protected Overrides Sub OnSourceInitialized(e As EventArgs)
        MyBase.OnSourceInitialized(e)
        Dim helper = New WindowInteropHelper(Me)
        _source = HwndSource.FromHwnd(helper.Handle)
        _source.AddHook(AddressOf HwndHook)
        RegisterHotKey()
    End Sub

    Protected Overrides Sub OnClosed(e As EventArgs)
        _source.RemoveHook(AddressOf HwndHook)
        _source = Nothing
        UnregisterHotKey()
        MyBase.OnClosed(e)
    End Sub


    Private Sub RegisterHotKey()
        UnregisterHotKey() ' Désenregistrement de tous les raccourcis existants

        Dim helper = New WindowInteropHelper(Me)
        Const VK_E As UInteger = &H45
        Const MOD_CTRL As UInteger = &H2
        Const MOD_SHIFT As UInteger = &H4
        Const VK_F9 As UInteger = &H78
        Const VK_F10 As UInteger = &H79
        Const VK_F11 As UInteger = &H7A
        Const VK_F12 As UInteger = &H7B


        'RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_E)

        If My.Settings.CtrlF9Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID1, MOD_CTRL, VK_F9)
        End If
        If My.Settings.CtrlF10Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID2, MOD_CTRL, VK_F10)
        End If
        If My.Settings.CtrlF11Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID3, MOD_CTRL, VK_F11)
        End If
        If My.Settings.CtrlF12Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID4, MOD_CTRL, VK_F12)
        End If


        If My.Settings.ShiftF9Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID5, MOD_SHIFT, VK_F9)
        End If
        If My.Settings.ShiftF10Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID6, MOD_SHIFT, VK_F10)
        End If
        If My.Settings.ShiftF11Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID7, MOD_SHIFT, VK_F11)
        End If
        If My.Settings.ShiftF12Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID8, MOD_SHIFT, VK_F12)
        End If

    End Sub

    Private Sub UnregisterHotKey()
        Dim helper = New WindowInteropHelper(Me)
        UnregisterHotKey(helper.Handle, HOTKEY_ID)
        UnregisterHotKey(helper.Handle, HOTKEY_ID1)
        UnregisterHotKey(helper.Handle, HOTKEY_ID2)
        UnregisterHotKey(helper.Handle, HOTKEY_ID3)
        UnregisterHotKey(helper.Handle, HOTKEY_ID4)
        UnregisterHotKey(helper.Handle, HOTKEY_ID5)
        UnregisterHotKey(helper.Handle, HOTKEY_ID6)
        UnregisterHotKey(helper.Handle, HOTKEY_ID7)
        UnregisterHotKey(helper.Handle, HOTKEY_ID8)
    End Sub

    Private Function HwndHook(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, ByRef handled As Boolean) As IntPtr
        Const WM_HOTKEY As Integer = &H312
        Select Case msg
            Case WM_HOTKEY
                Select Case wParam.ToInt32()
                    Case HOTKEY_ID
                        OnHotKeyPressed0()
                    Case HOTKEY_ID1
                        OnHotKeyPressed1()
                    Case HOTKEY_ID2
                        OnHotKeyPressed2()
                    Case HOTKEY_ID3
                        OnHotKeyPressed3()
                    Case HOTKEY_ID4
                        OnHotKeyPressed4()
                    Case HOTKEY_ID5
                        OnHotKeyPressed5()
                    Case HOTKEY_ID6
                        OnHotKeyPressed6()
                    Case HOTKEY_ID7
                        OnHotKeyPressed7()
                    Case HOTKEY_ID8
                        OnHotKeyPressed8()
                        Exit Select
                End Select
                Exit Select
        End Select
        Return IntPtr.Zero
    End Function

    'Raccourci Ctrl + E
    Private Sub OnHotKeyPressed0()
        Me.WindowState = WindowState.Normal
        Me.Topmost = True
        Me.Topmost = False
        Me.Focus()
    End Sub

    'Raccourci Ctrl + F9
    Private Sub OnHotKeyPressed1()

        Try
            logger.Debug("Exécution du raccourci Ctrl + F9")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.CtrlF9, "RDC")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccourci Ctrl + F9 : " & ex.Message)
        End Try

    End Sub


    'Raccourci Ctrl + F10
    Private Sub OnHotKeyPressed2()

        Try
            logger.Debug("Exécution du raccourci Ctrl + F10")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.CtrlF10, "RDC")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccourci Ctrl + F10 : " & ex.Message)
        End Try

    End Sub

    'Raccourci Ctrl + F11
    Private Sub OnHotKeyPressed3()

        Try
            logger.Debug("Exécution du raccourci Ctrl + F11")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.CtrlF11, "RDC")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccourci Ctrl + F11 : " & ex.Message)
        End Try

    End Sub

    'Raccourci Ctrl + F12
    Private Sub OnHotKeyPressed4()

        Try
            logger.Debug("Exécution du raccourci Ctrl + F12")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.CtrlF12, "RDC")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccouci Ctrl + F12 : " & ex.Message)
        End Try

    End Sub

    'Raccourci Shift + F9
    Private Sub OnHotKeyPressed5()

        Try
            logger.Debug("Exécution du raccourci Shift + F9")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.ShiftF9, "1er")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccouci Shift + F9 : " & ex.ToString)
        End Try


    End Sub


    'Raccourci Shift + F10
    Private Sub OnHotKeyPressed6()

        Try
            logger.Debug("Exécution du raccourci Shift + F10")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.ShiftF10, "1er")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccouci Shift + F10 : " & ex.ToString)
        End Try

    End Sub

    'Raccourci Shift + F11
    Private Sub OnHotKeyPressed7()

        Try
            logger.Debug("Exécution du raccourci Shift + F11")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.ShiftF11, "1er")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccouci Shift + F11 : " & ex.ToString)
        End Try

    End Sub

    'Raccourci Shift + F12
    Private Sub OnHotKeyPressed8()

        Try
            logger.Debug("Exécution du raccourci Shift + F12")
            Me.WindowState = WindowState.Normal
            Me.Topmost = True
            Me.Topmost = False
            Me.Focus()
            OpenPatientDialogue("ODG", My.Settings.ShiftF12, "1er")
            Dim windowList As New List(Of String)()
            EnumWindows(AddressOf EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            logger.Error("Erreur lors de l'éxécution du raccouci Shift + F12 : " & ex.ToString)
        End Try

    End Sub




#End Region

#Region "Fênetre Patient Exams"

    Private Sub OpenPatientDialogue(ByVal Eye As String, ByVal ExamName As String, ByVal Floor As String)
        Me.CustomDialogBox.SetCurrentValue(ChildWindow.IsOpenProperty, True)

        ' Recherchez l'élément Eye dans la ComboBox
        Dim EyeToSelect As ComboBoxItem = FindComboBoxItemByContent(PatientEyeSelect, Eye)
        ' Sélectionnez l'élément Eye si trouvé
        If EyeToSelect IsNot Nothing Then
            PatientEyeSelect.SelectedItem = EyeToSelect
        End If

        ' Recherchez l'élément Floor dans la ComboBox
        Dim FloorToSelect As ComboBoxItem = FindComboBoxItemByContent(PatientFloorSelect, Floor)
        ' Sélectionnez l'élément Floor si trouvé
        If FloorToSelect IsNot Nothing Then
            PatientFloorSelect.SelectedItem = FloorToSelect
        End If

        Dim selectedExam As ExamOption = ExamOptions.FirstOrDefault(Function(exam) exam.Name = ExamName)
        If selectedExam IsNot Nothing Then
            PatientExamSelect.SelectedItem = selectedExam
        End If


    End Sub


    Private Function FindComboBoxItemByContent(ByVal comboBox As ComboBox, ByVal content As String) As ComboBoxItem
        For Each item As ComboBoxItem In comboBox.Items
            If item.Content IsNot Nothing AndAlso item.Content.ToString() = content Then
                Return item
            End If
        Next

        Return Nothing
    End Function
#End Region

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If IsNameInList(Users, "A Tous") Then
            SelectUser("A Tous")
            SelectUserByName("A Tous")
        End If

    End Sub



    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        'CheckForUpdates("beta")
        XmlConfigurator.Configure()
        logger.Logger.Repository.Threshold = log4net.Core.Level.Error
        Me.ShowCloseButton = False



        'test si les paramètres sont présent
        Try
            ' test si My.Settings.windowsName est vide
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
            If String.IsNullOrWhiteSpace(My.Settings.UniqueId) Then
                My.Settings.UniqueId = GenerateUniqueId()
                My.Settings.Id = GetUniqueIdHashCode()
                My.Settings.Save()
                logger.Info($"UniqueId : {My.Settings.UniqueId}")
            End If
        Catch ex As Exception
            logger.Error($"Erreur lors de la récupération du UniqueId : {ex.Message}")
        End Try


        'test si les dossier sont présent
        Filetest()

        ' Initialise la collection de messages
        Try
            Messages = If(LoadMessagesFromJson(), New ObservableCollection(Of Message)())
            logger.Info("Messages chargé avec succé")
        Catch ex As Exception
            logger.Error($"Erreur lors de l'initialisation de la collection des messages : {ex.Message}")
        End Try

        ' Initialise la collection des utilisateurs
        Try
            Users = If(LoadUsersFromJson(), New ObservableCollection(Of User)())
            If Users.Count = 0 Then
                Addusers("A Tous")
                Addusers("Secrétariat")
                SaveUsersToJson(Users)
            End If
        Catch ex As Exception
            logger.Error($"Erreur lors de l'initialisation de la collection des utilisateurs : {ex.Message}")
        End Try

        ' Initialise la collection des patients
        Try
            LoadPatientsFromJson()
        Catch ex As Exception
            logger.Error($"Erreur lors de l'nitialise la collection des patients : {ex.Message}")
        End Try

        ' Initialise la collection des ordinateurs 
        Try
            LoadComputersFromJson()
        Catch ex As Exception
            logger.Error($"Erreur lors de l'nitialise la collection des ordinateurs : {ex.Message}")
        End Try

        ' Initialise la collection du planning
        Try
            LoadPlanningFromJson()
        Catch ex As Exception
            logger.Error($"Erreur lors de l'nitialise la collection du planning : {ex.Message}")
        End Try


        ' Initialisez la collection de messages selectionné
        SelectedUserMessages = New ObservableCollection(Of Message)()


        ' Définissez le DataContext sur la fenêtre elle-même
        Me.DataContext = Me
        Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "avataaars.png")


        'SaveUsersToJson(Users)

        InitializeSender()
        InitializeReceiver()

        'SavePatientsToJson(Patients)
        'SelectUserList("Benoit")
        'SelectUserList("benoit")

        'PatientAdd("Mr", "muller", "benoit", "SK", "od", "RDC", "Blue", "benoit", Date.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"))
        'PatientAdd("Mr", "durand", "benoit", "SK", Nothing, "1er", "Green", "benoit", Date.Now)

        jsonData = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "dataphrases.json"))
        phrasesData = JsonConvert.DeserializeObject(Of EggPhrasesData)(jsonData)


        'Charge les options d'examen à partir du fichier JSON
        loadExamOption()


        MahApps.Metro.Controls.HeaderedControlHelper.SetHeaderFontSize(PatientTabCtrl, CInt(My.Settings.AppSizeDisplay))


        ' Charger les messages à partir du fichier JSON
        loadSpeedMessage()

        ' Créer un ContextMenu pour la TextBox
        Dim contextMenu As New ContextMenu()

        ' Ajouter les messages au menu contextuel
        For Each message In speedMessages
            Dim menuItem As New System.Windows.Controls.MenuItem()
            menuItem.Header = message.Title
            menuItem.Tag = message
            AddHandler menuItem.Click, AddressOf MessageMenuItem_Click
            contextMenu.Items.Add(menuItem)
        Next

        ' Affecter le ContextMenu à la TextBox
        SendTextBox.ContextMenu = contextMenu



        If My.Settings.PlanningMode2 = True Then

            ' Créez et configurez le DispatcherTimer
            timer = New DispatcherTimer()

            ' Vérifiez si l'heure actuelle est antérieure à 8h00
            Dim currentTime As DateTime = DateTime.Now
            Dim targetTime As New DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 0, 0)

            If currentTime < targetTime Then
                ' Si l'heure actuelle est antérieure à 8h00, calculez l'intervalle
                Dim timeUntilTarget As TimeSpan = targetTime - currentTime

                ' Définissez l'intervalle de la minuterie pour correspondre au temps restant
                timer.Interval = timeUntilTarget

                ' Gérez l'événement Tick
                AddHandler timer.Tick, AddressOf Timer_Tick

                My.Settings.UserName = My.Settings.PlanningName2
                My.Settings.Save()

                ' Démarrez la minuterie
                timer.Start()
            Else
                ' L'heure actuelle est postérieure à 8h00, vous pouvez exécuter l'action immédiatement ici
                If My.Settings.PlanningMode = True Then
                    Try
                        Dim dayTranslations As New Dictionary(Of String, String)() From {
                    {"Monday", "Lundi"},
                    {"Tuesday", "Mardi"},
                    {"Wednesday", "Mercredi"},
                    {"Thursday", "Jeudi"},
                    {"Friday", "Vendredi"},
                    {"Saturday", "Samedi"},
                    {"Sunday", "Dimanche"}
        }

                        Dim today = DateTime.Now.DayOfWeek.ToString() ' Récupère le jour actuel de la semaine en anglais
                        Dim frenchDay As String = dayTranslations(today) ' Traduction française du jour

                        ' Recherche le planning correspondant au jour en cours dans la liste des plannings
                        Dim planningForToday = Plannings.FirstOrDefault(Function(p) p.Day = frenchDay)

                        ' Mise à jour du nom d'utilisateur dans les paramètres
                        If planningForToday IsNot Nothing AndAlso planningForToday.User IsNot Nothing Then
                            My.Settings.UserName = planningForToday.User
                            My.Settings.Save()
                        Else
                            My.Settings.UserName = "Benoit"
                            My.Settings.Save()
                        End If
                    Catch ex As Exception
                        My.Settings.UserName = "Benoit"
                        My.Settings.Save()
                    End Try

                End If
            End If
        End If

        SendMessage("USR01" & My.Settings.UserName & "|" & Environment.UserName)
    End Sub

    Private Sub Timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        ' Code à exécuter à 8h00 précises
        SendMessage("USR02" & My.Settings.UserName & "|" & Environment.UserName)

        If My.Settings.PlanningMode = True Then
            Try
                Dim dayTranslations As New Dictionary(Of String, String)() From {
                {"Monday", "Lundi"},
                {"Tuesday", "Mardi"},
                {"Wednesday", "Mercredi"},
                {"Thursday", "Jeudi"},
                {"Friday", "Vendredi"},
                {"Saturday", "Samedi"},
                {"Sunday", "Dimanche"}
    }

                Dim today = DateTime.Now.DayOfWeek.ToString() ' Récupère le jour actuel de la semaine en anglais
                Dim frenchDay As String = dayTranslations(today) ' Traduction française du jour

                ' Recherche le planning correspondant au jour en cours dans la liste des plannings
                Dim planningForToday = Plannings.FirstOrDefault(Function(p) p.Day = frenchDay)

                ' Mise à jour du nom d'utilisateur dans les paramètres
                If planningForToday IsNot Nothing AndAlso planningForToday.User IsNot Nothing Then
                    My.Settings.UserName = planningForToday.User
                    My.Settings.Save()
                Else
                    My.Settings.UserName = "Benoit"
                    My.Settings.Save()
                End If
            Catch ex As Exception
                My.Settings.UserName = "Benoit"
                My.Settings.Save()
            End Try

        End If

        SendMessage("USR01" & My.Settings.UserName & "|" & Environment.UserName)


        ' Arrêtez la minuterie si nécessaire pour éviter d'exécuter l'action à plusieurs reprises
        timer.Stop()
    End Sub

    Private Sub MessageMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedMessage As SpeedMessage = DirectCast(DirectCast(sender, System.Windows.Controls.MenuItem).Tag, SpeedMessage)
        '"SMF01UserName|Destinataitre|message|Option1|Option2|Option3"
        SendMessage("SMF01" & My.Settings.UserName & "|" & selectedMessage.Destinataire & "|" & selectedMessage.Message.Replace("[ROOM]", Environment.UserName) & "|" & selectedMessage.Options)
        SendMessage("MSG01" & My.Settings.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & selectedMessage.Destinataire & " " & selectedMessage.Message.Replace("[ROOM]", Environment.UserName) & "|benoit.png")



    End Sub


    Private Async Function LaunchGitHubSiteAsync() As Task
        ' Exemple : Ouvrir le site GitHub dans le navigateur par défaut
        Dim url As String = "https://github.com/lapinus57/EyeChat"
        Process.Start(url)
        Await ShowMessageAsync("This is the title", "Some message")

    End Function


    Public Function GenerateUniqueId() As String

        ' Génération d'un identifiant unique basé sur le nom d'utilisateur de Windows et le nom du PC
        Dim windowsUser As String = Environment.UserName
        Dim computerName As String = Environment.MachineName

        Return $"{windowsUser}_{computerName}"
    End Function


    Public Function GetUniqueIdHashCode() As Integer
        ' Génération d'un integer unique basé sur le UniqueId
        Return My.Settings.UniqueId.GetHashCode()
    End Function

    'Chargement des listes de patients 
    Private Sub LoadPatientsFromJson()
        Dim allPatients As ObservableCollection(Of Patient) = Patient.LoadPatientsFromJson()

        If allPatients IsNot Nothing Then
            ' Filtrer les patients pour chaque étage et les ajouter aux listes correspondantes
            PatientsRDC = New ObservableCollection(Of Patient)(allPatients.Where(Function(p) p.Position = "RDC"))
            Patients1er = New ObservableCollection(Of Patient)(allPatients.Where(Function(p) p.Position = "1er"))
            PatientsALL = allPatients
        Else
            ' Si le fichier JSON n'existe pas ou est vide, initialiser les listes comme vides
            PatientsRDC = New ObservableCollection(Of Patient)()
            Patients1er = New ObservableCollection(Of Patient)()
            PatientsALL = New ObservableCollection(Of Patient)()
        End If
    End Sub


    ' Méthode pour ajouter un nouveau message
    Public Sub AddMessage(ByVal name As String, ByVal sender As String, ByVal room As String, ByVal content As String, ByVal isAlignedRight As Boolean, ByVal avatar As String)
        If Messages IsNot Nothing Then
            Messages.Add(New Message With {.name = name, .sender = sender, .room = room, .content = content, .isAlignedRight = isAlignedRight, .Timestamp = DateTime.Now, .avatar = avatar})
            SaveMessagesToJson(Messages)
        End If
    End Sub

    Public Sub Addusers(ByVal UserName As String)
        Users.Add(New User With {.Name = UserName})
        SaveUsersToJson(Users)
    End Sub



    ' Méthode pour sélectionner un utilisateur et afficher uniquement ses messages
    Public Shared Sub SelectUser(ByVal name As String)
        SelectedUserMessages.Clear()
        If Messages IsNot Nothing Then ' Vérifiez si la collection Messages est non nulle
            ' Utilisez LINQ pour filtrer les messages de l'utilisateur sélectionné
            Dim userMessages = Messages.Where(Function(m) m.Name = name)
            ' Ajoutez chaque message filtré à la nouvelle ObservableCollection
            For Each message In userMessages
                SelectedUserMessages.Add(message)
            Next
        End If
    End Sub







    Public Sub Updatemsglist()
        SelectUser(SelectedUser)
        MahApps.Metro.Controls.HeaderedControlHelper.SetHeaderFontSize(PatientTabCtrl, CInt(My.Settings.AppSizeDisplay))
    End Sub


#Region "Gestion de l'ajout, suppression et modification d'un patient"

    Public Sub PatientAdd(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Annotation As String, ByVal Position As String, ByVal Examinator As String, ByVal Hold_Time As String)

        ' Obtenir la couleur en fonction du type d'examen du patient
        Dim examType As String = Exams ' Remplacez cela par le type d'examen réel du patient
        Dim examOption As ExamOption = ExamOptions.FirstOrDefault(Function(ExamOptions) ExamOptions.Name = examType)
        If examOption IsNot Nothing Then
            Dim patientColor As String = examOption.Color
            ' Utilisez la couleur pour mettre à jour la propriété Colors du patient
            Dim newPatient As New Patient With {.Title = Title, .LastName = LastName, .FirstName = FirstName, .Exams = Exams, .Annotation = Annotation, .Position = Position, .Hold_Time = Hold_Time, .IsTaken = False, .Colors = patientColor, .Examinator = Examinator}
            PatientsALL.Add(newPatient)
            SavePatientsToJson(PatientsALL)

            If newPatient.Position = "RDC" Then
                PatientsRDC.Add(newPatient)
            ElseIf newPatient.Position = "1er" Then
                Patients1er.Add(newPatient)
            End If
        End If
        UpdateList()
        SavePatientsToJson(PatientsALL)

    End Sub


    Public Sub PatientRemove(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal Hold_Time As String)

        Dim patientToRemove As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = Hold_Time)
        If patientToRemove IsNot Nothing Then

            'SendTextBox.Text = patientToRemove.ToString
            ' Supprimer le patient de PatientsALL
            PatientsALL.Remove(patientToRemove)

            ' Si le patient est dans PatientsRDC, le supprimer également
            If patientToRemove.Position = "RDC" Then
                PatientsRDC.Remove(patientToRemove)
            ElseIf patientToRemove.Position = "1er" Then
                Patients1er.Remove(patientToRemove)
            End If

            ' Si le patient est dans Patients1er, le supprimer également
            If Patients1er.Contains(patientToRemove) Then
                Patients1er.Remove(patientToRemove)
            End If
        End If
        UpdateList()
        SavePatientsToJson(PatientsALL)
    End Sub

    Public Sub PatientUpdate(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal OldHold_Time As String, ByVal NewHold_Time As String)
        Dim oldHoldTimeDateTime As DateTime
        Dim newHoldTimeDateTime As DateTime

        If DateTime.TryParseExact(OldHold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, oldHoldTimeDateTime) AndAlso
       DateTime.TryParseExact(NewHold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, newHoldTimeDateTime) Then

            Dim patientToUpdate As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = oldHoldTimeDateTime)

            If patientToUpdate IsNot Nothing Then
                ' Retirez le patient de la liste actuelle
                PatientsALL.Remove(patientToUpdate)

                ' Si le patient est dans PatientsRDC, le supprimer également
                If patientToUpdate.Position = "RDC" Then
                    PatientsRDC.Remove(patientToUpdate)
                ElseIf patientToUpdate.Position = "1er" Then
                    Patients1er.Remove(patientToUpdate)
                End If

                ' Mise à jour des propriétés du patient
                patientToUpdate.Hold_Time = newHoldTimeDateTime
                ' Vous pouvez également mettre à jour d'autres propriétés si nécessaire

                ' Ajoutez le patient mis à jour aux listes appropriées
                PatientsALL.Add(patientToUpdate)
                If patientToUpdate.Position = "RDC" Then
                    PatientsRDC.Add(patientToUpdate)
                ElseIf patientToUpdate.Position = "1er" Then
                    Patients1er.Add(patientToUpdate)
                End If
            End If

            SavePatientsToJson(PatientsALL)
            ' Effacez les listes existantes pour préparer la mise à jour
            UpdateList()
            SavePatientsToJson(PatientsALL)


        End If
    End Sub

    Public Sub PatientCheckPass(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal Hold_Time As String, ByVal OperatorName As String)
        Dim HoldTimeDateTime As DateTime

        DateTime.TryParseExact(Hold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, HoldTimeDateTime)

        Dim patientToUpdate As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = HoldTimeDateTime)

        If patientToUpdate IsNot Nothing Then
            ' Retirez le patient de la liste actuelle
            PatientsALL.Remove(patientToUpdate)

            ' Si le patient est dans PatientsRDC, le supprimer également
            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Remove(patientToUpdate)
            End If
            If patientToUpdate.Position = "1er" Then
                Patients1er.Remove(patientToUpdate)
            End If

            ' Mise à jour des propriétés du patient
            patientToUpdate.IsTaken = True
            patientToUpdate.OperatorName = OperatorName
            patientToUpdate.Colors = "gray"
            patientToUpdate.Pick_up_Time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
            patientToUpdate.Time_Order = patientToUpdate.Pick_up_Time - patientToUpdate.Hold_Time

            PatientsALL.Add(patientToUpdate)

            ' Vous pouvez également mettre à jour d'autres propriétés si nécessaire

            ' Ajoutez le patient mis à jour aux listes appropriées

            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Add(patientToUpdate)
            ElseIf patientToUpdate.Position = "1er" Then
                Patients1er.Add(patientToUpdate)
            End If



            SavePatientsToJson(PatientsALL)
            UpdateList()
            SavePatientsToJson(PatientsALL)
        End If

    End Sub

    Public Sub PatientUndoPass(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal Hold_Time As String, ByVal OperatorName As String)
        Dim HoldTimeDateTime As DateTime

        DateTime.TryParseExact(Hold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, HoldTimeDateTime)

        Dim patientToUpdate As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = HoldTimeDateTime)

        If patientToUpdate IsNot Nothing Then
            ' Retirez le patient de la liste actuelle
            PatientsALL.Remove(patientToUpdate)

            ' Si le patient est dans PatientsRDC, le supprimer également
            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Remove(patientToUpdate)
            ElseIf patientToUpdate.Position = "1er" Then
                Patients1er.Remove(patientToUpdate)
            End If

            ' Mise à jour des propriétés du patient
            ' Obtenir la couleur en fonction du type d'examen du patient
            Dim examType As String = patientToUpdate.Exams ' Remplacez cela par le type d'examen réel du patient
            Dim examOption As ExamOption = ExamOptions.FirstOrDefault(Function(ExamOptions) ExamOptions.Name = examType)

            If examOption IsNot Nothing Then
                Dim patientColor As String = examOption.Color
                ' Utilisez la couleur pour mettre à jour la propriété Colors du patient
                patientToUpdate.Colors = patientColor
            End If

            patientToUpdate.IsTaken = False
            patientToUpdate.OperatorName = Nothing
            patientToUpdate.Pick_up_Time = Nothing
            patientToUpdate.Time_Order = Nothing

            ' Vous pouvez également mettre à jour d'autres propriétés si nécessaire

            ' Ajoutez le patient mis à jour aux listes appropriées

            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Add(patientToUpdate)
                PatientsALL.Add(patientToUpdate)
            ElseIf patientToUpdate.Position = "1er" Then
                Patients1er.Add(patientToUpdate)
                PatientsALL.Add(patientToUpdate)
            End If
            SavePatientsToJson(PatientsALL)
            UpdateList()
            SavePatientsToJson(PatientsALL)
        End If


    End Sub


    Public Sub ModifyPatient(ByVal lastName As String, ByVal updatedPatient As Patient)
        For index As Integer = 0 To PatientsALL.Count - 1
            If PatientsALL(index).LastName = lastName Then
                PatientsALL(index) = updatedPatient
                Exit For
            End If
        Next
    End Sub




#End Region

#Region "Gestion UDP et messages"
    Private Sub InitializeSender()
        sendingClient = New UdpClient(broadcastAddress, port) With {
            .EnableBroadcast = True
        }
    End Sub

    Private Sub InitializeReceiver()
        Dim receiveThread As New Thread(AddressOf ReceiveMessages) With {
            .IsBackground = True
        }
        receiveThread.Start()
    End Sub

    Private Sub ReceiveMessages()
        receivingClient = New UdpClient(port)
        Dim endPoint As New IPEndPoint(IPAddress.Any, port)

        While True
            Dim receiveBytes As Byte() = receivingClient.Receive(endPoint)
            Dim receivedMessage As String = Encoding.UTF8.GetString(receiveBytes)

            ' Utilisation de Dispatcher.Invoke pour exécuter la méthode MessageReceived sur le thread de l'interface utilisateur
            Dispatcher.Invoke(Sub() MessageReceived(receivedMessage))
        End While
    End Sub

    Private Sub MessageReceived(ByVal receivedMessage As String)
        ' Traitez le message reçu en fonction du code
        Dim messageCode As String = receivedMessage.Substring(0, 5)



        Select Case messageCode
            Case "USR01"
                ' Connexion d'un utilisateur
                ' "USR01|UserName|IdentifiantPC"
                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    logger.Error("Réception d'un message utilisateur")

                    If parts.Length >= 2 Then
                        Dim UserName As String = parts(0)
                        Dim IdentifiantPC As String = parts(1)

                        Dim userToUpdate As User = Users.FirstOrDefault(Function(user) user.Name = UserName)
                        logger.Error("Réception d'un message utilisateur " & UserName & " avec comme poste " & IdentifiantPC)

                        If userToUpdate Is Nothing Then
                            ' Ajouter l'utilisateur s'il n'existe pas
                            Addusers(UserName)
                            userToUpdate = Users.FirstOrDefault(Function(user) user.Name = UserName)
                            Try
                                Dim localIPAddress As IPAddress = GetLocalIPAddress()
                                SendMessageWithCode("DBG02", My.Settings.UniqueId & "|" & Environment.UserName & "|" & localIPAddress.ToString)
                                logger.Debug("Envoi de l'ID du PC aux autres applications")
                            Catch ex As Exception
                                logger.Error("Erreur lors de l'envoi de l'ID du PC aux autres applications : " & ex.Message)
                            End Try
                        End If

                        If userToUpdate.Status = "Offline" Then
                            ' Mettre à jour le statut de l'utilisateur
                            userToUpdate.Status = IdentifiantPC
                            logger.Error("Utilisateur mis à jour avec un seul identifiant")
                            Try
                                Dim localIPAddress As IPAddress = GetLocalIPAddress()
                                SendMessageWithCode("DBG02", My.Settings.UniqueId & "|" & Environment.UserName & "|" & localIPAddress.ToString)
                                logger.Debug("Envoi de l'ID du PC aux autres applications")
                            Catch ex As Exception
                                logger.Error("Erreur lors de l'envoi de l'ID du PC aux autres applications : " & ex.Message)
                            End Try
                        ElseIf Not userToUpdate.Status.Contains(IdentifiantPC) Then
                            ' Mettre à jour le statut de l'utilisateur avec un nouveau poste
                            userToUpdate.Status += " | " & IdentifiantPC
                            logger.Error("Utilisateur mis à jour avec plusieurs postes")
                            Try
                                Dim localIPAddress As IPAddress = GetLocalIPAddress()
                                SendMessageWithCode("DBG02", My.Settings.UniqueId & "|" & Environment.UserName & "|" & localIPAddress.ToString)
                                logger.Debug("Envoi de l'ID du PC aux autres applications")
                            Catch ex As Exception
                                logger.Error("Erreur lors de l'envoi de l'ID du PC aux autres applications : " & ex.Message)
                            End Try
                        Else
                            logger.Error("Rien à faire, utilisateur déjà mis à jour")
                            Exit Sub ' Éviter la boucle infinie
                        End If

                        ' Envoyer un message de confirmation à l'utilisateur
                        SendMessage("USR01" & My.Settings.UserName & "|" & Environment.UserName)

                        ' Sauvegarder les modifications et mettre à jour l'interface utilisateur
                        SaveUsersToJson(Users)
                        Users = LoadUsersFromJson()
                        ListUseres.ItemsSource = Users
                    Else
                        ' Gérer le cas où le message n'a pas le bon nombre de parties
                    End If
                Catch ex As Exception
                    ' Gérer l'exception liée au traitement du message d'ajout d'utilisateur
                End Try



            Case "USR02"
                ' Déconnexion d'un utilisateur
                ' "USR02UserName|IdentifiantPC"
                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)

                    If parts.Length >= 2 Then
                        Dim UserName As String = parts(0)
                        Dim IdentifiantPC As String = parts(1)

                        Dim userToUpdate As User = Users.FirstOrDefault(Function(user) user.Name = UserName)

                        If userToUpdate IsNot Nothing Then
                            If userToUpdate.Status = IdentifiantPC Then
                                ' Mettre à zéro le statut d'utilisateur
                                userToUpdate.Status = "Offline"
                            ElseIf userToUpdate.Status.Contains("|") Then
                                ' Diviser le statut en parties distinctes en utilisant le séparateur "|"
                                Dim statusParts As String() = userToUpdate.Status.Split("|"c)

                                ' Réorganiser les parties du statut en supprimant les occurrences de "IdentifiantPC"
                                userToUpdate.Status = ""
                                For Each part As String In statusParts
                                    If part <> IdentifiantPC Then
                                        ' Ajouter la partie au statut sauf si c'est "IdentifiantPC"
                                        If userToUpdate.Status <> "" Then
                                            userToUpdate.Status &= " | "
                                        End If
                                        userToUpdate.Status &= part
                                    End If
                                Next
                            End If

                            ' Sauvegarder les modifications et mettre à jour l'interface utilisateur
                            SaveUsersToJson(Users)
                            Users = LoadUsersFromJson()
                            ListUseres.ItemsSource = Users
                        End If
                    Else
                        ' Gérer le cas où le message n'a pas le bon nombre de parties
                    End If
                Catch ex As Exception
                    ' Gérer l'exception liée au traitement du message de déconnexion d'utilisateur
                End Try

            Case "USR03"
                ' Code suppresion d'un user
                '"USR01|UserName Ancien|UserName new"
            Case "USR04"
                 ' Code message pour le changement de coleur
                '"USR01|UserName|NewColor"

            Case "USR11"
                ' Code message

            Case "SMF01"
                'Reception d'un SpeedMessage envoyer 
                ' exemple d'une message reçu "SMF01UserName|Destinataitre|message|Option1|Option2|Option3"
                Try
                    ' Extraire le contenu du message à partir de la position 5 pour ignorer le code
                    Dim messageContent As String = receivedMessage.Substring(5)

                    ' Diviser le contenu du message en parties en utilisant le caractère '|'
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim UserSend As String = parts(0)
                    Dim Destinataire As String = parts(1)
                    Dim Message As String = parts(2)
                    Dim Option1 As String = parts(3)
                    Dim Option2 As String = parts(4)
                    Dim Option3 As String = parts(5)

                    If Destinataire = My.Settings.UserName Then

                        Me.WindowState = WindowState.Normal
                        Me.Topmost = True
                        Me.Topmost = False
                        Me.Focus()
                        SpeedMessageDialog(UserSend, UserSend & Message, Option1, Option2, Option3)
                    End If

                Catch ex As Exception
                    logger.Error("Erreur de réception d'un message SpeedMessage SMF01 " & ex.Message)
                End Try


            Case "PTN01"
                ' Code de message pour ajouter un patient 
                ' exemple de message "PTN01Mr|BENOIT|Muller|FO|ODG |RDC|Benoit|2023-08-25T16:33:30.496"
                Try
                    ' Extraire le contenu du message à partir de la position 5 pour ignorer le code
                    Dim messageContent As String = receivedMessage.Substring(5)

                    ' Diviser le contenu du message en parties en utilisant le caractère '|'
                    Dim parts As String() = messageContent.Split("|"c)

                    ' Extraire les informations du patient à partir des parties
                    Dim Title As String = parts(0)
                    Dim LastName As String = parts(1)
                    Dim FirstName As String = parts(2)
                    Dim Exam As String = parts(3)
                    Dim Comments As String = parts(4)
                    Dim Floor As String = parts(5)
                    Dim Examinator As String = parts(6)
                    Dim Hold_Time As String = parts(7)

                    ' Appeler la fonction PatientAdd pour ajouter le patient avec les informations extraites
                    PatientAdd(Title, LastName, FirstName, Exam, Comments, Floor, Examinator, Hold_Time)

                Catch ex As Exception
                    ' Gérer toute exception qui pourrait survenir lors du traitement du message
                    logger.Error("Erreur de réception d'un message PatientAdd PTN01 " & ex.Message)
                End Try


            Case "PTN02"
                ' Code de message pour la mise à jour d'un patient                        
                ' "PTN02Titre|Nom|Prénom|Exams|Comments|Floor|Examinator|OperatorName"
                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim Title As String = parts(0)
                    Dim LastName As String = parts(1)
                    Dim FirstName As String = parts(2)
                    Dim Exam As String = parts(3)
                    Dim Comments As String = parts(4)
                    Dim Floor As String = parts(5)
                    Dim Examinator As String = parts(6)
                    Dim Hold_Time As String = parts(7)
                    Dim OperatorName As String = parts(8)

                    PatientCheckPass(Title, LastName, FirstName, Exam, Comments, Floor, Examinator, Hold_Time, OperatorName)
                Catch ex As Exception
                    ' Gérer toute exception qui pourrait survenir lors du traitement du message
                    logger.Error("Erreur de réception d'un message PatientCheckPass PTN02 " & ex.Message)
                End Try


            Case "PTN03"
                ' Code de message pour supprimer un patient               
                ' "PTN03Titre|Nom|Prénom|Exams|Comments|Floor|Examinator|Hold_Time"
                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim Title As String = parts(0)
                    Dim LastName As String = parts(1)
                    Dim FirstName As String = parts(2)
                    Dim Exam As String = parts(3)
                    Dim Comments As String = parts(4)
                    Dim Floor As String = parts(5)
                    Dim Examinator As String = parts(6)
                    Dim Hold_Time As String = parts(7)

                    PatientRemove(Title, LastName, FirstName, Exam, Comments, Floor, Examinator, Hold_Time)
                Catch ex As Exception
                    ' Gérer toute exception qui pourrait survenir lors du traitement du message
                    logger.Error("Erreur de réception d'un message PatientRemove PTN03 " & ex.Message)
                End Try

            Case "PTN04"
                ' Code de message pour supprimer un patient               
                ' "PTN04Titre|Nom|Prénom|Exams|Comments|Floor|Examinator|OldHold_Time|NewHold_Time"

                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim Title As String = parts(0)
                    Dim LastName As String = parts(1)
                    Dim FirstName As String = parts(2)
                    Dim Exam As String = parts(3)
                    Dim Comments As String = parts(4)
                    Dim Floor As String = parts(5)
                    Dim Examinator As String = parts(6)
                    Dim OldHold_Time As String = parts(7)
                    Dim NewHold_Time As String = parts(8)


                    PatientUpdate(Title, LastName, FirstName, Exam, Comments, Floor, Examinator, OldHold_Time, NewHold_Time)

                Catch ex As Exception
                    ' Gérer toute exception qui pourrait survenir lors du traitement du message
                    logger.Error("Erreur de réception d'un message PatientUpdate PTN04 " & ex.Message)

                End Try

            Case "PTN05"
                ' Code de message pour la mise à jour d'un patient                        
                ' "PTN05Titre|Nom|Prénom|Exams|Comments|Floor|Examinator|OperatorName"
                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim Title As String = parts(0)
                    Dim LastName As String = parts(1)
                    Dim FirstName As String = parts(2)
                    Dim Exam As String = parts(3)
                    Dim Comments As String = parts(4)
                    Dim Floor As String = parts(5)
                    Dim Examinator As String = parts(6)
                    Dim Hold_Time As String = parts(7)
                    Dim OperatorName As String = parts(8)

                    PatientUndoPass(Title, LastName, FirstName, Exam, Comments, Floor, Examinator, Hold_Time, OperatorName)
                    logger.Debug("Réception d'un message PatientUndoPass PTN05")
                Catch ex As Exception
                    logger.Error("Erreur de réception d'un message PatientUndoPass PTN05 " & ex.Message)
                End Try


            Case "SYS01"
                ' Code pour fermer la boite à distance
                ' Exemple du message envoyé : "SYS01|IdentifiantPC(cible)|IdentifiantPC(auteur)"

                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim targetPC As String = parts(0)
                    Dim authorPC As String = parts(1)

                    ' Vérification de l'identifiant PC de la cible
                    If targetPC = My.Settings.UniqueId Then
                        logger.Debug("Réception d'un message SYS01 pour fermeture à distance.")
                        SaveMessagesToJson(Messages)
                        SaveUsersToJson(Users)
                        Me.Close() ' Fermer la fenêtre
                    End If
                Catch ex As Exception
                    ' Gestion de l'exception
                    logger.Error("Une erreur s'est produite lors du traitement du message SYS01 : " & ex.Message)
                End Try


            Case "SYS02"
                ' Code pour déconnecter l'utilisateur à distance
                ' Exemple du message envoyé : "SYS02|IdentifiantPC(cible)|IdentifiantPC(auteur)"

                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim targetPC As String = parts(0)
                    Dim authorPC As String = parts(1)
                    If targetPC = My.Settings.UniqueId Then
                        logger.Debug("Réception d'un message SYS02 pour déconnexion à distance.")
                        SaveMessagesToJson(Messages)
                        SaveUsersToJson(Users)
                        SelectedUserMessages.Clear()
                        Users.Clear()
                    End If
                Catch ex As Exception
                    logger.Error("Erreur lors de la déconnexion à distance : " & ex.Message)
                End Try

            Case "SYS20"
                'code pour mettre le client en reception de fichier
                'Exemple du message envoyé : "SYS20ComputerID(cible)|ComputerID(auteur)|Folder|FileName"

                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim targetPC As String = parts(0)
                    Dim authorPC As String = parts(1)
                    Dim Folder As String = parts(2)
                    Dim FileName As String = parts(3)
                    If targetPC = My.Settings.UniqueId Then
                        logger.Debug("Réception d'un message SYS20 pour mise en réception de fichier.")
                        If targetPC = My.Settings.UniqueId And authorPC <> My.Settings.UniqueId Then
                            ' Mettre le client en réception de fichier
                            Dim receiver As New FileReceiver()
                            Dim savePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Folder, FileName)
                            ' Remplacez par l'emplacement où vous souhaitez sauvegarder le fichier
                            Dim port As Integer = 12345 ' Remplacez par le même port que celui utilisé pour l'envoi
                            'SendMessage("SYS21" & authorPC & "|" & targetPC & "|" & Folder & "|" & FileName & "|" & port)

                            receiver.ReceiveFile(savePath, port)
                        End If
                    End If

                Catch ex As Exception
                    logger.Error("Erreur lors de la mise en réception de fichier :  " & ex.Message)
                End Try


                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim targetPC As String = parts(0)
                    Dim authorPC As String = parts(1)
                    Dim Folder As String = parts(2)
                    Dim FileName As String = parts(3)
                    If targetPC = My.Settings.UniqueId Then
                        logger.Debug("Réception d'un message SYS20 pour mise en réception de fichier.")
                        If targetPC = My.Settings.UniqueId And authorPC <> My.Settings.UniqueId Then
                            ' Mettre le client en réception de fichier
                            Dim receiver As New FileReceiver()
                            Dim savePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Folder, FileName)
                            ' Remplacez par l'emplacement où vous souhaitez sauvegarder le fichier
                            Dim port As Integer = 12345 ' Remplacez par le même port que celui utilisé pour l'envoi
                            'SendMessage("SYS21" & authorPC & "|" & targetPC & "|" & Folder & "|" & FileName & "|" & port)

                            receiver.ReceiveFile(savePath, port)
                        End If
                    End If

                Catch ex As Exception
                    logger.Error("Erreur lors de la mise en réception de fichier :  " & ex.Message)
                End Try



            Case "MSG01"

                'Ajouter un message
                'Exemple du message envoyé : "MSG01{My.Settings.UserName}|{selectedUser.Name}|{Message}|{Avatar}"
                Try

                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim author As String = parts(0)
                    Dim destinataire As String = parts(1)
                    Dim room As String = parts(2)
                    Dim message As String = parts(3)
                    Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", parts(4))


                    Dim messageAdded As Boolean = False ' Variable pour suivre si un message a été ajouté

                    Select Case destinataire
                        Case "A Tous"
                            If author = My.Settings.UserName Then
                                ' Auteur envoie un message "A Tous"
                                AddMessage("A Tous", author, room, message, True, avatarPath)
                                SelectUserByName("A Tous")
                                messageAdded = True
                            Else
                                ' Autre utilisateur envoie un message "A Tous"
                                AddMessage("A Tous", author, room, message, False, avatarPath)
                                SelectUserByName("A Tous")
                                messageAdded = True
                            End If

                        Case "Secrétariat"
                            If author = My.Settings.UserName Then
                                ' Auteur envoie un message à "Secrétaria"
                                AddMessage("Secrétariat", author, room, message, True, avatarPath)
                                SelectUserByName("Secrétariat")
                                messageAdded = True
                            End If

                            If My.Settings.SecretaryMode = True Then
                                ' Secrétaria mode est activé
                                AddMessage("Secrétariat", author, room, message, False, avatarPath)
                                AddMessage(author, author, room, message, False, avatarPath)
                                SelectUserByName(author)
                                messageAdded = True
                            End If

                        Case Else
                            If author = My.Settings.UserName Then
                                ' Auteur envoie un message à "quelqu'un"
                                AddMessage(destinataire, author, room, message, True, avatarPath)
                                SelectUserByName(destinataire)
                                messageAdded = True
                            End If

                            If destinataire = My.Settings.UserName Then
                                ' Destinataire est l'auteur
                                AddMessage(author, author, room, message, False, avatarPath)
                                SelectUserByName(author)
                                messageAdded = True
                            End If
                    End Select

                    ' Mettre à jour la fenêtre si un message a été ajouté
                    If messageAdded Then
                        Me.WindowState = WindowState.Normal
                        Me.Topmost = True
                        Me.Topmost = False
                        Me.Focus()
                    End If




                Catch ex As Exception
                    logger.Error("Erreur de réception d'un message MSG01 " & ex.Message)
                End Try

            Case "MSG02"
                'Pour supprimer un message



            Case "SLN01"
                'création d'un salon
            Case "SLN02"
                'invatition a un salon
            Case "SLN03"
                'suppresion d'un salon 


            Case "DBG01"
                'Code pour envoyer l'ID du PC au autres application
                Try
                    Dim localIPAddress As IPAddress = GetLocalIPAddress()
                    SendMessageWithCode("DBG02", My.Settings.UniqueId & "|" & Environment.UserName & "|" & localIPAddress.ToString)
                    logger.Debug("Envoi de l'ID du PC aux autres applications")
                Catch ex As Exception
                    logger.Error("Erreur lors de l'envoi de l'ID du PC aux autres applications : " & ex.Message)
                End Try


            Case "DBG02"
                'Code pour enregistrer l'ID des autre PC dans la collection computer
                Try
                    Dim messageContent As String = receivedMessage.Substring(5)
                    Dim parts As String() = messageContent.Split("|"c)
                    Dim ComputerID As String = parts(0)
                    Dim ComputerUser As String = parts(1)
                    Dim ComputerIP As String = parts(2)
                    If Not Computers.Any(Function(c) c.ComputerID = ComputerID) Then
                        Computers.Add(New Computer With {.ComputerID = ComputerID, .ComputerUser = ComputerUser, .ComputerIP = ComputerIP})

                        logger.Debug("Ajout d'un PC à la liste des PC : " & ComputerID)
                    End If
                    SaveComputersToJson() ' Sauvegarder la liste mise à jour dans le fichier JSON
                Catch ex As Exception
                    logger.Error("Erreur lors de la réception de l'ID des autres PC : " & ex.Message)
                End Try

            Case Else
                ' Code pour les messages non reconnus
                ' ...
        End Select
    End Sub

    ' Fonction permettant d'envoyer un code suivi d'un contenu spécifié
    Private Sub SendMessageWithCode(code As String, content As String)

        Try
            ' Vérifier si le code et le contenu ne sont pas nuls
            If code IsNot Nothing And content IsNot Nothing Then
                ' Concaténer le code et le contenu pour former le message complet
                Dim message As String = code + content

                ' Convertir le message en tableau d'octets en utilisant l'encodage UTF-8
                Dim messageBytes As Byte() = Encoding.UTF8.GetBytes(message)

                ' Envoyer les octets du message à travers le client d'envoi
                sendingClient.Send(messageBytes, messageBytes.Length)

                ' Afficher le message dans la console
                logger.Debug("Envoi d'un message avec code : " & code & " et message : " & content)
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de l'envoi d'un message avec code : " & code & " et message : " & content & " l'erreur suivante est apparue : " & ex.Message)
        End Try

    End Sub

    ' Fonction permettant d'envoyer un message complet
    Public Shared Sub SendMessage(message As String)
        Try
            ' Vérifier si le message n'est pas vide
            If message <> "" Then
                ' Convertir le message en tableau d'octets en utilisant l'encodage UTF-8
                Dim DataMessage() As Byte = Encoding.UTF8.GetBytes(message)

                ' Vérifier si le tableau d'octets du message n'est pas nul
                If DataMessage IsNot Nothing Then

                    ' Envoyer les octets du message à travers le client d'envoi
                    sendingClient.Send(DataMessage, DataMessage.Length)

                    ' Afficher le message dans la console
                    logger.Debug("Envoi d'un message : " & message)
                End If
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de l'envoi d'un message : " & message & " l'erreur suivante est apparue : " & ex.Message)
        End Try

    End Sub



    Private Async Sub SpeedMessageDialog(ByVal Titre As String, ByVal Message As String, ByVal option1 As String, ByVal option2 As String, ByVal option3 As String)
        Try
            Dim dialogSettings As New MetroDialogSettings With {
            .AffirmativeButtonText = option1,
            .NegativeButtonText = option2,
            .FirstAuxiliaryButtonText = option3
        }

            Dim selectedOption As String = Await ShowMessageDialogAsync(Titre, Message, dialogSettings, option1, option2, option3)

            If selectedOption IsNot Nothing Then
                SpeedSendMessage(selectedOption, Titre)
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de l'affichage d'un message dialog : " & ex.Message)
        End Try
    End Sub

    Private Async Function ShowMessageDialogAsync(ByVal Titre As String, ByVal Message As String, ByVal dialogSettings As MetroDialogSettings, ByVal option1 As String, ByVal option2 As String, ByVal option3 As String) As Task(Of String)
        Dim result As MessageDialogResult = Await DialogCoordinator.Instance.ShowMessageAsync(Me, Titre, Message, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, dialogSettings)

        Select Case result
            Case MessageDialogResult.Affirmative
                Return option1
            Case MessageDialogResult.Negative
                Return option2
            Case MessageDialogResult.FirstAuxiliary
                Return option3
            Case Else
                Return Nothing ' Aucune option sélectionnée
        End Select
    End Function

    Private Sub SpeedSendMessage(ByVal selectedOption As String, ByVal Titre As String)
        Dim message As String = GetMessage(selectedOption, Titre)
        If message IsNot Nothing Then
            SendMessage(message)
        End If
    End Sub

    Private Function GetMessage(ByVal selectedOption As String, ByVal Titre As String) As String
        Select Case selectedOption
            Case "0"
                Return "MSG01" & My.Settings.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient au plus vite" & "|benoit.png"
            Case "1"
                Return "MSG01" & My.Settings.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient dans 2 minutes " & "|benoit.png"
            Case "2"
                Return "MSG01" & My.Settings.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient dans 5 minutes " & "|benoit.png"
            Case "3"
                Return "MSG01" & My.Settings.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient dans 10 minutes " & "|benoit.png"
            Case "A"
                Return "MSG01" & My.Settings.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " met en attente " & "|benoit.png"
            Case Else
                Return Nothing ' Option non reconnue
        End Select
    End Function


#End Region

#Region "Gestion Suggestion des commandes Sendbox Et envoie message"

    ' Stockage des valeurs actuelles pour gérer les suggestions et la mise en forme du texte
    Private _currentInput As String = ""            ' Stocke le texte actuel dans la zone de saisie
    Private _currentSuggestion As String = ""       ' Stocke la suggestion actuelle basée sur le texte saisi
    Private _currentText As String = ""             ' Stocke le texte complet à afficher (suggestion + texte saisi)
    Private _selectionStart As Integer              ' Stocke la position de début de la sélection
    Private _selectionLength As Integer             ' Stocke la longueur de la sélection

    Private Sub SuggestionBoxOnTextChanged(sender As Object, e As TextChangedEventArgs) Handles SendTextBox.TextChanged
        Try
            ' Récupérer le texte actuellement saisi dans la zone de saisie
            Dim input = SendTextBox.Text

            ' Vérifier si la longueur du texte saisi a augmenté et si le texte est différent de la suggestion actuelle
            If input.Length > _currentInput.Length AndAlso input <> _currentSuggestion Then
                ' Recherche de la première suggestion qui commence par le texte saisi
                _currentSuggestion = SuggestionValues.FirstOrDefault(Function(x) x.StartsWith(input))

                ' Vérifier si une suggestion a été trouvée
                If _currentSuggestion IsNot Nothing Then
                    ' Mettre à jour le texte complet à afficher avec la suggestion trouvée
                    _currentText = _currentSuggestion

                    ' Mémoriser la position de début de la sélection dans le texte
                    _selectionStart = input.Length

                    ' Calculer la longueur de la sélection en se basant sur la suggestion trouvée
                    _selectionLength = _currentSuggestion.Length - input.Length

                    ' Mettre à jour le texte de la zone de saisie avec le texte complet (suggestion + texte saisi)
                    SendTextBox.Text = _currentText

                    ' Sélectionner la partie du texte correspondant à la suggestion
                    SendTextBox.[Select](_selectionStart, _selectionLength)

                    ' Journaliser l'action normale
                    logger.Debug("Suggestion mise à jour : " & _currentText)
                End If
            End If

            ' Mettre à jour la valeur de _currentInput avec le texte actuel de la zone de saisie
            _currentInput = input
        Catch ex As Exception
            ' Gérez l'exception ici en ajoutant des messages de journalisation d'erreur
            logger.Error("Erreur dans SuggestionBoxOnTextChanged : " & ex.Message)
        End Try
    End Sub

    Private Function SendTextBox_KeyDownAsync(sender As Object, e As KeyEventArgs) As Task Handles SendTextBox.KeyDown
        If e.Key = Key.Enter Then
            logger.Info("Touche entrée appuyée sur la Sendbox")
            SendTextBox.Text = SendTextBox.Text.TrimEnd()
            If Not String.IsNullOrEmpty(SendTextBox.Text) Then
                Select Case SendTextBox.Text
                    Case "/DEBUG"
                        ' Active la fonction de débogage
                        logger.Info("Commande /DEBUG dans la Sendbox")
                        Try
                            ' Ajouter l'utilisateur Marvin
                            Dim UserToAdd As User = Users.FirstOrDefault(Function(user) user.Name = "Marvin")
                            If UserToAdd Is Nothing Then
                                Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
                                Users.Add(New User With {.Name = "Marvin", .Avatar = avatarPath, .Status = "Don't Panic"})
                                logger.Debug("Utilisateur Marvin ajouté")
                            End If

                        Catch ex As Exception
                            logger.Error("Erreur lors de l'ajout de l'utilisateur Marvin avec la commande /DEBUG: " & ex.Message)
                        End Try

                    Case "/ENDDEBUG"
                        ' Désactive la fonction de débogage
                        logger.Info("Commande /ENDDEBUG dans la Sendbox")
                        Try
                            ' Supprimer l'utilisateur Marvin
                            Dim UserToRemove As User = Users.FirstOrDefault(Function(user) user.Name = "Marvin")
                            If UserToRemove IsNot Nothing Then
                                Users.Remove(UserToRemove)
                                logger.Debug("Utilisateur Marvin supprimé")
                            End If
                        Catch ex As Exception
                            logger.Error("Erreur lors de la suppression de l'utilisateur Marvin avec la commande /ENDDEBUG: " & ex.Message)
                        End Try

                    Case "/LSTCOMPUTER"
                        ' Commande pour lister les ordinateurs connectés
                        logger.Info("Commande /LSTCOMPUTER dans la Sendbox")
                        Try
                            ' Envoyer le message DBG01 pour demander les ordinateurs connectés
                            SendMessage("DBG01")
                            logger.Debug("Message DBG01 envoyé")
                        Catch ex As Exception
                            logger.Error("Erreur lors de l'envoi du message DBG01 avec la commande /LSTCOMPUTER: " & ex.Message)
                        End Try


                    Case "/DISPCOMPUTER"
                        ' Commande pour afficher les ordinateurs connectés dans la liste computers
                        logger.Info("Commande /DISPCOMPUTER dans la Sendbox")
                        Try
                            Dim computerString As New StringBuilder()
                            computerString.AppendLine("Actuellement il y a :")
                            For Each computer In Computers
                                computerString.AppendLine(computer.ComputerUser & " " & computer.ComputerIp)
                            Next
                            Dim avatarPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
                            AddMessage("Marvin", "Marvin", "Cœur en Or", computerString.ToString, False, avatarPath)
                            logger.Debug("Liste des ordinateurs affichée avec la commande /DISPCOMPUTER")
                        Catch ex As Exception
                            logger.Error("Erreur lors de l'affichage des ordinateurs avec la commande /DISPCOMPUTER: " & ex.Message)
                        End Try


                    Case "/TESTPATIENT"

                        ' Liste de prénoms, noms et titres
                        Dim testprenoms As String() = {"Jean", "Marie", "Claire", "Pierre", "Sophie"}
                        Dim testnoms As String() = {"Dubois", "Martin", "Lefebvre", "Dupont", "Moreau"}

                        ' Identificateur et autres informations communes
                        Dim identifier As String = "PTN01Iel"
                        Dim other_info As String = $"RDC|System|{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")}"

                        ' Créer une instance de Random
                        Dim random As New Random()

                        ' Boucle pour envoyer les messages avec différents codes
                        For Each optioncode As ExamOption In ExamOptions
                            Dim code As String = optioncode.Name
                            Dim testprenom As String = testprenoms(random.Next(testprenoms.Length))
                            Dim testnom As String = testnoms(random.Next(testnoms.Length))
                            Dim annotation As String = "ODG " & optioncode.Annotation
                            Dim message As String = $"{identifier}|{testnom}|{testprenom}|{code}|{annotation}|{other_info}"
                            SendMessage(message)

                        Next

                    Case "/TESTSENDFILE"

                        CreateTextFile("Core", "test.txt", "fichier test")
                        SendFileOverNetwork("Core", "test.txt")

                    Case "/TESTMARVIN"
                        ' Créer une instance de la classe Random
                        Dim random As New Random()

                        ' Vérifier si la liste contient des phrases
                        If phrasesData IsNot Nothing AndAlso phrasesData.MarvinPhrases.Count > 0 AndAlso phrasesData.JoyPhrases.Count > 0 Then
                            ' Générer un index aléatoire dans la plage des indices de la liste
                            Dim randomIndex As Integer = random.Next(0, phrasesData.MarvinPhrases.Count)
                            Dim randomIndexJoy As Integer = random.Next(0, phrasesData.JoyPhrases.Count)

                            ' Récupérer la phrase aléatoire en utilisant l'index généré
                            Dim randomPhrase As String = phrasesData.MarvinPhrases(randomIndex)
                            Dim randomPhraseJoy As String = phrasesData.JoyPhrases(randomIndexJoy)

                            ' Faire quelque chose avec la phrase aléatoire
                            MessageBox.Show(randomPhrase)
                            MessageBox.Show(randomPhraseJoy)
                        End If


                    Case Else
                        ' Vérifier si le message commence par un code MSG ou et un simple message
                        logger.Info("Vérification si le message de la Sendbox commence par un code MSG")
                        Dim startsWithCodeMSG As Boolean = False
                        Dim matchedOption As ExamOption = Nothing

                        Try
                            For Each Exaoption As ExamOption In ExamOptions
                                If SendTextBox.Text.StartsWith(Exaoption.CodeMSG) Then
                                    startsWithCodeMSG = True
                                    matchedOption = Exaoption
                                    Exit For
                                End If
                            Next
                        Catch ex As Exception
                            logger.Error("Erreur lors du parcours de la liste des ExamOptions : " & ex.Message)
                        End Try



                        ' Vérifier si le message commence par un code MSG et si un code MSG a été trouvé
                        If startsWithCodeMSG AndAlso matchedOption IsNot Nothing Then
                            ' Le message commence par un code MSG et un code MSG a été trouvé
                            ' Récupérer le code MSG et l'annotation
                            logger.Info("Le message de la Sendbox commence par un code MSG et un code MSG a été trouvé")
                            Dim codeMSG As String = matchedOption.CodeMSG

                            Dim annotation As String = matchedOption.Annotation
                            ' Utilisez la variable codeMSG comme vous le souhaitez...
                            Dim spaceIndex As Integer = SendTextBox.Text.IndexOf(" ", codeMSG.Length)

                            ' Création des variables pour le patient
                            Dim patientTitre As String = ""
                            Dim patientNom As String = ""
                            Dim patientPrenom As String = ""
                            codeMSG = codeMSG.Replace("=", "")


                            ' Vérifier s'il y a un espace après le CodeMSG
                            If spaceIndex > codeMSG.Length Then
                                'il y a un espace après le codeMSG
                                ExtractInfoFromInput(SendTextBox.Text.Substring(spaceIndex + 1), patientTitre, patientNom, patientPrenom)
                            Else
                                ' Pas d'espace après le CodeMSG
                                ExtractInfoFromInput(SendTextBox.Text.Substring(codeMSG.Length), patientTitre, patientNom, patientPrenom)
                            End If

                            Dim Text As String = "PTN01" & patientTitre & "|" & patientNom & "|" & patientPrenom & "|" & codeMSG & "|" & annotation & "|RDC|" & My.Settings.UserName & "|" & Date.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
                            SendMessage(Text)


                        Else



                            ' C'est un message simple
                            ' Formate le message sous la forme "MSG01{My.Settings.UserName}|{selectedUser.Name}|{Message}|{Avatar}"
                            logger.Info("Le message de la Sendbox est un message simple")
                            logger.Info("Le message de la Sendbox est un message simple")
                            Try
                                Dim text As String
                                Dim selectedUser As User = TryCast(ListUseres.SelectedItem, User)
                                If selectedUser IsNot Nothing Then
                                    Dim selectedUserName As String = selectedUser.Name
                                    ' Faites quelque chose avec le nom de l'utilisateur sélectionné
                                    ' ...

                                    If SendTextBox.Text.Contains("marvin") Then
                                        ' Créer une instance de la classe Random
                                        Dim random As New Random()

                                        ' Vérifier si la liste contient des phrases
                                        If phrasesData IsNot Nothing AndAlso phrasesData.MarvinPhrases.Count > 0 AndAlso phrasesData.JoyPhrases.Count > 0 Then
                                            ' Générer un index aléatoire dans la plage des indices de la liste
                                            Dim randomIndexMarvin As Integer = random.Next(0, phrasesData.MarvinPhrases.Count)
                                            Dim randomIndexJoy As Integer = random.Next(0, phrasesData.JoyPhrases.Count)

                                            ' Récupérer la phrase aléatoire en utilisant l'index généré
                                            Dim randomPhraseMarvin As String = phrasesData.MarvinPhrases(randomIndexMarvin)
                                            Dim randomPhraseJoy As String = phrasesData.JoyPhrases(randomIndexJoy)

                                            ' Vérifie si Marvin est dans la liste des users
                                            If IsNameInList(Users, "Marvin") Then
                                                ' Marvin est présent dans la liste, on ajoute le message
                                                Dim avatarPathMarvin As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
                                                Dim avatarPathJoy As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "joy.png")
                                                AddMessage("Marvin", "Marvin", "Cœur en Or", randomPhraseMarvin, False, avatarPathMarvin)
                                                AddMessage("Marvin", "Joy", "Riley", randomPhraseJoy, False, avatarPathJoy)
                                            Else
                                                ' Marvin n'est pas présent dans la liste, on créer le user et on ajoute le message
                                                Dim avatarPathMarvin As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "system.png")
                                                Dim avatarPathJoy As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "joy.png")
                                                Users.Add(New User With {.Name = "Marvin", .Avatar = avatarPathMarvin, .Status = "Don't Panic"})
                                                AddMessage("Marvin", "Marvin", "Cœur en Or", randomPhraseMarvin, False, avatarPathMarvin)
                                                AddMessage("Marvin", "Joy", "Riley", randomPhraseJoy, False, avatarPathJoy)
                                            End If
                                        End If

                                    End If

                                    text = "MSG01" & My.Settings.UserName & "|" & selectedUserName & "|" & My.Settings.WindowsName & "|" & SendTextBox.Text & "|avataaars.png"
                                    SendMessage(text)
                                    MessageList.ScrollToEnd()
                                    logger.Debug("Message simple envoyé  ")
                                End If


                            Catch ex As Exception
                                logger.Error("Erreur lors de l'envoi du message simple : " & ex.Message)
                            End Try

                        End If


                End Select

            End If

            SendTextBox.Clear()


        ElseIf e.Key = Key.Tab Then
            ' La touche Tab a été appuyée
            ' Insérer la suggestion dans la zone de texte
            Try
                If Not String.IsNullOrEmpty(_currentSuggestion) Then
                    SendTextBox.Text = _currentSuggestion
                    SendTextBox.SelectionStart = SendTextBox.Text.Length
                End If
                ' Empêcher le focus de se déplacer vers le contrôle suivant
                e.Handled = True
            Catch ex As Exception
                logger.Error("Erreur lors de l'insertion de la suggestion dans la zone de texte : " & ex.Message)
            End Try

        End If
        Return Task.CompletedTask
    End Function

#End Region

    Private Sub ListUseres_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListUseres.SelectionChanged

        logger.Info("Selection d'un user dans la liste des users.")
        Try
            Dim selectedUser As User = TryCast(ListUseres.SelectedItem, User)
            If selectedUser IsNot Nothing Then
                selectedUserName = selectedUser.Name
                ' Selectionne le user dans la liste des users
                SelectUser(selectedUser.Name)
                logger.Debug($"Selection de {selectedUser.Name} dans la liste des users.")
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la selection d'un user dans la liste des users.")
        End Try

    End Sub



    Function IsNameInList(users As ObservableCollection(Of User), nameToSearch As String) As Boolean
        ' Fonction qui teste la présence d'un user et retourne une boolean
        logger.Info($"Test de la présence de {nameToSearch} dans la liste des users.")
        Try
            Return users.Any(Function(u) u.Name = nameToSearch)
            logger.Debug($"{nameToSearch} est dans la liste des users ")
        Catch ex As Exception
            Return False
            logger.Error($"Erreur lors du test de la présence de {nameToSearch} dans la liste des users.")
        End Try

    End Function




    Private Function ConnectionButon_Click(sender As Object, e As RoutedEventArgs) As Task




        'If ConnectionButon.Content = "Connection" Then
        'ShowLoginDialogPreview()
        'Else
        'ConnectionButon.Content = "Connection"
        'logger.Debug($" {My.Settings.UserName} s'est déconnecté du poste.")
        'SaveMessagesToJson(Messages)
        'SaveUsersToJson(Users)
        'SavePatientsToJson(PatientsALL)
        'SelectedUserMessages.Clear()

        'Dim sortedPatientsRDC As CollectionViewSource = CType(FindResource("SortedPatientsRDC"), CollectionViewSource)
        'Dim patientsList As ObservableCollection(Of Patient) = CType(sortedPatientsRDC.Source, ObservableCollection(Of Patient))

        'If patientsList IsNot Nothing Then
        'patientsList.Clear()
        'End If

        'Users.Clear()
        'My.Settings.UserName = ""
        'My.Settings.Save()

        'End If
        'SendMessage("USR02" & My.Settings.UserName & "|" & Environment.UserName)

    End Function

    Private Shared _instance As MainWindow
    Public Async Function lapinAsync() As Task
        Await ShowChildWindowAsync(New InfoPatient(), RootGrid)
    End Function



    Private Async Sub ShowLoginDialogPreview()

        Dim result = Await Me.ShowInputAsync("Connection", "Entrer votre nom :", New MetroDialogSettings() With {.NegativeButtonText = "Annuler", .AffirmativeButtonText = "Connection"})
        'Dim result = Await Me.ShowLoginAsync("Eyechat connection", "", New LoginDialogSettings() With {.NegativeButtonVisibility = True, .NegativeButtonText = "Annuler"})
        If result Is Nothing Then
            ' L'utilisateur a appuyé sur Annuler ou a fermé la boîte de dialogue
        Else
            If result = "benoit" Then
                My.Settings.UserName = result
                My.Settings.Save()
                ConnectionButon.Content = "Deconnection"
                Dim loadedUsers = LoadUsersFromJson()
                Users.Clear()
                For Each user In loadedUsers
                    Users.Add(user)
                Next
                LoadPatientsFromJson()
                SelectUserByName("A Tous")
            End If
        End If

    End Sub


    Private Sub SelectUserByName(userName As String)
        ' Recherche de l'utilisateur par son nom dans la collection Users
        logger.Info("Recherche de l'utilisateur par son nom dans la collection Users")
        Try
            Dim userToSelect As User = Users.FirstOrDefault(Function(u) u.Name = userName)

            ' Vérification si un utilisateur avec le nom spécifié a été trouvé
            If userToSelect IsNot Nothing Then
                ' Sélection de l'utilisateur trouvé dans la liste des utilisateurs
                ListUseres.SelectedItem = userToSelect

                ' Appel d'une fonction/méthode pour gérer la sélection de l'utilisateur
                ' en passant le nom de l'utilisateur trouvé comme argument
                SelectUser(userToSelect.Name)
                logger.Debug("Sélection de l'utilisateur trouvé dans la liste des utilisateurs")
            End If
        Catch ex As Exception
            logger.Error("Erreur dans la recherche de l'utilisateur par son nom dans la collection Users " & ex.Message)
        End Try

    End Sub

    Private _selectedListUser As User
    Public Property SelectedListUser As User
        Get
            Return _selectedListUser
        End Get
        Set(value As User)
            _selectedListUser = value
            ' Faites quelque chose avec l'utilisateur sélectionné (par exemple, récupérer le nom)
            If _selectedListUser IsNot Nothing Then
                Dim selectedUserName As String = _selectedListUser.Name
                ' Utilisez le nom de l'utilisateur sélectionné comme vous le souhaitez
            End If
        End Set
    End Property

#Region "Envoie de fichier"

    Public Shared Sub SendFileOverNetwork(folder As String, fileToSend As String, Optional user As String = "")
        Try
            ' Vérifie si un utilisateur a été spécifié
            If String.IsNullOrEmpty(user) Then
                ' Aucun utilisateur spécifié, envoyez le message à tous les ordinateurs répertoriés
                For Each computer In Computers
                    ' Assurez-vous de ne pas vous envoyer un message à vous-même
                    If computer.ComputerID <> My.Settings.UniqueId Then
                        Dim message As String = $"SYS20{computer.ComputerID}|{My.Settings.UniqueId}|{folder}|{fileToSend}"
                        ' Envoyer le message
                        SendMessage(message)
                        ' Envoyer le fichier à l'ordinateur actuel
                        Dim senderFile As New FileSender()
                        Dim filePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", fileToSend)
                        Dim ipAddress As String = computer.ComputerIp ' Remplacez par l'adresse IP du destinataire
                        Dim port As Integer = 12345 ' Remplacez par le port que vous souhaitez utiliser
                        senderFile.SendFile(filePath, ipAddress, port)
                    End If
                Next
            Else
                ' Un utilisateur a été spécifié, envoyez le message uniquement à cet utilisateur
                Dim targetComputer As Computer = Computers.FirstOrDefault(Function(comp) comp.ComputerUser = user)
                If targetComputer IsNot Nothing Then
                    Dim message As String = $"SYS20{targetComputer.ComputerID}|{My.Settings.UniqueId}|{folder}|{fileToSend}"
                    ' Envoyer le message à l'utilisateur spécifié
                    SendMessage(message)
                    ' Envoyer le fichier à l'utilisateur spécifié
                    Dim senderFile As New FileSender()
                    Dim filePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", fileToSend)
                    Dim ipAddress As String = targetComputer.ComputerIp ' Remplacez par l'adresse IP du destinataire
                    Dim port As Integer = 12345 ' Remplacez par le port que vous souhaitez utiliser
                    senderFile.SendFile(filePath, ipAddress, port)
                Else
                    ' Gérer le cas où l'utilisateur spécifié n'existe pas
                    MessageBox.Show("L'utilisateur spécifié n'existe pas.")
                End If
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de l'envoi du fichier : " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Gestion boite de dialogue d'ajout d'un patint"


    Private Sub ClosePatientBox_OnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Fermer la boîte de dialogue personnalisée
        logger.Info("Fermeture de la boite de dialogue d'ajout d'un patient")
        Try
            Me.CustomDialogBox.Close()
            logger.Debug("Fermeture de la boite de dialogue d'ajout d'un patient")
        Catch ex As Exception
            logger.Error("Erreur dans la fermeture de la boite de dialogue d'ajout d'un patient " & ex.Message)
        End Try

    End Sub

    Private Sub ValidPatientBox_OnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Fermer la boîte de dialogue personnalisée
        Me.CustomDialogBox.Close()

        ' Initialisation des variables pour stocker les informations du patient
        Dim patientTitre As String = ""
        Dim patientNom As String = ""
        Dim patientPrenom As String = ""

        ' Récupérer la chaîne d'entrée à partir de la zone de texte
        Dim inputString As String = PatientNameBox.Text

        ' Appeler la fonction ExtractInfoFromInput pour extraire les informations du patient
        ExtractInfoFromInput(inputString, patientTitre, patientNom, patientPrenom)

        ' Vérifier si les informations ont été correctement extraites
        If String.IsNullOrEmpty(patientNom) OrElse String.IsNullOrEmpty(patientPrenom) Then
            ' Afficher un message d'erreur et quitter la fonction
            MessageBox.Show("Le format d'entrée doit être 'Titre Nom Prénom' ou 'Nom Prénom'.")
            Return
        End If

        ' Créer la chaîne de texte du patient au format souhaité
        Dim Text As String = "PTN01" & patientTitre & "|" & patientNom & "|" & patientPrenom & "|"

        ' Récupérer l'option d'examen sélectionnée
        Dim selectedExamOption As ExamOption = DirectCast(PatientExamSelect.SelectedItem, ExamOption)
        If selectedExamOption IsNot Nothing Then
            Text += selectedExamOption.Name + "|" ' Utiliser le nom de l'option        
        End If

        ' Récupérer l'œil du patient et le commentaire
        Dim selectedPatientEye As ComboBoxItem = CType(PatientEyeSelect.SelectedItem, ComboBoxItem)
        If selectedPatientEye IsNot Nothing Then
            Text += selectedPatientEye.Content.ToString() + " " + PatientCommentBox.Text + "|"
        End If

        ' Récupérer l'étage du patient
        Dim selectedPatientFloor As ComboBoxItem = CType(PatientFloorSelect.SelectedItem, ComboBoxItem)
        If selectedPatientFloor IsNot Nothing Then
            Text += selectedPatientFloor.Content.ToString + "|"
        End If

        ' Ajouter le nom de l'utilisateur et l'horodatage actuel
        Text += My.Settings.UserName + "|" + Date.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")

        ' Appeler la fonction Sendmessage avec la chaîne de texte du patient
        SendMessage(Text)

    End Sub


    Private Function EnumWindowCallBack(ByVal hwnd As IntPtr, ByVal lParam As IntPtr) As Boolean
        Try
            Dim windowText As New StringBuilder(256)
            GetWindowText(hwnd, windowText, windowText.Capacity)
            Dim text As String = windowText.ToString().Trim()
            If text.Length > 0 Then
                If text.StartsWith("REFRACTION") OrElse text.Contains("LENTILLES") OrElse text.Contains("PATHOLOGIES") OrElse text.Contains("ORTHOPTIE") OrElse text.Contains("TRAITEMENT") Then
                    Dim inputString As String = text
                    ' Expression régulière pour extraire le nom complet de la chaîne d'entrée
                    Dim regexPattern As String = "(REFRACTION de|LENTILLES de|PATHOLOGIES de|ORTHOPTIE de|TRAITEMENT de)\s+(Monsieur|Madame|Mademoiselle|Enfant|Maître|Docteur)\s+((?:[A-ZÀ-ÖØ-Ý\-']+\s?)+)(?:(Née)\s+([A-ZÀ-ÖØ-Ý\-']+\s?)+)?\s+(([A-ZÀ-ÖØ-Ý][a-zà-öø-ý\-']*([ -][A-ZÀ-ÖØ-Ý][a-zà-öø-ý\-']*)*)).*$"

                    ' Création d'un objet Regex à partir de l'expression régulière
                    Dim regex As New Regex(regexPattern, RegexOptions.IgnoreCase)
                    ' Extraction du nom complet à partir de la chaîne d'entrée
                    Dim match As Match = regex.Match(inputString)
                    ' Construction de la chaîne de sortie à partir des groupes capturés par l'expression régulière

                    ' Extraction du titre à partir de la chaîne d'entrée
                    Dim titre As String = match.Groups(2).Value

                    ' Remplacement du titre complet par son abréviation correspondante
                    Select Case titre
                        Case "Monsieur"
                            titre = "Mr"
                        Case "Madame"
                            titre = "Mme"
                        Case "Mademoiselle"
                            titre = "Mlle"
                        Case "Enfant"
                            titre = "Enfant"
                        Case "Maître"
                            titre = "Me"
                        Case "Docteur"
                            titre = "Dr"
                    End Select
                    Dim outputString As String = titre & " " & match.Groups(3).Value.Trim() & " " & match.Groups(6).Value.TrimEnd()
                    PatientNameBox.Text = outputString
                    PatientNameBox.Select(SendTextBox.Text.Length, 0)

                    ' Enregistre des informations de débogage
                    logger.Debug("Extraction des informations du texte de la fenêtre réussie.")
                    logger.Debug("Texte extrait : " & outputString)
                End If
            End If
        Catch ex As Exception
            ' Gestion des erreurs qui pourraient se produire lors de l'extraction des informations
            ' Vous pouvez ajouter des logs ici ou effectuer d'autres actions appropriées en cas d'erreur.
            logger.Error("Erreur lors de l'extraction des informations du texte de la fenêtre : " & ex.Message)
        End Try

        Return True
    End Function

    Private Sub ExtractInfoFromInput(inputText As String, ByRef patientTitre As String, ByRef patientNom As String, ByRef patientPrenom As String)
        Try
            ' Modèle pour extraire le titre, le nom et le prénom
            Dim patternTitleName As String = "^(?i)(Mr|Mme|Mlle|Enfant)?\s*(?<name>[^\d\s\-]+)\s+(?<firstName>[^\s]+)"

            Dim matchTitleName As Match = Regex.Match(inputText, patternTitleName)

            If matchTitleName.Success Then
                Dim titre As String = matchTitleName.Groups(1).Value
                Dim nom As String = matchTitleName.Groups("name").Value
                Dim prenom As String = matchTitleName.Groups("firstName").Value

                ' Mettre en majuscules le nom du patient
                patientNom = nom.ToUpper()

                ' Si titre est vide, définir "Iel" par défaut
                If String.IsNullOrWhiteSpace(titre) Then
                    patientTitre = "Iel"
                Else
                    ' Utiliser le titre tel quel (en minuscules)
                    patientTitre = titre
                End If

                ' Mettre en majuscules seulement la première lettre du prénom
                If Not String.IsNullOrEmpty(prenom) Then
                    patientPrenom = Char.ToUpper(prenom(0)) + prenom.Substring(1)
                Else
                    patientPrenom = ""
                End If

                ' Enregistre des informations de débogage
                logger.Debug("Extraction des informations du texte d'entrée réussie.")
                logger.Debug("Titre : " & patientTitre)
                logger.Debug("Nom : " & patientNom)
                logger.Debug("Prénom : " & patientPrenom)
            Else
                ' Aucune correspondance trouvée
                patientTitre = ""
                patientNom = ""
                patientPrenom = ""
            End If
        Catch ex As Exception
            ' Gestion des erreurs lors de l'extraction des informations
            ' Vous pouvez ajouter des logs ici ou effectuer d'autres actions appropriées en cas d'erreur.
            logger.Error("Erreur lors de l'extraction des informations du texte d'entrée : " & ex.Message)

            ' Réinitialise les valeurs en cas d'erreur
            patientTitre = ""
            patientNom = ""
            patientPrenom = ""
        End Try
    End Sub


#End Region


#Region "Gestion de la mise a jour"



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
            Dim result As MessageDialogResult = Await ShowMessageAsync("Mise à jour disponible", message, MessageDialogStyle.AffirmativeAndNegative, dialogSettings)

            If result = MessageDialogResult.Affirmative Then
                ' Si l'utilisateur accepte, lance le téléchargement et l'installation de la mise à jour
                Await DownloadAndInstallUpdate(channel)
            End If
        Else
            ' Affiche un message si aucune mise à jour n'est disponible
            Await ShowMessageAsync("Pas de mise à jour", "Vous utilisez la dernière version de l'application.")

            ' Ajoute un message de débogage pour indiquer qu'aucune mise à jour n'est disponible
            logger.Debug("Aucune mise à jour disponible. Version actuelle : " & Version)
        End If
    End Sub


    ' Cette fonction récupère la version la plus récente du serveur dans le canal spécifié.
    ' Elle effectue une requête HTTP pour obtenir les informations de version à partir du fichier JSON.
    ' Paramètre channel : Le canal (par exemple "beta" ou "final") dans lequel chercher la version.
    ' Renvoie la version la plus récente sous forme de chaîne, ou Nothing en cas d'erreur.
    Private Async Function GetServerVersion(channel As String) As Task(Of String)
        logger.Info("Récupération de la version du serveur dans le canal " & channel)
        Try
            ' Construit l'URL du fichier JSON de version sur GitHub en fonction du canal
            Dim apiUrl As String = $"https://raw.githubusercontent.com/{RepoOwner}/{RepoName}/master/releases/{channel}/version.json"
            Dim webClient As New WebClient()

            ' Télécharge les informations de version en tant que chaîne JSON
            Dim versionJson As String = Await webClient.DownloadStringTaskAsync(apiUrl)
            Dim versionObject As JObject = JObject.Parse(versionJson)

            ' Extrait et renvoie la version à partir de l'objet JSON
            Dim serverVersion As String = versionObject("version").ToString()

            ' Ajoute un message de débogage pour indiquer la version du serveur obtenue
            logger.Debug($"Version du serveur obtenue avec succès : {serverVersion}")

            Return serverVersion
        Catch ex As Exception
            ' En cas d'erreur, retourne Nothing et ajoute un message de débogage pour l'erreur
            logger.Error($"Erreur lors de la récupération de la version du serveur : {ex.Message}")
            Return Nothing
        End Try
    End Function

    ' Cette fonction compare deux numéros de version dans le format NuGet et renvoie un résultat de comparaison.
    ' Paramètres version1 et version2 : Les numéros de version à comparer sous forme de chaînes (format NuGet).
    ' Renvoie 1 si version1 > version2, -1 si version1 < version2, 0 si version1 = version2.
    Private Function CompareVersions(version1 As String, version2 As String) As Integer
        logger.Info("Comparaison des versions " & version1 & " et " & version2)
        Try
            ' Parse les numéros de version en objets NuGetVersion
            Dim nugetVersion1 As NuGetVersion = NuGetVersion.Parse(version1)
            Dim nugetVersion2 As NuGetVersion = NuGetVersion.Parse(version2)

            ' Compare les versions en utilisant la méthode CompareTo de NuGetVersion
            Dim result As Integer = nugetVersion1.CompareTo(nugetVersion2)

            ' Messages de débogage pour enregistrer les informations de comparaison
            If result > 0 Then
                logger.Debug("La version " & version1 & " est supérieure à la version " & version2)
            ElseIf result < 0 Then
                logger.Debug("La version " & version1 & " est inférieure à la version " & version2)
            Else
                logger.Debug("La version " & version1 & " est égale à la version " & version2)
            End If

            Return result
        Catch ex As Exception
            ' En cas d'erreur lors de la comparaison des versions
            logger.Error("Erreur lors de la comparaison des versions : " & ex.Message)
            ' Vous pouvez choisir de renvoyer une valeur par défaut ou de gérer l'erreur autrement ici
            Return 0 ' Par exemple, renvoyer 0 en cas d'erreur
        End Try
    End Function

    Private Async Function DownloadAndInstallUpdate(channel As String) As Task
        logger.Info("Téléchargement et installation de la mise à jour depuis le canal " & channel)
        Dim webClient As New WebClient()
        Dim updateUrl As String = Await GetUpdateUrl(channel) ' Obtient l'URL de mise à jour depuis le fichier JSON

        Try
            ' Affiche une fenêtre de progression pendant le téléchargement
            Dim progressController = Await ShowProgressAsync("Téléchargement de la mise à jour", "Téléchargement en cours...")

            ' Démarre le téléchargement du fichier de mise à jour en arrière-plan
            webClient.DownloadFileAsync(New Uri(updateUrl), "update.zip")

            ' Gère l'événement lorsque le téléchargement est terminé
            AddHandler webClient.DownloadFileCompleted, Async Sub(sender, args)
                                                            ' Réinitialise la barre de progression
                                                            progressController.SetIndeterminate()
                                                            ' Ferme la fenêtre de progression
                                                            Await progressController.CloseAsync()

                                                            ' Code pour extraire et installer les fichiers de mise à jour
                                                            Try
                                                                logger.Debug("Début de l'extraction et de l'installation des fichiers de mise à jour.")

                                                                ' Chemin vers le fichier ZIP de mise à jour téléchargé
                                                                Dim updateZipPath As String = "update.zip"

                                                                ' Chemin vers le dossier où les fichiers de mise à jour seront extraits
                                                                Dim updateExtractPath As String = "update_temp"

                                                                ' Chemin vers le fichier exécutable de mise à jour
                                                                Dim updateExePath As String = Path.Combine(updateExtractPath, "update.exe")

                                                                ' Chemin vers le dossier de l'application actuelle
                                                                Dim appDirectory As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

                                                                ' Extrait les fichiers du ZIP vers le dossier temporaire
                                                                ZipFile.ExtractToDirectory(updateZipPath, updateExtractPath)

                                                                ' Vérifiez si le fichier de mise à jour exécutable existe
                                                                If File.Exists(updateExePath) Then
                                                                    ' Ferme l'application actuelle (peut nécessiter des ajustements pour une fermeture propre)
                                                                    Me.Close()

                                                                    ' Exécute le fichier de mise à jour
                                                                    Process.Start(updateExePath)

                                                                    ' Supprime le fichier ZIP et le dossier temporaire
                                                                    File.Delete(updateZipPath)
                                                                    'Directory.Delete(updateExtractPath, True)

                                                                    logger.Debug("Mise à jour installée avec succès.")
                                                                Else
                                                                    ' En cas d'absence du fichier exécutable de mise à jour, affiche un message d'erreur
                                                                    Await ShowMessageAsync("Erreur de mise à jour", "Le fichier de mise à jour est incomplet ou endommagé.")
                                                                    logger.Error("Le fichier de mise à jour est incomplet ou endommagé.")
                                                                End If
                                                            Catch ex As Exception
                                                                ' Gestion des erreurs lors de l'extraction et de l'installation des fichiers de mise à jour
                                                                ' Vous pouvez afficher un message d'erreur ici ou effectuer d'autres actions appropriées
                                                                logger.Error("Erreur lors de l'extraction et de l'installation des fichiers de mise à jour : " & ex.Message)
                                                            End Try
                                                        End Sub
        Catch ex As Exception
            ' Gère les erreurs qui pourraient se produire lors du téléchargement ou de l'installation de la mise à jour
            logger.Error("Erreur lors du téléchargement et de l'installation de la mise à jour : " & ex.Message)
        End Try
    End Function

    ' Cette fonction obtient l'URL de téléchargement de la mise à jour depuis le fichier JSON de version.
    ' Paramètre channel : Le canal (par exemple "beta" ou "final") dans lequel chercher l'URL de mise à jour.
    ' Renvoie l'URL de téléchargement de la mise à jour, ou Nothing en cas d'erreur.
    Private Async Function GetUpdateUrl(channel As String) As Task(Of String)
        logger.Info("Obtention de l'URL de mise à jour depuis le canal " & channel)
        Try
            ' Construit l'URL du fichier JSON de version sur GitHub en fonction du canal
            Dim apiUrl As String = $"https://raw.githubusercontent.com/{RepoOwner}/{RepoName}/master/releases/{channel}/version.json"
            Dim webClient As New WebClient()

            ' Télécharge les informations de version en tant que chaîne JSON
            Dim versionJson As String = Await webClient.DownloadStringTaskAsync(apiUrl)
            Dim versionObject As JObject = JObject.Parse(versionJson)

            ' Extrait et renvoie l'URL de mise à jour à partir de l'objet JSON
            Dim updateUrl As String = versionObject("updateUrl").ToString()

            ' Ajoute un message de débogage pour indiquer l'URL de mise à jour obtenue
            logger.Debug($"URL de mise à jour obtenue avec succès : {updateUrl}")

            Return updateUrl
        Catch ex As Exception
            ' En cas d'erreur, retourne Nothing et ajoute un message de débogage pour l'erreur
            logger.Error($"Erreur lors de la récupération de l'URL de mise à jour : {ex.Message}")
            Return Nothing
        End Try
    End Function

#End Region


    Private Sub Filetest()
        logger.Info("Verification des fichiers")
        Try
            Dim dossier As String = "HistoricPatient"
            ' Vérifier si le dossier existe
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
            If Not Directory.Exists(dossier) Then
                ' Créer le dossier s'il n'existe pas
                Directory.CreateDirectory(dossier)
                logger.Debug("Creation du dossier Users")
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la creation du dossier Users")
        End Try

    End Sub

    Public Sub CreateTextFile(ByVal Folder As String, ByVal FileName As String, ByVal content As String)
        ' Chemin du fichier que vous souhaitez créer
        Dim filePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Folder, FileName)

        ' Créez le fichier et écrivez le contenu
        File.WriteAllText(filePath, content)

    End Sub

    Public Shared Sub loadExamOption()
        Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "examOptions.json"))
        ExamOptions = JsonConvert.DeserializeObject(Of List(Of ExamOption))(json)
    End Sub

    Public Shared Sub loadSpeedMessage()
        Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "SpeedMessage.json"))
        speedMessages = JsonConvert.DeserializeObject(Of List(Of SpeedMessage))(json)

    End Sub


    Public Shared Function GetLocalIPAddress() As IPAddress
        logger.Info("Obtention de l'adresse IP locale")
        Try
            ' Récupère toutes les interfaces réseau disponibles sur la machine de l'utilisateur
            Dim networkInterfaces As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()

            ' Itère à travers chaque interface réseau
            For Each networkInterface As NetworkInterface In networkInterfaces
                ' Vérifie si l'interface réseau est opérationnelle (en état de fonctionnement)
                If networkInterface.OperationalStatus = OperationalStatus.Up Then
                    ' Obtient les propriétés IP de l'interface réseau
                    Dim ipProperties As IPInterfaceProperties = networkInterface.GetIPProperties()

                    ' Récupère la collection des adresses IP unicast associées à cette interface réseau
                    Dim unicastIPAddresses As UnicastIPAddressInformationCollection = ipProperties.UnicastAddresses

                    ' Itère à travers chaque adresse IP unicast
                    For Each ipAddressInfo As UnicastIPAddressInformation In unicastIPAddresses
                        ' Vérifie si l'adresse IP est de type IPv4 (InterNetwork)
                        If ipAddressInfo.Address.AddressFamily = AddressFamily.InterNetwork Then
                            ' Enregistre un message de débogage indiquant l'adresse IP trouvée
                            logger.Debug("L'adresse IP de l'utilisateur est : " & ipAddressInfo.Address.ToString)

                            ' Renvoie l'adresse IP trouvée
                            Return ipAddressInfo.Address
                        End If
                    Next
                End If
            Next

            ' Si aucune adresse IPv4 n'est trouvée dans aucune interface réseau, renvoie Nothing
            Return Nothing
        Catch ex As Exception
            ' En cas d'erreur lors de l'exécution du code, capture l'exception
            logger.Error("Erreur sur GetLocalIPAddress : " & ex.Message)

            ' Renvoie également Nothing en cas d'exception
            Return Nothing
        End Try
    End Function

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        logger.Info("Fermeture de la fenêtre MainWindow")
        Try
            ' Envoyer un message lors de la fermeture de la fenêtre
            SendMessage("USR02" & My.Settings.UserName & "|" & Environment.UserName)
            logger.Debug("L'envoi du message lors de la fermeture de la fenêtre MainWindow s'est déroulé avec succès.")
        Catch ex As Exception
            ' En cas d'erreur lors de l'envoi du message de fermeture
            logger.Error("Erreur sur MainWindow_Closing lors de l'envoi du message de fermeture : " & ex.Message)
        End Try

        Try
            ' Parcours de la liste des utilisateurs
            For Each user As User In Users
                ' Vérification du nom de l'utilisateur
                If user.Name = "A Tous" OrElse user.Name = "Secrétariat" Then
                    ' Si l'utilisateur a le nom "A Tous" ou "Secrétariat", définissez la valeur de status à "0/0"
                    user.Status = "0/0"
                Else
                    ' Sinon, définissez la valeur de status à "Offline"
                    user.Status = "Offline"
                End If
            Next
            ' Sauvegarder la liste des utilisateurs dans un fichier JSON
            SaveUsersToJson(Users)
            ' Enregistrer les paramètres de l'application
            logger.Debug("Mise a jour et sauvegarde des user lors de la fermeture de l'application")
        Catch ex As Exception
            ' En cas d'erreur lors du traitement de la liste des utilisateurs
            logger.Error("Erreur sur MainWindow_Closing lors du traitement de la liste des utilisateurs : " & ex.Message)
        End Try
    End Sub


End Class




