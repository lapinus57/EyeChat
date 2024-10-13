Imports log4net

Public Class NfcManager
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)



    ' Référence au lecteur NFC
    'Private Shared MyACR122U As ACR122U

    ' Méthode pour vérifier le mode NFC et exécuter les actions appropriées
    Public Shared Sub HandleNfcMode(_userSettings As UserSettings)
        If _userSettings.NFCMode = True Then
            ' Si le mode NFC est activé, initialiser le NFC
            MainWindow.hidePatientList()
            InitializeNfc()
        Else
            ' Si le mode NFC est désactivé, afficher la liste des patients
            MainWindow.showPatientList()
            SendManager.SendMessage("USR01" & _userSettings.UserName & "|" & Environment.UserName)
        End If
    End Sub

    ' Méthode pour initialiser le NFC
    Private Shared Sub InitializeNfc()
        Try
            ' Initialiser le lecteur NFC (ACR122U)
            'MyACR122U = New ACR122U()

            ' Ajouter les gestionnaires d'événements pour l'insertion et le retrait de la carte
            'AddHandler MyACR122U.CardInserted, AddressOf Acr122u_CardInserted
            'AddHandler MyACR122U.CardRemoved, AddressOf Acr122u_CardRemoved

            ' Initialisation du lecteur NFC avec les paramètres appropriés
            'MyACR122U.Init(True, 50, 4, 4, 200)

            logger.Info("NFC initialisé avec succès")
        Catch ex As Exception
            ' Gestion des erreurs lors de l'initialisation du NFC
            logger.Error($"Erreur lors de l'initialisation du NFC : {ex.Message}")
        End Try
    End Sub

    ' Méthode appelée lors de l'insertion d'une carte NFC
    Private Sub Acr122u_CardRemoved()
        ' Console.WriteLine("Card Removed")
        'Dispatcher.Invoke(Sub()
        'logger.Info($"Carte NFC retirée : {Uidcard}")
        'Dim userconnected As User = Users.FirstOrDefault(Function(user) user.UUID = Uidcard)
        'SendMessage("USR02" & userconnected.Name & "|OCT")
        'SendMessage("USR02" & My.Settings.UserName & "|OCT")
        'SendMessage("USR01" & My.Settings.UserName & "|" & Environment.UserName)

        'hidePatientList()
        'End Sub)
    End Sub

    Private Sub Acr122u_CardInserted(ByVal reader As PCSC.ICardReader)
        'Console.WriteLine("Card Inserted")

        'Dim Card As String = BitConverter.ToString(MyACR122U.GetUID(reader)).Replace("-", "")
        'Uidcard = Card



        'Dispatcher.Invoke(Sub()
        'logger.Info($"Carte NFC insérée : {Uidcard}")
        'showPatientList()

        'Dim userconnected As User = Users.FirstOrDefault(Function(user) user.UUID = Uidcard)
        'SendMessage("USR02" & My.Settings.UserName & "|" & Environment.UserName)

        'SendMessage("USR01" & userconnected.Name & "|OCT")
        'AddMessage("Benoit", "Benoit", "Room", Uidcard, False, Nothing)


        'End Sub)

    End Sub
End Class

