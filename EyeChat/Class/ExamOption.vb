﻿Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports EyeChat.MainWindow
Imports log4net
Imports Newtonsoft.Json

Public Class ExamOption
    Implements INotifyPropertyChanged
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)


    Private _index As Integer
    Public Property index As Integer
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

    Private _name As String
    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            If _name <> value Then
                _name = value
                NotifyPropertyChanged("Name")
            End If
        End Set
    End Property

    Private _color As String
    Public Property Color As String
        Get
            Return _color
        End Get
        Set(value As String)
            If _color <> value Then
                _color = value
                NotifyPropertyChanged("Color")
            End If
        End Set
    End Property

    Private _CodeMSG As String
    Public Property CodeMSG As String
        Get
            Return _CodeMSG
        End Get
        Set(value As String)
            If _CodeMSG <> value Then
                _CodeMSG = value
                NotifyPropertyChanged("CodeMSG")
            End If
        End Set
    End Property

    Private _Annotation As String
    Public Property Annotation As String
        Get
            Return _Annotation
        End Get
        Set(value As String)
            If _Annotation <> value Then
                _Annotation = value
                NotifyPropertyChanged("Annotation")
            End If
        End Set
    End Property

    Private _Floor As String
    Public Property Floor As String
        Get
            Return _Floor
        End Get
        Set(value As String)
            If _Floor <> value Then
                _Floor = value
                NotifyPropertyChanged("Floor")
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub NotifyPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Shared Sub LoadExamOptionFromJson()
        Try
            If File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "ExamOption.json")) Then
                Dim json As String = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "ExamOption.json"))
                ExamOptions = JsonConvert.DeserializeObject(Of ObservableCollection(Of ExamOption))(json)
            Else
                ExamOptions = New ObservableCollection(Of ExamOption)()
                'SaveExamOptionToJson(ExamOptions)
            End If
        Catch ex As Exception
            ' Gérer les erreurs de chargement (par exemple, journaliser l'erreur)
            logger.Error($"Erreur lors du chargement des options d'examen : {ex.Message}")
        End Try
    End Sub

End Class