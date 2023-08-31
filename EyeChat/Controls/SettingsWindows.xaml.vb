Imports ControlzEx.Theming
Imports MahApps.Metro.Controls
Imports System.Drawing
Imports EyeChat
Imports EyeChat.SettingsViewModel
Imports EyeChat.MainWindow
Imports EyeChat.User
Imports EyeChat.ExamOption
Imports System.Collections.ObjectModel

Public Class SettingsWindows

    Private _settings As SettingsViewModel



    Public ReadOnly Property Settings As SettingsViewModel
        Get
            If _settings Is Nothing Then
                _settings = New SettingsViewModel()
            End If
            Return _settings
        End Get
    End Property



    Private Sub AppColorChanged(sender As Object, e As SelectionChangedEventArgs)
        SetTheme()
    End Sub



    Private Sub AppThemeChanged(sender As Object, e As SelectionChangedEventArgs)

        SetTheme()

    End Sub



    Private Sub SettingsWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        InitializeComponent()
        Dim settings As New SettingsViewModel()
        Me.DataContext = settings


    End Sub



    Private Sub ToggleSwitch_Toggled(sender As Object, e As RoutedEventArgs)

        My.Application.MainWindow.UpdateLayout()

    End Sub

    Private Sub cbodisplay_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbodisplay.SelectionChanged
        If cbodisplay.SelectedValue = "A" Then
            My.Settings.NameDisplayUsers = Visibility.Visible
            My.Settings.RoomNameDisplayUsers = Visibility.Collapsed
            My.Settings.NameRoomDisplayUsers = Visibility.Collapsed
        ElseIf cbodisplay.SelectedValue = "B" Then
            My.Settings.NameDisplayUsers = Visibility.Collapsed
            My.Settings.RoomNameDisplayUsers = Visibility.Visible
            My.Settings.NameRoomDisplayUsers = Visibility.Collapsed
        ElseIf cbodisplay.SelectedValue = "C" Then
            My.Settings.NameDisplayUsers = Visibility.Collapsed
            My.Settings.RoomNameDisplayUsers = Visibility.Collapsed
            My.Settings.NameRoomDisplayUsers = Visibility.Visible
        End If
        My.Settings.Save()
        Users.Clear()
        Dim loadedUsers = LoadUsersFromJson()
        For Each user In loadedUsers
            Users.Add(user)
        Next
    End Sub

    Private Sub ExamDataGrid_CellEditEnding(sender As Object, e As DataGridCellEditEndingEventArgs) Handles ExamDataGrid.CellEditEnding
        Dim editedItem As ExamOption = TryCast(e.Row.Item, ExamOption)

        If editedItem IsNot Nothing Then
            ' Mettre à jour les données dans la collection _examOptions
            ' en fonction des modifications apportées dans le DataGrid
            Dim editedValue As String = TryCast((TryCast(e.EditingElement, TextBox))?.Text, String)
            If e.Column.Header.ToString() = "Name" Then
                editedItem.Name = editedValue
            ElseIf e.Column.Header.ToString() = "Color" Then
                editedItem.Color = editedValue
                ' ... Répéter pour d'autres propriétés ...
            End If

            ' Sauvegarder les modifications dans le fichier JSON
            'Settings.SaveExamOptionsToJson()
        End If
    End Sub
    Private Sub SaveExamChangesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim examOptionList As List(Of ExamOption) = ExamDataGrid.ItemsSource.Cast(Of ExamOption)().ToList()
        Dim examOptionCollection As New ObservableCollection(Of ExamOption)(examOptionList)

        SaveExamOptionsToJson(examOptionCollection)
    End Sub



    Private Sub SaveSpeedMessageChangesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim SpeedMessageList As List(Of SpeedMessage) = SpeedMessageDataGrid.ItemsSource.Cast(Of SpeedMessage)().ToList()
        Dim SpeedMessageCollection As New ObservableCollection(Of SpeedMessage)(SpeedMessageList)

        SaveSpeedMessageToJson(SpeedMessageCollection)
    End Sub

    Private Sub ExamDataGrid_AddingNewItem(sender As Object, e As AddingNewItemEventArgs) Handles ExamDataGrid.AddingNewItem
        ' Créez un nouvel objet ExamOption et attribuez-lui l'index approprié
        Dim newExamOption As New ExamOption()
        newExamOption.index = Settings.ExamOptions.Count + 1 ' Incrémente l'index à chaque ajout

        ' Assurez-vous que l'objet nouvellement créé est associé à l'élément ajouté
        e.NewItem = newExamOption

        Dim examOptionList As List(Of ExamOption) = ExamDataGrid.ItemsSource.Cast(Of ExamOption)().ToList()
        Dim examOptionCollection As New ObservableCollection(Of ExamOption)(examOptionList)

        SaveExamOptionsToJson(examOptionCollection)
    End Sub

    Private Sub SpeedMessageGrid_AddingNewItem(sender As Object, e As AddingNewItemEventArgs) Handles SpeedMessageDataGrid.AddingNewItem
        ' Créez un nouvel objet SpeedMessage et attribuez-lui l'index approprié
        Dim newSpeedMessage As New SpeedMessage()
        newSpeedMessage.Index = Settings.SpeedMessage.Count + 1 ' Incrémente l'index à chaque ajout

        ' Assurez-vous que l'objet nouvellement créé est associé à l'élément ajouté
        e.NewItem = newSpeedMessage

        Dim SpeedMessageList As List(Of SpeedMessage) = SpeedMessageDataGrid.ItemsSource.Cast(Of SpeedMessage)().ToList()
        Dim SpeedMessageCollection As New ObservableCollection(Of SpeedMessage)(SpeedMessageList)

        SaveSpeedMessageToJson(SpeedMessageCollection)
    End Sub
End Class




