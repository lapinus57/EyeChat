Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.UI.WebControls
Imports System.Windows.Interop
Imports System.Windows.Threading
Imports EyeChat.Computer
Imports EyeChat.Message
Imports EyeChat.PatientBubbleCtrl
Imports EyeChat.Planning
Imports EyeChat.User
Imports log4net
Imports log4net.Config
Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro.SimpleChildWindow
Imports System.Timers
Imports Newtonsoft.Json



Public Class EggPhrasesData
    Public Property MarvinPhrases As List(Of String)
    Public Property JoyPhrases As List(Of String)
End Class


Class MainWindow
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _orthoMode As Boolean

    Public Shared selectedUserName As String
    Public Shared Property Computers As ObservableCollection(Of Computer)

    Public Shared Property ExamOptions As New ObservableCollection(Of ExamOption)()

    Public Shared Property speedMessages As New List(Of SpeedMessage)()

    Public Shared Property Plannings As New ObservableCollection(Of Planning)()
    Public Shared Property Messages As ObservableCollection(Of Message)
    Public Shared Property Users As ObservableCollection(Of User)
    Public Shared Property SelectedUserMessages As ObservableCollection(Of Message)
    Public Shared Property SelectedUser As String

    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public Property DebugLevels As New ObservableCollection(Of String)() From {"DEBUG", "INFO", "WARN", "ERROR"}


    Public settingsMap As New Dictionary(Of String, Dictionary(Of String, String))


    Private timer As DispatcherTimer


    Private Shared ReadOnly SuggestionValues As String() = {"/DEBUG", "/ENDDEBUG", "/LSTCOMPUTER", "/DISPCOMPUTER", "/TESTPATIENT", "/TESTSENDFILE", "/TESTMARVIN"}

    Dim jsonData As String
    Public Shared phrasesData As EggPhrasesData



    Public Shared _userSettingsMain As UserSettings


    Structure UserUpdateInfo
        Dim UserName As String
        Dim LastIdentifiantPC As String
    End Structure

    ' Stocker les informations de mise à jour pour chaque utilisateur
    Dim userUpdates As New Dictionary(Of String, UserUpdateInfo)()

#Region "HotKeyManager"

    Private _hotKeyManager As HotKeyManager

    Protected Overrides Sub OnSourceInitialized(e As EventArgs)
        MyBase.OnSourceInitialized(e)
        Dim helper = New WindowInteropHelper(Me)
        _hotKeyManager = New HotKeyManager(HwndSource.FromHwnd(helper.Handle))
        _hotKeyManager.RegisterHotKeys(helper, _userSettingsMain)
    End Sub

    Protected Overrides Sub OnClosed(e As EventArgs)
        Dim helper = New WindowInteropHelper(Me)
        _hotKeyManager.UnregisterHotKeys(helper)
        MyBase.OnClosed(e)
    End Sub
#End Region

#Region "PostitEditor"
    ' Définir la variable pour suivre l'instance de PostitEditor
    Private postItWindowInstance As PostitEditor
#End Region

#Region "FileWatcher"
    Private fileWatcher As FileWatcherSV
    Private iniFilePath As String = "c:\studiov2000\STUDIOV.ini"
    Private _numPatValue As Integer
    Public Property NumPatValue As Integer
        Get
            Return _numPatValue
        End Get
        Set(value As Integer)
            _numPatValue = value
            ' Mettre à jour l'interface utilisateur ou effectuer d'autres actions nécessaires
        End Set
    End Property
#End Region

#Region "PatientManager"

    'Liste des patients du RDC
    Public Shared Property PatientsRDC As ObservableCollection(Of Patient)
    'Liste des parients du 1er
    Public Shared Property Patients1er As ObservableCollection(Of Patient)
    'liste de tous les patients
    Public Shared Property PatientsALL As ObservableCollection(Of Patient)

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
#End Region


