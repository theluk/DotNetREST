Imports System.Data.Linq

Public Class ModelRESTHandler

    Overridable ReadOnly Property TargetType As Type
        Get
            Return Nothing
        End Get
    End Property

    Friend _httpContext As System.Web.HttpContext
    Friend _Options As IDictionary(Of String, Object)

    Public ReadOnly Property Context As System.Web.HttpContext
        Get
            Return _httpContext
        End Get
    End Property

    Public ReadOnly Property Options As IDictionary(Of String, Object)
        Get
            Return _Options
        End Get
    End Property

    Overridable Function [Get]() As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Overridable Function [Get](ByVal id As Object) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Overridable Function Update(ByVal value As Object) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Overridable Function Delete(ByVal value As Object) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Overridable Function Insert(ByVal value As Object) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Protected Overridable Function wrapError(ByVal method As String, ByVal ex As Exception) As ModelRESTUpdateResult

#If DEBUG Then
        Return New ModelRESTUpdateResult With {.message = String.Format("Action '{0}' failed with following Result: {1}", method, ex.ToString),
                                                .success = False}
#Else
        Return New ModelRESTUpdateResult With {.message = String.Format("Action '{0}' failed.", method),
                                                    .success = False}
#End If

    End Function


End Class

Public Class ModelRESTHandler(Of T)
    Inherits ModelRESTHandler

    Public Overrides ReadOnly Property TargetType As System.Type
        Get
            Return GetType(T)
        End Get
    End Property

    Public Overrides Function Update(ByVal value As Object) As ModelRESTUpdateResult
        Return Update(CType(value, T))
    End Function

    Public Overrides Function Insert(ByVal value As Object) As ModelRESTUpdateResult
        Return Insert(CType(value, T))
    End Function

    Overridable Overloads Function Update(ByVal value As T) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Overridable Overloads Function Insert(ByVal value As T) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Public Overrides Function [Get]() As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Public Overrides Function [Get](ByVal id As Object) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

End Class

Public Class ModelRESTHandler(Of T, TId)
    Inherits ModelRESTHandler(Of T)

    Overridable Overloads Function [Get](ByVal id As TId) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Overridable Overloads Function Delete(ByVal id As TId) As ModelRESTUpdateResult
        Return New ModelRESTUpdateResult
    End Function

    Public Overrides Function Delete(ByVal value As Object) As ModelRESTUpdateResult
        Return Delete(CType(value, TId))
    End Function

    Public Overrides Function [Get](ByVal id As Object) As ModelRESTUpdateResult
        Return [Get](CType(id, TId))
    End Function

End Class

Public Class LinqModelRESTHandler(Of T As Class)
    Inherits ModelRESTHandler(Of T)

    Private _db As DataContext
    Overridable Property DB As DataContext
        Get
            Return _db
        End Get
        Set(ByVal value As DataContext)
            _db = value
        End Set
    End Property

    Public Overrides Function Update(ByVal value As T) As ModelRESTUpdateResult

        Try

            DB.GetTable(Of T)().Attach(value, True)
            DB.SubmitChanges()
            Return New ModelRESTUpdateResult With {.item = value}

        Catch ex As Exception
            Return wrapError("Update", ex)
        End Try

    End Function

    Public Overrides Function Insert(ByVal value As T) As ModelRESTUpdateResult

        Try

            DB.GetTable(Of T)().InsertOnSubmit(value)
            DB.SubmitChanges()
            Return New ModelRESTUpdateResult With {.item = value}

        Catch ex As Exception
            Return wrapError("Insert", ex)
        End Try

    End Function

    Public Overrides Function [Get]() As ModelRESTUpdateResult
        Try
            Return New ModelRESTUpdateResult With {.items = (From i In DB.GetTable(Of T)() Select i).ToArray}
        Catch ex As Exception
            Return wrapError("Get()", ex)
        End Try
    End Function

End Class

Public Class LinqModelRESTHandler(Of T As Class, TId)
    Inherits ModelRESTHandler(Of T, TId)

    Private _DB As DataContext
    Public Overridable Property DB() As DataContext
        Get
            Return _DB
        End Get
        Set(ByVal value As DataContext)
            _DB = value
        End Set
    End Property

    Protected Overridable Function GetByID(ByVal id As TId) As T
        Return Nothing
    End Function

    Public Overrides Function Update(ByVal value As T) As ModelRESTUpdateResult

        Try

            DB.GetTable(Of T)().Attach(value, True)
            DB.SubmitChanges()
            Return New ModelRESTUpdateResult With {.item = value}

        Catch ex As Exception
            Return wrapError("Update", ex)
        End Try

    End Function

    Public Overrides Function Insert(ByVal value As T) As ModelRESTUpdateResult

        Try

            DB.GetTable(Of T)().InsertOnSubmit(value)
            DB.SubmitChanges()
            Return New ModelRESTUpdateResult With {.item = value}

        Catch ex As Exception
            Return wrapError("Insert", ex)
        End Try

    End Function


    Public Overrides Function Delete(ByVal id As TId) As ModelRESTUpdateResult
        Try
            DB.GetTable(Of T)().DeleteOnSubmit(GetByID(id))
            DB.SubmitChanges()
            Return New ModelRESTUpdateResult

        Catch ex As Exception
            Return wrapError("Delete", ex)
        End Try
    End Function

    Public Overrides Function [Get](ByVal id As TId) As ModelRESTUpdateResult
        Try
            Return New ModelRESTUpdateResult With {.item = GetByID(id)}
        Catch ex As Exception
            Return wrapError("Get(id)", ex)
        End Try
    End Function

    Public Overrides Function [Get]() As ModelRESTUpdateResult
        Try
            Return New ModelRESTUpdateResult With {.items = (From i In DB.GetTable(Of T)() Select i).ToArray}
        Catch ex As Exception
            Return wrapError("Get()", ex)
        End Try
    End Function


End Class

Public Class ModelRESTUpdateResult

    Public success As Boolean = True
    Public message As String

    Public items As Object()
    Public item As Object

End Class

