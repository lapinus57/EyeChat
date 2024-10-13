Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Interop
Imports log4net

Public Class HotKeyManager
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

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
    Private Const HOTKEY_ID10 As Integer = 9010

    Const VK_E As UInteger = &H45
    Const MOD_CTRL As UInteger = &H2
    Const MOD_SHIFT As UInteger = &H4
    Const VK_F1 As UInteger = &H70
    Const VK_F2 As UInteger = &H71
    Const VK_F5 As UInteger = &H74
    Const VK_F6 As UInteger = &H75
    Const VK_F7 As UInteger = &H76
    Const VK_F8 As UInteger = &H77
    Const VK_F9 As UInteger = &H78
    Const VK_F10 As UInteger = &H79
    Const VK_F11 As UInteger = &H7A
    Const VK_F12 As UInteger = &H7B

    'test
    Const KEYEVENTF_KEYUP As UInteger = &H2
    Const VK_CONTROL As Integer = &H11
    Const VK_V As Integer = &H56

    <DllImport("User32.dll")>
    Private Shared Function RegisterHotKey(<[In]> hWnd As IntPtr, <[In]> id As Integer, <[In]> fsModifiers As UInteger, <[In]> vk As UInteger) As Boolean
    End Function

    <DllImport("User32.dll")>
    Private Shared Function UnregisterHotKey(<[In]> hWnd As IntPtr, <[In]> id As Integer) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Sub keybd_event(bVk As Byte, bScan As Byte, dwFlags As UInteger, dwExtraInfo As UInteger)
    End Sub

    Public Sub New(source As HwndSource)
        _source = source
        _source.AddHook(AddressOf HwndHook)
    End Sub

    Public Sub RegisterHotKeys(helper As WindowInteropHelper, userSettings As UserSettings)
        logger.Info("Enregistrement des raccourcis clavier")
        UnregisterHotKeys(helper)

        RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_E)
        RegisterHotKey(helper.Handle, HOTKEY_ID10, 0, VK_F5)
        RegisterHotKey(helper.Handle, HOTKEY_ID4, MOD_CTRL, VK_F12)

        If userSettings.CtrlF9Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID1, MOD_CTRL, VK_F9)
        End If
        If userSettings.CtrlF10Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID2, MOD_CTRL, VK_F10)
        End If
        If userSettings.CtrlF11Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID3, MOD_CTRL, VK_F11)
        End If

        If userSettings.ShiftF9Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID5, MOD_SHIFT, VK_F9)
        End If
        If userSettings.ShiftF10Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID6, MOD_SHIFT, VK_F10)
        End If
        If userSettings.ShiftF11Enabled Then
            RegisterHotKey(helper.Handle, HOTKEY_ID7, MOD_SHIFT, VK_F11)
        End If

    End Sub

    Public Sub UnregisterHotKeys(helper As WindowInteropHelper)
        logger.Info("Désenregistrement des raccourcis clavier")
        UnregisterHotKey(helper.Handle, HOTKEY_ID)
        UnregisterHotKey(helper.Handle, HOTKEY_ID1)
        UnregisterHotKey(helper.Handle, HOTKEY_ID2)
        UnregisterHotKey(helper.Handle, HOTKEY_ID3)
        UnregisterHotKey(helper.Handle, HOTKEY_ID4)
        UnregisterHotKey(helper.Handle, HOTKEY_ID5)
        UnregisterHotKey(helper.Handle, HOTKEY_ID6)
        UnregisterHotKey(helper.Handle, HOTKEY_ID7)
        UnregisterHotKey(helper.Handle, HOTKEY_ID8)
        UnregisterHotKey(helper.Handle, HOTKEY_ID9)
        UnregisterHotKey(helper.Handle, HOTKEY_ID10)
    End Sub

    Private Function HwndHook(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, ByRef handled As Boolean) As IntPtr
        Const WM_HOTKEY As Integer = &H312
        Select Case msg
            Case WM_HOTKEY
                Select Case wParam.ToInt32()
                    Case HOTKEY_ID
                        logger.Info("Raccourci clavier Ctrl + E")
                        OnHotKeyPressed0()
                    Case HOTKEY_ID1
                        logger.Info("Raccourci clavier Ctrl + F9")
                        OnHotKeyPressed1()
                    Case HOTKEY_ID2
                        logger.Info("Raccourci clavier Ctrl + F10")
                        OnHotKeyPressed2()
                    Case HOTKEY_ID3
                        logger.Info("Raccourci clavier Ctrl + F11")
                        OnHotKeyPressed3()
                    Case HOTKEY_ID4
                        logger.Info("Raccourci clavier Ctrl + F12")
                        OnHotKeyPressed4()
                    Case HOTKEY_ID5
                        logger.Info("Raccourci clavier Shift + F9")
                        OnHotKeyPressed5()
                    Case HOTKEY_ID6
                        logger.Info("Raccourci clavier Shift + F10")
                        OnHotKeyPressed6()
                    Case HOTKEY_ID7
                        logger.Info("Raccourci clavier Shift + F11")
                        OnHotKeyPressed7()
                    Case HOTKEY_ID8
                        logger.Info("Raccourci clavier Shift + F12")
                        OnHotKeyPressed8()
                    Case HOTKEY_ID10
                        logger.Info("Raccourci clavier F5")
                        OnHotKeyPressed10()
                End Select
        End Select
        Return IntPtr.Zero
    End Function

    'Raccourci Ctrl + E
    Private Sub OnHotKeyPressed0()
        ' Implémentation de la méthode
        Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

        mainWindow.WindowState = WindowState.Normal
        mainWindow.Topmost = True
        mainWindow.Topmost = False
        mainWindow.Focus()
    End Sub

    'Raccourci Ctrl + F9
    Private Sub OnHotKeyPressed1()
        ' Implémentation de la méthode
        Try
            ' Obtenir une référence à la fenêtre principale
            Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

            ' Journaliser l'exécution du raccourci
            logger.Debug("Exécution du raccourci Ctrl + F9")

            ' Restaurer la fenêtre principale si elle est minimisée ou maximisée
            mainWindow.WindowState = WindowState.Normal

            ' Rendre la fenêtre principale temporairement Topmost pour attirer l'attention
            mainWindow.Topmost = True
            mainWindow.Topmost = False

            ' Donner le focus à la fenêtre principale
            mainWindow.Focus()

            ' Ouvrir le dialogue patient avec des paramètres spécifiques
            mainWindow.OpenPatientDialogue("ODG", MainWindow._userSettingsMain.CtrlF9, "RDC")

            ' Créer une liste de fenêtres et énumérer les fenêtres
            Dim windowList As New List(Of String)()
            mainWindow.EnumWindows(AddressOf mainWindow.EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            ' Journaliser l'erreur si quelque chose ne va pas
            logger.Error("Erreur lors de l'éxécution du raccourci Ctrl + F9 : " & ex.Message)
        End Try
    End Sub


    'Raccourci Ctrl + F10
    Private Sub OnHotKeyPressed2()
        ' Implémentation de la méthode
        Try
            ' Obtenir une référence à la fenêtre principale
            Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

            ' Journaliser l'exécution du raccourci
            logger.Debug("Exécution du raccourci Ctrl + F10")

            ' Restaurer la fenêtre principale si elle est minimisée ou maximisée
            mainWindow.WindowState = WindowState.Normal

            ' Rendre la fenêtre principale temporairement Topmost pour attirer l'attention
            mainWindow.Topmost = True
            mainWindow.Topmost = False

            ' Donner le focus à la fenêtre principale
            mainWindow.Focus()

            ' Ouvrir le dialogue patient avec des paramètres spécifiques
            mainWindow.OpenPatientDialogue("ODG", MainWindow._userSettingsMain.CtrlF10, "RDC")

            ' Créer une liste de fenêtres et énumérer les fenêtres
            Dim windowList As New List(Of String)()
            MainWindow.EnumWindows(AddressOf mainWindow.EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            ' Journaliser l'erreur si quelque chose ne va pas
            logger.Error("Erreur lors de l'éxécution du raccourci Ctrl + F10 : " & ex.Message)
        End Try
    End Sub

    'Raccourci Ctrl + F11
    Private Sub OnHotKeyPressed3()
        ' Implémentation de la méthode
        Try
            ' Obtenir une référence à la fenêtre principale
            Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

            ' Journaliser l'exécution du raccourci
            logger.Debug("Exécution du raccourci Ctrl + F11")

            ' Restaurer la fenêtre principale si elle est minimisée ou maximisée
            mainWindow.WindowState = WindowState.Normal

            ' Rendre la fenêtre principale temporairement Topmost pour attirer l'attention
            mainWindow.Topmost = True
            mainWindow.Topmost = False

            ' Donner le focus à la fenêtre principale
            mainWindow.Focus()

            ' Ouvrir le dialogue patient avec des paramètres spécifiques
            mainWindow.OpenPatientDialogue("ODG", MainWindow._userSettingsMain.CtrlF11, "RDC")

            ' Créer une liste de fenêtres et énumérer les fenêtres
            Dim windowList As New List(Of String)()
            MainWindow.EnumWindows(AddressOf mainWindow.EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            ' Journaliser l'erreur si quelque chose ne va pas
            logger.Error("Erreur lors de l'éxécution du raccourci Ctrl + F11 : " & ex.Message)
        End Try
    End Sub

    'Raccourci Ctrl + F12
    Private Sub OnHotKeyPressed4()
        ' Implémentation de la méthode
    End Sub

    'Raccourci Shift + F9
    Private Sub OnHotKeyPressed5()
        ' Implémentation de la méthode
        Try
            ' Obtenir une référence à la fenêtre principale
            Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

            ' Journaliser l'exécution du raccourci
            logger.Debug("Exécution du raccourci Shift + F9")

            ' Restaurer la fenêtre principale si elle est minimisée ou maximisée
            mainWindow.WindowState = WindowState.Normal

            ' Rendre la fenêtre principale temporairement Topmost pour attirer l'attention
            mainWindow.Topmost = True
            mainWindow.Topmost = False

            ' Donner le focus à la fenêtre principale
            mainWindow.Focus()

            ' Ouvrir le dialogue patient avec des paramètres spécifiques
            mainWindow.OpenPatientDialogue("ODG", MainWindow._userSettingsMain.ShiftF9, "RDC")

            ' Créer une liste de fenêtres et énumérer les fenêtres
            Dim windowList As New List(Of String)()
            MainWindow.EnumWindows(AddressOf mainWindow.EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            ' Journaliser l'erreur si quelque chose ne va pas
            logger.Error("Erreur lors de l'éxécution du raccourci Shift + F9 : " & ex.Message)
        End Try
    End Sub

    'Raccourci Shift + F10
    Private Sub OnHotKeyPressed6()
        Try
            ' Obtenir une référence à la fenêtre principale
            Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

            ' Journaliser l'exécution du raccourci
            logger.Debug("Exécution du raccourci Shift + F10")

            ' Restaurer la fenêtre principale si elle est minimisée ou maximisée
            mainWindow.WindowState = WindowState.Normal

            ' Rendre la fenêtre principale temporairement Top
            mainWindow.Topmost = True
            mainWindow.Topmost = False

            'donner le focus à la fenêtre principale
            mainWindow.Focus()

            ' Ouvrir le dialogue patient avec des paramètres spécifiques
            mainWindow.OpenPatientDialogue("ODG", MainWindow._userSettingsMain.ShiftF10, "RDC")

            ' Créer une liste de fenêtres et énumérer les fenêtres
            Dim windowList As New List(Of String)()
            MainWindow.EnumWindows(AddressOf mainWindow.EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            ' Journaliser l'erreur si quelque chose ne va pas
            logger.Error("Erreur lors de l'éxécution du raccourci Shift + F10 : " & ex.Message)
        End Try
    End Sub

    'Raccourci Shift + F11
    Private Sub OnHotKeyPressed7()
        ' Implémentation de la méthode
        Try
            ' Obtenir une référence à la fenêtre principale
            Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)

            ' Journaliser l'exécution du raccourci
            logger.Debug("Exécution du raccourci Shift + F11")

            ' Restaurer la fenêtre principale si elle est minimisée ou maximisée
            mainWindow.WindowState = WindowState.Normal

            ' Rendre la fenêtre principale temporairement Topmost pour attirer l'attention
            mainWindow.Topmost = True
            mainWindow.Topmost = False

            ' Donner le focus à la fenêtre principale
            mainWindow.Focus()

            ' Ouvrir le dialogue patient avec des paramètres spécifiques
            mainWindow.OpenPatientDialogue("ODG", MainWindow._userSettingsMain.ShiftF11, "RDC")

            ' Créer une liste de fenêtres et énumérer les fenêtres
            Dim windowList As New List(Of String)()
            MainWindow.EnumWindows(AddressOf mainWindow.EnumWindowCallBack, IntPtr.Zero)
        Catch ex As Exception
            ' Journaliser l'erreur si quelque chose ne va pas
            logger.Error("Erreur lors de l'éxécution du raccourci Shift + F11 : " & ex.Message)
        End Try
    End Sub

    'Raccourci Shift + F12
    Private Sub OnHotKeyPressed8()
        ' Implémentation de la méthode
    End Sub

    Private Sub OnHotKeyPressed10()
        ' Implémentation de la méthode
    End Sub

    Public Shared Sub PasteText()
        ' Simuler la pression de la touche CTRL
        keybd_event(VK_CONTROL, 0, 0, 0)
        ' Simuler la pression de la touche V
        keybd_event(VK_V, 0, 0, 0)
        ' Relâcher la touche V
        keybd_event(VK_V, 0, KEYEVENTF_KEYUP, 0)
        ' Relâcher la touche CTRL
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0)
        ' simuler la pression de la touche F12
        keybd_event(VK_F12, 0, 0, 0)
        ' Relâcher la touche F12
        keybd_event(VK_F12, 0, KEYEVENTF_KEYUP, 0)
    End Sub
End Class

