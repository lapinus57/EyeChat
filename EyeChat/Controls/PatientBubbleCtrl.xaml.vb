Imports System.Collections.ObjectModel
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
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)



            UpdateList()
        End If
    End Sub

    Private Sub MenuItem_DelteClick(sender As Object, e As RoutedEventArgs)
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)
            Patients.Remove(patient)

            UpdateList()
        End If
    End Sub
    Private Sub MenuItem_upClick(sender As Object, e As RoutedEventArgs)
        If TypeOf sender Is MenuItem Then
            ' Obtenez une référence au MenuItem
            Dim menuItem As MenuItem = DirectCast(sender, MenuItem)

            ' Obtenez l'objet Patient associé au MenuItem
            Dim patient As Patient = DirectCast(menuItem.DataContext, Patient)

            UpdateList()
        End If



    End Sub

    Private Sub UpdateList()
        SavePatientsToJson(Patients)
        Patients.Clear()
        Dim loadedpatient = LoadPatientsFromJson()
        For Each patient In loadedpatient
            Patients.Add(patient)
        Next
    End Sub




End Class
