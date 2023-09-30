Imports ControlzEx.Theming
Imports MahApps.Metro.Controls
Imports System.Drawing
Imports EyeChat
Imports EyeChat.SettingsViewModel
Imports EyeChat.MainWindow
Imports EyeChat.User
Imports EyeChat.ExamOption
Imports EyeChat.Planning
Imports System.Collections.ObjectModel
Imports System.Globalization


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

            End If

            ' Sauvegarder les modifications dans le fichier JSON
            Dim examOptionList As List(Of ExamOption) = ExamDataGrid.ItemsSource.Cast(Of ExamOption)().ToList()
            Dim examOptionCollection As New ObservableCollection(Of ExamOption)(examOptionList)

            SaveExamOptionsToJson(examOptionCollection)
        End If
    End Sub
    Private Sub SaveExamChangesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim examOptionList As List(Of ExamOption) = ExamDataGrid.ItemsSource.Cast(Of ExamOption)().ToList()
        Dim examOptionCollection As New ObservableCollection(Of ExamOption)(examOptionList)

        SaveExamOptionsToJson(examOptionCollection)
    End Sub

    Private Sub SavePlanningChangesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim PlanningList As List(Of Planning) = PlanningDataGrid.ItemsSource.Cast(Of Planning)().ToList()
        Dim PlanningCollection As New ObservableCollection(Of Planning)(PlanningList)

        SavePlanningToJson(PlanningCollection)
    End Sub

    Private Sub SaveSpeedMessageChangesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim SpeedMessageList As List(Of SpeedMessage) = SpeedMessageDataGrid.ItemsSource.Cast(Of SpeedMessage)().ToList()
        Dim SpeedMessageCollection As New ObservableCollection(Of SpeedMessage)(SpeedMessageList)

        SaveSpeedMessageToJson(SpeedMessageCollection)
    End Sub

    Private Sub ExamDataGrid_AddingNewItem(sender As Object, e As AddingNewItemEventArgs) Handles ExamDataGrid.AddingNewItem
        Dim examOptionList As List(Of ExamOption) = ExamDataGrid.ItemsSource.Cast(Of ExamOption)().ToList()

        ' Créez un nouvel objet ExamOption et attribuez-lui l'index approprié
        Dim newExamOption As New ExamOption()
        newExamOption.index = examOptionList.Count + 1 ' Incrémente l'index à chaque ajout

        ' Assurez-vous que l'objet nouvellement créé est associé à l'élément ajouté
        e.NewItem = newExamOption

        Dim examOptionCollection As New ObservableCollection(Of ExamOption)(examOptionList)

        SaveExamOptionsToJson(examOptionCollection)
    End Sub

    Private Sub SpeedMessageGrid_AddingNewItem(sender As Object, e As AddingNewItemEventArgs) Handles SpeedMessageDataGrid.AddingNewItem
        Dim SpeedMessageList As List(Of SpeedMessage) = SpeedMessageDataGrid.ItemsSource.Cast(Of SpeedMessage)().ToList()

        ' Créez un nouvel objet SpeedMessage et attribuez-lui l'index approprié
        Dim newSpeedMessage As New SpeedMessage()
        newSpeedMessage.Index = SpeedMessageList.Count + 1 ' Incrémente l'index à chaque ajout

        ' Assurez-vous que l'objet nouvellement créé est associé à l'élément ajouté
        e.NewItem = newSpeedMessage

        Dim SpeedMessageCollection As New ObservableCollection(Of SpeedMessage)(SpeedMessageList)

        SaveSpeedMessageToJson(SpeedMessageCollection)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub ColorPicker_SelectedColorChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of System.Windows.Media.Color?))

    End Sub

    Private Sub ColorPicker_DropDownClosed(sender As Object, e As EventArgs)
        SetTheme()
    End Sub
End Class
Public Class BoolToVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is Boolean AndAlso DirectCast(value, Boolean) Then
            Return Visibility.Visible
        Else
            Return Visibility.Collapsed
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function




End Class





