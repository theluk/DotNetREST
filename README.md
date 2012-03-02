﻿Simple ASP.NET RESTFull Service
===============================

This is a little library which I thought might be usefull if working with ASP.NET and some javascript library like Backbone.js
It´s nothing you should use since it´s just a result of mid-night coding. I would love to get some help here!

**Note:** For now it is just usefull if you use **Backbone.js**, **ASP.NET Routing**, **LinqToSQL**

Using with LinqToSQL
====================

The problem when using LinqToSql is that there are sometimes relations. With them you run into problems.
I poked around with some walkarounds and found that using views instead of tables makes this whole thing possible.
If you just make a copy of your table into a view, no relations will be added in your dbml designer, and you can expose
just properties you want. One hack you need to do though is to remove all update-checks on the properties, and mark
the ids in your views as primery keys. This hack needs to be done in the designer.

How it should look like:
------------------------
![first step](res/one.jpg)
![second step](res/two.jpg)

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

