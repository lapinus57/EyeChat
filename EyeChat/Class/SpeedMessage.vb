Imports System.ComponentModel

Public Class SpeedMessage
    Implements INotifyPropertyChanged

    Private _Index As Integer
    Public Property Index As Integer
        Get
            Return _index
        End Get
        Set(value As Integer)
            If _index <> value Then
                _index = value
                NotifyPropertyChanged("Index")
            End If
        End Set
    End Property

    Private _Title As String
    Public Property Title As String
        Get
            Return _Title
        End Get
        Set(value As String)
            If _Title <> value Then
                _Title = value
                NotifyPropertyChanged("Title")
            End If
        End Set
    End Property

    Private _destinataire As String
    Public Property Destinataire As String
        Get
            Return _destinataire
        End Get
        Set(value As String)
            If _destinataire <> value Then
                _destinataire = value
                NotifyPropertyChanged("Destinataire")
            End If
        End Set
    End Property

    Private _message As String
    Public Property Message As String
        Get
            Return _message
        End Get
        Set(value As String)
            If _message <> value Then
                _message = value
                NotifyPropertyChanged("Message")
            End If
        End Set
    End Property

    Private _options As String
    Public Property Options As String
        Get
            Return _options
        End Get
        Set(value As String)
            If _options <> value Then
                _options = value
                NotifyPropertyChanged("Options")
            End If
        End Set
    End Property

    Private _Load As Boolean
    Public Property Load As Boolean
        Get
            Return _Load
        End Get
        Set(value As Boolean)
            If _Load <> value Then
                _Load = value
                NotifyPropertyChanged("Load")
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub NotifyPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
