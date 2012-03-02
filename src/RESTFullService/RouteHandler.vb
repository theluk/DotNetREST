Public Class RouteHandler
    Implements System.Web.Routing.IRouteHandler

    Public Function GetHttpHandler(ByVal requestContext As System.Web.Routing.RequestContext) As System.Web.IHttpHandler Implements System.Web.Routing.IRouteHandler.GetHttpHandler
        Return New Service(requestContext)
    End Function

End Class
