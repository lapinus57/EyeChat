Imports System.Collections.ObjectModel
Imports System.IO
Imports Newtonsoft.Json

Public Class Patient

    Public Shared ReadOnly PatientFilePath As String = Path.Combine("HistoricPatient", DateTime.Now.ToString("ddMMyyyy") & ".json")

    <JsonProperty("Id")>
    Public Property Id As String

    <JsonProperty("Colors")>
    Public Property Colors As String

    <JsonProperty("Title")>
    Public Property Title As String

    <JsonProperty("LastName")>
    Public Property LastName As String

    <JsonProperty("FirstName")>
    Public Property FirstName As String

    <JsonProperty("Exams")>
    Public Property Exams As String

    <JsonProperty("Annotation")>
    Public Property Annotation As String

    <JsonProperty("Position")>
    Public Property Position As String

    <JsonProperty("Hold_Time")>
    Public Property Hold_Time As DateTime

    <JsonProperty("Pick_up_Time")>
    Public Property Pick_up_Time As DateTime

    <JsonProperty("Time_Order")>
    Public Property Time_Order As TimeSpan

    <JsonProperty("Examinator")>
    Public Property Examinator As String

    <JsonProperty("OperatorName")>
    Public Property OperatorName As String

    <JsonProperty("IsTaken")>
    Public Property IsTaken As Boolean



    Public Shared Function LoadPatientsFromJson() As ObservableCollection(Of Patient)
        Dim patients As ObservableCollection(Of Patient) = Nothing

        If File.Exists(PatientFilePath) Then
            Using streamReader As New StreamReader(PatientFilePath)
                Using jsonReader As New JsonTextReader(streamReader)
                    Dim serializer As New JsonSerializer()
                    patients = serializer.Deserialize(Of ObservableCollection(Of Patient))(jsonReader)
                End Using
            End Using
        End If

        Return patients
    End Function

    ' Enregistrement des messages dans le fichier JSON
    Public Shared Sub SavePatientsToJson(ByVal patients As ObservableCollection(Of Patient))

        Dim dossier As String = "HistoricPatient"
        ' Vérifier si le dossier existe
        If Not Directory.Exists(dossier) Then
            ' Créer le dossier s'il n'existe pas
            Directory.CreateDirectory(dossier)
        End If

        Dim serializedPatients As String = "[" & String.Join("," & Environment.NewLine, patients.Select(Function(m) JsonConvert.SerializeObject(New With {m.Id, m.Colors, m.Title, m.LastName, m.FirstName, m.Exams, m.Annotation, m.Position, m.Hold_Time, m.Pick_up_Time, m.Time_Order, m.Examinator, m.OperatorName, m.IsTaken}))) & "]"
        File.WriteAllText(PatientFilePath, serializedPatients)

    End Sub

End Class
