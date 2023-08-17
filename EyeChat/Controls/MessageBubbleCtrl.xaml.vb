Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Controls
Imports EyeChat.MainWindow
Imports EyeChat.Message

Public Class MessageBubbleCtrl
    Inherits UserControl
    Implements INotifyPropertyChanged

    Public Shared ReadOnly ContentProperty As DependencyProperty = DependencyProperty.Register("Content", GetType(String), GetType(MessageBubbleCtrl))
    Public Shared ReadOnly IsAlignedRightProperty As DependencyProperty = DependencyProperty.Register("IsAlignedRight", GetType(Boolean), GetType(MessageBubbleCtrl))
    Public Shared ReadOnly SizeProperty As DependencyProperty = DependencyProperty.Register("SizeDisplayUsers", GetType(Double), GetType(MessageBubbleCtrl))

    Public Property Content As String
        Get
            Return CStr(GetValue(ContentProperty))
        End Get
        Set(value As String)
            SetValue(ContentProperty, value)
        End Set
    End Property



    Public Property Size As Double
        Get
            Return CDbl(GetValue(SizeProperty))
        End Get
        Set(value As Double)
            SetValue(SizeProperty, value)
        End Set
    End Property

    Public Property IsAlignedRight As Boolean
        Get
            Return CBool(GetValue(IsAlignedRightProperty))
        End Get
        Set(value As Boolean)
            SetValue(IsAlignedRightProperty, value)
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
        Size = My.Settings.AppSizeDisplay
    End Sub
    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub CopyMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Dim message As Message = DirectCast(DataContext, Message)
        If message IsNot Nothing Then
            Dim textToCopy As String = $"{message.Name} & {message.Sender}: {message.Content}, à {message.Timestamp:HH:mm}"
            'Dim textToCopy As String = $"{message.Name} & {message.Sender}: {message.Content}, à {message.Timestamp.ToString("HH:mm")}"
            Clipboard.SetText(textToCopy)
        End If
    End Sub

    Public Sub DeleteMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Dim message As Message = DirectCast(DataContext, Message)
        If message IsNot Nothing Then

            Dim messageToRemove As Message = Messages.FirstOrDefault(Function(m) m.Name = message.Name AndAlso m.Sender = message.Sender AndAlso m.Content = message.Content AndAlso m.Timestamp = message.Timestamp)

            If messageToRemove IsNot Nothing Then
                Messages.Remove(messageToRemove)
                SaveMessagesToJson(Messages)
                SelectUser(SelectedUser)
            End If
        End If
    End Sub

    Private Sub AMenuItem_Click(sender As Object, e As RoutedEventArgs)

    End Sub
    Private Sub BMenuItem_Click(sender As Object, e As RoutedEventArgs)

    End Sub
    Private Sub CMenuItem_Click(sender As Object, e As RoutedEventArgs)

    End Sub
    Private Sub DMenuItem_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class