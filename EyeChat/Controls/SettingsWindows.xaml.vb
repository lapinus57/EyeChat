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
Imports MaterialDesignThemes.Wpf
Imports System.IO
Imports Microsoft.Win32
Imports MahApps.Metro.Controls.Dialogs

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

        LoadAvatars()


    End Sub



    Public Sub LoadAvatars()
        ' Obtenez le chemin du dossier contenant les avatars
        Dim avatarFolder As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar")

        ' Vérifiez si le dossier existe
        If Directory.Exists(avatarFolder) Then
            ' Effacez la ComboBox pour éviter d'ajouter des doublons si cette méthode est appelée à plusieurs reprises
            cboAvatars.Items.Clear()

            ' Obtenez la liste des fichiers d'images dans le dossier
            Dim imageFiles() As String = Directory.GetFiles(avatarFolder, "*.png") ' Vous pouvez spécifier d'autres extensions d'image si nécessaire

            ' Ajoutez un élément "Importer un avatar" à la ComboBox
            ' Ajoutez un élément spécial pour importer un avatar
            cboAvatars.Items.Add(New AvatarItem() With {
                 .ImagePath = "", ' Laissez l'ImagePath vide pour cet élément
                  .Width = 25,
                  .Height = 25,
                 .Tag = "Importer un avatar" ' Texte pour l'élément
                })
            ' Parcourez les fichiers et ajoutez-les à la ComboBox
            For Each imagePath As String In imageFiles
                Dim fileName As String = Path.GetFileName(imagePath)
                cboAvatars.Items.Add(New AvatarItem() With {
                .ImagePath = imagePath,
                .Width = 25, ' Largeur souhaitée
                .Height = 25, ' Hauteur souhaitée
                .Tag = fileName ' Enregistrez le nom de fichier dans le Tag pour une utilisation ultérieure si nécessaire
            })
            Next
        Else
            ' Le dossier n'existe pas, affichez un message d'erreur ou gérez-le selon vos besoins
        End If
    End Sub


    Private Sub cboAvatars_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim selectedItem As AvatarItem = TryCast(cboAvatars.SelectedItem, AvatarItem)

        ' Vérifiez si l'utilisateur a sélectionné "Importer un avatar"
        If selectedItem IsNot Nothing AndAlso selectedItem.Tag = "Importer un avatar" Then
            ' Ici, vous pouvez déclencher une action d'importation d'avatar
            ' par exemple, en affichant une boîte de dialogue pour sélectionner un fichier image
            ' et en enregistrant cet avatar dans votre dossier d'avatar.
            ' Configurez la boîte de dialogue
            SelectImageFile()
        End If
    End Sub

    Private Sub SelectImageFile()
        ' Configurez la boîte de dialogue
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Title = "Sélectionnez une image"
        openFileDialog.Filter = "Fichiers d'image|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Tous les fichiers|*.*"

        ' Affichez la boîte de dialogue et attendez la sélection de l'utilisateur
        Dim result As Boolean? = openFileDialog.ShowDialog()

        ' Si l'utilisateur a sélectionné un fichier, affichez le chemin du fichier
        If result = True Then
            Dim selectedImagePath As String = openFileDialog.FileName

            ' Chargez l'image pour obtenir ses dimensions en pixels
            Dim image As New BitmapImage(New Uri(selectedImagePath))

            ' Spécifiez les dimensions maximales autorisées (par exemple, largeur maximale de 800 pixels et hauteur maximale de 600 pixels)
            Dim maxWidthPixels As Integer = 1000
            Dim maxHeightPixels As Integer = 1000

            If image.PixelWidth <= maxWidthPixels AndAlso image.PixelHeight <= maxHeightPixels Then
                ' Obtenez le chemin de destination (le dossier "avatar" de votre application)
                Dim destinationFolder As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar")

                ' Assurez-vous que le dossier de destination existe, sinon, créez-le
                If Not Directory.Exists(destinationFolder) Then
                    Directory.CreateDirectory(destinationFolder)
                End If

                ' Obtenez le nom de fichier sans le chemin complet
                Dim fileName As String = System.IO.Path.GetFileName(selectedImagePath)

                ' Construisez le chemin complet de destination en combinant le dossier de destination avec le nom de fichier
                Dim destinationPath As String = System.IO.Path.Combine(destinationFolder, fileName)

                ' Copiez le fichier
                File.Copy(selectedImagePath, destinationPath, True)

                ' Videz la ComboBox et rechargez les avatars
                cboAvatars.Items.Clear()
                LoadAvatars()
                ' Vous pouvez également utiliser MahApps.Metro DialogCoordinator pour afficher un message de confirmation.

            Else
                ' L'image est trop grande, affichez un message d'erreur ou gérez-le selon vos besoins
                ' Vous pouvez également utiliser MahApps.Metro DialogCoordinator pour afficher un message de confirmation.

            End If
        End If

    End Sub

    Private Sub ToggleSwitch_Toggled(sender As Object, e As RoutedEventArgs)

        My.Application.MainWindow.UpdateLayout()

    End Sub

    Private Sub ExamDataGrid_CellEditEnding(sender As Object, e As DataGridCellEditEndingEventArgs) Handles ExamDataGrid.CellEditEnding
        Dim editedItem As ExamOption = TryCast(e.Row.Item, ExamOption)

        If editedItem IsNot Nothing Then
            If e.Column.Header.ToString() = "Name" Then
                Dim editedValue As String = TryCast((TryCast(e.EditingElement, TextBox))?.Text, String)
                editedItem.Name = editedValue
            ElseIf e.Column.Header.ToString() = "Color" Then
                ' Accédez à la propriété "SelectedColor" du ColorPicker
                Dim colorPicker As MahApps.Metro.Controls.ColorPicker = TryCast(e.EditingElement, MahApps.Metro.Controls.ColorPicker)
                If colorPicker IsNot Nothing Then
                    editedItem.Color = colorPicker.SelectedColor.ToString()
                End If
            End If

            ' Sauvegardez les modifications dans le fichier JSON
            Dim examOptionList As List(Of ExamOption) = ExamDataGrid.ItemsSource.Cast(Of ExamOption)().ToList()
            Dim examOptionCollection As New ObservableCollection(Of ExamOption)(examOptionList)

            SaveExamOptionsToJson(examOptionCollection)

        End If
    End Sub

    Private Sub SaveExamChangesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim examOptionList As List(Of ExamOption) = ExamDataGrid.ItemsSource.Cast(Of ExamOption)().ToList()
        Dim examOptionCollection As New ObservableCollection(Of ExamOption)(examOptionList)
        SaveExamOptionsToJson(examOptionCollection)
        SendFileOverNetwork("Core", "examOptions.json")
        loadExamOption()
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

Public Class AvatarItem
    Public Property ImagePath As String
    Public Property Width As Double
    Public Property Height As Double
    Public Property Tag As String
End Class





