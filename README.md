Simple ASP.NET RESTFull Service
===============================

This is a little library which I thought might be usefull if working with ASP.NET and some javascript library like Backbone.js
It´s nothing you should use since it´s just a result of mid-night coding. I would love to get some help here!

**Note:** For now it is just usefull if you use **Backbone.js**, **ASP.NET Routing**, **LinqToSQL**

Using with LinqToSQL
====================

The problem when using LinqToSql is that there are sometimes relations. With them you run into problems.
I poked around with some walkarounds and found that **using views instead of tables** makes this whole thing possible.
If you just make a copy of your table into a view, no relations will be added in your dbml designer, and you can expose
just properties you want. One hack you need to do though is to remove all update-checks on the properties, and mark
the ids in your views as primery keys. This hack needs to be done in the designer.

### How it should look like:###

![first step](Simple-ASP.NET-RESTFull-Service/raw/master/res/one.jpg)
![second step](Simple-ASP.NET-RESTFull-Service/raw/master/res/two.jpg)

This enables us to make something like this

	db.GetTable(of Model)().attach(UpdatedModel, true)

The second parameter tells always "I´m modified" and this is only possible if we disable the updatechecks.
Since when getting a model from client, we instantiate a view as a new one, but actually it should not
be a new one but one, we want to be updated. And since this whole LinqToSql works with events, so that
if you change something a event is triggered and the model is marked as dirty, we need to disable this,
because nothing will be triggered if we deserialize the model from client, as it would be recognized as a new
model. Without no previous state, no change would be triggered.

Setup 
=====

Ok, the goal is to be able to set up the whole thing using web.config

web.config
----------

    <configSections>
		<section name="RESTFullService" type="RESTFullService.Configuration.RESTFullService, RESTFullService"/>
    </configSections>

    <RESTFullService>
		<RequestDefaultOptions>
			<add Name="size" Value="25" />
		</RequestDefaultOptions>
		<RESTHandlers>
			<add Name="MyModel" Type="MyProj.MyModelHandler, MyProj"/>
		</RESTHandlers>
    </RESTFullService>

**RequestDefaultOptions:** This is some kind of querystring mixed with the values of the routingcontext values. you can use them in your **RESTHandler** to limit for example the number of results.

RESTHandlers
------------

There are a few types you can inherit to build a RESTHandler that will handle you request.

The simplest way to create a handler is when you use LinqToSql. Then you would do just this.

	Imports RESTFullService

	Public Class MyModelHandler
		Inherits LinqModelRESTHandler(Of MyModelView, Integer)

		Sub New()
			Me.DB = new testDataContext()
		End Sub

		Protected Overrides Function GetByID(ByVal id As Integer) As MyModelView
			Return (From i In CType(DB, testDataContext).MyModelView Where id = i.id Select i).SingleOrDefault
		End Function

	End Class

This simple one handles **Insert**, **Update**, **Delete**, **Get(id)** and **Get**
You can override all these Functions

Routing
-------

	Public Class Global_asax
		Inherits System.Web.HttpApplication

		Shared Sub RegisterRoutes(ByVal routes As RouteCollection)

			Dim models As New Route("API/{type}", New RESTFullService.RouteHandler)
			Dim model As New Route("API/{type}/{id}", New RESTFullService.RouteHandler)

			routes.Add(model)
			routes.Add(models)

		End Sub

		Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)

			RegisterRoutes(RouteTable.Routes)

		End Sub

	End Class

The **type** parameter is the **name** property you added in your web.config. 

	GET		localhost/API/MyModel	->	MyProj.MyModelHandler.Get()		-> ArrayOfMyModel
	GET		localhost/API/MyModel/1 ->	MyProj.MyModelHandler.Get(1)	-> MyModel
	POST	localhost/API/MyModel	->	MyProj.MyModelHandler.Insert(o)	-> MyModel
	PUT		localhost/API/MyModel/1	->	MyProj.MyModelHandler.Update(o)	-> MyModel
	DELETE	localhost/API/MyModel/1	->	MyProj.MyModelHandler.Delete(1)	-> Null