#Region "test"






    Private Function ResolveAssembly(sender As Object, args As ResolveEventArgs) As System.Reflection.Assembly
        Dim folderPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll")
        Dim assemblyName As New System.Reflection.AssemblyName(args.Name)
        Dim assemblyPath As String = System.IO.Path.Combine(folderPath, assemblyName.Name & ".dll")

        If System.IO.File.Exists(assemblyPath) Then
            Return System.Reflection.Assembly.LoadFrom(assemblyPath)
        End If

        Return Nothing
    End Function








    ' Importation de la fonction SetForegroundWindow de user32.dll
    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As Boolean
    End Function

    Private checkProcessTimer As System.Timers.Timer


    ' Délégation pour EnumWindows
    Private Delegate Function EnumWindowsProc2(hWnd As IntPtr, lParam As IntPtr) As Boolean
    Dim patientInfo As String = String.Format("Mr MULLLER Benoit CS 6 (donné) - 1 {0,-10} (Prog + prox)" & vbCrLf &
                                          "{1,-10} (Mensuelle + journalière)" & vbCrLf &
                                          "{2,-10}" & vbCrLf &
                                          "Supprimer RDV dans 6 mois", "LUN*2", "LEN*2", "TT*2")
    Public Shared isPostItOpened As Boolean = False ' Suivre l'état d'ouverture du Post-it
    Public Shared postItWindow As PostItWindow = Nothing ' Référence à la fenêtre Post-it
    Private wasPatientWindowFound As Boolean = False ' Suivre si la fenêtre [Fiche PATIENT] a été trouvée lors du dernier tick

    ' Méthode pour parcourir les fenêtres et vérifier les titres
    Private Sub CheckForPatientWindow()
        EnumWindows(AddressOf EnumWindowsCallback2, IntPtr.Zero)
    End Sub

    ' Callback pour EnumWindows
    Private Function EnumWindowsCallback2(hWnd As IntPtr, lParam As IntPtr) As Boolean
        Dim title As New StringBuilder(256)
        GetWindowText(hWnd, title, title.Capacity)

        ' Vérifier si le titre contient "[Fiche PATIENT]"
        If title.ToString().Contains("[Fiche PATIENTS") Then
            ' Si la fenêtre a été trouvée pour la première fois ou après qu'elle ait été fermée
            If Not wasPatientWindowFound Then
                Me.Dispatcher.Invoke(Sub()
                                         If Not isPostItOpened Then

                                             postItWindow = New PostItWindow(patientInfo)
                                             postItWindow.Show()
                                             isPostItOpened = True
                                         End If
                                     End Sub)
            End If
            wasPatientWindowFound = True ' La fenêtre a été trouvée
            Return False ' Arrêter l'énumération après avoir trouvé la fenêtre
        End If

        Return True ' Continuer l'énumération
    End Function

    ' Appeler cette méthode pour vérifier si le post-it doit être fermé
    Private Sub checkProcessTimer_Tick(sender As Object, e As ElapsedEventArgs)
        ' Appeler la méthode qui vérifie la présence d'une fenêtre avec "[Fiche PATIENT]" dans son titre
        CheckForPatientWindow()

        ' Si aucune fenêtre "[Fiche PATIENT]" n'est trouvée et si elle était trouvée auparavant
        If Not wasPatientWindowFound AndAlso isPostItOpened Then
            Me.Dispatcher.Invoke(Sub()
                                     If postItWindow IsNot Nothing AndAlso postItWindow.IsVisible Then
                                         postItWindow.CloseProgrammatically()
                                         postItWindow = Nothing
                                         isPostItOpened = False
                                     End If
                                 End Sub)
        End If

        ' Réinitialiser l'état de la fenêtre trouvée
        wasPatientWindowFound = False
    End Sub

    Private Sub OpenPostIt(content As String)
        Dim postItWindow As New PostItWindow(content)
        postItWindow.Show()
    End Sub



#End Region



    Public Property OrthoMode As Boolean
        Get
            Return _orthoMode
        End Get
        Set(value As Boolean)
            If _orthoMode <> value Then
                _orthoMode = value
                OnPropertyChanged(NameOf(OrthoMode))
            End If
        End Set
    End Property

    Protected Sub OnPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub


    Private Sub UpdateOrthoMode()
        OrthoMode = MainWindow._userSettingsMain.OrthoMode
        ' Assurez-vous d'appeler cette méthode lorsque vous savez que My.Settings.OrthoMode a changé
    End Sub

