Imports System.Collections.ObjectModel
Imports System.Globalization
Imports EyeChat.MainWindow
Imports EyeChat.PatientBubbleCtrl
Imports EyeChat.Patient

Public Class PatientManager

    Public Sub New()

    End Sub

    Public Shared Sub PatientAdd(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Annotation As String, ByVal Position As String, ByVal Examinator As String, ByVal Hold_Time As String)
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

    Public Shared Sub PatientRemove(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal Hold_Time As String)
        Dim patientToRemove As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = Hold_Time)
        If patientToRemove IsNot Nothing Then
            PatientsALL.Remove(patientToRemove)
            If patientToRemove.Position = "RDC" Then
                PatientsRDC.Remove(patientToRemove)
            ElseIf patientToRemove.Position = "1er" Then
                Patients1er.Remove(patientToRemove)
            End If
        End If
        UpdateList()
        SavePatientsToJson(PatientsALL)
    End Sub

    Public Shared Sub PatientUpdate(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal OldHold_Time As String, ByVal NewHold_Time As String)
        Dim oldHoldTimeDateTime As DateTime
        Dim newHoldTimeDateTime As DateTime

        If DateTime.TryParseExact(OldHold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, oldHoldTimeDateTime) AndAlso
           DateTime.TryParseExact(NewHold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, newHoldTimeDateTime) Then

            Dim patientToUpdate As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = oldHoldTimeDateTime)

            If patientToUpdate IsNot Nothing Then
                PatientsALL.Remove(patientToUpdate)
                If patientToUpdate.Position = "RDC" Then
                    PatientsRDC.Remove(patientToUpdate)
                ElseIf patientToUpdate.Position = "1er" Then
                    Patients1er.Remove(patientToUpdate)
                End If

                patientToUpdate.Hold_Time = newHoldTimeDateTime
                PatientsALL.Add(patientToUpdate)
                If patientToUpdate.Position = "RDC" Then
                    PatientsRDC.Add(patientToUpdate)
                ElseIf patientToUpdate.Position = "1er" Then
                    Patients1er.Add(patientToUpdate)
                End If
            End If

            UpdateList()
            SavePatientsToJson(PatientsALL)
        End If
    End Sub

    Public Shared Sub PatientCheckPass(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal Hold_Time As String, ByVal OperatorName As String)
        Dim HoldTimeDateTime As DateTime
        DateTime.TryParseExact(Hold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, HoldTimeDateTime)

        Dim patientToUpdate As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = HoldTimeDateTime)

        If patientToUpdate IsNot Nothing Then
            PatientsALL.Remove(patientToUpdate)
            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Remove(patientToUpdate)
            ElseIf patientToUpdate.Position = "1er" Then
                Patients1er.Remove(patientToUpdate)
            End If

            patientToUpdate.IsTaken = True
            patientToUpdate.OperatorName = OperatorName
            patientToUpdate.Colors = "gray"
            patientToUpdate.Pick_up_Time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
            patientToUpdate.Time_Order = patientToUpdate.Pick_up_Time - patientToUpdate.Hold_Time

            PatientsALL.Add(patientToUpdate)
            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Add(patientToUpdate)
            ElseIf patientToUpdate.Position = "1er" Then
                Patients1er.Add(patientToUpdate)
            End If

            UpdateList()
            SavePatientsToJson(PatientsALL)
        End If
    End Sub

    Public Shared Sub PatientUndoPass(ByVal Title As String, ByVal LastName As String, ByVal FirstName As String, ByVal Exams As String, ByVal Comments As String, ByVal Floor As String, ByVal Examinator As String, ByVal Hold_Time As String, ByVal OperatorName As String)
        Dim HoldTimeDateTime As DateTime
        DateTime.TryParseExact(Hold_Time, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, HoldTimeDateTime)

        Dim patientToUpdate As Patient = PatientsALL.FirstOrDefault(Function(patient) patient.Title = Title And patient.LastName = LastName And patient.FirstName = FirstName And patient.Exams = Exams And patient.Annotation = Comments And patient.Position = Floor And patient.Examinator = Examinator And patient.Hold_Time = HoldTimeDateTime)

        If patientToUpdate IsNot Nothing Then
            PatientsALL.Remove(patientToUpdate)
            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Remove(patientToUpdate)
            ElseIf patientToUpdate.Position = "1er" Then
                Patients1er.Remove(patientToUpdate)
            End If

            Dim examType As String = patientToUpdate.Exams
            Dim examOption As ExamOption = ExamOptions.FirstOrDefault(Function(ExamOptions) ExamOptions.Name = examType)
            If examOption IsNot Nothing Then
                Dim patientColor As String = examOption.Color
                patientToUpdate.Colors = patientColor
            End If

            patientToUpdate.IsTaken = False
            patientToUpdate.OperatorName = Nothing
            patientToUpdate.Pick_up_Time = Nothing
            patientToUpdate.Time_Order = Nothing

            If patientToUpdate.Position = "RDC" Then
                PatientsRDC.Add(patientToUpdate)
                PatientsALL.Add(patientToUpdate)
            ElseIf patientToUpdate.Position = "1er" Then
                Patients1er.Add(patientToUpdate)
                PatientsALL.Add(patientToUpdate)
            End If

            UpdateList()
            SavePatientsToJson(PatientsALL)
        End If
    End Sub

    Public Shared Sub ModifyPatient(ByVal lastName As String, ByVal updatedPatient As Patient)
        For index As Integer = 0 To PatientsALL.Count - 1
            If PatientsALL(index).LastName = lastName Then
                PatientsALL(index) = updatedPatient
                Exit For
            End If
        Next
    End Sub

End Class
