Imports Newtonsoft.Json

Public Class Cotation

    ' Mr MULLLER  Benoit CS  6 (donné) - 1  LUN*2 (Prog + prox)
    '                                       LEN*2 (Mensuelle + journalière)
    '                                       TT*2 
    '             Supprimer RDV dans 6 mois



    'id StudioVision
    Public Property Id As Integer

    Public Property Title As String

    Public Property LastName As String

    Public Property FirstName As String

    'Code de la cotation CS , OR , CV , etc
    Public Property CodeCotation As String

    'Quand est le prochain RDV et description
    Public Property RDV As String

    'S'il y a une ordonnance LUN
    Public Property OrdoLUN As Boolean
    'Nombre d'ordo LUN
    Public Property OrdoLUNNum As Integer
    'Description de l'ordo LUN pas PROG juste VL, VL+VP, etc
    Public Property OrdoLUNDes As String

    'S'il y a une ordonnance LEN
    Public Property OrdoLEN As Boolean
    'Nombre d'ordo LEN
    Public Property OrdoLENNum As Integer
    'Description de l'ordo LEN Journalière, mensuelle etc

    'S'il y a une ordonnance TT
    Public Property OrdoTT As Boolean
    'Nombre d'ordo TT
    Public Property OrdoTTNum As Integer
    'Description de l'ordo TT
    Public Property OrdoTTDes As String

    'Commentaire
    Public Property Comment As String



End Class
