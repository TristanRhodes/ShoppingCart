# ShoppingCart
* Written in .net 
* Compiled in .net core (.Net Framework 4.6.1)
* Developed in Visual Studio 2015
* Time taken: Too long.

## Assumptions
* Stock seed data is defined by the csv.
* Went with in memory storage for proof of concept. (Mistake)
* Aiming for a HATEOS API with Specflow tests on top (Failed)

## Structure
Solution composed of 4 projects
* ShoppingCart - .Net Core Application, just the hosting shell for ShoppingCart.Core.
* ShoppingCart.Acceptance.Tests - Specflow tests. Needs to be run sperately and target the server.
* ShoppingCart.Core - Controllers, Models and Services.
* ShoppingCart.Core.Tests - Unit tests for Core library.

## Endpoints
The API has the following REST endpoints to support basket functionality:
* GET /heartbeat - HealthCheck endpoint
* GET /api/stock - Get List of available stock
* GET /api/{userId}/basket - Get user basket
* PUT /api/{userId}/basket/add - Add Item to basket
* POST /api/{userId}/basket/add - Bulk add items to basket
* PUT /api/{userId}/basket/remove - Add Item from basket
* POST /api/{userId}/basket/checkout - Checkout basket, deduct stock

## Packages
* Shouldly
* NUnit
* Specflow
* NSubstitute
* .Net Core

## What went well
* Learned a lot about the fascinating variety of weird behavior in .net core.
* Actually enjoyed using the .net core setup once I got it working with NCrunch.
* Good application of fluent code.
* Solid test coverage, and feel I completed the objectives with unit test coverage.
* Had a solid plan for a REST set of endpoints but didn't have the time to really mature this.
* Got some specflow acceptance tests put together, but they are not mature.

## What went wrong
* Made a huge mistake of trying to spin up a fresh new web environment. This burned a lot of time as I am used to a standardised package set. Starting again, I would have just done a library with unit tests.
* Trying to get a familiar test setup in .Net Core, WebAPI 2 or Nancy, working with NCrunch all cost a lot of time, on top of unexpected hardware problems.
* Lack of exposure to Basket type problems mean that I came at it from the wrong direction and didn't realise it until a few hours in.
* Specflow functional tests were an attempt to show some higher level API testing, but proved to also be too time consuming, covered first two scenarios.
* Stateful server choice at the begining makes it tricky to work with acceptance tests as the state is difficult to re-set.
* Controller is far too fat and complex, found myself commited to a particular approach after a few hours that significantly slowed me down, and didn't really feel I had the time to correct it.

## Possible Future Work
* Going down the API route, more refined HATEOS endpoints that provide links to continuing actions. For example, BasketItem to include links to Add / Remove a unit, and link to full product.
* Specflow tests fleshed out to better test core business scenarios. Tests packaged and deployed as part of build / deploy pipeline.
* Add some kind of automatic build to the repo.
* Refactor logic out of controller and seperate from web concerns.
* Make the Speflow test url configurable.
* Fix errors with CLI setup of project.

## Do differently
* Console app. For a demo project, this was massive overkill, for learning .net core and playing with REST, it was fun.
* Simplify the code, can probably drop a lot of the code and compact it into fewer classes.
* Break down and clarify the Basket / Stock / Product relationship and put it into some kind of relational storage.
* Creating Model library for sharing between specflow tests and main app.

## Overall
Not really happy with how this turned out. Took too much time to achieve too little, and made it feel complicated. Will probably re-visit this in a few days and tackle it again.

## To Run
To Run the server, go to the [ShoppingCart](https://github.com/TristanRhodes/ShoppingCart/tree/master/ShoppingCart) folder and run the following powershell command: 

> dotnet restore

> $env:ASPNETCORE_URLS="https://*:localhost:51998" ; dotnet run

*NOTE: Previous step does not work, dotnet is unable to resolve local dependencies. Could only get the restore / build part working via VisualStudio. Seems to be same behavior as [this issue](https://github.com/dotnet/cli/issues/3199).*

To run the acceptance tests, run the following command from the [ShoppingCart.Acceptance.Tests](https://github.com/TristanRhodes/ShoppingCart/tree/master/ShoppingCart.Acceptance.Tests) folder and run the following command:

> Pending. To run the specflow tests, start the service using the previous command and run from Visual Studio or via the NUnit test runner.

