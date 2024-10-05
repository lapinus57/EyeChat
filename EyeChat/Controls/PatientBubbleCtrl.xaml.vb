Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports EyeChat.MainWindow
Imports EyeChat.Patient
Imports MahApps.Metro.SimpleChildWindow

Public Class PatientBubbleCtrl
    Inherits UserControl
    Implements INotifyPropertyChanged

    Public Sub New()
        InitializeComponent()

    End Sub

    Private _mainWindowInstance As MainWindow

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    Public Property MainWindowInstance As MainWindow
        Get
            Return _mainWindowInstance
        End Get
        Set(value As MainWindow)
            _mainWindowInstance = value
        End Set
    End Property


    Private Sub MenuItem_PassageClick(sender As Object, e As RoutedEventArgs)
        ' Vérifiez si le sender est bien un MenuItem
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            If patient.IsTaken = False Then

                ' Formatez Hold_Time avec les fractions de secondes
                Dim formattedHoldTime As String = patient.Hold_Time.ToString("yyyy-MM-ddTHH:mm:ss.fff")

                ' Construisez la chaîne de texte à envoyer
                Dim Text As String = "PTN02" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|" & patient.Position & "|" & patient.Examinator & "|" & formattedHoldTime & "|" & My.Settings.UserName

                ' Envoyez le message
                SendMessage(Text)



                'Await (CType(Application.Current.MainWindow, MahApps.Metro.Controls.MetroWindow)).ShowChildWindowAsync(New InfoPatient())

            Else

                ' Formatez Hold_Time avec les fractions de secondes
                Dim formattedHoldTime As String = patient.Hold_Time.ToString("yyyy-MM-ddTHH:mm:ss.fff")

                ' Construisez la chaîne de texte à envoyer
                Dim Text As String = "PTN05" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|" & patient.Position & "|" & patient.Examinator & "|" & formattedHoldTime & "|" & My.Settings.UserName

                ' Envoyez le message
                SendMessage(Text)

            End If

        End If
    End Sub

    Private Sub MenuItem_AttenteClick(sender As Object, e As RoutedEventArgs)
        ' Vérifiez si le sender est bien un MenuItem
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            If patient.IsTaken = False Then

                ' Formatez Hold_Time avec les fractions de secondes
                Dim formattedHoldTime As String = patient.Hold_Time.ToString("yyyy-MM-ddTHH:mm:ss.fff")

                ' Construisez la chaîne de texte à envoyer
                Dim Text As String = "PTN02" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|" & patient.Position & "|" & patient.Examinator & "|" & formattedHoldTime & "|" & My.Settings.UserName

                ' Envoyez le message
                SendMessage(Text)

                Dim newexams As String

                ' Construisez la chaîne de texte à envoyer
                Text = "PTN01" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|RDC|" & patient.Examinator & "|" & formattedHoldTime & "|" & My.Settings.UserName

                ' Envoyez le message
                SendMessage(Text)


                'Await (CType(Application.Current.MainWindow, MahApps.Metro.Controls.MetroWindow)).ShowChildWindowAsync(New InfoPatient())

            Else

                ' Formatez Hold_Time avec les fractions de secondes
                Dim formattedHoldTime As String = patient.Hold_Time.ToString("yyyy-MM-ddTHH:mm:ss.fff")

                ' Construisez la chaîne de texte à envoyer
                Dim Text As String = "PTN05" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|" & patient.Position & "|" & patient.Examinator & "|" & formattedHoldTime & "|" & My.Settings.UserName

                ' Envoyez le message
                SendMessage(Text)

            End If

        End If
    End Sub

    Private Async Sub MenuItem_InfoPatient(sender As Object, e As RoutedEventArgs)
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patientall As Patient = DirectCast(menuItem.DataContext, Patient)

            Dim childWindow As New InfoPatient()
            childWindow.Title = "Information Patient" ' Remplacez "Nouveau titre de la fenêtre" par le titre souhaité.
            childWindow.PatientName.Text = patientall.Title & " " & patientall.LastName & " " & patientall.FirstName
            childWindow.Examinator.Text = patientall.Examinator
            childWindow.Hold_Time.Text = patientall.Hold_Time.ToString("HH:mm")
            childWindow.Annotation.Text = patientall.Annotation
            childWindow.Exams.Text = patientall.Exams
            childWindow.Time_Order.Text = patientall.Time_Order.ToString("hh\:mm")
            childWindow.OperatorName.Text = patientall.OperatorName
            childWindow.Pick_up_Time.Text = patientall.Pick_up_Time.ToString("HH:mm")

            Await (CType(Application.Current.MainWindow, MahApps.Metro.Controls.MetroWindow)).ShowChildWindowAsync(childWindow)


        End If
    End Sub

    Private Sub MenuItem_DelteClick(sender As Object, e As RoutedEventArgs)
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            ' Formatez Hold_Time avec les fractions de secondes
            Dim formattedHoldTime As String = patient.Hold_Time.ToString("yyyy-MM-ddTHH:mm:ss.fff")

            ' Construisez la chaîne de texte à envoyer
            Dim Text As String = "PTN03" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|" & patient.Position & "|" & patient.Examinator & "|" & formattedHoldTime

            ' Envoyez le message
            SendMessage(Text)

            ' Mettez à jour la liste
            'UpdateList()
        End If
    End Sub

    Private Sub MenuItem_upClick(sender As Object, e As RoutedEventArgs)
        ' Gère le clic sur le bouton "Up" afin de déplacer le patient sélectionné vers le haut d'une case

        PatientsALL = New ObservableCollection(Of Patient)(PatientsALL.OrderBy(Function(p) p.Hold_Time))

        If TypeOf sender Is MenuItem Then
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            ' Obtenir l'index du patient actuel dans la liste
            Dim index As Integer = PatientsALL.IndexOf(patient)

            ' Trouver le patient N-1 au même étage (position)
            Dim patientN1 As Patient = Nothing
            For i As Integer = index - 1 To 0 Step -1
                If PatientsALL(i).Position = patient.Position Then
                    patientN1 = PatientsALL(i)
                    Exit For
                End If
            Next

            ' Trouver le patient N-2 au même étage (position)
            Dim patientN2 As Patient = Nothing
            If patientN1 IsNot Nothing Then
                For i As Integer = PatientsALL.IndexOf(patientN1) - 1 To 0 Step -1
                    If PatientsALL(i).Position = patient.Position Then
                        patientN2 = PatientsALL(i)
                        Exit For
                    End If
                Next
            End If

            If patientN1 IsNot Nothing And patientN2 IsNot Nothing Then
                ' Vous avez trouvé les patients N-1 et N-2 au même étage
                ' Calculer la différence de temps entre N-1 et N-2
                Dim timeDiffN1N2 As TimeSpan = patientN1.Hold_Time.Subtract(patientN2.Hold_Time)

                ' Calculer l'ajustement en ajoutant la moitié de la différence de temps
                Dim adjustment As TimeSpan = TimeSpan.FromTicks(timeDiffN1N2.Ticks / 2)

                ' Calculer le nouvel horaire en reculant de l'ajustement
                Dim newHoldTime As DateTime = patientN1.Hold_Time.Subtract(adjustment)

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)
            ElseIf patientN1 IsNot Nothing Then
                ' Vous avez trouvé le patient N-1 au même étage
                ' Soustraire 2 minutes de l'heure du patient N-1
                Dim newHoldTime As DateTime = patientN1.Hold_Time.Subtract(TimeSpan.FromMinutes(2))

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)
            End If
        End If
    End Sub

    Private Sub MenuItem_downClick(sender As Object, e As RoutedEventArgs)
        ' Gère le clic sur le bouton "Down" afin de déplacer le patient sélectionné vers le bas d'une case

        PatientsALL = New ObservableCollection(Of Patient)(PatientsALL.OrderBy(Function(p) p.Hold_Time))

        If TypeOf sender Is MenuItem Then
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            ' Obtenir l'index du patient actuel dans la liste
            Dim index As Integer = PatientsALL.IndexOf(patient)

            ' Trouver le patient N+1 au même étage (position)
            Dim patientN1 As Patient = Nothing
            For i As Integer = index + 1 To PatientsALL.Count - 1
                If PatientsALL(i).Position = patient.Position Then
                    patientN1 = PatientsALL(i)
                    Exit For
                End If
            Next

            ' Trouver le patient N+2 au même étage (position)
            Dim patientN2 As Patient = Nothing
            If patientN1 IsNot Nothing Then
                For i As Integer = PatientsALL.IndexOf(patientN1) + 1 To PatientsALL.Count - 1
                    If PatientsALL(i).Position = patient.Position Then
                        patientN2 = PatientsALL(i)
                        Exit For
                    End If
                Next
            End If

            If patientN1 IsNot Nothing And patientN2 IsNot Nothing Then
                ' Vous avez trouvé les patients N+1 et N+2 au même étage
                ' Calculer la différence de temps entre N+2 et N+1
                Dim timeDiffN1N2 As TimeSpan = patientN2.Hold_Time.Subtract(patientN1.Hold_Time)

                ' Calculer l'ajustement en ajoutant la moitié de la différence de temps
                Dim adjustment As TimeSpan = TimeSpan.FromTicks(timeDiffN1N2.Ticks / 2)

                ' Calculer le nouvel horaire en avançant de l'ajustement
                Dim newHoldTime As DateTime = patientN1.Hold_Time.Add(adjustment)

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)
            ElseIf patientN1 IsNot Nothing Then
                ' Vous avez trouvé le patient N+1 au même étage
                ' Ajouter 2 minutes à l'heure du patient N+1
                Dim newHoldTime As DateTime = patientN1.Hold_Time.Add(TimeSpan.FromMinutes(2))

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)
            End If
        End If
    End Sub

    Private Sub MenuItem_TopClick(sender As Object, e As RoutedEventArgs)
        PatientsALL = New ObservableCollection(Of Patient)(PatientsALL.OrderBy(Function(p) p.Hold_Time))
        If TypeOf sender Is MenuItem Then
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            ' Obtenir le premier patient non pris (IsTaken = False) au même étage (position)
            Dim firstAvailablePatient As Patient = PatientsALL.FirstOrDefault(Function(p) p.Position = patient.Position AndAlso Not p.IsTaken)
            ' Obtenir le dernier patient pris (IsTaken = True) au même étage (position)
            Dim lastAvailablePatient As Patient = PatientsALL.LastOrDefault(Function(p) p.Position = patient.Position AndAlso p.IsTaken)

            If firstAvailablePatient IsNot Nothing And lastAvailablePatient Is Nothing Then
                ' Calculer l'ajustement en fonction de l'écart entre l'horaire du premier patient disponible et l'horaire actuel
                'Dim adjustment As TimeSpan = firstAvailablePatient.Hold_Time.Subtract(TimeSpan.FromMinutes(5))

                Dim random As New Random()
                Dim randomSeconds As Integer = random.Next(60, 361) ' Generates a random number between 60 and 360 (inclusive)
                Dim randomTimeSpan As TimeSpan = TimeSpan.FromSeconds(randomSeconds)

                ' Calculer le nouvel horaire en reculant de l'ajustement
                'Dim newHoldTime As DateTime = patient.Hold_Time.Subtract(adjustment)
                Dim newHoldTime As DateTime = firstAvailablePatient.Hold_Time.Subtract(randomTimeSpan)

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)
            ElseIf firstAvailablePatient IsNot Nothing And lastAvailablePatient IsNot Nothing Then

                ' Calculer le temps moyen entre les deux heures de patients
                Dim averageTimeSpan As TimeSpan = TimeSpan.FromTicks((lastAvailablePatient.Hold_Time - firstAvailablePatient.Hold_Time).Ticks \ 2)

                ' Générer un décalage aléatoire dans une certaine plage autour de la moyenne
                Dim random As New Random()
                Dim randomOffsetTicks As Long = random.Next(-averageTimeSpan.Ticks, averageTimeSpan.Ticks + 1)
                Dim randomTimeSpan As TimeSpan = TimeSpan.FromTicks(averageTimeSpan.Ticks + randomOffsetTicks)

                ' Calculer la nouvelle heure de prise en soustrayant le laps de temps aléatoire
                Dim newHoldTime As DateTime = lastAvailablePatient.Hold_Time.Subtract(randomTimeSpan)

                ' Mettre à jour l'heure de prise du patient
                UpdatePatientHoldTime(patient, newHoldTime)

            Else
                ' Si aucun patient non pris n'est disponible, ajuster en ajoutant 5 minutes à l'horaire actuel
                'Dim newHoldTime As DateTime = patient.Hold_Time.Subtract(TimeSpan.FromMinutes(5))
                'UpdatePatientHoldTime(patient, newHoldTime)
            End If
        End If
    End Sub

    Private Sub UpdatePatientHoldTime(patient As Patient, newHoldTime As DateTime)
        ' Mise à jour de l'heure Hold_Time du patient
        'patient.Hold_Time = newHoldTime

        ' Envoyer le message avec la nouvelle heure Hold_Time au serveur
        Dim formattedNewHoldTime As String = newHoldTime.ToString("yyyy-MM-ddTHH:mm:ss.fff")
        Dim formattedOldHoldTime As String = patient.Hold_Time.ToString("yyyy-MM-ddTHH:mm:ss.fff")
        Dim Text As String = "PTN04" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|" & patient.Position & "|" & patient.Examinator & "|" & formattedOldHoldTime & "|" & formattedNewHoldTime
        SendMessage(Text)
    End Sub


    Public Shared Sub UpdateList()
        ' Triez la liste des patients par Hold_Time avant de la sauvegarder
        'SortPatientsByHoldTime()
        ' Enregistrez la liste triée dans le fichier JSON
        SavePatientsToJson(PatientsALL)

        ' Effacez les listes existantes pour préparer la mise à jour
        PatientsALL.Clear()
        PatientsRDC.Clear()
        Patients1er.Clear()

        ' Chargez les patients à partir du fichier JSON
        Dim loadedpatient = LoadPatientsFromJson()

        ' Parcourez la liste des patients chargés et ajoutez-les aux listes appropriées
        For Each patient In loadedpatient
            PatientsALL.Add(patient)
            If patient.Position = "RDC" Then
                PatientsRDC.Add(patient)
            ElseIf patient.Position = "1er" Then
                Patients1er.Add(patient)
            End If
        Next
    End Sub

    Public Shared Sub ClearList()
        ' Triez la liste des patients par Hold_Time avant de la sauvegarder
        'SortPatientsByHoldTime()
        ' Enregistrez la liste triée dans le fichier JSON
        SavePatientsToJson(PatientsALL)

        ' Effacez les listes existantes pour préparer la mise à jour
        PatientsALL.Clear()
        PatientsRDC.Clear()
        Patients1er.Clear()


    End Sub

    Private Sub SortPatientsByHoldTime()
        PatientsALL = New ObservableCollection(Of Patient)(PatientsALL.OrderBy(Function(p) p.Hold_Time))
    End Sub

    Private Sub UserControl_ContextMenuOpening(sender As Object, e As ContextMenuEventArgs)
        Dim contextMenu As ContextMenu = Me.ContextMenu
        If contextMenu IsNot Nothing Then

            ' Ajustez la visibilité des MenuItem en fonction de OrthoMode
            ' Exemple pour le premier MenuItem; répétez pour les autres selon le besoin
            DirectCast(contextMenu.Items(1), MenuItem).Visibility = If(My.Settings.OrthoMode, Visibility.Visible, Visibility.Collapsed)
            DirectCast(contextMenu.Items(3), MenuItem).Visibility = If(My.Settings.AdvanvedMode, Visibility.Visible, Visibility.Collapsed)

            ' Répétez pour d'autres MenuItem si nécessaire
        End If
    End Sub

End Class
