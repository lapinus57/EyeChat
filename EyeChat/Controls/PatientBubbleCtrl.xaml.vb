Imports System.Collections.ObjectModel
Imports System.Globalization
Imports EyeChat.MainWindow
Imports EyeChat.Patient
Public Class PatientBubbleCtrl
    Inherits UserControl
    Public Sub New()
        InitializeComponent()
    End Sub


    Private Sub MenuItem_PassageClick(sender As Object, e As RoutedEventArgs)
        ' Vérifiez si le sender est bien un MenuItem
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            If patient.IsTaken = False Then
                ' Mettez à jour la propriété Colors de l'objet Patient
                patient.Colors = "gray"
                patient.Pick_up_Time = DateTime.Now
                patient.IsTaken = True

            Else
                ' Mettez à jour la propriété Colors de l'objet Patient
                patient.Pick_up_Time = Nothing
                If patient.Exams = "FO" Then
                    patient.Colors = "Red"
                ElseIf patient.Exams = "SK" Then
                    patient.Colors = "Yellow"
                End If
                patient.IsTaken = False

            End If

            UpdateList()

        End If
    End Sub

    Private Sub MenuItem_AnnulerClick(sender As Object, e As RoutedEventArgs)
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patientall As Patient = DirectCast(menuItem.DataContext, Patient)



            UpdateList()
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
            Sendmessage(Text)

            ' Mettez à jour la liste
            UpdateList()
        End If
    End Sub

    Private Sub MenuItem_upClick(sender As Object, e As RoutedEventArgs)
        ' Gère le clic sur le bouton "Up" afin de déplacer le patient sélectionné vers le haut d'une case

        If TypeOf sender Is MenuItem Then
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            ' Obtenir l'index du patient actuel dans la liste
            Dim index As Integer = PatientsALL.IndexOf(patient)

            If index > 1 Then ' Au moins deux patients précédents sont nécessaires
                ' Obtenir les patients N-1 et N-2
                Dim patientN1 As Patient = PatientsALL(index - 1)
                Dim patientN2 As Patient = PatientsALL(index - 2)

                ' Calculer la différence de temps entre N-1 et N-2
                Dim timeDiffN1N2 As TimeSpan = patientN2.Hold_Time.Subtract(patientN1.Hold_Time)

                ' Calculer l'ajustement en ajoutant la moitié de la différence de temps
                Dim adjustment As TimeSpan = TimeSpan.FromTicks(timeDiffN1N2.Ticks / 2)

                ' Calculer le nouvel horaire en reculant de l'ajustement
                Dim newHoldTime As DateTime = patientN1.Hold_Time.Subtract(adjustment)

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)

                ' Mettre à jour la liste des patients
                UpdateList()
            ElseIf index = 1 Then ' Si le patient est le deuxième dans la liste
                ' Obtenir le patient N-1
                Dim patientN1 As Patient = PatientsALL(index - 1)

                ' Soustraire 2 minutes de l'heure du patient N-1
                Dim newHoldTime As DateTime = patientN1.Hold_Time.Subtract(TimeSpan.FromMinutes(2))

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)

                ' Mettre à jour la liste des patients
                UpdateList()
            End If
        End If
    End Sub

    Private Sub MenuItem_downClick(sender As Object, e As RoutedEventArgs)
        ' Gère le clic sur le bouton "Down" afin de déplacer le patient sélectionné vers le bas d'une case

        If TypeOf sender Is MenuItem Then
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            ' Obtenir l'index du patient actuel dans la liste
            Dim index As Integer = PatientsALL.IndexOf(patient)

            If index < PatientsALL.Count - 2 Then ' Au moins deux patients suivants sont nécessaires
                ' Obtenir les patients N+1 et N+2
                Dim patientN1 As Patient = PatientsALL(index + 1)
                Dim patientN2 As Patient = PatientsALL(index + 2)

                ' Calculer la différence de temps entre N+2 et N+1
                Dim timeDiffN1N2 As TimeSpan = patientN2.Hold_Time.Subtract(patientN1.Hold_Time)

                ' Calculer l'ajustement en ajoutant la moitié de la différence de temps
                Dim adjustment As TimeSpan = TimeSpan.FromTicks(timeDiffN1N2.Ticks / 2)

                ' Calculer le nouvel horaire en avançant de l'ajustement
                Dim newHoldTime As DateTime = patientN1.Hold_Time.Add(adjustment)

                ' Mettre à jour l'horaire du patient
                UpdatePatientHoldTime(patient, newHoldTime)

                ' Mettre à jour la liste des patients
                UpdateList()
            End If
        End If
    End Sub

    Private Sub UpdatePatientHoldTime(patient As Patient, newHoldTime As DateTime)
        ' Mise à jour de l'heure Hold_Time du patient
        patient.Hold_Time = newHoldTime

        ' Envoyer le message avec la nouvelle heure Hold_Time au serveur
        Dim formattedHoldTime As String = newHoldTime.ToString("yyyy-MM-ddTHH:mm:ss.fff")
        Dim Text As String = "PTN04" & patient.Title & "|" & patient.LastName & "|" & patient.FirstName & "|" & patient.Exams & "|" & patient.Annotation & "|" & patient.Position & "|" & patient.Examinator & "|" & formattedHoldTime
        Sendmessage(Text)
    End Sub


    Private Sub UpdateList()
        ' Triez la liste des patients par Hold_Time avant de la sauvegarder
        SortPatientsByHoldTime()
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

    Private Sub SortPatientsByHoldTime()
        PatientsALL = New ObservableCollection(Of Patient)(PatientsALL.OrderBy(Function(p) p.Hold_Time))
    End Sub


End Class