#Region "Gestion Capture nom des fêntre"
    <DllImport("user32.dll")>
    Public Shared Function EnumWindows(ByVal lpEnumFunc As EnumWindowsDelegate, ByVal lParam As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowText(ByVal hWnd As IntPtr, ByVal lpString As StringBuilder, ByVal nMaxCount As Integer) As Integer
    End Function

    Public Delegate Function EnumWindowsDelegate(ByVal hWnd As IntPtr, ByVal lParam As IntPtr) As Boolean

#End Region

#Region "Gestion des raccourcis clavier"






    Private Sub OnHotKeyPressed10()

        EnumWindows(AddressOf ReturnStartString, IntPtr.Zero)

    End Sub

    Private Sub PasteTextToActiveWindow(text As String)
        ' Stocker le texte dans le presse-papiers
        Clipboard.SetText(text)
        'PasteText()
    End Sub






#End Region

#Region "Fênetre Patient Exams"

    Public Sub OpenPatientDialogue(ByVal Eye As String, ByVal ExamName As String, ByVal Floor As String)
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

    Public Sub New()
        InitializeComponent()
        _userSettingsMain = UserSettings.Load()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        _userSettingsMain = UserSettings.Load()
        'test

        ' Initialisation du Timer pour vérifier toutes les 2 secondes
        checkProcessTimer = New System.Timers.Timer(1000) ' Intervalle en millisecondes
        AddHandler checkProcessTimer.Elapsed, AddressOf checkProcessTimer_Tick
        checkProcessTimer.Start()

        ' Initialiser le FileWatcherSV
        fileWatcher = New FileWatcherSV(iniFilePath, Me)
        'fin test



        If IsNameInList(Users, "A Tous") Then
            ' SelectUser("A Tous")
            ' SelectUserByName("A Tous")
        End If



        ' Créer un nouvel onglet
        Dim newTabItem As New TabItem With {
    .Header = "Nouveau Patient", ' Texte de l'onglet
    .Content = New TextBlock With {.Text = "Contenu de l'onglet"} ' Contenu de l'onglet
}

        ' Ajouter l'onglet au MetroTabControl nommé "PatientTabCtrl"
        'PatientTabCtrl.Items.Add(newTabItem)
    End Sub


    Public Shared Sub loadExamOption()
        Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "examOptions.json"))
        ExamOptions = JsonConvert.DeserializeObject(Of ObservableCollection(Of ExamOption))(json)
    End Sub



    Public ReadOnly Property AppSizeDisplay As Double
        Get
            Try
                logger.Info("Accès à la propriété AppSizeDisplay")
                Return _userSettingsMain.AppSizeDisplay
                My.Application.MainWindow.FontSize = _userSettingsMain.AppSizeDisplay
                MahApps.Metro.Controls.HeaderedControlHelper.SetHeaderFontSize(PatientTabCtrl, CInt(_userSettingsMain.AppSizeDisplay))
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppSizeDisplay : {ex.Message}")
                Return 10.0 ' Valeur par défaut en cas d'erreur
            End Try
        End Get
    End Property
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        _userSettingsMain = UserSettings.Load()

        'CheckForUpdates("beta")
        XmlConfigurator.Configure()

        ' Initialisation des dossiers et vérification des paramètres
        InitFirstload.Load()

        Me.ShowCloseButton = False

        InitializeComponent()
        UpdateOrthoMode()


        Select Case _userSettingsMain.DebugLevel
            Case "DEBUG"
                logger.Logger.Repository.Threshold = log4net.Core.Level.Debug
            Case "INFO"
                logger.Logger.Repository.Threshold = log4net.Core.Level.Info
            Case "WARN"
                logger.Logger.Repository.Threshold = log4net.Core.Level.Warn
            Case "ERROR"
                logger.Logger.Repository.Threshold = log4net.Core.Level.Error
        End Select



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



        'SavePatientsToJson(Patients)
        'SelectUserList("Benoit")
        'SelectUserList("benoit")


        'PatientManager.PatientAdd("Mr", "muller", "benoit", "SK", "od", "RDC", "Blue", "benoit")
        'PatientAdd("Mr", "durand", "benoit", "SK", Nothing, "1er", "Green", "benoit", Date.Now)

        '''''''''
        'jsonData = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "dataphrases.json"))
        'phrasesData = JsonConvert.DeserializeObject(Of EggPhrasesData)(jsonData)
        ''''''''''''

        'Charge les options d'examen à partir du fichier JSON
        JsonManager.loadExamOption()



        MahApps.Metro.Controls.HeaderedControlHelper.SetHeaderFontSize(PatientTabCtrl, CInt(_userSettingsMain.AppSizeDisplay))


        ' Charger les messages à partir du fichier JSON
        JsonManager.loadSpeedMessage()
        JsonManager.EggPhrases()

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

        loadExamOption()

        ' Initialisation de SendManager

        SendManager.InitializeSender()


        ' Initialisation de ReceiveManager
        Dim receiveManager As New ReceiveManager(Dispatcher, Me)

        ' Démarrer la réception de messages
        receiveManager.StartReceiving()

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

        AddHandler My.Settings.PropertyChanged, AddressOf SettingsChanged

        'NfcManager.HandleNfcMode(_userSettingsMain)



        'SendManager.SendMessage("USR01" & _userSettingsMain.UserName & "|" & Environment.UserName)

        If IsNameInList(Users, "Alix") Then
            SelectUser("Alix")
            SelectUserByName("Alix")
        End If
    End Sub

    Private _settings As SettingsViewModel

    Public ReadOnly Property Settings As SettingsViewModel
        Get
            If _settings Is Nothing Then
                _settings = New SettingsViewModel()
            End If
            Return _settings
        End Get
    End Property

    Private Sub SettingsChanged(sender As Object, e As PropertyChangedEventArgs)
        ' Mettez à jour le dictionnaire ici
        InitializeSettingsMaps(settingsMap)
    End Sub






    Public Shared Sub hidePatientList()
        Dim mainWindow As MainWindow = Application.Current.MainWindow
        mainWindow.PatientListRDC.Visibility = Visibility.Hidden
        mainWindow.PatientList1er.Visibility = Visibility.Hidden
        mainWindow.PatientListAll.Visibility = Visibility.Hidden
        mainWindow.MessageList.Visibility = Visibility.Hidden
    End Sub

    Public Shared Sub showPatientList()
        Dim mainWindow As MainWindow = Application.Current.MainWindow
        mainWindow.PatientListRDC.Visibility = Visibility.Visible
        mainWindow.PatientList1er.Visibility = Visibility.Visible
        mainWindow.PatientListAll.Visibility = Visibility.Visible
        mainWindow.MessageList.Visibility = Visibility.Visible
    End Sub




    Private Sub Timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        ' Code à exécuter à 8h00 précises
        SendManager.SendMessage("USR02" & _userSettingsMain.UserName & "|" & My.Settings.WindowsName)

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

        'SendManager.SendMessage("USR01Benoit1|" & My.Settings.WindowsName)


        ' Arrêtez la minuterie si nécessaire pour éviter d'exécuter l'action à plusieurs reprises
        timer.Stop()
    End Sub

    Private Sub MessageMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedMessage As SpeedMessage = DirectCast(DirectCast(sender, System.Windows.Controls.MenuItem).Tag, SpeedMessage)
        '"SMF01UserName|Destinataitre|message|Option1|Option2|Option3"
        SendManager.SendMessage("SMF01" & _userSettingsMain.UserName & "|" & selectedMessage.Destinataire & "|" & selectedMessage.Message.Replace("[ROOM]", My.Settings.WindowsName) & "|" & selectedMessage.Options)
        SendManager.SendMessage("MSG01" & _userSettingsMain.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & selectedMessage.Destinataire & " " & selectedMessage.Message.Replace("[ROOM]", My.Settings.WindowsName) & "|" & _userSettingsMain.UserAvatar)



    End Sub


    Private Async Function LaunchGitHubSiteAsync() As Task
        ' Exemple : Ouvrir le site GitHub dans le navigateur par défaut
        Dim url As String = "https://github.com/lapinus57/EyeChat"
        Process.Start(url)
        Await ShowMessageAsync("This is the title", "Some message")

    End Function




    ' Méthode pour ajouter un nouveau message
    Public Sub AddMessage(ByVal name As String, ByVal sender As String, ByVal room As String, ByVal content As String, ByVal isAlignedRight As Boolean, ByVal avatar As String)
        If Messages IsNot Nothing Then
            Messages.Add(New Message With {.Name = name, .Sender = sender, .Room = room, .Content = content, .IsAlignedRight = isAlignedRight, .Timestamp = DateTime.Now, .Avatar = avatar})
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





    Public Sub SetHeaderFontSize(ByVal value As Integer)
        ' Exemple de mise à jour de la taille des en-têtes
        MahApps.Metro.Controls.HeaderedControlHelper.SetHeaderFontSize(PatientTabCtrl, CInt(value))
        For Each tabItem As TabItem In PatientTabCtrl.Items
            tabItem.FontSize = value
        Next
        Me.UpdateLayout() ' Assurez-vous que la mise à jour de la disposition est appelée ici
    End Sub

    Public Sub Updatemsglist()
        SelectUser(SelectedUser)
        'MahApps.Metro.Controls.HeaderedControlHelper.SetHeaderFontSize(PatientTabCtrl, CInt(SettingsViewModel.))
    End Sub



#Region "Gestion UDP et messages"






    Public Async Sub SpeedMessageDialog(ByVal Titre As String, ByVal Message As String, ByVal option1 As String, ByVal option2 As String, ByVal option3 As String)
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
            SendManager.SendMessage(message)
        End If
    End Sub

    Private Function GetMessage(ByVal selectedOption As String, ByVal Titre As String) As String
        Select Case selectedOption
            Case "0"
                Return "MSG01" & _userSettingsMain.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient au plus vite" & "|" & _userSettingsMain.UserAvatar
            Case "1"
                Return "MSG01" & _userSettingsMain.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient dans 2 minutes " & "|" & _userSettingsMain.UserAvatar
            Case "2"
                Return "MSG01" & _userSettingsMain.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient dans 5 minutes " & "|" & _userSettingsMain.UserAvatar
            Case "3"
                Return "MSG01" & _userSettingsMain.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " je vient dans 10 minutes " & "|" & _userSettingsMain.UserAvatar
            Case "A"
                Return "MSG01" & _userSettingsMain.UserName & "|A Tous|" & My.Settings.WindowsName & "|" & Titre & " met en attente " & "|" & _userSettingsMain.UserAvatar
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

                            Me.UsersDialogBox.SetCurrentValue(ChildWindow.IsOpenProperty, True)

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
                            SendManager.SendMessage("DBG01")
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
                            SendManager.SendMessage(message)

                        Next

                    Case "/TESTSENDFILE"

                        CreateTextFile("Core", "test.txt", "fichier test")
                        FilesTransferManager.SendFileOverNetwork("Core", "test.txt")

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

                            Dim Text As String = "PTN01" & patientTitre & "|" & patientNom & "|" & patientPrenom & "|" & codeMSG & "|" & annotation & "|RDC|" & _userSettingsMain.UserName & "|" & Date.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
                            SendManager.SendMessage(Text)


                        Else



                            ' C'est un message simple
                            ' Formate le message sous la forme "MSG01{_userSettingsMain.UserName}|{selectedUser.Name}|{Message}|{Avatar}"
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

                                    text = "MSG01" & _userSettingsMain.UserName & "|" & selectedUserName & "|" & My.Settings.WindowsName & "|" & SendTextBox.Text & "|" & _userSettingsMain.UserAvatar
                                    SendManager.SendMessage(text)
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
        UpdateList()
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
        'logger.Debug($" {_userSettingsMain.UserName} s'est déconnecté du poste.")
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
        'SendMessage("USR02" & _userSettingsMain.UserName & "|" & My.Settings.WindowsName)
        Return Task.CompletedTask

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


    Public Sub SelectUserByName(userName As String)
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
            ' Notification de changement pour SelectedListUser
            OnPropertyChanged(NameOf(SelectedListUser))
            If _selectedListUser IsNot Nothing Then
                Dim selectedUserName As String = _selectedListUser.Name
                ' Utilisez le nom de l'utilisateur sélectionné comme vous le souhaitez
            End If
        End Set
    End Property





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
        Text += _userSettingsMain.UserName + "|" + Date.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")

        ' Appeler la fonction Sendmessage avec la chaîne de texte du patient
        SendManager.SendMessage(Text)

    End Sub

    Public Function EnumWindowCallBack(ByVal hwnd As IntPtr, ByVal lParam As IntPtr) As Boolean
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
                    patientTitre = ""
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


#Region "Gestion des entêtes"
    Private Sub InitializeSettingsMaps(ByRef settingsMap As Dictionary(Of String, Dictionary(Of String, String)))
        ' Initialisation des mappings pour F5, F6, et F7
        settingsMap.Add("F5", New Dictionary(Of String, String) From {
        {"REFRACTION", "My.Settings.F5Text1"},
        {"LENTILLES", "My.Settings.F5Text2"},
        {"PATHOLOGIES", "My.Settings.F5Text3"},
        {"ORTHOPTIE", "My.Settings.F5Text4"},
        {"TRAITEMENT", "My.Settings.F5Text5"}
    })
        settingsMap.Add("F6", New Dictionary(Of String, String) From {
        {"REFRACTION", "My.Settings.F6Text1"},
        {"LENTILLES", "My.Settings.F6Text2"},
        {"PATHOLOGIES", "My.Settings.F6Text3"},
        {"ORTHOPTIE", "My.Settings.F6Text4"},
        {"TRAITEMENT", "My.Settings.F6Text5"}
    })
        settingsMap.Add("F7", New Dictionary(Of String, String) From {
        {"REFRACTION", "My.Settings.F7Text1"},
        {"LENTILLES", "My.Settings.F7Text2"},
        {"PATHOLOGIES", "My.Settings.F7Text3"},
        {"ORTHOPTIE", "My.Settings.F7Text4"},
        {"TRAITEMENT", "My.Settings.F7Text5"}
    })
        settingsMap.Add("F8", New Dictionary(Of String, String) From {
       {"REFRACTION", "My.Settings.F8Text1"},
       {"LENTILLES", "My.Settings.F8Text2"},
       {"PATHOLOGIES", "My.Settings.F8Text3"},
       {"ORTHOPTIE", "My.Settings.F8Text4"},
       {"TRAITEMENT", "My.Settings.F8Text5"}
   })
    End Sub

    Private Function ReturnStartString(ByVal hwnd As IntPtr, ByVal lParam As IntPtr) As Boolean
        Try
            Dim windowText As New StringBuilder(256)
            GetWindowText(hwnd, windowText, windowText.Capacity)
            Dim text As String = windowText.ToString().Trim()
            If text.Length > 0 Then
                If text.StartsWith("REFRACTION") Then

                    PasteTextToActiveWindow(TextConnsultation("My.Settings.F5Text1"))

                ElseIf text.Contains("LENTILLES") Then

                    PasteTextToActiveWindow(TextConnsultation("My.Settings.F5Text2"))

                ElseIf text.Contains("ORTHOPTIE") Then

                    PasteTextToActiveWindow(TextConnsultation("My.Settings.F5Text4"))


                ElseIf text.Contains("PATHOLOGIES") Then

                    PasteTextToActiveWindow(TextConnsultation("My.Settings.F5Text3"))
                ElseIf text.Contains("TRAITEMENT") Then

                    PasteTextToActiveWindow(TextConnsultation("My.Settings.F5Text5"))

                End If
            End If
        Catch ex As Exception
            ' Gestion des erreurs qui pourraient se produire lors de l'extraction des informations
            ' Vous pouvez ajouter des logs ici ou effectuer d'autres actions appropriées en cas d'erreur.
            logger.Error("Erreur lors de l'extraction des informations du texte de la fenêtre : " & ex.Message)
        End Try
        Return True
    End Function


    Public Function TextConnsultation(ByVal test As String) As String
        ' Trouver l'utilisateur actuel basé sur un critère, ici je prends le premier pour l'exemple
        Dim currentUser As User = Users.FirstOrDefault(Function(u) u.Name = _userSettingsMain.UserName)
        Dim text As String = test
        text = text.Replace("[IN]", currentUser.Initials)
        text = text.Replace("[ROOM]", Environment.UserName)
        text = text.Replace("[NEWLINE]", vbNewLine)
        text = text.Replace("[TIME]", DateTime.Now.ToString("HH:mm"))
        Return text
    End Function

#End Region


    Public Sub CreateTextFile(ByVal Folder As String, ByVal FileName As String, ByVal content As String)
        ' Chemin du fichier que vous souhaitez créer
        Dim filePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Folder, FileName)

        ' Créez le fichier et écrivez le contenu
        File.WriteAllText(filePath, content)

    End Sub

    Public Function GetSelectedUserName() As String
        Dim selectedUser As User = CType(Me.ListUseres.SelectedItem, User)
        If selectedUser IsNot Nothing Then
            Return selectedUser.Name
        End If
        Return "A Tous"
    End Function

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        logger.Info("Fermeture de la fenêtre MainWindow")
        Try
            ' Envoyer un message lors de la fermeture de la fenêtre
            SendManager.SendMessage("USR02" & _userSettingsMain.UserName & "|" & Environment.UserName)
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
